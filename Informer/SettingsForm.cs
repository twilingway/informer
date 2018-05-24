using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Informer
{
    public partial class SettingsForm : Form
    {

       

        private INIManager manager;
        private LogFile _logs, _errors;

        public SettingsForm()
        {
            InitializeComponent();
            string fullPath = Application.StartupPath.ToString();
            manager = new INIManager(fullPath + "\\my.ini");
            _logs = new LogFile("log_settins");
            _errors = new LogFile("error_settings");
            ini();
        }

        private void ZeroingOut()
        {

            GlobalVars.timer_r_min = -100;
            GlobalVars.timer_t_min = -100;
            GlobalVars.timer_t_max = -100;
            GlobalVars.timer_fan_max = -100;
            GlobalVars.timer_fan_min = -100;
            GlobalVars.timer_clock = -100;
            GlobalVars.timer_memory = -100;
            GlobalVars.timer_inet = -100;
            GlobalVars.timer_t_card = -100;
            GlobalVars.timer_load_gpu = -100;

        }
       
        private void BtSaveClick(object sender, EventArgs e)
        {
            try
            {

                
                manager.WritePrivateString("main", "temp_max", tbTempMax.Text);
                GlobalVars.temp_max = Convert.ToInt32(tbTempMax.Text);
                manager.WritePrivateString("main", "time_temp_max", tbTempMaxSec.Text);
                GlobalVars.time_temp_max = Convert.ToInt32(tbTempMaxSec.Text);

                manager.WritePrivateString("main", "temp_min", tbTempMin.Text);
                GlobalVars.temp_min = Convert.ToInt32(tbTempMin.Text);
                manager.WritePrivateString("main", "time_temp_min", tbTempMinSec.Text);
                GlobalVars.time_temp_min = Convert.ToInt32(tbTempMinSec.Text);

                manager.WritePrivateString("main", "fan_max", tbFanMax.Text);
                GlobalVars.fan_max = Convert.ToInt32(tbFanMax.Text);
                manager.WritePrivateString("main", "time_fan_max", tbFanMaxSec.Text);
                GlobalVars.time_fan_max = Convert.ToInt32(tbFanMaxSec.Text);

                manager.WritePrivateString("main", "fan_min", tbFanMin.Text);
                GlobalVars.fan_min = Convert.ToInt32(tbFanMin.Text);
                manager.WritePrivateString("main", "time_fan_min", tbFanMinSec.Text);
                GlobalVars.time_fan_min = Convert.ToInt32(tbFanMinSec.Text);

                manager.WritePrivateString("main", "reload_temp_min_file", tbReloadTempFile.Text);
                GlobalVars.reload_temp_min_file = Convert.ToInt32(tbReloadTempFile.Text);
                manager.WritePrivateString("main", "reload_time_min_file", tbReloadFileSec.Text);
                GlobalVars.reload_time_min_file = Convert.ToInt32(tbReloadFileSec.Text);

                manager.WritePrivateString("main", "clock", tbClockMin.Text);
                GlobalVars.core_clock = Convert.ToInt32(tbClockMin.Text);
                manager.WritePrivateString("main", "time_clock", tbClockMinSec.Text);
                GlobalVars.time_clock_min = Convert.ToInt32(tbClockMinSec.Text);

                manager.WritePrivateString("main", "memory", tbMemoryMin.Text);
                GlobalVars.memory = Convert.ToInt32(tbMemoryMin.Text);
                manager.WritePrivateString("main", "time_memory", tbMemoryMinSec.Text);
                GlobalVars.time_mem_min = Convert.ToInt32(tbMemoryMinSec.Text);

                manager.WritePrivateString("main", "count_GPU", tbCountGPU.Text);
                GlobalVars.count_GPU = Convert.ToInt32(tbCountGPU.Text);
                manager.WritePrivateString("main", "time_count_GPU", tbCountGPUSec.Text);
                GlobalVars.time_count_GPU = Convert.ToInt32(tbCountGPUSec.Text);

                manager.WritePrivateString("main", "load_GPU", tbLoadGPU.Text);
                GlobalVars.load_GPU = Convert.ToInt32(tbLoadGPU.Text);
                manager.WritePrivateString("main", "time_load_GPU", tbLoadGPUSec.Text);
                GlobalVars.time_load_GPU_min = Convert.ToInt32(tbLoadGPUSec.Text);

                manager.WritePrivateString("main", "time_internet", tbInternetOffSec.Text);
                GlobalVars.time_lost_inet = Convert.ToInt32(tbInternetOffSec.Text);

                manager.WritePrivateString("main", "time_start", combTimeStart.Text);
                GlobalVars.time_start = Convert.ToInt32(combTimeStart.Text);

                

                ZeroingOut();

                this.Close();
            }
            catch (Exception ex)
            {
                _errors.writeLogLine(ex.Message, "error_settings");
                MessageBox.Show("Значения не могут быть пустыми");
            }
        }
        public void ini()
        {
            

            string reboot_temp_max = manager.GetPrivateString("main", "reboot_temp_max");
            string temp_max = manager.GetPrivateString("main", "temp_max");
            string time_temp_max = manager.GetPrivateString("main", "time_temp_max");

            string reboot_temp_min = manager.GetPrivateString("main", "reboot_temp_min");
            string temp_min = manager.GetPrivateString("main", "temp_min");
            string time_temp_min = manager.GetPrivateString("main", "time_temp_min");
            
            
            string reboot_max_fan = manager.GetPrivateString("main", "reboot_max_fan");
            string fan_max = manager.GetPrivateString("main", "fan_max");
            string time_fan_max = manager.GetPrivateString("main", "time_fan_max");

            string reboot_min_fan = manager.GetPrivateString("main", "reboot_min_fan");
            string fan_min = manager.GetPrivateString("main", "fan_min");
            string time_fan_min = manager.GetPrivateString("main", "time_fan_min");

            string reload_file = manager.GetPrivateString("main", "reload_file");
            string reload_temp_min_file = manager.GetPrivateString("main", "reload_temp_min_file");
            string reload_time_min_file = manager.GetPrivateString("main", "reload_time_min_file");
            string path = manager.GetPrivateString("main", "path");
            string path2 = manager.GetPrivateString("main", "path2");

            string reboot_clock = manager.GetPrivateString("main", "reboot_clock");
            string clock = manager.GetPrivateString("main", "clock");
            string time_clock = manager.GetPrivateString("main", "time_clock");

            string reboot_memory = manager.GetPrivateString("main", "reboot_memory");
            string memory = manager.GetPrivateString("main", "memory");
            string time_memory = manager.GetPrivateString("main", "time_memory");

            string reboot_GPU = manager.GetPrivateString("main", "reboot_GPU");
            string count_GPU = manager.GetPrivateString("main", "count_GPU");
            string time_count_GPU = manager.GetPrivateString("main", "time_count_GPU");

            string reboot_load_GPU = manager.GetPrivateString("main", "reboot_load_GPU");
            string load_GPU = manager.GetPrivateString("main", "load_GPU");
            string time_load_GPU = manager.GetPrivateString("main", "time_load_GPU");

            string reboot_internet = manager.GetPrivateString("main", "reboot_internet");
            string time_internet= manager.GetPrivateString("main", "time_internet");

            string time_start = manager.GetPrivateString("main", "time_start");
            //не используется
            string stat = manager.GetPrivateString("main", "stat");
            string pool = manager.GetPrivateString("main", "pool");
            string wallet = manager.GetPrivateString("main", "wallet");
            //
            
            
            if (reboot_temp_max == "1")
            {
                cbTempMaxGPU.Checked = true;
            }
            else
            {
                cbTempMaxGPU.Checked = false;
            }
           
            if (reboot_temp_min == "1")
            {
                cbTempMinGPU.Checked = true;
            }
            else
            {
                cbTempMinGPU.Checked = false;
            }

            if (reboot_max_fan == "1")
            {
                cbFanMaxGPU.Checked = true;
            }
            else
            {
                cbFanMaxGPU.Checked = false;
            }
            if (reboot_min_fan == "1")
            {
                cbFanMinGPU.Checked = true;
            }
            else
            {
                cbFanMinGPU.Checked = false;
            }

            if (reload_file == "1")
            {
                cbReloadFile.Checked = true;
            }
            else
            {
                cbReloadFile.Checked = false;
            }
            
            if (reboot_clock == "1")
            {
                cbClockMinGPU.Checked = true;
            }
            else
            {
                cbClockMinGPU.Checked = false;
            }
            if (reboot_memory == "1")
            {
                cbMemoryMinGPU.Checked = true;
            }
            else
            {
                cbMemoryMinGPU.Checked = false;
            }
            if (reboot_GPU == "1")
            {
                cbFellOffGPU.Checked = true;
            }
            else
            {
                cbFellOffGPU.Checked = false;
            }

            if (reboot_load_GPU == "1")
            {
                cbLoadGPU.Checked = true;
            }
            else
            {
                cbLoadGPU.Checked = false;
            }

            if (reboot_internet == "1")
            {
                cbInternetOff.Checked = true;
            }
            else
            {
                cbInternetOff.Checked = false;
            }
            
            tbTempMax.Text = temp_max;
            tbTempMaxSec.Text = time_temp_max;

            tbTempMin.Text = temp_min;
            tbTempMinSec.Text = time_temp_min;

            tbFanMax.Text = fan_max;
            tbFanMaxSec.Text = time_fan_max;

            tbFanMin.Text = fan_min;
            tbFanMinSec.Text = time_fan_min;

            tbReloadTempFile.Text = reload_temp_min_file;
            tbReloadFileSec.Text = reload_time_min_file;

            combTimeStart.Text = time_start;
            tbPath1.Text = path;
            tbPath2.Text = path2;
            
            tbClockMin.Text = clock;
            tbClockMinSec.Text = time_clock;

            tbMemoryMin.Text = memory;
            tbMemoryMinSec.Text = time_memory;

            tbCountGPU.Text = count_GPU;
            tbCountGPUSec.Text = time_count_GPU;

            tbLoadGPU.Text = load_GPU;
            tbLoadGPUSec.Text = time_load_GPU;

            tbInternetOffSec.Text = time_internet;
            
        }
        
        private void CheckBoxTempMax_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTempMaxGPU.Checked)
            {
                manager.WritePrivateString("main", "reboot_temp_max", "1");
                GlobalVars.reboot_temp_max = "1";
                ZeroingOut();
            }
            else
            {
                manager.WritePrivateString("main", "reboot_temp_max", "0");
                GlobalVars.reboot_temp_max = "0";
              
            }
        }

      
        private void CheckBoxTempMin_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTempMinGPU.Checked)
            {
              
                manager.WritePrivateString("main", "reboot_temp_min", "1");
                GlobalVars.reboot_temp_min = "1";
                ZeroingOut();
            }
            else
            {
             
                manager.WritePrivateString("main", "reboot_temp_min", "0");
                GlobalVars.reboot_temp_min = "0";
              
            }
        }



        

        private void CheckBoxFanMax_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFanMaxGPU.Checked)
            {
             
                manager.WritePrivateString("main", "reboot_max_fan", "1");
                GlobalVars.reboot_max_fan = "1";
                ZeroingOut();

            }
            else
            {
              
                manager.WritePrivateString("main", "reboot_max_fan", "0");
                GlobalVars.reboot_max_fan = "0";
              
            }
        }

        private void CheckBoxFanMin_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFanMinGPU.Checked)
            {

                manager.WritePrivateString("main", "reboot_min_fan", "1");
                GlobalVars.reboot_min_fan = "1";
                ZeroingOut();
            }
            else
            {

                manager.WritePrivateString("main", "reboot_min_fan", "0");
                GlobalVars.reboot_min_fan = "0";
               
            }
        }

        private void CheckBoxReloadFile_CheckedChanged(object sender, EventArgs e)
        {
            if (cbReloadFile.Checked)
            {

                manager.WritePrivateString("main", "reload_file", "1");
                GlobalVars.reload_file = "1";
                ZeroingOut();
            }
            else
            {
                manager.WritePrivateString("main", "reload_file", "0");
                GlobalVars.reload_file = "0";
               
            }
        }

        private void CheckBoxClockMin_CheckedChanged(object sender, EventArgs e)
        {
            if (cbClockMinGPU.Checked)
            {

                manager.WritePrivateString("main", "reboot_clock", "1");
                GlobalVars.reboot_clock = "1";
                ZeroingOut();
            }
            else
            {

                manager.WritePrivateString("main", "reboot_clock", "0");
                GlobalVars.reboot_clock = "0";
              
            }
        }

        private void CheckBoxMemory_CheckedChanged(object sender, EventArgs e)
        {
            if (cbMemoryMinGPU.Checked)
            {

                manager.WritePrivateString("main", "reboot_memory", "1");
                GlobalVars.reboot_memory = "1";
                ZeroingOut();
            }
            else
            {

                manager.WritePrivateString("main", "reboot_memory", "0");
                GlobalVars.reboot_memory = "0";
               
            }
        }

        

        private void cbLoadGPU_CheckedChanged(object sender, EventArgs e)
        {

            if (cbLoadGPU.Checked)
            {

                manager.WritePrivateString("main", "reboot_load_GPU", "1");
                GlobalVars.reboot_load_GPU = "1";
                ZeroingOut();
            }
            else
            {

                manager.WritePrivateString("main", "reboot_load_GPU", "0");
                GlobalVars.reboot_load_GPU = "0";
                ZeroingOut();
            }

        }

        private void CheckBoxFellOff_CheckedChanged(object sender, EventArgs e)
        {
            if (cbFellOffGPU.Checked)
            {

                manager.WritePrivateString("main", "reboot_GPU", "1");
                GlobalVars.reboot_GPU = "1";
                ZeroingOut();
            }
            else
            {

                manager.WritePrivateString("main", "reboot_GPU", "0");
                GlobalVars.reboot_GPU = "0";
            }
        }

        private void CheckBoxInternetOff_CheckedChanged(object sender, EventArgs e)
        {
            if (cbInternetOff.Checked)
            {

                manager.WritePrivateString("main", "reboot_internet", "1");
                GlobalVars.reboot_internet = "1";
                ZeroingOut();
            }
            else
            {

                manager.WritePrivateString("main", "reboot_internet", "0");
                GlobalVars.reboot_internet = "0";
            }
        }


        private void buttonView1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            string filename1 = openFileDialog1.SafeFileName;
            string dir = Path.GetDirectoryName(openFileDialog1.FileName);
            tbPath1.Text = filename;

            manager.WritePrivateString("main", "path", filename);
            manager.WritePrivateString("main", "filename", filename1);
            manager.WritePrivateString("main", "dir", dir);
            GlobalVars.dir = dir;
            GlobalVars.pathreload = filename;
            GlobalVars.filename = filename1;

            
            ZeroingOut();

        }

        private void buttonView2_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog2.FileName;
            string filename1 = openFileDialog2.SafeFileName;
            string dir = Path.GetDirectoryName(openFileDialog2.FileName);

            tbPath2.Text = filename;

            manager.WritePrivateString("main", "path2", filename);
            manager.WritePrivateString("main", "filename2", filename1);
            manager.WritePrivateString("main", "dir2", dir);
            GlobalVars.dir2 = dir;
            GlobalVars.pathreload2 = filename;
            GlobalVars.filename2 = filename1;

           
            ZeroingOut();
        }

        private void tbTempMax_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbTempMaxSec_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbTempMin_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbTempMinSec_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbFanMax_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbFanMaxSec_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbFanMin_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbFanMinSec_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbReloadTempFile_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbReloadFileSec_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbClockMin_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbClockMinSec_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbMemoryMin_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbMemoryMinSec_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbCountGPU_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbCountGPUSec_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbLoadGPU_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

        private void tbLoadGPUSec_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }

       
        private void combTimeStart_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            manager.WritePrivateString("main", "time_start", combTimeStart.GetItemText(combTimeStart.SelectedItem));
            int.TryParse(combTimeStart.GetItemText(combTimeStart.SelectedItem), out GlobalVars.time_start);
            ZeroingOut();
           
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void tbInternetOffSec_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ввод в texBox только цифр и кнопки Backspace
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }
        
    }
}
