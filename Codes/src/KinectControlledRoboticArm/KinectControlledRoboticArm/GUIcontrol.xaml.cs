using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.IO.Ports;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DexterER2;
using Microsoft.Kinect;

namespace KinectControlledRoboticArm
{
    /// <summary>
    /// Interaction logic for GUIcontrol.xaml
    /// </summary>
    
    /*
     * Class: GUIcontrol
     * Control window class
     */
    public partial class GUIcontrol : Window
    {
        public bool RoboticArmMovement;
        int Connection_Status = 1;
        public int ArmSpeed = 90;
        public ArmCtrl Arm = new ArmCtrl(0,180,200);
        KinectSensor sensor;
        public int inputmethod=1;
         
        public GUIcontrol()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                sensor = KinectSensor.KinectSensors[0];
                RoboticArmMovement = false;
                InitializeComponent();
                InitializeComports();
                this.StartKinect();
                StartStopBtn.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Kinect Device not found !!!");
                
            }
        }
 
     
        private void StartStopBtn_Click(object sender, RoutedEventArgs e)
        {
            
            RoboticArmMovement = !RoboticArmMovement;
            if (RoboticArmMovement)
            {
                StartStopBtn.Content = "Stop";
            }
            else
            {
                StartStopBtn.Content = "Start";
                
            }
            Arm.reset();
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
                if (this.comportbox.SelectedItem == null)
                {
                    if (this.comportbox.Items.Count == 0)
                    {
                        MessageBox.Show("No Port available \nArm is missing or not connected properly");
                        return;
                    }
                    MessageBox.Show("Cannot Connect : No Port selected\nPlease Try Later");
                    return;
                }
                Connection_Status = Arm.connect_port(this.comportbox.SelectedItem.ToString());
                if (Connection_Status == 0)
                {
                    MessageBox.Show("Connected");
                    this.ConnectBtn.Content = "Disconnect";
                    this.comportbox.IsEnabled = false;
                    this.StartStopBtn.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Not Connected");
                }
            }
            else
            {
                Connection_Status = 1 - Arm.disconnect_port();
                if (Connection_Status == 1)
                {
                    MessageBox.Show("Disconnected");
                    this.ConnectBtn.Content = "Connect";
                    this.comportbox.IsEnabled = true;
                    this.StartStopBtn.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("Not Disconnected");
                }

            }
        }

        
        // Event handler for Button Down to decrease kinect elevation angle
        private void Click_down(object sender, RoutedEventArgs e)
        {
            if (sensor.ElevationAngle - 4 > sensor.MinElevationAngle)
            {
                sensor.ElevationAngle -= 4;
            }
            int angle = this.sensor.ElevationAngle;
            labelAngle.Text = Convert.ToString(angle);
        }


        // Event handler for Button Up to increase kinect elevation angle
        private void Click_up(object sender, RoutedEventArgs e)
        {
            if (sensor.ElevationAngle + 4 < sensor.MaxElevationAngle)
            {
                sensor.ElevationAngle += 4;
            }
            int angle = this.sensor.ElevationAngle;
            labelAngle.Text = Convert.ToString(angle);
            
        }

        private void StartKinect()
        {

            sensor.Start();            // set the elevation Angle to 0 when start
            this.sensor.ElevationAngle = 0;
            labelAngle.Text = Convert.ToString(this.sensor.ElevationAngle);
        }

        private void changeSpeed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           ArmSpeed = Convert.ToInt32(slider1.Value) ;
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            inputmethod = InputMethod.SelectedIndex;
        }
    }
}
