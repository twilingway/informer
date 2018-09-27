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
            this.tbRigName = new System.Windows.Forms.TextBox();
            this.labelRigName = new System.Windows.Forms.Label();
            this.btStart = new System.Windows.Forms.Button();
            this.btStop = new System.Windows.Forms.Button();
            this.InfoLabel = new System.Windows.Forms.Label();
            this.labelTimeWork = new System.Windows.Forms.Label();
            this.linkLabelUpdate = new System.Windows.Forms.LinkLabel();
            this.GetTempretureTimer = new System.Windows.Forms.Timer(this.components);
            this.NextAutoStart = new System.Windows.Forms.Timer(this.components);
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
            this.GPUFanMinTimer = new System.Windows.Forms.Timer(this.components);
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
            this.labelClockMin = new System.Windows.Forms.Label();
            this.labelMemoryMin = new System.Windows.Forms.Label();
            this.labelStatusClockMin = new System.Windows.Forms.Label();
            this.labelCounterClockMin = new System.Windows.Forms.Label();
            this.labelStatusMemoryMin = new System.Windows.Forms.Label();
            this.labelCounterMemoryMin = new System.Windows.Forms.Label();
            this.DontHaveInternetTimer = new System.Windows.Forms.Timer(this.components);
            this.FellOffGPUTimer = new System.Windows.Forms.Timer(this.components);
            this.labelFellOffGPU = new System.Windows.Forms.Label();
            this.labelStatusGPULost = new System.Windows.Forms.Label();
            this.labelCounterGPULost = new System.Windows.Forms.Label();
            this.labelInternet = new System.Windows.Forms.Label();
            this.labelStatusInternet = new System.Windows.Forms.Label();
            this.labelCounterInternet = new System.Windows.Forms.Label();
            this.labelCounterLoadMin = new System.Windows.Forms.Label();
            this.labelStatusLoadMin = new System.Windows.Forms.Label();
            this.labelLoadMin = new System.Windows.Forms.Label();
            this.GPULoadMinTimer = new System.Windows.Forms.Timer(this.components);
            this.labelTest = new System.Windows.Forms.Label();
            this.GetEWBF_ZcashTimer = new System.Windows.Forms.Timer(this.components);
            this.labelToken = new System.Windows.Forms.Label();
            this.tbToken = new System.Windows.Forms.TextBox();
            this.btExit = new System.Windows.Forms.Button();
            this.cbLocalize = new System.Windows.Forms.ComboBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.GPUCoreMaxTimer = new System.Windows.Forms.Timer(this.components);
            this.GPUMemMaxTimer = new System.Windows.Forms.Timer(this.components);
            this.labelClockMax = new System.Windows.Forms.Label();
            this.labelLoadMax = new System.Windows.Forms.Label();
            this.labelStatusMemoryMax = new System.Windows.Forms.Label();
            this.labelMemoryMax = new System.Windows.Forms.Label();
            this.labelStatusClockMax = new System.Windows.Forms.Label();
            this.labelStatusLoadMax = new System.Windows.Forms.Label();
            this.labelCounterMemoryMax = new System.Windows.Forms.Label();
            this.labelCounterClockMax = new System.Windows.Forms.Label();
            this.labelCounterLoadMax = new System.Windows.Forms.Label();
            this.GPULoadMaxTimer = new System.Windows.Forms.Timer(this.components);
            this.PingTimer = new System.Windows.Forms.Timer(this.components);
            this.MqttConnectTimer = new System.Windows.Forms.Timer(this.components);
            this.OHMTimer = new System.Windows.Forms.Timer(this.components);
            this.GPUStatusTimer = new System.Windows.Forms.Timer(this.components);
            this.labelListGPU = new System.Windows.Forms.Label();
            this.labelTestGPU = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbRigName
            // 
            resources.ApplyResources(this.tbRigName, "tbRigName");
            this.tbRigName.Name = "tbRigName";
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
            this.GetTempretureTimer.Interval = 5000;
            this.GetTempretureTimer.Tag = "get temp";
            this.GetTempretureTimer.Tick += new System.EventHandler(this.GetTempretureTimerTick);
            // 
            // NextAutoStart
            // 
            this.NextAutoStart.Interval = 60000;
            this.NextAutoStart.Tag = "active 1 timer";
            this.NextAutoStart.Tick += new System.EventHandler(this.NextAutoStart_Tick);
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
            this.labelStatusTempMax.Click += new System.EventHandler(this.GPUMemoryMaxHzTimer_Tick);
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
            // GPUFanMinTimer
            // 
            this.GPUFanMinTimer.Interval = 1000;
            this.GPUFanMinTimer.Tag = "fan_min";
            this.GPUFanMinTimer.Tick += new System.EventHandler(this.FanMinTimerTick);
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
            this.SendDataTimer.Interval = 1000;
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
            // labelClockMin
            // 
            resources.ApplyResources(this.labelClockMin, "labelClockMin");
            this.labelClockMin.Name = "labelClockMin";
            // 
            // labelMemoryMin
            // 
            resources.ApplyResources(this.labelMemoryMin, "labelMemoryMin");
            this.labelMemoryMin.Name = "labelMemoryMin";
            // 
            // labelStatusClockMin
            // 
            resources.ApplyResources(this.labelStatusClockMin, "labelStatusClockMin");
            this.labelStatusClockMin.Name = "labelStatusClockMin";
            // 
            // labelCounterClockMin
            // 
            resources.ApplyResources(this.labelCounterClockMin, "labelCounterClockMin");
            this.labelCounterClockMin.Name = "labelCounterClockMin";
            // 
            // labelStatusMemoryMin
            // 
            resources.ApplyResources(this.labelStatusMemoryMin, "labelStatusMemoryMin");
            this.labelStatusMemoryMin.Name = "labelStatusMemoryMin";
            // 
            // labelCounterMemoryMin
            // 
            resources.ApplyResources(this.labelCounterMemoryMin, "labelCounterMemoryMin");
            this.labelCounterMemoryMin.Name = "labelCounterMemoryMin";
            // 
            // DontHaveInternetTimer
            // 
            this.DontHaveInternetTimer.Interval = 1000;
            this.DontHaveInternetTimer.Tag = "internet_r";
            this.DontHaveInternetTimer.Tick += new System.EventHandler(this.InternetInactiveTimerTick);
            // 
            // FellOffGPUTimer
            // 
            this.FellOffGPUTimer.Interval = 1000;
            this.FellOffGPUTimer.Tag = "card leave";
            this.FellOffGPUTimer.Tick += new System.EventHandler(this.FellOffTimerTick);
            // 
            // labelFellOffGPU
            // 
            resources.ApplyResources(this.labelFellOffGPU, "labelFellOffGPU");
            this.labelFellOffGPU.Name = "labelFellOffGPU";
            // 
            // labelStatusGPULost
            // 
            resources.ApplyResources(this.labelStatusGPULost, "labelStatusGPULost");
            this.labelStatusGPULost.Name = "labelStatusGPULost";
            // 
            // labelCounterGPULost
            // 
            resources.ApplyResources(this.labelCounterGPULost, "labelCounterGPULost");
            this.labelCounterGPULost.Name = "labelCounterGPULost";
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
            // labelCounterInternet
            // 
            resources.ApplyResources(this.labelCounterInternet, "labelCounterInternet");
            this.labelCounterInternet.Name = "labelCounterInternet";
            // 
            // labelCounterLoadMin
            // 
            resources.ApplyResources(this.labelCounterLoadMin, "labelCounterLoadMin");
            this.labelCounterLoadMin.Name = "labelCounterLoadMin";
            // 
            // labelStatusLoadMin
            // 
            resources.ApplyResources(this.labelStatusLoadMin, "labelStatusLoadMin");
            this.labelStatusLoadMin.Name = "labelStatusLoadMin";
            // 
            // labelLoadMin
            // 
            resources.ApplyResources(this.labelLoadMin, "labelLoadMin");
            this.labelLoadMin.Name = "labelLoadMin";
            // 
            // GPULoadMinTimer
            // 
            this.GPULoadMinTimer.Interval = 1000;
            this.GPULoadMinTimer.Tag = "load";
            this.GPULoadMinTimer.Tick += new System.EventHandler(this.GPULoadMin_Tick);
            // 
            // labelTest
            // 
            resources.ApplyResources(this.labelTest, "labelTest");
            this.labelTest.Name = "labelTest";
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
            // GPUCoreMaxTimer
            // 
            this.GPUCoreMaxTimer.Interval = 1000;
            this.GPUCoreMaxTimer.Tag = "clock";
            this.GPUCoreMaxTimer.Tick += new System.EventHandler(this.GpuCoreMaxHzTimerTick);
            // 
            // GPUMemMaxTimer
            // 
            this.GPUMemMaxTimer.Interval = 1000;
            this.GPUMemMaxTimer.Tag = "memory";
            this.GPUMemMaxTimer.Tick += new System.EventHandler(this.GPUMemoryMaxHzTimer_Tick);
            // 
            // labelClockMax
            // 
            resources.ApplyResources(this.labelClockMax, "labelClockMax");
            this.labelClockMax.Name = "labelClockMax";
            // 
            // labelLoadMax
            // 
            resources.ApplyResources(this.labelLoadMax, "labelLoadMax");
            this.labelLoadMax.Name = "labelLoadMax";
            // 
            // labelStatusMemoryMax
            // 
            resources.ApplyResources(this.labelStatusMemoryMax, "labelStatusMemoryMax");
            this.labelStatusMemoryMax.Name = "labelStatusMemoryMax";
            // 
            // labelMemoryMax
            // 
            resources.ApplyResources(this.labelMemoryMax, "labelMemoryMax");
            this.labelMemoryMax.Name = "labelMemoryMax";
            // 
            // labelStatusClockMax
            // 
            resources.ApplyResources(this.labelStatusClockMax, "labelStatusClockMax");
            this.labelStatusClockMax.Name = "labelStatusClockMax";
            // 
            // labelStatusLoadMax
            // 
            resources.ApplyResources(this.labelStatusLoadMax, "labelStatusLoadMax");
            this.labelStatusLoadMax.Name = "labelStatusLoadMax";
            // 
            // labelCounterMemoryMax
            // 
            resources.ApplyResources(this.labelCounterMemoryMax, "labelCounterMemoryMax");
            this.labelCounterMemoryMax.Name = "labelCounterMemoryMax";
            // 
            // labelCounterClockMax
            // 
            resources.ApplyResources(this.labelCounterClockMax, "labelCounterClockMax");
            this.labelCounterClockMax.Name = "labelCounterClockMax";
            // 
            // labelCounterLoadMax
            // 
            resources.ApplyResources(this.labelCounterLoadMax, "labelCounterLoadMax");
            this.labelCounterLoadMax.Name = "labelCounterLoadMax";
            // 
            // GPULoadMaxTimer
            // 
            this.GPULoadMaxTimer.Interval = 1000;
            this.GPULoadMaxTimer.Tag = "load";
            this.GPULoadMaxTimer.Tick += new System.EventHandler(this.GPULoadMaxTimer_Tick);
            // 
            // PingTimer
            // 
            this.PingTimer.Interval = 10000;
            this.PingTimer.Tick += new System.EventHandler(this.PingTimer_Tick);
            
            // 
            // MqttConnectTimer
            // 
            this.MqttConnectTimer.Interval = 5000;
            this.MqttConnectTimer.Tick += new System.EventHandler(this.MqttConnectTimer_Tick);
           
            // 
            // OHMTimer
            // 
            this.OHMTimer.Interval = 5000;
            this.OHMTimer.Tick += new System.EventHandler(this.OHMTimer_Tick);
            // 
            // GPUStatusTimer
            // 
            this.GPUStatusTimer.Interval = 1000;
            this.GPUStatusTimer.Tick += new System.EventHandler(this.GPUStatusTimer_Tick);
            // 
            // labelListGPU
            // 
            resources.ApplyResources(this.labelListGPU, "labelListGPU");
            this.labelListGPU.Name = "labelListGPU";
            // 
            // labelTestGPU
            // 
            resources.ApplyResources(this.labelTestGPU, "labelTestGPU");
            this.labelTestGPU.Name = "labelTestGPU";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelTestGPU);
            this.Controls.Add(this.labelListGPU);
            this.Controls.Add(this.labelCounterLoadMax);
            this.Controls.Add(this.labelCounterClockMax);
            this.Controls.Add(this.labelCounterMemoryMax);
            this.Controls.Add(this.labelStatusLoadMax);
            this.Controls.Add(this.labelStatusClockMax);
            this.Controls.Add(this.labelStatusMemoryMax);
            this.Controls.Add(this.labelMemoryMax);
            this.Controls.Add(this.labelLoadMax);
            this.Controls.Add(this.labelClockMax);
            this.Controls.Add(this.cbLocalize);
            this.Controls.Add(this.btExit);
            this.Controls.Add(this.tbToken);
            this.Controls.Add(this.labelToken);
            this.Controls.Add(this.labelTest);
            this.Controls.Add(this.labelCounterLoadMin);
            this.Controls.Add(this.labelStatusLoadMin);
            this.Controls.Add(this.labelLoadMin);
            this.Controls.Add(this.labelCounterInternet);
            this.Controls.Add(this.labelStatusInternet);
            this.Controls.Add(this.labelInternet);
            this.Controls.Add(this.labelCounterGPULost);
            this.Controls.Add(this.labelStatusGPULost);
            this.Controls.Add(this.labelFellOffGPU);
            this.Controls.Add(this.labelCounterMemoryMin);
            this.Controls.Add(this.labelStatusMemoryMin);
            this.Controls.Add(this.labelCounterClockMin);
            this.Controls.Add(this.labelStatusClockMin);
            this.Controls.Add(this.labelMemoryMin);
            this.Controls.Add(this.labelClockMin);
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
            this.Controls.Add(this.tbRigName);
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
        private System.Windows.Forms.TextBox tbRigName;
        private System.Windows.Forms.Label labelRigName;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.Label InfoLabel;
        private System.Windows.Forms.Label labelTimeWork;
        private System.Windows.Forms.LinkLabel linkLabelUpdate;
        private System.Windows.Forms.Timer GetTempretureTimer;
        private System.Windows.Forms.Timer NextAutoStart;
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
        private System.Windows.Forms.Timer GPUFanMinTimer;
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
        private System.Windows.Forms.Label labelClockMin;
        private System.Windows.Forms.Label labelMemoryMin;
        private System.Windows.Forms.Label labelStatusClockMin;
        private System.Windows.Forms.Label labelCounterClockMin;
        private System.Windows.Forms.Label labelStatusMemoryMin;
        private System.Windows.Forms.Label labelCounterMemoryMin;
        private System.Windows.Forms.Timer DontHaveInternetTimer;
        private System.Windows.Forms.Timer FellOffGPUTimer;
        private System.Windows.Forms.Label labelFellOffGPU;
        private System.Windows.Forms.Label labelStatusGPULost;
        private System.Windows.Forms.Label labelCounterGPULost;
        private System.Windows.Forms.Label labelInternet;
        private System.Windows.Forms.Label labelStatusInternet;
        private System.Windows.Forms.Label labelCounterInternet;
        private System.Windows.Forms.Label labelCounterLoadMin;
        private System.Windows.Forms.Label labelStatusLoadMin;
        private System.Windows.Forms.Label labelLoadMin;
        private System.Windows.Forms.Timer GPULoadMinTimer;
        private System.Windows.Forms.Label labelTest;
        private System.Windows.Forms.Timer GetEWBF_ZcashTimer;
        private System.Windows.Forms.Label labelToken;
        private System.Windows.Forms.TextBox tbToken;
        private System.Windows.Forms.Button btExit;
        private System.Windows.Forms.ComboBox cbLocalize;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Timer GPUCoreMaxTimer;
        private System.Windows.Forms.Timer GPUMemMaxTimer;
        private System.Windows.Forms.Label labelClockMax;
        private System.Windows.Forms.Label labelLoadMax;
        private System.Windows.Forms.Label labelStatusMemoryMax;
        private System.Windows.Forms.Label labelMemoryMax;
        private System.Windows.Forms.Label labelStatusClockMax;
        private System.Windows.Forms.Label labelStatusLoadMax;
        private System.Windows.Forms.Label labelCounterMemoryMax;
        private System.Windows.Forms.Label labelCounterClockMax;
        private System.Windows.Forms.Label labelCounterLoadMax;
        private System.Windows.Forms.Timer GPULoadMaxTimer;
        private System.Windows.Forms.Timer PingTimer;
        private System.Windows.Forms.Timer MqttConnectTimer;
        private System.Windows.Forms.Timer OHMTimer;
        private System.Windows.Forms.Timer GPUStatusTimer;
        private System.Windows.Forms.Label labelListGPU;
        private System.Windows.Forms.Label labelTestGPU;
    }
}

