namespace Client
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ok1 = new Button();
            downloadURL = new TextBox();
            label1 = new Label();
            cancel1 = new Button();
            addToQueue = new Button();
            start1 = new Button();
            targetPath = new TextBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            ok2 = new Button();
            label5 = new Label();
            Progress1 = new Label();
            start2 = new Button();
            cancel2 = new Button();
            comboBox1 = new ComboBox();
            listView1 = new ListView();
            URL = new ColumnHeader();
            Status = new ColumnHeader();
            Progress = new ColumnHeader();
            Priority = new ColumnHeader();
            progressBar1 = new ProgressBar();
            progressBar2 = new ProgressBar();
            progressBar3 = new ProgressBar();
            cancel3 = new Button();
            start3 = new Button();
            label7 = new Label();
            label8 = new Label();
            fileData = new Label();
            pause1 = new Button();
            pause2 = new Button();
            pause3 = new Button();
            SuspendLayout();
            // 
            // ok1
            // 
            ok1.Location = new Point(900, 18);
            ok1.Name = "ok1";
            ok1.Size = new Size(104, 31);
            ok1.TabIndex = 0;
            ok1.Text = "OK";
            ok1.UseVisualStyleBackColor = true;
            // 
            // downloadURL
            // 
            downloadURL.Location = new Point(287, 21);
            downloadURL.Name = "downloadURL";
            downloadURL.Size = new Size(596, 31);
            downloadURL.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(79, 180);
            label1.Name = "label1";
            label1.Size = new Size(151, 50);
            label1.TabIndex = 2;
            label1.Text = "Download Queue\r\n\r\n";
            // 
            // cancel1
            // 
            cancel1.Location = new Point(912, 491);
            cancel1.Name = "cancel1";
            cancel1.Size = new Size(92, 37);
            cancel1.TabIndex = 3;
            cancel1.Text = "Cancel";
            cancel1.UseVisualStyleBackColor = true;
            cancel1.Click += stop1_Click;
            // 
            // addToQueue
            // 
            addToQueue.Location = new Point(496, 119);
            addToQueue.Name = "addToQueue";
            addToQueue.Size = new Size(143, 37);
            addToQueue.TabIndex = 4;
            addToQueue.Text = "Add to Queue";
            addToQueue.UseVisualStyleBackColor = true;
            addToQueue.Click += addToQueue_Click;
            // 
            // start1
            // 
            start1.Location = new Point(710, 491);
            start1.Name = "start1";
            start1.Size = new Size(98, 37);
            start1.TabIndex = 5;
            start1.Text = "Start";
            start1.UseVisualStyleBackColor = true;
            start1.Click += start1_Click;
            // 
            // targetPath
            // 
            targetPath.Location = new Point(287, 65);
            targetPath.Name = "targetPath";
            targetPath.Size = new Size(596, 31);
            targetPath.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(79, 65);
            label2.Name = "label2";
            label2.Size = new Size(144, 25);
            label2.TabIndex = 8;
            label2.Text = "Enter Target Path";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(79, 21);
            label3.Name = "label3";
            label3.Size = new Size(175, 25);
            label3.TabIndex = 9;
            label3.Text = "Enter Download URL";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(79, 119);
            label4.Name = "label4";
            label4.Size = new Size(119, 25);
            label4.TabIndex = 10;
            label4.Text = "Select Priority";
            // 
            // ok2
            // 
            ok2.Location = new Point(900, 65);
            ok2.Name = "ok2";
            ok2.Size = new Size(104, 31);
            ok2.TabIndex = 11;
            ok2.Text = "OK";
            ok2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(79, 654);
            label5.Name = "label5";
            label5.Size = new Size(836, 25);
            label5.TabIndex = 13;
            label5.Text = "To view other download details and remaining time, please select one of the URL items from the listview.";
            // 
            // Progress1
            // 
            Progress1.AutoSize = true;
            Progress1.Location = new Point(79, 472);
            Progress1.Name = "Progress1";
            Progress1.Size = new Size(0, 25);
            Progress1.TabIndex = 16;
            // 
            // start2
            // 
            start2.Location = new Point(710, 537);
            start2.Name = "start2";
            start2.Size = new Size(98, 37);
            start2.TabIndex = 17;
            start2.Text = "Start";
            start2.UseVisualStyleBackColor = true;
            start2.Click += start2_Click;
            // 
            // cancel2
            // 
            cancel2.Location = new Point(912, 537);
            cancel2.Name = "cancel2";
            cancel2.Size = new Size(92, 37);
            cancel2.TabIndex = 18;
            cancel2.Text = "Cancel";
            cancel2.UseVisualStyleBackColor = true;
            cancel2.Click += stop2_Click;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(287, 120);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(182, 33);
            comboBox1.TabIndex = 19;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { URL, Status, Progress, Priority });
            listView1.Location = new Point(79, 224);
            listView1.Name = "listView1";
            listView1.Size = new Size(927, 208);
            listView1.TabIndex = 20;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // URL
            // 
            URL.Text = "Url";
            URL.Width = 480;
            // 
            // Status
            // 
            Status.Text = "Status";
            Status.TextAlign = HorizontalAlignment.Center;
            Status.Width = 150;
            // 
            // Progress
            // 
            Progress.Text = "Progress";
            Progress.TextAlign = HorizontalAlignment.Center;
            Progress.Width = 135;
            // 
            // Priority
            // 
            Priority.Text = "Priority";
            Priority.TextAlign = HorizontalAlignment.Center;
            Priority.Width = 135;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(79, 494);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(603, 34);
            progressBar1.TabIndex = 21;
            // 
            // progressBar2
            // 
            progressBar2.Location = new Point(79, 540);
            progressBar2.Name = "progressBar2";
            progressBar2.Size = new Size(603, 34);
            progressBar2.TabIndex = 22;
            // 
            // progressBar3
            // 
            progressBar3.Location = new Point(79, 589);
            progressBar3.Name = "progressBar3";
            progressBar3.Size = new Size(603, 34);
            progressBar3.TabIndex = 26;
            // 
            // cancel3
            // 
            cancel3.Location = new Point(912, 586);
            cancel3.Name = "cancel3";
            cancel3.Size = new Size(92, 37);
            cancel3.TabIndex = 25;
            cancel3.Text = "Cancel";
            cancel3.UseVisualStyleBackColor = true;
            cancel3.Click += stop3_Click;
            // 
            // start3
            // 
            start3.Location = new Point(710, 586);
            start3.Name = "start3";
            start3.Size = new Size(98, 37);
            start3.TabIndex = 24;
            start3.Text = "Start";
            start3.UseVisualStyleBackColor = true;
            start3.Click += start3_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(79, 451);
            label7.Name = "label7";
            label7.Size = new Size(279, 25);
            label7.TabIndex = 23;
            label7.Text = "Progress Bar of Downloaded Files";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(468, 297);
            label8.Name = "label8";
            label8.Size = new Size(0, 25);
            label8.TabIndex = 28;
            // 
            // fileData
            // 
            fileData.AutoSize = true;
            fileData.Location = new Point(79, 698);
            fileData.Name = "fileData";
            fileData.Size = new Size(0, 25);
            fileData.TabIndex = 29;
            // 
            // pause1
            // 
            pause1.Location = new Point(814, 491);
            pause1.Name = "pause1";
            pause1.Size = new Size(92, 37);
            pause1.TabIndex = 33;
            pause1.Text = "Pause";
            pause1.UseVisualStyleBackColor = true;
            pause1.Click += pause1_Click;
            // 
            // pause2
            // 
            pause2.Location = new Point(814, 537);
            pause2.Name = "pause2";
            pause2.Size = new Size(92, 37);
            pause2.TabIndex = 34;
            pause2.Text = "Pause";
            pause2.UseVisualStyleBackColor = true;
            pause2.Click += pause2_Click;
            // 
            // pause3
            // 
            pause3.Location = new Point(814, 586);
            pause3.Name = "pause3";
            pause3.Size = new Size(92, 37);
            pause3.TabIndex = 35;
            pause3.Text = "Pause";
            pause3.UseVisualStyleBackColor = true;
            pause3.Click += pause3_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1109, 876);
            Controls.Add(pause3);
            Controls.Add(pause2);
            Controls.Add(pause1);
            Controls.Add(fileData);
            Controls.Add(label8);
            Controls.Add(progressBar3);
            Controls.Add(cancel3);
            Controls.Add(start3);
            Controls.Add(label7);
            Controls.Add(progressBar2);
            Controls.Add(progressBar1);
            Controls.Add(listView1);
            Controls.Add(comboBox1);
            Controls.Add(cancel2);
            Controls.Add(start2);
            Controls.Add(Progress1);
            Controls.Add(label5);
            Controls.Add(ok2);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(targetPath);
            Controls.Add(start1);
            Controls.Add(addToQueue);
            Controls.Add(cancel1);
            Controls.Add(label1);
            Controls.Add(downloadURL);
            Controls.Add(ok1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ok1;
        private TextBox downloadURL;
        private Label label1;
        private Button cancel1;
        private Button addToQueue;
        private Button start1;
        private TextBox targetPath;
        private Label label2;
        private Label label3;
        private Label label4;
        private Button ok2;
        private Label label5;
        private Label Progress1;
        private Button start2;
        private Button cancel2;
        private ComboBox comboBox1;
        private ListView listView1;
        private ColumnHeader URL;
        private ColumnHeader Status;
        private ColumnHeader Progress;
        private ColumnHeader Priority;
        private ProgressBar progressBar1;
        private ProgressBar progressBar2;
        private ProgressBar progressBar3;
        private Button cancel3;
        private Button start3;
        private Label label7;
        private Label label8;
        private Label fileData;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button pause1;
        private Button pause2;
        private Button pause3;
    }
}
