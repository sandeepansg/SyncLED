namespace Test_1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.About = new System.Windows.Forms.Button();
            this.CPU_Name = new System.Windows.Forms.TextBox();
            this.GPU_Name = new System.Windows.Forms.TextBox();
            this.temp_CPU = new System.Windows.Forms.Label();
            this.temp_GPU = new System.Windows.Forms.Label();
            this.CPU_temp = new System.Windows.Forms.TextBox();
            this.GPU_temp = new System.Windows.Forms.TextBox();
            this.CPUButton = new System.Windows.Forms.RadioButton();
            this.GPUButton = new System.Windows.Forms.RadioButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.MinimizeSystemTray = new System.Windows.Forms.CheckBox();
            this.AutoSync = new System.Windows.Forms.Button();
            this.connectionStatus = new System.Windows.Forms.Label();
            this.Progress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // About
            // 
            this.About.Location = new System.Drawing.Point(304, 98);
            this.About.Name = "About";
            this.About.Size = new System.Drawing.Size(64, 23);
            this.About.TabIndex = 5;
            this.About.Text = "About";
            this.About.UseVisualStyleBackColor = true;
            this.About.Click += new System.EventHandler(this.About_Click);
            // 
            // CPU_Name
            // 
            this.CPU_Name.Location = new System.Drawing.Point(65, 12);
            this.CPU_Name.Name = "CPU_Name";
            this.CPU_Name.ReadOnly = true;
            this.CPU_Name.Size = new System.Drawing.Size(200, 20);
            this.CPU_Name.TabIndex = 7;
            // 
            // GPU_Name
            // 
            this.GPU_Name.Location = new System.Drawing.Point(65, 37);
            this.GPU_Name.Name = "GPU_Name";
            this.GPU_Name.ReadOnly = true;
            this.GPU_Name.Size = new System.Drawing.Size(200, 20);
            this.GPU_Name.TabIndex = 8;
            this.GPU_Name.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // temp_CPU
            // 
            this.temp_CPU.AutoSize = true;
            this.temp_CPU.Location = new System.Drawing.Point(274, 15);
            this.temp_CPU.Name = "temp_CPU";
            this.temp_CPU.Size = new System.Drawing.Size(45, 13);
            this.temp_CPU.TabIndex = 9;
            this.temp_CPU.Text = "Thermal";
            // 
            // temp_GPU
            // 
            this.temp_GPU.AutoSize = true;
            this.temp_GPU.Location = new System.Drawing.Point(274, 40);
            this.temp_GPU.Name = "temp_GPU";
            this.temp_GPU.Size = new System.Drawing.Size(45, 13);
            this.temp_GPU.TabIndex = 10;
            this.temp_GPU.Text = "Thermal";
            // 
            // CPU_temp
            // 
            this.CPU_temp.Location = new System.Drawing.Point(325, 12);
            this.CPU_temp.Name = "CPU_temp";
            this.CPU_temp.ReadOnly = true;
            this.CPU_temp.Size = new System.Drawing.Size(40, 20);
            this.CPU_temp.TabIndex = 11;
            this.CPU_temp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.CPU_temp.TextChanged += new System.EventHandler(this.CPU_temp_TextChanged);
            // 
            // GPU_temp
            // 
            this.GPU_temp.Location = new System.Drawing.Point(326, 37);
            this.GPU_temp.Name = "GPU_temp";
            this.GPU_temp.ReadOnly = true;
            this.GPU_temp.Size = new System.Drawing.Size(40, 20);
            this.GPU_temp.TabIndex = 12;
            this.GPU_temp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.GPU_temp.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // CPUButton
            // 
            this.CPUButton.AutoSize = true;
            this.CPUButton.Location = new System.Drawing.Point(12, 13);
            this.CPUButton.Name = "CPUButton";
            this.CPUButton.Size = new System.Drawing.Size(50, 17);
            this.CPUButton.TabIndex = 17;
            this.CPUButton.TabStop = true;
            this.CPUButton.Text = "CPU:";
            this.CPUButton.UseVisualStyleBackColor = true;
            this.CPUButton.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // GPUButton
            // 
            this.GPUButton.AutoSize = true;
            this.GPUButton.Location = new System.Drawing.Point(12, 38);
            this.GPUButton.Name = "GPUButton";
            this.GPUButton.Size = new System.Drawing.Size(51, 17);
            this.GPUButton.TabIndex = 18;
            this.GPUButton.TabStop = true;
            this.GPUButton.Tag = "GPU";
            this.GPUButton.Text = "GPU:";
            this.GPUButton.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "SyncLED™";
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // MinimizeSystemTray
            // 
            this.MinimizeSystemTray.AutoSize = true;
            this.MinimizeSystemTray.Location = new System.Drawing.Point(12, 102);
            this.MinimizeSystemTray.Name = "MinimizeSystemTray";
            this.MinimizeSystemTray.Size = new System.Drawing.Size(135, 17);
            this.MinimizeSystemTray.TabIndex = 1;
            this.MinimizeSystemTray.Text = "Minimize to System tray";
            this.MinimizeSystemTray.UseVisualStyleBackColor = true;
            // 
            // AutoSync
            // 
            this.AutoSync.Location = new System.Drawing.Point(304, 69);
            this.AutoSync.Name = "AutoSync";
            this.AutoSync.Size = new System.Drawing.Size(64, 23);
            this.AutoSync.TabIndex = 20;
            this.AutoSync.Text = "AutoSync";
            this.AutoSync.UseVisualStyleBackColor = true;
            this.AutoSync.Click += new System.EventHandler(this.AutoSync_Click);
            // 
            // connectionStatus
            // 
            this.connectionStatus.AutoSize = true;
            this.connectionStatus.Location = new System.Drawing.Point(62, 74);
            this.connectionStatus.Name = "connectionStatus";
            this.connectionStatus.Size = new System.Drawing.Size(138, 13);
            this.connectionStatus.TabIndex = 22;
            this.connectionStatus.Text = "Not connected to SyncLED";
            this.connectionStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Progress
            // 
            this.Progress.AutoSize = true;
            this.Progress.Location = new System.Drawing.Point(22, 74);
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(40, 13);
            this.Progress.TabIndex = 26;
            this.Progress.Text = "Status:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 133);
            this.Controls.Add(this.Progress);
            this.Controls.Add(this.connectionStatus);
            this.Controls.Add(this.AutoSync);
            this.Controls.Add(this.GPUButton);
            this.Controls.Add(this.CPUButton);
            this.Controls.Add(this.GPU_temp);
            this.Controls.Add(this.CPU_temp);
            this.Controls.Add(this.temp_GPU);
            this.Controls.Add(this.temp_CPU);
            this.Controls.Add(this.GPU_Name);
            this.Controls.Add(this.CPU_Name);
            this.Controls.Add(this.About);
            this.Controls.Add(this.MinimizeSystemTray);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SyncLED™";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing_1);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button About;
        private System.Windows.Forms.TextBox CPU_Name;
        private System.Windows.Forms.TextBox GPU_Name;
        private System.Windows.Forms.Label temp_CPU;
        private System.Windows.Forms.Label temp_GPU;
        private System.Windows.Forms.TextBox GPU_temp;
        private System.Windows.Forms.RadioButton CPUButton;
        private System.Windows.Forms.RadioButton GPUButton;
        private System.Windows.Forms.Timer timer1;
        internal System.Windows.Forms.TextBox CPU_temp;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.CheckBox MinimizeSystemTray;
        private System.Windows.Forms.Button AutoSync;
        private System.Windows.Forms.Label connectionStatus;
        private System.Windows.Forms.Label Progress;
    }
}

