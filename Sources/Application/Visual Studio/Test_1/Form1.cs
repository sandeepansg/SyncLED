using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Management;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Hardware;
using Microsoft.Win32;
using System.IO;
using System.Threading;
using System.Timers;


namespace Test_1
{

    public partial class Form1 : Form
    {
        Computer thisComputer;
        
        //RegistryKey reg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
       // string[] portnames = System.IO.Ports.SerialPort.GetPortNames();
        bool deviceFound = false;
        string temperature = null;
        SerialPort comPort = new SerialPort();
        int Temp_min = 21;   //Read from XML
        int Temp_MAX = 84;   //Read from XML
        string[] validPorts = new string[5];
        System.Timers.Timer tm = new System.Timers.Timer(700);
        
        int red = 0;
        int green = 0;
        int blue = 0;

        //int selection = 
        public Form1()
        {
            InitializeComponent();
            thisComputer = new Computer() { CPUEnabled = true, GPUEnabled = true };
           
            thisComputer.Open();
        }


        private long map(long x, long in_min, long in_max, long out_min, long out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private long constrain(long y, long range_min, long range_max)
        {
            if (y < range_min)
            { return range_min; }

            if (y > range_max)
            { return range_max; }
     
            return y;
        }
        
        //**********************************************  CPU NAME  **************************************************//
        private void getCPU_Name()
        {

            ManagementObjectSearcher mosProcessor = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (ManagementObject moProcessor in mosProcessor.Get())
            {
                if (moProcessor["name"] != null)
                    CPU_Name.Text = moProcessor["name"].ToString().Replace("(TM)", "™")
                   .Replace("(tm)", "™")
                   .Replace("(R)", "®")
                   .Replace("(r)", "®")
                   .Replace("(C)", "©")
                   .Replace("(c)", "©")
                   .Replace("    ", " ")
                   .Replace("  ", " "); ;
            }
        }
        //******************************  END  ******************************//


        //*******************************************  LIST ALL COMPORTS  ****************************************************//
        /*  private void list_SerialPorts()
          {
              try
              {
                  using (var searcher1 = new ManagementObjectSearcher
                      ("SELECT * FROM WIN32_SerialPort"))
                  {
                      string[] portnames = SerialPort.GetPortNames();
                      //string[] diffPorts = portnames.Except(oldPorts.ToArray());    // get the differences between currentPorts[] and oldPorts-list
                      var ports = searcher1.Get().Cast<ManagementBaseObject>().ToList();
                      var tList = (from n in portnames
                                   join p in ports on n equals p["DeviceID"].ToString()
                                   select p["Caption"]).ToList();
 
                      foreach (string s in tList)
                      {
                          //comboBox1.Items.Add(s);
                      }
 
                  }
              }
              catch (ManagementException em)
              {
                  MessageBox.Show("An error occurred: " + em.Message);
              }
          }
          //*******************  END  ************************
          */

        //**********************************************  GPU NAME  **************************************************//
        private void getGPU_Name()
        {
            ManagementObjectSearcher mosGPU = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");

            string graphicsCard = string.Empty;
            foreach (ManagementObject mo in mosGPU.Get())
            {
                foreach (PropertyData property in mo.Properties)
                {
                    if (property.Name == "Name")
                    {
                        graphicsCard = property.Value.ToString();
                    }
                }
            }
            GPU_Name.Text = graphicsCard.ToString();
        }



        private void Form1_Load(object sender, EventArgs e)
        {            
            getCPU_Name();
            getGPU_Name();
            //list_SerialPorts();
            radioButtonSelect();
            deviceFound = false;
            Thread.Sleep(100);
            /*string[] newPort = new string[255];
            newPort = System.IO.Ports.SerialPort.GetPortNames();
            SerialPort newPorts = new SerialPort();  
            foreach(string p in newPort )
            {       
                newPorts.PortName = p;
                //label4.Text = p.ToString();
                newPorts.Close();
            }*/
        }

        //**********************  GENERATE RANDOM NUMBER  ********************//
        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        //**********************  SUM TILL ONE DIGIT  ********************//
        private int sumTillOneDigit(Int32 num)
        {
            Int32 sum = 0;
            while (num > 0)
            {
                sum += num % 10;
                num /= 10;
            }
            if (sum > 9)
            {
                sum = sumTillOneDigit(sum);
            }
            return sum;
        }
        private Int32 generateRandomNumberInRange()
        {
            Int32 random = RandomNumber(0, 16777215);
            return random;
        }

        //********************** DATA FOR HANDSHAKING WITH ARDUIO   ********************//
        private string numberToHex(Int32 randomNumberData)
        {
            string hexData = randomNumberData.ToString("X");
            //randomNum.Text = hexData;
            string addZero = "";
            if (hexData.Length == 6)
                addZero = "";
            else if (hexData.Length == 5)
                addZero = "0";
            else if (hexData.Length == 4)
                addZero = "00";
            else if (hexData.Length == 3)
                addZero = "000";
            else if (hexData.Length == 2)
                addZero = "0000";
            else if (hexData.Length == 1)
                addZero = "00000";
            else
                addZero = "";
            string serialData = addZero + hexData;
            return serialData.ToString();
        }


        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }        

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void GPU_temp_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }


