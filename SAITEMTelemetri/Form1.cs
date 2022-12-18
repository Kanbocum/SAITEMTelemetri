using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms.Design;
using System.IO.Ports;



namespace SAITEMTelemetri
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


            Control.CheckForIllegalCrossThreadCalls = false;
            /*
            panelNav.Height = panelFirst.Height;
            panelNav.Top = panelFirst.Top;
            panelNav.Left = panelFirst.Left;
            */
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            foreach (var portGirisleri in SerialPort.GetPortNames())
            {
                comboBoxPort.Items.Add(portGirisleri);

                comboBoxPort.SelectedIndex = 0;

            }

        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        //***Begin Values***

        double speed;

        double batteryVoltage;

        double motorTemperature;

        double batteryTemperature;

        double remainingEnergy;

        //***End Values***



        //Begin Serial Port Communication



        //Begin Log Record


        public void DosyayaYaz(string logRecord)
        {
            string logPath = "C:/Users/Kanber/Desktop/logSAITEM.csv";

            string ilkSatir = "Tarih" + ";" + "Hız" + ";" + "Batarya Sıcaklığı" + ";" + "Motor Sıcaklığı" + ";" + "Batarya Gerilimi" + ";" + "Kalan Enerji";


            if (!File.Exists(logPath))
            {
                File.WriteAllText(logPath, ilkSatir + Environment.NewLine);

            }


            else if (File.Exists(logPath))
            {
                File.AppendAllText(logPath, logRecord + Environment.NewLine);

            }


            File.AppendAllText(logPath, logRecord + Environment.NewLine);

        }

        //End Log Record



        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }

        //End Serial Port Communication



        public void Ayikla(string gelenveri)
        {
            string[] firstSplit = gelenveri.Split('$');


            if (firstSplit.Length > 2)
            {
                string secondSplit = firstSplit[1];

                string[] lastResult = secondSplit.Split(',');


                switch (lastResult[1])
                {
                    case "H":
                        speed = Convert.ToDouble(lastResult[2].Replace(".", ","));

                        guna2RadialGauge1.Value = (int)speed;

                        circularSpeed.Text = speed.ToString();


                        break;


                    case "BT":
                        batteryTemperature = Convert.ToDouble(lastResult[2].Replace(".", ","));

                        circularBTemperature.Value = (int)batteryTemperature;

                        circularBTemperature.Text = batteryTemperature.ToString();


                        break;


                    case "BV":
                        batteryVoltage = Convert.ToDouble(lastResult[2].Replace(".", ","));

                        circularBVoltage.Value = (int)batteryVoltage;

                        circularBVoltage.Text = batteryVoltage.ToString();


                        break;


                    case "WH":
                        remainingEnergy = Convert.ToDouble(lastResult[2].Replace(".", ","));

                        circularREnergy.Value = (int)remainingEnergy;

                        labelREnergy.Text = remainingEnergy.ToString();


                        if (remainingEnergy < 15)
                        {
                            labelREnergyStatus.Visible = true;
                            panelREnergyStatus.Visible = true;

                        }


                        else if (remainingEnergy >= 15)
                        {
                            labelREnergyStatus.Visible = false;
                            panelREnergyStatus.Visible = false;

                        }


                        break;


                    case "MT":
                        motorTemperature = Convert.ToDouble(lastResult[2].Replace(".", ","));

                        circularMTemperature.Value = (int)motorTemperature;

                        circularMTemperature.Text = motorTemperature.ToString();


                        break;

                }

            }

        }


        private void timer1_Tick_1(object sender, EventArgs e)
        {
            //panelFalse.Visible = false;

            bool statusArduino = serialPort1.IsOpen;

            string gelenveriArduino;


            while (statusArduino)
            {
                gelenveriArduino = serialPort1.ReadExisting();


                Ayikla(gelenveriArduino);


                logkaydiTimer.Start();


                break;

            }

        }

        //Begin Split() Function


        //End Split() Funciton



        //Begin Menu Section

        public void guna2Button3_Click(object sender, EventArgs e)
        {
            //this.pictureBoxAnaMenu.Image = Image.FromFile("C:/Users/Kanber/Desktop/BataryaSimgesi.png");
            /*
            labelhomepage.Text = "Anasayfa";

            panelNav.Height = panelFirst.Height;
            panelNav.Top = panelFirst.Top;
            panelNav.Left = panelFirst.Left;
            */
        }


        public void guna2Button2_Click(object sender, EventArgs e)
        {
            //this.pictureBoxAnaMenu.Image = Image.FromFile("C:/Users/Kanber/Desktop/BataryaSimgesi.png");
            /*
            labelhomepage.Text = "Bataryalar";


            panelNav.Height = panelSecond.Height;
            panelNav.Top = panelSecond.Top;
            panelNav.Left = panelSecond.Left;
            */
        }


        public void buttonReduce_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }


        public void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();


            string logPath = "C:/Users/Kanber/Desktop/logSAITEM.csv";


            if (File.Exists(logPath))
            {
                File.Delete(logPath);

            }

        }


        private void logkaydiTimer_Tick_1(object sender, EventArgs e)
        {
            string logDate = (DateTime.Now).ToString();

            string veriler = logDate + ";" + speed + ";" + batteryTemperature + ";" + motorTemperature + ";" + batteryVoltage + ";" + remainingEnergy;

            DosyayaYaz(veriler);

        }


        private void buttonPause_Click(object sender, EventArgs e)
        {

            if (serialPort1.IsOpen)
            {
                serialPort1.Close();


                timer1.Stop();
                logkaydiTimer.Stop();

                /*
                labelStatus.ForeColor = Color.Crimson;

                labelStatus.Text = "Paused";


                panelFalse.Visible = false;


                panelPause.Visible = true;
                panelPause.Location = new Point(751, 13);


                buttonConnect.Enabled = true;
                buttonPause.Enabled = false;
                */

            }

        }


        private void buttonConnect_Click(object sender, EventArgs e)
        {

            if (!serialPort1.IsOpen)
            {
                serialPort1.PortName = comboBoxPort.Text;
                serialPort1.BaudRate = 9600;
                serialPort1.DataBits = 8;
                serialPort1.Parity = Parity.None;
                serialPort1.StopBits = StopBits.One;


                try
                {
                    serialPort1.Open();


                    timer1.Start();

                }


                catch
                {
                    /*
                    panelFalse.Visible = true;
                    panelFalse.Location = new Point(751, 13);


                    labelStatus.ForeColor = Color.Crimson;
                    labelStatus.Text = "No Match";
                    */
                }

            }


            if (serialPort1.IsOpen)
            {
                buttonConnect.Enabled = false;
                buttonPause.Enabled = true;

                /*
                panelPause.Visible = false;


                labelStatus.ForeColor = Color.LightGreen;

                labelStatus.Text = "Connected";


                pictureBoxConnect.Visible = true;

                pictureBoxConnect.Location = new Point(745, 11);
                */
            }

        }

        //End Menu Section



    }

}









            