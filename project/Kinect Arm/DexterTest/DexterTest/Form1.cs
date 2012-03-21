using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using DexterER2;

namespace DexterTest
{
    public partial class Form1 : Form
    {
        int Connection_Status=1;
        ArmCtrl Arm = new ArmCtrl(20,30);

        public Form1()
        {
            InitializeComponent();
            InitializeComports();

        }

        private void InitializeComports()
        {
            this.comportbox.Items.Clear();
            foreach (string str in SerialPort.GetPortNames())
            {
                this.comportbox.Items.Add(str);
            }
            if (this.comportbox.Items.Count > 0)
            {
                this.comportbox.SelectedIndex = 0;
            }
        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            if (Connection_Status == 1)
            {
                Connection_Status = Arm.connect_port(this.comportbox.SelectedItem.ToString());
                if (Connection_Status == 0)
                {
                    MessageBox.Show("Connected");
                    this.ConnectBtn.Text = "Disconnect";
                    this.comportbox.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Not Connected");
                }
            }
            else
            {
                Connection_Status = 1-Arm.disconnect_port();
                if (Connection_Status == 1)
                {
                    MessageBox.Show("Disconnected");
                    this.ConnectBtn.Text = "Connect";
                    this.comportbox.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Not Disconnected");
                }

            }
        }

        private void RunBtn_Click(object sender, EventArgs e)
        {
            //NEWTON
            //connect_port();
            if (Connection_Status==0)
            {
                int angle = 20;
                int velocity = 50;
                Arm.servo_move(4, angle, velocity);
                System.Threading.Thread.Sleep(3000);

                Arm.servo_move(3, 40, velocity);
                System.Threading.Thread.Sleep(3000);

                //string angle3 = "70";
                //string velocity3 = "90";
                //this.servo_move3(angle3, velocity3);
                Arm.reset();

            }
            else
            {
                MessageBox.Show("Not Connected");
            }

        }
    }
}
