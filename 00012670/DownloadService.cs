using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace _00012670
{

    public class DownloadDetails
    {
        public int Progress { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public long BytesCopied { get; set; }
        public long TotalFileSize { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime PauseTime { get; set; }
        public ManualResetEvent PauseEvent { get; set; }
    }


    // This class provides the implementation for the IDownloadService interface.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    
    public class DownloadService : IDownloadService
    {
        // A lock object to ensure thread safety.
        private readonly object lockObject = new object();

        // A dictionary to hold the details of each download.
        private Dictionary<int, DownloadDetails> downloadDetailsDictionary = new Dictionary<int, DownloadDetails>();



        // Constant representing the status when the progress of a download is not found.
        private const int ProgressNotFound = -1;
        // Constant representing the status when a download is paused.
        private const int ProgressPaused = -2;




        // This method is used to create the destination file path.
        private string CreateDestinationFilePath(string downloadUrl, string targetPath)
        {
            // Get the file name without extension from the download URL.
            string sourceFileName = Path.GetFileNameWithoutExtension(downloadUrl);
            // Get the file extension from the download URL.
            string sourceFileExtension = Path.GetExtension(downloadUrl);
            // Combine the target path with the source file name and extension to create the destination file path.
            string destinationFilePath = Path.Combine(targetPath, sourceFileName + sourceFileExtension);

            // If a file already exists at the destination file path, append a timestamp to the file name to avoid overwriting the existing file.
            if (File.Exists(destinationFilePath))
            {
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                destinationFilePath = Path.Combine(targetPath, $"{sourceFileName}_{timestamp}{sourceFileExtension}");
            }

            // Return the destination file path.
            return destinationFilePath;
        }




        // Method to download a file.
        public async Task<bool> DownloadFile(string downloadUrl, string targetPath, int downloadId)
        {
            // Create a CancellationTokenSource object.
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            // Get the CancellationToken from the CancellationTokenSource.
            CancellationToken cancellationToken = tokenSource.Token;

            try
            {
                // Check if the source file exists.
                if (!File.Exists(downloadUrl))
                {
                    throw new FileNotFoundException("Source file not found.", downloadUrl);
                }

                // Check if the target directory exists, if not, create it.
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                // Create the destination file path.
                string destinationFilePath = CreateDestinationFilePath(downloadUrl, targetPath);

                // Get the size of the source file.
                long sourceFileSize = new FileInfo(downloadUrl).Length;

                // Lock the download details dictionary to ensure thread safety.
                lock (lockObject)
                {
                    // Initialize the download details for the current download.
                    downloadDetailsDictionary[downloadId] = new DownloadDetails
                    {
                        Progress = 0,
                        CancellationTokenSource = tokenSource,
                        BytesCopied = 0,
                        TotalFileSize = sourceFileSize,
                        StartTime = DateTime.Now,
                        PauseEvent = new ManualResetEvent(true)
                    };
                }

                long copiedBytes = 0;
                int lastProgress = 0;

                // Open the source file for reading.
                using (FileStream sourceStream = new FileStream(downloadUrl, FileMode.Open, FileAccess.Read))

                // Create the destination file for writing.
                using (FileStream destinationStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;

                    // Read from the source file until no more data is available.
                    while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        // Wait for the pause event to be set.
                        downloadDetailsDictionary[downloadId].PauseEvent.WaitOne();

                        // If a cancellation is requested, delete the destination file and return false.
                        if (cancellationToken.IsCancellationRequested)
                        {
                            if (File.Exists(destinationFilePath))
                            {
                                File.Delete(destinationFilePath);
                            }
                            return false;
                        }

                        // Write the read data to the destination file.
                        destinationStream.Write(buffer, 0, bytesRead);

                        // Update the number of copied bytes.
                        copiedBytes += bytesRead;

                        // Lock the download details dictionary to ensure thread safety.
                        lock (lockObject)
                        {
                            // Update the number of copied bytes in the download details.
                            downloadDetailsDictionary[downloadId].BytesCopied = copiedBytes;
                        }

                        // Delay the task for 1 millisecond to allow other tasks to run.
                        await Task.Delay(1, cancellationToken);

                        // Update the progress of the download.
                        lastProgress = UpdateProgress(copiedBytes, sourceFileSize, downloadId, lastProgress);
                    }
                    // If the download completed successfully, return true.
                    return true;
                }
            }
            catch (OperationCanceledException)
            {
                // If the operation was canceled, set the progress to ProgressNotFound and return false.
                downloadDetailsDictionary[downloadId].Progress = ProgressNotFound;
                return false;
            }
            catch (Exception)
            {
                // If an exception occurred, set the progress to ProgressNotFound and return false.
                downloadDetailsDictionary[downloadId].Progress = ProgressNotFound;
                return false;
            }
        }




        // Method to get the progress of a download.
        public int GetDownloadProgress(int downloadId)
        {
            // Lock the download details dictionary to ensure thread safety.
            lock (lockObject)
            {
                // Check if the download details for the given download ID exist.
                if (downloadDetailsDictionary.TryGetValue(downloadId, out var downloadDetails))
                {
                    // If the download is paused, ProgressPaused.
                    if (!downloadDetails.PauseEvent.WaitOne(0))
                    {
                        return ProgressPaused;
                    }
                    // Return the progress of the download.
                    return downloadDetails.Progress;
                }
                // If the download details for the given download ID do not exist, return ProgressNotFound.
                return ProgressNotFound;
            }
        }




        // This method is used to get the remaining time for a download.
        public string GetRemainingTime(int downloadId)
        {
            // Lock the download details dictionary to ensure thread safety.
            lock (lockObject)
            {
                // Check if the download details for the given download ID exist.
                if (downloadDetailsDictionary.TryGetValue(downloadId, out var downloadDetails))
                {
                    // If the download is paused, return an empty string.
                    if (!downloadDetails.PauseEvent.WaitOne(0))
                    {
                        return ProgressPaused.ToString();
                    }
                    // If the download has started, calculate the remaining time.
                    if (downloadDetails.Progress > 0)
                    {
                        // Calculate the remaining bytes.
                        long remainingBytes = downloadDetails.TotalFileSize - downloadDetails.BytesCopied;

                        // Calculate the elapsed time in seconds.
                        double elapsedSeconds = (DateTime.Now - downloadDetails.StartTime).TotalSeconds;

                        // If no time has elapsed, return "...".
                        if (elapsedSeconds == 0)
                        {
                            return "...";
                        }

                        // Calculate the download speed in bytes per second.
                        double bytesPerSecond = downloadDetails.BytesCopied / elapsedSeconds;

                        // Calculate the remaining time in seconds.
                        double remainingSeconds = remainingBytes / bytesPerSecond;

                        // Return the remaining time in the format "hh:mm:ss".
                        return TimeSpan.FromSeconds(remainingSeconds).ToString(@"hh\:mm\:ss");
                    }
                    // If the download has not started yet, return "...".
                    return "...";
                }
                // If the download details for the given download ID do not exist, return "error".
                return "error";
            }
        }




        // This method is used to update the progress of a download.
        private int UpdateProgress(long copiedBytes, long sourceFileSize, int downloadId, int lastProgress)
        {
            // Calculate the current progress as a percentage of the total file size.
            int currentProgress = (int)(copiedBytes * 100 / sourceFileSize);

            // If the current progress is greater than the last progress, update the progress in the download details dictionary.
            if (currentProgress > lastProgress)
            {
                lock (lockObject)
                {
                    downloadDetailsDictionary[downloadId].Progress = currentProgress;
                    // Update the last progress to the current progress.
                    lastProgress = currentProgress;
                }
            }

            // Return the last progress.
            return lastProgress;
        }




        // This method is used to pause a download.
        public void PauseDownload(int downloadId)
        {
            // Check if the download details for the given download ID exist.
            if (downloadDetailsDictionary.TryGetValue(downloadId, out var downloadDetails))
            {
                // Set the pause time to the current time.
                downloadDetails.PauseTime = DateTime.Now;

                // Reset the pause event to pause the download.
                downloadDetails.PauseEvent.Reset();
            }
        }


        // This method is used to resume a download.
        public void ResumeDownload(int downloadId)
        {
            // Check if the download details for the given download ID exist.
            if (downloadDetailsDictionary.TryGetValue(downloadId, out var downloadDetails))
            {
                // Adjust the start time by the duration of the pause.
                downloadDetails.StartTime += DateTime.Now - downloadDetails.PauseTime;
                // Set the pause event to resume the download.
                downloadDetails.PauseEvent.Set();
            }
        }




        // Method to cancel a download.
        public void CancelDownload(int downloadId)
        {
            // Check if the download details for the given download ID exist.
            if (downloadDetailsDictionary.TryGetValue(downloadId, out var downloadDetails))
            {
                // Cancel the download by triggering the cancellation token.
                downloadDetails.CancellationTokenSource.Cancel();
            }
        }

    }
}