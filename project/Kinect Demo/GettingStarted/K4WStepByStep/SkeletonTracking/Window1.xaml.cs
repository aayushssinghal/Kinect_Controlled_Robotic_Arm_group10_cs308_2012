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

namespace SkeletonTracking
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    
    public partial class Window1 : Window
    {
        public bool RoboticArmMovement;
        int Connection_Status = 1;
        public ArmCtrl Arm = new ArmCtrl(0,60);
        
        public Window1()
        {
            RoboticArmMovement = false;
            InitializeComponent();
            InitializeComports();
            StartStopBtn.IsEnabled = false;
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

       
      
        
        
    }
    
}
