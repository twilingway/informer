namespace Informer
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tbEmail = new System.Windows.Forms.TextBox();
            this.tbSecret = new System.Windows.Forms.TextBox();
            this.tbRigName = new System.Windows.Forms.TextBox();
            this.labelEmail = new System.Windows.Forms.Label();
            this.labelSecret = new System.Windows.Forms.Label();
            this.labelRigName = new System.Windows.Forms.Label();
            this.btStart = new System.Windows.Forms.Button();
            this.btStop = new System.Windows.Forms.Button();
            this.InfoLabel = new System.Windows.Forms.Label();
            this.labelTimeWork = new System.Windows.Forms.Label();
            this.linkLabelUpdate = new System.Windows.Forms.LinkLabel();
            this.GetTempretureTimer = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.AutoStartTimer = new System.Windows.Forms.Timer(this.components);
            this.CheckNewVersionTimer = new System.Windows.Forms.Timer(this.components);
            this.TimeWorkTimer = new System.Windows.Forms.Timer(this.components);
            this.btSettings = new System.Windows.Forms.Button();
            this.GPUTempMaxTimer = new System.Windows.Forms.Timer(this.components);
            this.GPUTempMinTimer = new System.Windows.Forms.Timer(this.components);
            this.ReloadMinerTimer = new System.Windows.Forms.Timer(this.components);
            this.InformationLabel = new System.Windows.Forms.Label();
            this.labelStatusTempMax = new System.Windows.Forms.Label();
            this.labelTimeWork2 = new System.Windows.Forms.Label();
            this.GPUFanMaxTimer = new System.Windows.Forms.Timer(this.components);
            this.GPUFanMinTImer = new System.Windows.Forms.Timer(this.components);
            this.labelCounterTempMax = new System.Windows.Forms.Label();
            this.labelTempMax = new System.Windows.Forms.Label();
            this.SendDataTimer = new System.Windows.Forms.Timer(this.components);
            this.labelFanMax = new System.Windows.Forms.Label();
            this.labelStatusTempMin = new System.Windows.Forms.Label();
            this.labelCounterTempMin = new System.Windows.Forms.Label();
            this.labelTempMin = new System.Windows.Forms.Label();
            this.labelReloadFile = new System.Windows.Forms.Label();
            this.labelStatusReloadFile = new System.Windows.Forms.Label();
            this.labelCounterReloadFile = new System.Windows.Forms.Label();
            this.labelFanMin = new System.Windows.Forms.Label();
            this.labelStatusFanMax = new System.Windows.Forms.Label();
            this.labelCounterFanMax = new System.Windows.Forms.Label();
            this.labelStatusFanMin = new System.Windows.Forms.Label();
            this.labelCounterFanMin = new System.Windows.Forms.Label();
            this.GPUCoreMinTimer = new System.Windows.Forms.Timer(this.components);
            this.GPUMemMinTimer = new System.Windows.Forms.Timer(this.components);
            this.labelClock = new System.Windows.Forms.Label();
            this.labelMemory = new System.Windows.Forms.Label();
            this.labelStatusClock = new System.Windows.Forms.Label();
            this.labelCounterClock = new System.Windows.Forms.Label();
            this.labelStatusMemory = new System.Windows.Forms.Label();
            this.labelCounterMemory = new System.Windows.Forms.Label();
            this.PingInternetTimer = new System.Windows.Forms.Timer(this.components);
            this.labelInternetPing = new System.Windows.Forms.Label();
            this.labelStatusInternetPing = new System.Windows.Forms.Label();
            this.DontHaveInternetTimer = new System.Windows.Forms.Timer(this.components);
            this.FellOffGPUTimer = new System.Windows.Forms.Timer(this.components);
            this.labelFellOffGPU = new System.Windows.Forms.Label();
            this.labelStatusFellOffGPU = new System.Windows.Forms.Label();
            this.labelCounerFellOff = new System.Windows.Forms.Label();
            this.labelInternet = new System.Windows.Forms.Label();
            this.labelStatusInternet = new System.Windows.Forms.Label();
            this.label3CounterInternet = new System.Windows.Forms.Label();
            this.labelCounterLoadGPU = new System.Windows.Forms.Label();
            this.labelStatusLoadGPU = new System.Windows.Forms.Label();
            this.labelLoadGPU = new System.Windows.Forms.Label();
            this.GPULoadMin = new System.Windows.Forms.Timer(this.components);
            this.labelTest = new System.Windows.Forms.Label();
            this.GetEWBF_ZcashTimer = new System.Windows.Forms.Timer(this.components);
            this.labelToken = new System.Windows.Forms.Label();
            this.tbToken = new System.Windows.Forms.TextBox();
            this.btExit = new System.Windows.Forms.Button();
            this.cbLocalize = new System.Windows.Forms.ComboBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // tbEmail
            // 
            resources.ApplyResources(this.tbEmail, "tbEmail");
            this.tbEmail.Name = "tbEmail";
            // 
            // tbSecret
            // 
            resources.ApplyResources(this.tbSecret, "tbSecret");
            this.tbSecret.Name = "tbSecret";
            // 
            // tbRigName
            // 
            resources.ApplyResources(this.tbRigName, "tbRigName");
            this.tbRigName.Name = "tbRigName";
            this.tbRigName.TextChanged += new System.EventHandler(this.tbRigName_TextChanged);
            // 
            // labelEmail
            // 
            resources.ApplyResources(this.labelEmail, "labelEmail");
            this.labelEmail.Name = "labelEmail";
            // 
            // labelSecret
            // 
            resources.ApplyResources(this.labelSecret, "labelSecret");
            this.labelSecret.Name = "labelSecret";
            // 
            // labelRigName
            // 
            resources.ApplyResources(this.labelRigName, "labelRigName");
            this.labelRigName.Name = "labelRigName";
            // 
            // btStart
            // 
            resources.ApplyResources(this.btStart, "btStart");
            this.btStart.Name = "btStart";
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.BtStartClick);
            // 
            // btStop
            // 
            resources.ApplyResources(this.btStop, "btStop");
            this.btStop.Name = "btStop";
            this.btStop.UseVisualStyleBackColor = true;
            this.btStop.Click += new System.EventHandler(this.BtStopClick);
            // 
            // InfoLabel
            // 
            resources.ApplyResources(this.InfoLabel, "InfoLabel");
            this.InfoLabel.Name = "InfoLabel";
            // 
            // labelTimeWork
            // 
            resources.ApplyResources(this.labelTimeWork, "labelTimeWork");
            this.labelTimeWork.Name = "labelTimeWork";
            // 
            // linkLabelUpdate
            // 
            resources.ApplyResources(this.linkLabelUpdate, "linkLabelUpdate");
            this.linkLabelUpdate.Name = "linkLabelUpdate";
            this.linkLabelUpdate.TabStop = true;
            this.linkLabelUpdate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // GetTempretureTimer
            // 
            this.GetTempretureTimer.Interval = 1000;
            this.GetTempretureTimer.Tag = "get temp";
            this.GetTempretureTimer.Tick += new System.EventHandler(this.GetTempretureTimerTick);
            // 
            // timer2
            // 
            this.timer2.Interval = 300000;
            this.timer2.Tag = "active 1 timer";
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // AutoStartTimer
            // 
            this.AutoStartTimer.Interval = 1000;
            this.AutoStartTimer.Tag = "reverse time";
            this.AutoStartTimer.Tick += new System.EventHandler(this.AutoStart_Tick);
            // 
            // CheckNewVersionTimer
            // 
            this.CheckNewVersionTimer.Enabled = true;
            this.CheckNewVersionTimer.Interval = 600000;
            this.CheckNewVersionTimer.Tag = "update";
            this.CheckNewVersionTimer.Tick += new System.EventHandler(this.CheckNewVersionTimerTick);
            // 
            // TimeWorkTimer
            // 
            this.TimeWorkTimer.Interval = 60000;
            this.TimeWorkTimer.Tag = "time active";
            this.TimeWorkTimer.Tick += new System.EventHandler(this.UptimeTimerTick);
            // 
            // btSettings
            // 
            resources.ApplyResources(this.btSettings, "btSettings");
            this.btSettings.Name = "btSettings";
            this.btSettings.UseVisualStyleBackColor = true;
            this.btSettings.Click += new System.EventHandler(this.BtnOpenSettingsFormClick);
            // 
            // GPUTempMaxTimer
            // 
            this.GPUTempMaxTimer.Interval = 1000;
            this.GPUTempMaxTimer.Tag = "t_max";
            this.GPUTempMaxTimer.Tick += new System.EventHandler(this.TempretureTimerTick);
            // 
            // GPUTempMinTimer
            // 
            this.GPUTempMinTimer.Interval = 1000;
            this.GPUTempMinTimer.Tag = "t_min";
            this.GPUTempMinTimer.Tick += new System.EventHandler(this.LowTempretureTimerTick);
            // 
            // ReloadMinerTimer
            // 
            this.ReloadMinerTimer.Interval = 1000;
            this.ReloadMinerTimer.Tag = "r_min";
            this.ReloadMinerTimer.Tick += new System.EventHandler(this.RerunTimerTick);
            // 
            // InformationLabel
            // 
            resources.ApplyResources(this.InformationLabel, "InformationLabel");
            this.InformationLabel.Name = "InformationLabel";
            // 
            // labelStatusTempMax
            // 
            resources.ApplyResources(this.labelStatusTempMax, "labelStatusTempMax");
            this.labelStatusTempMax.Name = "labelStatusTempMax";
            // 
            // labelTimeWork2
            // 
            resources.ApplyResources(this.labelTimeWork2, "labelTimeWork2");
            this.labelTimeWork2.Name = "labelTimeWork2";
            // 
            // GPUFanMaxTimer
            // 
            this.GPUFanMaxTimer.Interval = 1000;
            this.GPUFanMaxTimer.Tag = "fan_max";
            this.GPUFanMaxTimer.Tick += new System.EventHandler(this.FanMaxTimerTick);
            // 
            // GPUFanMinTImer
            // 
            this.GPUFanMinTImer.Interval = 1000;
            this.GPUFanMinTImer.Tag = "fan_min";
            this.GPUFanMinTImer.Tick += new System.EventHandler(this.FanMinTimerTick);
            // 
            // labelCounterTempMax
            // 
            resources.ApplyResources(this.labelCounterTempMax, "labelCounterTempMax");
            this.labelCounterTempMax.Name = "labelCounterTempMax";
            // 
            // labelTempMax
            // 
            resources.ApplyResources(this.labelTempMax, "labelTempMax");
            this.labelTempMax.Name = "labelTempMax";
            // 
            // SendDataTimer
            // 
            this.SendDataTimer.Interval = 60000;
            this.SendDataTimer.Tag = "send sait";
            this.SendDataTimer.Tick += new System.EventHandler(this.SendDataTimerTick);
            // 
            // labelFanMax
            // 
            resources.ApplyResources(this.labelFanMax, "labelFanMax");
            this.labelFanMax.Name = "labelFanMax";
            // 
            // labelStatusTempMin
            // 
            resources.ApplyResources(this.labelStatusTempMin, "labelStatusTempMin");
            this.labelStatusTempMin.Name = "labelStatusTempMin";
            // 
            // labelCounterTempMin
            // 
            resources.ApplyResources(this.labelCounterTempMin, "labelCounterTempMin");
            this.labelCounterTempMin.Name = "labelCounterTempMin";
            // 
            // labelTempMin
            // 
            resources.ApplyResources(this.labelTempMin, "labelTempMin");
            this.labelTempMin.Name = "labelTempMin";
            // 
            // labelReloadFile
            // 
            resources.ApplyResources(this.labelReloadFile, "labelReloadFile");
            this.labelReloadFile.Name = "labelReloadFile";
            // 
            // labelStatusReloadFile
            // 
            resources.ApplyResources(this.labelStatusReloadFile, "labelStatusReloadFile");
            this.labelStatusReloadFile.Name = "labelStatusReloadFile";
            // 
            // labelCounterReloadFile
            // 
            resources.ApplyResources(this.labelCounterReloadFile, "labelCounterReloadFile");
            this.labelCounterReloadFile.Name = "labelCounterReloadFile";
            // 
            // labelFanMin
            // 
            resources.ApplyResources(this.labelFanMin, "labelFanMin");
            this.labelFanMin.Name = "labelFanMin";
            // 
            // labelStatusFanMax
            // 
            resources.ApplyResources(this.labelStatusFanMax, "labelStatusFanMax");
            this.labelStatusFanMax.Name = "labelStatusFanMax";
            // 
            // labelCounterFanMax
            // 
            resources.ApplyResources(this.labelCounterFanMax, "labelCounterFanMax");
            this.labelCounterFanMax.Name = "labelCounterFanMax";
            // 
            // labelStatusFanMin
            // 
            resources.ApplyResources(this.labelStatusFanMin, "labelStatusFanMin");
            this.labelStatusFanMin.Name = "labelStatusFanMin";
            // 
            // labelCounterFanMin
            // 
            resources.ApplyResources(this.labelCounterFanMin, "labelCounterFanMin");
            this.labelCounterFanMin.Name = "labelCounterFanMin";
            // 
            // GPUCoreMinTimer
            // 
            this.GPUCoreMinTimer.Interval = 1000;
            this.GPUCoreMinTimer.Tag = "clock";
            this.GPUCoreMinTimer.Tick += new System.EventHandler(this.GpuCoreMinHzTimerTick);
            // 
            // GPUMemMinTimer
            // 
            this.GPUMemMinTimer.Interval = 1000;
            this.GPUMemMinTimer.Tag = "memory";
            this.GPUMemMinTimer.Tick += new System.EventHandler(this.GpuMemoryMinHzTimerTick);
            // 
            // labelClock
            // 
            resources.ApplyResources(this.labelClock, "labelClock");
            this.labelClock.Name = "labelClock";
            // 
            // labelMemory
            // 
            resources.ApplyResources(this.labelMemory, "labelMemory");
            this.labelMemory.Name = "labelMemory";
            // 
            // labelStatusClock
            // 
            resources.ApplyResources(this.labelStatusClock, "labelStatusClock");
            this.labelStatusClock.Name = "labelStatusClock";
            // 
            // labelCounterClock
            // 
            resources.ApplyResources(this.labelCounterClock, "labelCounterClock");
            this.labelCounterClock.Name = "labelCounterClock";
            // 
            // labelStatusMemory
            // 
            resources.ApplyResources(this.labelStatusMemory, "labelStatusMemory");
            this.labelStatusMemory.Name = "labelStatusMemory";
            // 
            // labelCounterMemory
            // 
            resources.ApplyResources(this.labelCounterMemory, "labelCounterMemory");
            this.labelCounterMemory.Name = "labelCounterMemory";
            // 
            // PingInternetTimer
            // 
            this.PingInternetTimer.Enabled = true;
            this.PingInternetTimer.Interval = 10000;
            this.PingInternetTimer.Tag = "internet";
            this.PingInternetTimer.Tick += new System.EventHandler(this.PingTimerTick);
            // 
            // labelInternetPing
            // 
            resources.ApplyResources(this.labelInternetPing, "labelInternetPing");
            this.labelInternetPing.Name = "labelInternetPing";
            // 
            // labelStatusInternetPing
            // 
            resources.ApplyResources(this.labelStatusInternetPing, "labelStatusInternetPing");
            this.labelStatusInternetPing.Name = "labelStatusInternetPing";
            // 
            // DontHaveInternetTimer
            // 
            this.DontHaveInternetTimer.Interval = 1000;
            this.DontHaveInternetTimer.Tag = "internet_r";
            this.DontHaveInternetTimer.Tick += new System.EventHandler(this.InternetInactiveTimerTick);
            // 
            // FellOffGPUTimer
            // 
            this.FellOffGPUTimer.Tag = "card leave";
            this.FellOffGPUTimer.Tick += new System.EventHandler(this.FellOffTimerTick);
            // 
            // labelFellOffGPU
            // 
            resources.ApplyResources(this.labelFellOffGPU, "labelFellOffGPU");
            this.labelFellOffGPU.Name = "labelFellOffGPU";
            // 
            // labelStatusFellOffGPU
            // 
            resources.ApplyResources(this.labelStatusFellOffGPU, "labelStatusFellOffGPU");
            this.labelStatusFellOffGPU.Name = "labelStatusFellOffGPU";
            // 
            // labelCounerFellOff
            // 
            resources.ApplyResources(this.labelCounerFellOff, "labelCounerFellOff");
            this.labelCounerFellOff.Name = "labelCounerFellOff";
            // 
            // labelInternet
            // 
            resources.ApplyResources(this.labelInternet, "labelInternet");
            this.labelInternet.Name = "labelInternet";
            // 
            // labelStatusInternet
            // 
            resources.ApplyResources(this.labelStatusInternet, "labelStatusInternet");
            this.labelStatusInternet.Name = "labelStatusInternet";
            // 
            // label3CounterInternet
            // 
            resources.ApplyResources(this.label3CounterInternet, "label3CounterInternet");
            this.label3CounterInternet.Name = "label3CounterInternet";
            // 
            // labelCounterLoadGPU
            // 
            resources.ApplyResources(this.labelCounterLoadGPU, "labelCounterLoadGPU");
            this.labelCounterLoadGPU.Name = "labelCounterLoadGPU";
            // 
            // labelStatusLoadGPU
            // 
            resources.ApplyResources(this.labelStatusLoadGPU, "labelStatusLoadGPU");
            this.labelStatusLoadGPU.Name = "labelStatusLoadGPU";
            // 
            // labelLoadGPU
            // 
            resources.ApplyResources(this.labelLoadGPU, "labelLoadGPU");
            this.labelLoadGPU.Name = "labelLoadGPU";
            // 
            // GPULoadMin
            // 
            this.GPULoadMin.Interval = 1000;
            this.GPULoadMin.Tag = "load";
            this.GPULoadMin.Tick += new System.EventHandler(this.GPULoadMin_Tick);
            // 
            // labelTest
            // 
            resources.ApplyResources(this.labelTest, "labelTest");
            this.labelTest.Name = "labelTest";
            // 
            // GetEWBF_ZcashTimer
            // 
            this.GetEWBF_ZcashTimer.Tick += new System.EventHandler(this.GetEWBF_ZcashTimer_Tick);
            // 
            // labelToken
            // 
            resources.ApplyResources(this.labelToken, "labelToken");
            this.labelToken.Name = "labelToken";
            // 
            // tbToken
            // 
            resources.ApplyResources(this.tbToken, "tbToken");
            this.tbToken.Name = "tbToken";
            this.tbToken.TextChanged += new System.EventHandler(this.tbToken_TextChanged);
            // 
            // btExit
            // 
            resources.ApplyResources(this.btExit, "btExit");
            this.btExit.Name = "btExit";
            this.btExit.UseVisualStyleBackColor = true;
            this.btExit.Click += new System.EventHandler(this.BtnExitClick);
            // 
            // cbLocalize
            // 
            this.cbLocalize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLocalize.FormattingEnabled = true;
            resources.ApplyResources(this.cbLocalize, "cbLocalize");
            this.cbLocalize.Name = "cbLocalize";
            // 
            // notifyIcon1
            // 
            resources.ApplyResources(this.notifyIcon1, "notifyIcon1");
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbLocalize);
            this.Controls.Add(this.btExit);
            this.Controls.Add(this.tbToken);
            this.Controls.Add(this.labelToken);
            this.Controls.Add(this.labelTest);
            this.Controls.Add(this.labelCounterLoadGPU);
            this.Controls.Add(this.labelStatusLoadGPU);
            this.Controls.Add(this.labelLoadGPU);
            this.Controls.Add(this.label3CounterInternet);
            this.Controls.Add(this.labelStatusInternet);
            this.Controls.Add(this.labelInternet);
            this.Controls.Add(this.labelCounerFellOff);
            this.Controls.Add(this.labelStatusFellOffGPU);
            this.Controls.Add(this.labelFellOffGPU);
            this.Controls.Add(this.labelStatusInternetPing);
            this.Controls.Add(this.labelInternetPing);
            this.Controls.Add(this.labelCounterMemory);
            this.Controls.Add(this.labelStatusMemory);
            this.Controls.Add(this.labelCounterClock);
            this.Controls.Add(this.labelStatusClock);
            this.Controls.Add(this.labelMemory);
            this.Controls.Add(this.labelClock);
            this.Controls.Add(this.labelCounterFanMin);
            this.Controls.Add(this.labelStatusFanMin);
            this.Controls.Add(this.labelCounterFanMax);
            this.Controls.Add(this.labelStatusFanMax);
            this.Controls.Add(this.labelFanMin);
            this.Controls.Add(this.labelCounterReloadFile);
            this.Controls.Add(this.labelStatusReloadFile);
            this.Controls.Add(this.labelReloadFile);
            this.Controls.Add(this.labelTempMin);
            this.Controls.Add(this.labelCounterTempMin);
            this.Controls.Add(this.labelStatusTempMin);
            this.Controls.Add(this.labelFanMax);
            this.Controls.Add(this.labelTempMax);
            this.Controls.Add(this.labelCounterTempMax);
            this.Controls.Add(this.labelTimeWork2);
            this.Controls.Add(this.labelStatusTempMax);
            this.Controls.Add(this.InformationLabel);
            this.Controls.Add(this.btSettings);
            this.Controls.Add(this.linkLabelUpdate);
            this.Controls.Add(this.labelTimeWork);
            this.Controls.Add(this.InfoLabel);
            this.Controls.Add(this.btStop);
            this.Controls.Add(this.btStart);
            this.Controls.Add(this.labelRigName);
            this.Controls.Add(this.labelSecret);
            this.Controls.Add(this.labelEmail);
            this.Controls.Add(this.tbRigName);
            this.Controls.Add(this.tbSecret);
            this.Controls.Add(this.tbEmail);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbEmail;
        private System.Windows.Forms.TextBox tbSecret;
        private System.Windows.Forms.TextBox tbRigName;
        private System.Windows.Forms.Label labelEmail;
        private System.Windows.Forms.Label labelSecret;
        private System.Windows.Forms.Label labelRigName;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.Label InfoLabel;
        private System.Windows.Forms.Label labelTimeWork;
        private System.Windows.Forms.LinkLabel linkLabelUpdate;
        private System.Windows.Forms.Timer GetTempretureTimer;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Timer AutoStartTimer;
        private System.Windows.Forms.Timer CheckNewVersionTimer;
        private System.Windows.Forms.Timer TimeWorkTimer;
        private System.Windows.Forms.Button btSettings;
        private System.Windows.Forms.Timer GPUTempMaxTimer;
        private System.Windows.Forms.Timer GPUTempMinTimer;
        private System.Windows.Forms.Timer ReloadMinerTimer;
        private System.Windows.Forms.Label InformationLabel;
        private System.Windows.Forms.Label labelStatusTempMax;
        private System.Windows.Forms.Label labelTimeWork2;
        private System.Windows.Forms.Timer GPUFanMaxTimer;
        private System.Windows.Forms.Timer GPUFanMinTImer;
        private System.Windows.Forms.Label labelCounterTempMax;
        private System.Windows.Forms.Label labelTempMax;
        private System.Windows.Forms.Timer SendDataTimer;
        private System.Windows.Forms.Label labelFanMax;
        private System.Windows.Forms.Label labelStatusTempMin;
        private System.Windows.Forms.Label labelCounterTempMin;
        private System.Windows.Forms.Label labelTempMin;
        private System.Windows.Forms.Label labelReloadFile;
        private System.Windows.Forms.Label labelStatusReloadFile;
        private System.Windows.Forms.Label labelCounterReloadFile;
        private System.Windows.Forms.Label labelFanMin;
        private System.Windows.Forms.Label labelStatusFanMax;
        private System.Windows.Forms.Label labelCounterFanMax;
        private System.Windows.Forms.Label labelStatusFanMin;
        private System.Windows.Forms.Label labelCounterFanMin;
        private System.Windows.Forms.Timer GPUCoreMinTimer;
        private System.Windows.Forms.Timer GPUMemMinTimer;
        private System.Windows.Forms.Label labelClock;
        private System.Windows.Forms.Label labelMemory;
        private System.Windows.Forms.Label labelStatusClock;
        private System.Windows.Forms.Label labelCounterClock;
        private System.Windows.Forms.Label labelStatusMemory;
        private System.Windows.Forms.Label labelCounterMemory;
        private System.Windows.Forms.Timer PingInternetTimer;
        private System.Windows.Forms.Label labelInternetPing;
        private System.Windows.Forms.Label labelStatusInternetPing;
        private System.Windows.Forms.Timer DontHaveInternetTimer;
        private System.Windows.Forms.Timer FellOffGPUTimer;
        private System.Windows.Forms.Label labelFellOffGPU;
        private System.Windows.Forms.Label labelStatusFellOffGPU;
        private System.Windows.Forms.Label labelCounerFellOff;
        private System.Windows.Forms.Label labelInternet;
        private System.Windows.Forms.Label labelStatusInternet;
        private System.Windows.Forms.Label label3CounterInternet;
        private System.Windows.Forms.Label labelCounterLoadGPU;
        private System.Windows.Forms.Label labelStatusLoadGPU;
        private System.Windows.Forms.Label labelLoadGPU;
        private System.Windows.Forms.Timer GPULoadMin;
        private System.Windows.Forms.Label labelTest;
        private System.Windows.Forms.Timer GetEWBF_ZcashTimer;
        private System.Windows.Forms.Label labelToken;
        private System.Windows.Forms.TextBox tbToken;
        private System.Windows.Forms.Button btExit;
        private System.Windows.Forms.ComboBox cbLocalize;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
    }
}