        private void CPU_temp_TextChanged(object sender, EventArgs e)
        {

        }

        private void CPU_Name_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void About_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private string TemperatureCPU()
        {
            String tempCPU = "";
            foreach (var hardwareItem in thisComputer.Hardware)
            {
                //*******************************  CPU Temperature  **********************************************
                if (hardwareItem.HardwareType == HardwareType.CPU)
                {
                    hardwareItem.Update();
                    foreach (IHardware subHardware in hardwareItem.SubHardware)
                        subHardware.Update();

                    foreach (var sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            if (sensor.Value.Value.ToString() != "")
                            {
                                tempCPU = String.Format(sensor.Value.Value.ToString());

                            } //return tempCPU;
                        }
                    }

                }
            }
            return tempCPU;
        }


        private string TemperatureGPU()
        {

            String tempGPU = "";
            foreach (var hardwareItem in thisComputer.Hardware)
            {

                //*******************************  GPU Temperature  **********************************************
                if (hardwareItem.HardwareType == HardwareType.GpuAti || hardwareItem.HardwareType == HardwareType.GpuNvidia)
                {
                    hardwareItem.Update();
                    foreach (IHardware subHardware in hardwareItem.SubHardware)
                        subHardware.Update();

                    foreach (var sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            if (sensor.Value.GetValueOrDefault().ToString() != "")
                            {
                                tempGPU = String.Format(sensor.Value.GetValueOrDefault().ToString());
                            }
                            //return tempGPU.ToString();

                        }
                    }
                }

            } return tempGPU;
        }

        private string rgbToHex(int r, int g, int b)
        {
            string Red = r.ToString("X");
            if (Red.Length == 1)
            {
                Red = "0" + Red;
            }
            string Green = g.ToString("X");
            if (Green.Length == 1)
            {
                Green = "0" + Green;
            }
            string Blue = b.ToString("X");
            if (Blue.Length == 1)
            {
                Blue = "0" + Blue;
            }
            string rgbHEX = Red.ToString() + Green.ToString() + Blue.ToString();
            return rgbHEX;
        }


        private string tempToRGBinHEX(string temp)
        {
            int temperature = Convert.ToInt32(temp);
            int constrainTemp = Convert.ToInt32(constrain(temperature, Temp_min, Temp_MAX));
            int new_PWM = Convert.ToInt32(map(constrainTemp, Temp_min, Temp_MAX, 0, 255));
            int new1_PWM;
            string hexValues;
            if (new_PWM >= 0 && new_PWM <= 127)
            {
                new1_PWM = Convert.ToInt32(map(new_PWM, 0, 127, 0, 255));
                blue = 255 - new1_PWM;
                green = new1_PWM;
                red = 0;
                hexValues = Convert.ToString(rgbToHex(red, green, blue));
                return hexValues;
            }
            else if (new_PWM >= 128 && new_PWM <= 255)
            {
                new1_PWM = Convert.ToInt32(map(new_PWM, 128, 255, 0, 255));
                red = new1_PWM;
                green = 255 - new1_PWM;
                blue = 0;                
                hexValues = Convert.ToString(rgbToHex(red, green, blue));
                return hexValues;
            }            

            return "";
        }

        private void sendData(string data)
        {
            try
            {
                if (comPort.IsOpen && deviceFound == true)
                {
                    int rgbCode_toNum = stringHexToNumber(data);
                    int rgbCode_UnitSum = sumTillOneDigit(rgbCode_toNum);
                    int randResp_Of_RGBCode = randResp(rgbCode_toNum);
                    string randResp_Of_RGBCode_inHEX = randResp_Of_RGBCode.ToString("X");
                    string RGBCodeData = "@" + randResp_Of_RGBCode_inHEX + ";" + "#" + data + ";";                    
                   // comPort.RtsEnable = true;
                   // comPort.DtrEnable = true;
                    comPort.WriteTimeout = 200;                    
                    comPort.WriteLine(RGBCodeData);                    
                    Thread.Sleep(1024);
                }
                else
                    comPort.Close();
            }
            catch (IOException)
                {
                    connectionStatus.Text = "Communication to SyncLED was interrupted !";
                    deviceFound = false;
                    AutoSync.Enabled = true;
                    
                }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Start();

           /* if (RunAtStartup.Checked == true)
            {
                reg.SetValue("SyncLED", Application.ExecutablePath.ToString());
            }*/

            if (TemperatureCPU() != null )
            CPU_temp.Text = Convert.ToString(TemperatureCPU()) + " °C";

            if (TemperatureGPU() != "")
                GPU_temp.Text = Convert.ToString(TemperatureGPU()) + " °C";
            else
            {
                GPU_temp.Text = " ";
            }
            
            string CPUTemp = TemperatureCPU();
            string GPUTemp = TemperatureGPU();
            if (CPUButton.Checked == true)
            {
               //label2.Text =  tempToRGBinHEX(CPUTemp);
                temperature = CPUTemp;

            }
            else if (GPUButton.Checked == true)
            {
                //label2.Text = tempToRGBinHEX(GPUTemp);
                temperature = GPUTemp;
            }
            //Marker
            string RGBhexData = tempToRGBinHEX(temperature);
            //label4.Text = temperature;
            sendData(RGBhexData);
            //Ends Here
        } 


        private void radioButtonSelect()
        {
            foreach (var hardwareItem in thisComputer.Hardware)
            {
                if (hardwareItem.HardwareType == HardwareType.GpuAti || hardwareItem.HardwareType == HardwareType.GpuNvidia)
                {
                    hardwareItem.Update();
                    GPUButton.Enabled = true;
                    GPUButton.PerformClick();
                }
                else
                {
                    hardwareItem.Update();
                    CPUButton.PerformClick();
                    GPUButton.Enabled = false;
                }
            }
        }


        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                if (MinimizeSystemTray.Checked == true)
                {
                    ShowInTaskbar = false;
                    notifyIcon.Visible = true;
                    notifyIcon.ShowBalloonTip(200, "SyncLED", "Running here...", ToolTipIcon.Info);

                }
                else
                {
                    ShowInTaskbar = true;
                }


            }
            else if (FormWindowState.Normal == WindowState)
            {
                notifyIcon.Visible = true;
            }
        }

        public class AutoClosingMessageBox
        {
            System.Threading.Timer _timeoutTimer;
            string _caption;
            AutoClosingMessageBox(string text, string caption, int timeout)
            {
                _caption = caption;
                _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                    null, timeout, System.Threading.Timeout.Infinite);
                using (_timeoutTimer)
                    MessageBox.Show(text, caption);
            }
            public static void Show(string text, string caption, int timeout)
            {
                new AutoClosingMessageBox(text, caption, timeout);
            }
            void OnTimerElapsed(object state)
            {
                IntPtr mbWnd = FindWindow("#32770", _caption); // lpClassName is #32770 for MessageBox
                if (mbWnd != IntPtr.Zero)
                    SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                _timeoutTimer.Dispose();
            }
            const int WM_CLOSE = 0x0010;
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowInTaskbar = true;
            notifyIcon.Visible = false;
            WindowState = FormWindowState.Normal;
        } 

        private Int32 randResp(Int32 information)
        {
            x:
            Int32 response = generateRandomNumberInRange();
            if (sumTillOneDigit(response) == sumTillOneDigit(information)  && response != information)
            {
                return response;
            }
            else
                goto x;
        }

        private Int32 stringHexToNumber(string numTobeConverted)
        {
            //int msgInInteger = Convert.ToInt32(numTobeConverted.ToString());            
            //string msgInInteger_ToHex = msgInInteger.ToString("X");
            Int32 decValue = Convert.ToInt32(numTobeConverted, 16);
            return decValue;
            //label2.Text = "msg in Integer";
        }

        
   private string parser (string info, char startByte, char endByte)
    {
       int size = Convert.ToInt16(info.Length);
       int start = info.IndexOf(startByte);
       int stop = info.IndexOf(endByte);
       string[] content = new String[1];
       string[] data = new String[size];
       //int j = 0;
       for (int i = 0; i < info.Length; i++ )
       {
           data[i] = info[i].ToString();
       }
       for (int i = start + 1; i < stop; i++)
       {
           content[0] += data[i];
       }
       return content[0];        
    }

        
        private void connectPort(string newPort)
        {
            
            //SerialPort comPort = new SerialPort();
            comPort.Close();
            comPort.PortName = newPort;
            comPort.BaudRate = 9600;

            //comPort.RtsEnable = true;
            //comPort.DtrEnable = true;
            AutoSync.Enabled = true;
            //comPort.Close();
            string syncLEDPort = "";
            //Boolean portBusy = false;
            comPort.Open();
            try
            {                
                if (comPort.IsOpen && deviceFound == false)
                {
                    AutoSync.Enabled = false;
                    
                    comPort.WriteTimeout = 200;
                    Int32 randomNumber = generateRandomNumberInRange();     // Generate random Number
                    Int32 num = randomNumber;                               
                    //string unitSum = "";
                    int unitSumVal = sumTillOneDigit(num);                  // Add the numbers till its a one digit
                    //unitSum = unitSumVal.ToString();                      // Convert it to String

                    string hexString = numberToHex(num).ToString();         // Convert the random number to Hex
                    string ping = "*" + hexString + ";";                    // Format the hex
                    //string serialHandshake = "*" + ping + ";";
                    comPort.Write(ping);
                    Thread.Sleep(500);
                    comPort.ReadTimeout = 100;
                    string msg = comPort.ReadExisting();                               // Serial read message
                    
                    string parsedMsg = parser(msg, '~', ';');                          // Parsed Serial Message
                    Int32 parsedMsg_ToNumber = stringHexToNumber(parsedMsg);           // Parsed message to  Number
                    int parsedMsg_TillOneDigit = sumTillOneDigit(parsedMsg_ToNumber);  // Parsed message to number then add the numbers til its a one digit
                    
                    if (unitSumVal == parsedMsg_TillOneDigit)
                    {               
                        //connectionStatus.Text = "Valid device found at " + comPort.PortName;
                        //syncLEDPort = comPort.PortName;
                        AutoSync.Enabled = true; 
                        /* int msgInInteger = Convert.ToInt32(msg[0].ToString());
                         string msgInInteger_ToHex = msgInInteger.ToString("X");
                         label2.Text = "msg in Integer";*/                        
                        Thread.Sleep(500);
                        Int32 randomResponse = randResp(parsedMsg_ToNumber);
                        string respString = numberToHex(randomResponse);
                        comPort.Write("~" + respString + ";");
                        deviceFound = deviceFound | true;
                        if (deviceFound == true)
                        {
                            connectionStatus.Text = "";
                            connectionStatus.Text += "SyncLED Connected";
                            validPorts[1] = Convert.ToString(comPort.PortName);
                            //validPort = comPort.PortName;
                            AutoSync.Enabled = true;                            
                        }

                        else
                        {
                            //deviceFound = false;
                            AutoSync.Enabled = true;
                            comPort.Close();
                        }
                    }
                    
                    else if (msg == null || msg == "")
                    {
                        AutoSync.Enabled = true;
                        syncLEDPort = "";
                        comPort.Close();
                        deviceFound = deviceFound | false;
                        if (deviceFound == false)
                        {
                            connectionStatus.Text = "";
                            connectionStatus.Text += "SyncLED not connected!";
                        }
                    }
                    else
                    {
                        AutoSync.Enabled = true;
                        comPort.Close();
                        deviceFound = deviceFound | false;
                        if (deviceFound == false)
                        {
                            connectionStatus.Text = "";
                            connectionStatus.Text += "Have you connected SyncLED?";
                        }
                    }
                }
                else 
                {                    
                    AutoSync.Enabled = true;
                    deviceFound = deviceFound | false;
                    if (deviceFound == false)
                    {
                        connectionStatus.Text = "";
                        connectionStatus.Text += "SyncLED not found !";
                    }
                    comPort.Close();
                }

            }

            catch (IOException)
            {
                //connectionStatus.Text = "";
                //connectionStatus.Text += "IO ERROR at " + comPort.PortName;
                AutoSync.Enabled = true;
                comPort.Close();
            }

            catch (DataException)
            {
                //connectionStatus.Text = "";
                //connectionStatus.Text += "Data exception ERROR at " + comPort.PortName;
                AutoSync.Enabled = true;
                comPort.Close();
            }
            catch (ObjectDisposedException)
            {
                //connectionStatus.Text = "";
                //connectionStatus.Text += "Object disposed ERROR at " + comPort.PortName;
                AutoSync.Enabled = true;
                comPort.Close();
            }
            catch (OperationAbortedException)
            {
                AutoSync.Enabled = true;
                comPort.Close();
            }
            catch (TimeoutException)
            {
                //connectionStatus.Text = "";
                //connectionStatus.Text += "Timeout ERROR at " + comPort.PortName;
                AutoSync.Enabled = true;
                comPort.Close();
            }
            catch (UnauthorizedAccessException)
            {
                // connectionStatus.Text = "";
                // connectionStatus.Text += "Accessed denied at " + comPort.PortName;
                AutoSync.Enabled = true;
                comPort.Close();
            }
            
          }

        private void AutoSync_Click(object sender, EventArgs e)
        {
            string[] portNamesNew = new string [255];
            portNamesNew = System.IO.Ports.SerialPort.GetPortNames();       
            deviceFound = false;
           if(deviceFound == false) 
           {
            foreach (string newCom in portNamesNew)
            {
                try
                {
                    connectPort(newCom);   
                }
                catch (IOException)
                {
                    // connectionStatus.Text = "";
                    //connectionStatus.Text += "IO ERROR at ";
                    AutoSync.Enabled = true;
                }

                catch (DataException)
                {
                    //connectionStatus.Text = "";
                    // connectionStatus.Text += "Data exception ERROR";
                    AutoSync.Enabled = true;
                }
                catch (ObjectDisposedException)
                {
                    // connectionStatus.Text = "";
                    // connectionStatus.Text += "Object disposed ERROR";
                    AutoSync.Enabled = true;

                }
                catch (TimeoutException)
                {
                    // connectionStatus.Text = "";
                    //connectionStatus.Text += "Timeout ERROR at " ;
                    AutoSync.Enabled = true;

                }
                catch (UnauthorizedAccessException)
                {
                    // connectionStatus.Text = "";
                    //connectionStatus.Text += "Timeout ERROR at ";
                    AutoSync.Enabled = true;

                }
              
            } 
        }
           
        }        

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            //MessageBox.Show("Exit Applocation", "Wanna exit and miss out all cool stuffs ?",MessageBoxButtons.YesNo);
            string message = "Wanna exit and miss out all cool stuffs ?";
            string title = "Close Window";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);

            if (result == DialogResult.Yes)
            {
                deviceFound = false; 
                Environment.Exit(0);
                Application.Exit();
            }

            else if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void label1_Click_2(object sender, EventArgs e)
        {

        }
        
    }
}