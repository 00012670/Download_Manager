using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Security.Policy;
using System;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Client
{
    public partial class Form1 : Form
    {
        ServiceReference1.DownloadServiceClient client = new ServiceReference1.DownloadServiceClient();

        // Semaphores to control the number of concurrent downloads and button clicks
        
        private Semaphore semaphore = new Semaphore(3, 3);
        private Semaphore buttonClickSemaphore = new Semaphore(1, 1);

        // Event to signal the start of a download
        private ManualResetEvent downloadResetEvent = new ManualResetEvent(false);

        // Global download ID to uniquely identify each download
        private static int globalDownloadId = 0;

        // ID of the currently active download
        private int activeDownloadId;

        // Button and progress bar for the currently active download
        private Button activeButton;
        private ProgressBar activeProgressBar;

        // Constant representing the status when the progress of a download is not found.
        private const int ProgressNotFound = -1;

        // Constant representing the status when a download is paused.
        private const int ProgressPaused = -2;


        // Flags to indicate if a download has been cancelled
        private bool isDownload1Cancelled = false;
        private bool isDownload2Cancelled = false;
        private bool isDownload3Cancelled = false;

        // Download IDs for each button
        private int downloadIdForButton1;
        private int downloadIdForButton2;
        private int downloadIdForButton3;


        public Form1()
        {
            // Initialize the form and set the operation timeout for the client
            InitializeComponent();
            client.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(5); // Set timeout to 5 minutes

            // Initially disable the cancel and pause buttons
            cancel1.Enabled = false;
            cancel2.Enabled = false;
            cancel3.Enabled = false;

            pause1.Enabled = false;
            pause2.Enabled = false;
            pause3.Enabled = false;


            // Add event handler for SelectedIndexChanged event
            listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Add items to the priority combo box and add event handlers for the text changed events
            comboBox1.Items.AddRange(new string[] { "Low", "Normal", "High" });

            downloadURL.Click += downloadURL_TextChanged;
            targetPath.Click += targetPath_TextChanged;
        }



        // Sets the download URL.
        private void downloadURL_TextChanged(object sender, EventArgs e)
        {
            // Open a file dialog and set the selected file as the download URL
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                downloadURL.Text = openFileDialog.FileName;
            }
        }



        // Sets the target path for the download.
        private void targetPath_TextChanged(object sender, EventArgs e)
        {
            // Open a folder dialog and set the selected folder as the target path
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                targetPath.Text = folderBrowserDialog.SelectedPath;
            }
        }



        // Gets the selected priority.
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected priority when it changes
            string selectedPriority = comboBox1.SelectedItem.ToString();
        }


        // This method is called when the "Add to Queue" button is clicked. It adds a download to the queue.
        private void addToQueue_Click(object sender, EventArgs e)
        {
            // Check if a URL, target path, and priority have been selected before adding a download to the queue
            if (string.IsNullOrEmpty(downloadURL.Text) || string.IsNullOrEmpty(targetPath.Text))
            {
                MessageBox.Show("Please select a URL and target path before adding to the queue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if a priority has been selected
            if (comboBox1.SelectedItem == null)
            {
                // If no priority has been selected, show a message box and return
                MessageBox.Show("Please select a priority before starting the download.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            // Start the download in a new thread with the selected priority
            Thread thrd = new Thread(StartDownload);
            ThreadPriority thrdPriority = ThreadPriority.Normal;
            switch (comboBox1.Text)
            {
                case "Low":
                    thrdPriority = ThreadPriority.Lowest;
                    break;
                case "Normal":
                    thrdPriority = ThreadPriority.Normal;
                    break;
                case "High":
                    thrdPriority = ThreadPriority.Highest;
                    break;
            }
            thrd.Priority = thrdPriority;
            thrd.Start();
        }


        // This method starts the download in a new thread.
        private async void StartDownload()
        {
            // Get a unique ID for the download and set it as the active download ID
            int currentDownloadId = globalDownloadId++;
            activeDownloadId = currentDownloadId;

            // Get the URL, target path, and priority for the download
            string url = downloadURL.Text;
            string targetUrl = targetPath.Text;
            string priority = GetPriority();

            // Create a new item in the list view for the download
            ListViewItem listViewItem = CreateListViewItem(url, targetUrl, priority);

            Invoke(new MethodInvoker(async () =>
            {
                // Update the fileData label with the details of the new download
                DisplayDefaultFileDetails(listView1.Items[0]);
            }));


            // Wait for the semaphores and reset event before starting the download
            semaphore.WaitOne();
            buttonClickSemaphore.WaitOne();
            downloadResetEvent.WaitOne();
            downloadResetEvent.Reset();
            buttonClickSemaphore.Release();

            // Update the status of the download to "Downloading"
            UpdateDownloadStatusInListView(listViewItem, DownloadStatus.Downloading);

            // Get the progress bar and button for the download and assign the download ID to the button
            ProgressBar progressBar = activeProgressBar;
            Button button = activeButton;
            AssignDownloadIdToButton(button, currentDownloadId);

            // Start the download asynchronously
            client.DownloadFileAsync(url, targetUrl, currentDownloadId);


            // Update the progress of the download until it is complete or cancelled
            int progress = 0;
            while (progress < 100)
            {
                if (CheckIfDownloadIsCancelled(progressBar))
                {
                    CancelDownload(currentDownloadId, button, listViewItem);
                    return;
                }

                progress = client.GetDownloadProgress(currentDownloadId);

                if (progress == ProgressPaused)
                {
                    UpdateDownloadStatusInListView(listViewItem, DownloadStatus.Paused);
                    await Task.Delay(100);
                    continue;
                }
                else
                {
                    UpdateDownloadStatusInListView(listViewItem, DownloadStatus.Downloading);
                }

                if (progress != ProgressNotFound)
                {
                    UpdateDownloadProgressInListView(listViewItem, progressBar, progress, currentDownloadId);
                    await Task.Delay(1000);
                }
            }

            Button pauseButton = null;
            if (button == start1)
            {
                pauseButton = pause1;
            }
            else if (button == start2)
            {
                pauseButton = pause2;
            }
            else if (button == start3)
            {
                pauseButton = pause3;
            }


            // Complete the download and release the semaphore
            CompleteDownload(button, listViewItem, pauseButton);
            semaphore.Release();
        }



        // Enum to represent the status of a download
        private enum DownloadStatus
        {
            Waiting,
            Downloading,
            Paused,
            Cancelled,
            Complete
        }



        // This method gets the selected priority from the combo box.
        private string GetPriority()
        {
            // Declare a string to hold the priority
            string priority = "";

            // Use Invoke to run code on the UI thread
            Invoke(new MethodInvoker(() =>
            {
                // Get the text of the combo box, which is the selected priority
                priority = comboBox1.Text;
            }));

            // Return the selected priority
            return priority;
        }



        // Creates a new list view item for the download.
        private ListViewItem CreateListViewItem(string url, string targetUrl, string priority)
        {
            // Create a new list view item with the URL as the text
            ListViewItem listViewItem = new ListViewItem(url);

            Invoke(new MethodInvoker(() =>
            {
                // Add sub items to the list view item for the status, progress, priority, remaining time, and target URL
                listViewItem.SubItems.Add(DownloadStatus.Waiting.ToString());
                listViewItem.SubItems.Add("0%");
                listViewItem.SubItems.Add(priority);
                listViewItem.SubItems.Add("");
                listViewItem.SubItems.Add(targetUrl);

                // Add the list view item to the list view
                listView1.Items.Add(listViewItem);
            }));

            // Return the created list view item
            return listViewItem;
        }



        private bool CheckIfDownloadIsCancelled(ProgressBar progressBar)
        {
            // Check if the download corresponding to the progress bar has been cancelled
            return (progressBar == progressBar1 && isDownload1Cancelled)
                || (progressBar == progressBar2 && isDownload2Cancelled)
                || (progressBar == progressBar3 && isDownload3Cancelled);
        }


    
        private void CancelDownload(int downloadId, Button button, ListViewItem listViewItem)
        {
            // Cancel the download with the given ID
            client.CancelDownload(downloadId);

            // Use Invoke to run code on the UI thread
            Invoke(new MethodInvoker(() =>
            {
                // Enable the button and update the status and remaining time of the list view item
                button.Enabled = true;
                listViewItem.SubItems[1].Text = DownloadStatus.Cancelled.ToString();
                listViewItem.SubItems[4].Text = ""; // Clear the remaining time
            }));

            // Release the semaphore to allow another download to start
            semaphore.Release();

            // Reset the isDownloadCancelled flag for the corresponding download
            if (button == start1)
            {
                isDownload1Cancelled = false;
            }
            else if (button == start2)
            {
                isDownload2Cancelled = false;
            }
            else if (button == start3)
            {
                isDownload3Cancelled = false;
            }
        }



        // Updates the status of a download in the list view.
        private void UpdateDownloadStatusInListView(ListViewItem listViewItem, DownloadStatus status)
        {
            Invoke(new MethodInvoker(() =>
            {
                // Update the status of the list view item
                listViewItem.SubItems[1].Text = status.ToString();
            }));
        }



        // Updates the progress of a download in the list view.
        private void UpdateDownloadProgressInListView(ListViewItem listViewItem, ProgressBar progressBar, int progress, int downloadId)
        {
            Invoke(new MethodInvoker(() =>
            {
                // Update the progress of the list view item and progress bar
                listViewItem.SubItems[2].Text = $"{progress}%";
                progressBar.Value = progress;

                // Get the remaining time for the download and update it if it has changed
                string remainingTime = client.GetRemainingTime(downloadId);
                if (!string.IsNullOrEmpty(remainingTime) && listViewItem.SubItems[4].Text != remainingTime)
                {
                    listViewItem.SubItems[4].Text = remainingTime;
                }
            }));
        }



        // This method is called when the selected item in the list view changes. It updates the file data text.
        private async void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the file data text box when a new item is selected in the list view
            while (listView1.SelectedItems.Count == 1)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                string url = selectedItem.Text;
                string status = selectedItem.SubItems[1].Text;
                string targetPath = selectedItem.SubItems[5].Text;
                string remainingTime = selectedItem.SubItems[4].Text;
                string progress = selectedItem.SubItems[2].Text;

                fileData.Text = $"Filename: {url} " +
                    $"\nTarget Path: {targetPath}" +
                    $"\nRemaining time: {remainingTime}" +
                    $"\nProgress: {progress}" +
                    $"\nStatus: {status}";
                await Task.Delay(100);
            }
        }



        //Shows first downloading file details as a default 
        private async void DisplayDefaultFileDetails(ListViewItem listViewItem)
        {
            while (true)
            {
                Invoke(new MethodInvoker(() => {
                    if (listView1.SelectedItems.Count < 1)
                    {
                        string url = listViewItem.Text;
                        string status = listViewItem.SubItems[1].Text;
                        string targetPath = listViewItem.SubItems[5].Text;
                        string remainingTime = listViewItem.SubItems[4].Text;
                        string progress = listViewItem.SubItems[2].Text;

                        fileData.Text = $"Filename: {url} " +
                            $"\nTarget Path: {targetPath}" +
                            $"\nRemaining time: {remainingTime}" +
                            $"\nProgress: {progress}" +
                            $"\nStatus: {status}";
                    }
                }));
                await Task.Delay(100);
            }
        }



        //Completes the download process.
        private void CompleteDownload(Button button, ListViewItem listViewItem, Button pauseButton)
        {
            // Invoke is used to ensure that UI updates are done on the main UI thread.
            Invoke(new MethodInvoker(() =>
            {
                // Enable the start button for the next download.
                button.Enabled = true;

                // Update the status of the download in the ListView to "Complete".
                listViewItem.SubItems[1].Text = DownloadStatus.Complete.ToString();

                // Disable the pause button as the download is complete.
                pauseButton.Enabled = false;

                // Depending on which download (start1, start2, or start3) has completed,
                // disable the corresponding cancel button.
                if (activeButton == start1)
                {
                    cancel1.Enabled = false;
                }
                else if (activeButton == start2)
                {
                    cancel2.Enabled = false;
                }
                else if (activeButton == start3)
                {
                    cancel3.Enabled = false;
                }
            }));
        }



        // Checks if there are any items in the queue that are waiting to be downloaded.
        private bool CheckIfQueueHasWaitingItems()
        {
            // Check if there are any items in the queue that are waiting to be downloaded
            if (!listView1.Items.Cast<ListViewItem>().Any(item => item.SubItems[1].Text == "Waiting"))
            {
                // If not, it shows a message box and return false
                MessageBox.Show("There are no items in the queue. Please add items to the queue before starting the download.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // If there are waiting items, return true
            return true;
        }



        // Sets up a download when a start button is clicked.
        private void SetupDownload(Button startButton, Button cancelButton, Button pauseButton, ProgressBar progressBar)
        {
            // Check if there are any items in the queue that are waiting to be downloaded
            if (!CheckIfQueueHasWaitingItems())
            {
                // If not, return without doing anything
                return;
            }

            // Disable the start button, enable the cancel and pause buttons, and set the active button and progress bar
            startButton.Enabled = false;
            cancelButton.Enabled = true;
            pauseButton.Enabled = true;
            activeButton = startButton;
            activeProgressBar = progressBar;

            // Signal the start of a download
            downloadResetEvent.Set();
        }


        // Cancels a download when a stop button is clicked.
        private void CancelDownload(ref bool isDownloadCancelled, Button cancelButton, Button pauseButton)
        {
            // Cancel the active download 
            client.CancelDownload(activeDownloadId);
            isDownloadCancelled = true;
            cancelButton.Enabled = false;
            pauseButton.Enabled = false;
        }



        // Assigns a download ID to a button.
        private void AssignDownloadIdToButton(Button button, int downloadId)
        {
            // Assign the download ID to the corresponding button
            if (button == start1)
            {
                downloadIdForButton1 = downloadId;
            }
            else if (button == start2)
            {
                downloadIdForButton2 = downloadId;
            }
            else if (button == start3)
            {
                downloadIdForButton3 = downloadId;
            }
        }



        // Setup the download for the start buttons
        private void start1_Click(object sender, EventArgs e)
        {
            SetupDownload(start1, cancel1, pause1, progressBar1);
        }

        private void start2_Click(object sender, EventArgs e)
        {
            SetupDownload(start2, cancel2, pause2, progressBar2);
        }

        private void start3_Click(object sender, EventArgs e)
        {
            SetupDownload(start3, cancel3, pause3, progressBar3);
        }



        // Cancel the download for the stop buttons
        private void stop1_Click(object sender, EventArgs e)
        {
            CancelDownload(ref isDownload1Cancelled, cancel1, pause1);
        }

        private void stop2_Click(object sender, EventArgs e)
        {
            CancelDownload(ref isDownload2Cancelled, cancel2, pause2);
        }

        private void stop3_Click(object sender, EventArgs e)
        {
            CancelDownload(ref isDownload3Cancelled, cancel3, pause3);
        }



        // Toggle the pause/resume state for buttons
        private void pause1_Click(object sender, EventArgs e)
        {
            TogglePauseResume(pause1, downloadIdForButton1);
        }

        private void pause2_Click(object sender, EventArgs e)
        {
            TogglePauseResume(pause2, downloadIdForButton2);
        }

        private void pause3_Click(object sender, EventArgs e)
        {
            TogglePauseResume(pause3, downloadIdForButton3);
        }

       

        private void TogglePauseResume(Button pauseButton, int downloadId)
        {
            // If the button text is "Pause", pause the download and change the text to "Resume"
            if (pauseButton.Text == "Pause")
            {
                client.PauseDownload(downloadId);
                pauseButton.Text = "Resume";
            }
            else
            {
                // If the button text is not "Pause", resume the download and change the text to "Pause"
                client.ResumeDownload(downloadId);
                pauseButton.Text = "Pause";
            }
        }
    }
}
