using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DexterER2.Properties;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.ComponentModel;
using System.Data;

namespace DexterER2
{
    public class ArmCtrl
    {
        private byte[] buffer = new byte[8];
        private byte[] buffer1 = new byte[4];
        private byte[] buffer10 = new byte[4];
        private byte[] buffer11 = new byte[4];
        private byte[] buffer12 = new byte[4];
        private byte[] buffer13 = new byte[4];
        private byte[] buffer14 = new byte[4];
        private byte[] buffer15 = new byte[4];
        private byte[] buffer16 = new byte[4];
        private byte[] buffer17 = new byte[4];
        private byte[] buffer18 = new byte[4];
        private byte[] buffer19 = new byte[4];
        private byte[] buffer2 = new byte[4];
        private byte[] buffer20 = new byte[4];
        private byte[] buffer3 = new byte[4];
        private byte[] buffer4 = new byte[4];
        private byte[] buffer5 = new byte[4];
        private byte[] buffer6 = new byte[4];
        private byte[] buffer7 = new byte[4];
        private byte[] buffer8 = new byte[4];
        private byte[] buffer9 = new byte[4];
        private TimerCallback cb;
        private SerialPort comport = new SerialPort();
        private int[] Decvalue = new int[] { 
            0x31, 50, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 60, 0x3d, 0x3e, 0x3f, 0x40, 
            0x41, 0x42, 0x43, 0x44
         };
        private int[] Disvalue = new int[] { 
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0x10, 
            0x11, 0x12, 0x13, 20
         };
        private bool flag = true;
        private bool lock_flag=false;
        private bool get_lock;
        private int[] last = new int[0x2710];
        private int sel_count;
        private byte[] selected_buffer = new byte[0x54];
        private byte[] send_buffer = new byte[0x54];
        private byte[] send_buffer1 = new byte[] { 
            0x23, 0x31, 90, 90, 0x23, 50, 90, 90, 0x23, 0x33, 90, 90, 0x23, 0x34, 90, 90, 
            0x23, 0x35, 90, 90, 0x23, 0x36, 90, 90, 0x23, 0x37, 90, 90, 0x23, 0x38, 90, 90, 
            0x23, 0x39, 90, 90, 0x23, 0x3a, 90, 90, 0x23, 0x3b, 90, 90, 0x23, 60, 90, 90, 
            0x23, 0x3d, 90, 90, 0x23, 0x3e, 90, 90, 0x23, 0x3f, 90, 90, 0x23, 0x40, 90, 90, 
            0x23, 0x41, 90, 90, 0x23, 0x42, 90, 90, 0x23, 0x43, 90, 90, 0x23, 0x44, 90, 90, 
            0x2a, 0x2a, 0x2a, 0x2a
         };
        private bool send_lock = true;
        private Stopwatch sw;
        private int[] team = new int[0x1388];
        private byte[] temp_buffer = new byte[1];
        private System.Threading.Timer thrdTimer;
        private int speed_limit;
        private int angle_limit;
        int timer_period;
        public void reset()
        {
            int default_angle = 90;
            int default_speed = 20;
            for (int i = 1; i <= 6; i++)
            {
                this.servo_move(i, default_angle, default_speed);
            }
        }

        public ArmCtrl(int _angle_limit, int _speed_limit,int _timer_period=100)
        {
            timer_period = _timer_period;
            if (_angle_limit >= 0 && _angle_limit <= 180 && _speed_limit >= 0 && _speed_limit <= 180)
            {
                angle_limit = _angle_limit;
                speed_limit = _speed_limit;
            }
            else
            {
                angle_limit = 30;
                speed_limit = 30;
            }
        }

        
        public int connect_port(string COMPortName)
        {
            if (!this.comport.IsOpen)
            {
                try
                {
                    this.comport.PortName = COMPortName;
                }
                catch (NullReferenceException)
                {
                }
                this.comport.BaudRate = Settings.Default.BaudRate;
                this.comport.DataBits = Settings.Default.DataBits;
                this.comport.ReadTimeout = 50;
                this.comport.WriteTimeout = -1;
                try
                {
                    this.comport.Open();
                    this.send_lock = true;
                    if (this.flag)
                    {
                        this.cb = new TimerCallback(this.send);
                        this.thrdTimer = new System.Threading.Timer(this.cb, 10, 0, timer_period);
                    }
                    return 0;
                }
                catch (Exception)
                {
                    this.comport.Close();
                    return 1;
                }
            }
            return 0;
        }

        public int disconnect_port()
        {
            try
            {
                this.comport.Close();
                this.flag = true;
                this.thrdTimer.Dispose();
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public bool IsConnected()
        {
            return this.comport.IsOpen;
        }


        public void servo_move(int i, int _angle, int _speed)
        {
            if (_angle < angle_limit )
            {
                _angle = angle_limit;
            }
            if( _angle > 180 - angle_limit)
            {
                _angle = 180-angle_limit;
            }
            if (_speed < speed_limit )
            {
                _speed = speed_limit;
            }
            if ( _speed > 180 - speed_limit)
            {
                _speed = 180-speed_limit;
            }

            string angle = _angle.ToString("X");
            string speed = _speed.ToString("X");
            switch (i)
            {
                case 1:
                    this.servo_move1(angle, speed); break;
                case 2:
                    if (_speed > 90) { 
                        _speed=90;
                        speed = _speed.ToString("X");
                    }
                    this.servo_move4(angle, speed); break;
                case 3:
                    this.servo_move7(angle, speed); break;
                case 4:
                    this.servo_move10(angle, speed); break;
                case 5:
                    this.servo_move13(angle, speed); break;
                case 6:
                    this.servo_move16(angle, speed); break;
            }
        }


        public void servo_move1(string str1, string str2)
        {
            byte[] buffer = this.HexToByte1(str1);
            byte[] buffer2 = this.HexToByte2(str2);
            byte num = 0x23;
            byte num2 = 0x31;
            byte num3 = buffer[0];
            byte num4 = buffer2[0];
            this.buffer1[0] = num;
            this.buffer1[1] = num2;
            this.buffer1[2] = num3;
            this.buffer1[3] = num4;
        }

        public void servo_move4(string str1, string str2)
        {
           byte[] buffer = this.HexToByte1(str1);
           byte[] buffer2 = this.HexToByte2(str2);
           byte num = 0x23;
           byte num2 = 50;
           byte num3 = buffer[0];
           byte num4 = buffer2[0];
           this.buffer4[0] = num;
           this.buffer4[1] = num2;
           this.buffer4[2] = num3;
           this.buffer4[3] = num4;
        }
        public void servo_move7(string str1, string str2)
        {
           byte[] buffer = this.HexToByte1(str1);
           byte[] buffer2 = this.HexToByte2(str2);
           byte num = 0x23;
           byte num2 = 0x33;
           byte num3 = buffer[0];
           byte num4 = buffer2[0];
           this.buffer7[0] = num;
           this.buffer7[1] = num2;
           this.buffer7[2] = num3;
           this.buffer7[3] = num4;
        }

        public void servo_move10(string str1, string str2)
        {
            byte[] buffer = this.HexToByte1(str1);
            byte[] buffer2 = this.HexToByte2(str2);
            byte num = 0x23;
            byte num2 = 0x34;
            byte num3 = buffer[0];
            byte num4 = buffer2[0];
            this.buffer10[0] = num;
            this.buffer10[1] = num2;
            this.buffer10[2] = num3;
            this.buffer10[3] = num4;
        }

        

        public void servo_move13(string str1, string str2)
        {
            byte[] buffer = this.HexToByte1(str1);
            byte[] buffer2 = this.HexToByte2(str2);
            byte num = 0x23;
            byte num2 = 0x35;
            byte num3 = buffer[0];
            byte num4 = buffer2[0];
            this.buffer13[0] = num;
            this.buffer13[1] = num2;
            this.buffer13[2] = num3;
            this.buffer13[3] = num4;
        }

        
        public void servo_move16(string str1, string str2)
        {
            byte[] buffer = this.HexToByte1(str1);
            byte[] buffer2 = this.HexToByte2(str2);
            byte num = 0x23;
            byte num2 = 0x36;
            byte num3 = buffer[0];
            byte num4 = buffer2[0];
            this.buffer16[0] = num;
            this.buffer16[1] = num2;
            this.buffer16[2] = num3;
            this.buffer16[3] = num4;
        }

        

        private byte[] HexToByte1(string str1)
        {
            int startIndex = 0;
            int index = 0;
            byte[] buffer = new byte[str1.Length];
            while (str1.Length > startIndex)
            {
                int num3 = Convert.ToInt32(str1.Substring(startIndex, str1.Length), 0x10);
                buffer[index] = Convert.ToByte(num3);
                startIndex += 2;
                index++;
            }
            return buffer;
        }

        private byte[] HexToByte2(string str1)
        {
            int startIndex = 0;
            int index = 0;
            byte[] buffer = new byte[str1.Length];
            while (str1.Length > startIndex)
            {
                long num3 = Convert.ToInt32(str1.Substring(startIndex, str1.Length), 0x10);
                buffer[index] = Convert.ToByte(num3);
                startIndex += 2;
                index++;
            }
            return buffer;
        }

        
        public void send(object args)
        {
            //MessageBox.Show("sending data");
            this.sw = Stopwatch.StartNew();
            try
            {

                int index = 0;
                if (true) //Axis 1
                {
                    this.send_lock = false;
                    this.get_lock = true;
                    this.send_buffer[index] = this.buffer1[0];
                    index++;
                    this.send_buffer[index] = this.buffer1[1];
                    index++;
                    this.send_buffer[index] = this.buffer1[2];
                    index++;
                    if (this.lock_flag)
                    {
                        //byte[] buffer = this.HexToByte1(this.mtbVelocityControl.Value.ToString("X"));
                        this.send_buffer[index] = buffer[0];
                        index++;
                    }
                    else
                    {
                        this.send_buffer[index] = this.buffer1[3];
                        index++;
                    }
                }
                if (true) //Axix 2
                {
                    this.send_lock = false;
                    this.get_lock = true;
                    this.send_buffer[index] = this.buffer4[0];
                    index++;
                    this.send_buffer[index] = this.buffer4[1];
                    index++;
                    this.send_buffer[index] = this.buffer4[2];
                    index++;
                    if (this.lock_flag)
                    {
                        //byte[] buffer2 = this.HexToByte1(this.mtbVelocityControl.Value.ToString("X"));
                        this.send_buffer[index] = buffer2[0];
                        index++;
                    }
                    else
                    {
                        this.send_buffer[index] = this.buffer4[3];
                        index++;
                    }
                }
                if (true)  //Axis 3
                {
                    this.send_lock = false;
                    this.get_lock = true;
                    this.send_buffer[index] = this.buffer7[0];
                    index++;
                    this.send_buffer[index] = this.buffer7[1];
                    index++;
                    this.send_buffer[index] = this.buffer7[2];
                    index++;
                    if (this.lock_flag)
                    {
                        //byte[] buffer3 = this.HexToByte1(this.mtbVelocityControl.Value.ToString("X"));
                        this.send_buffer[index] = buffer3[0];
                        index++;
                    }
                    else
                    {
                        this.send_buffer[index] = this.buffer7[3];
                        index++;
                    }
                }
                if (true)  //Axis 4
                {
                    this.send_lock = false;
                    this.get_lock = true;
                    this.send_buffer[index] = this.buffer10[0];
                    index++;
                    this.send_buffer[index] = this.buffer10[1];
                    index++;
                    this.send_buffer[index] = this.buffer10[2];
                    index++;
                    if (this.lock_flag)
                    {
                        //byte[] buffer4 = this.HexToByte1(this.mtbVelocityControl.Value.ToString("X"));
                        this.send_buffer[index] = buffer4[0];
                        index++;
                    }
                    else
                    {
                        this.send_buffer[index] = this.buffer10[3];
                        index++;
                    }
                }
                if (true)  //Axis 5
                {
                    this.send_lock = false;
                    this.get_lock = true;
                    this.send_buffer[index] = this.buffer13[0];
                    index++;
                    this.send_buffer[index] = this.buffer13[1];
                    index++;
                    this.send_buffer[index] = this.buffer13[2];
                    index++;
                    if (this.lock_flag)
                    {
                        //byte[] buffer5 = this.HexToByte1(this.mtbVelocityControl.Value.ToString("X"));
                        this.send_buffer[index] = buffer5[0];
                        index++;
                    }
                    else
                    {
                        this.send_buffer[index] = this.buffer13[3];
                        index++;
                    }
                }
                if (true)  //Axis 6
                {
                    this.send_lock = false;
                    this.get_lock = true;
                    this.send_buffer[index] = this.buffer16[0];
                    index++;
                    this.send_buffer[index] = this.buffer16[1];
                    index++;
                    this.send_buffer[index] = this.buffer16[2];
                    index++;
                    if (this.lock_flag)
                    {
                        //byte[] buffer6 = this.HexToByte1(this.mtbVelocityControl.Value.ToString("X"));
                        this.send_buffer[index] = buffer6[0];
                        index++;
                    }
                    else
                    {
                        this.send_buffer[index] = this.buffer16[3];
                        index++;
                    }
                }

                this.send_buffer[index] = 0x2a;
                index++;
                this.send_buffer[index] = 0x2a;
                index++;
                this.send_buffer[index] = 0x2a;
                index++;
                this.send_buffer[index] = 0x2a;
                index++;
                if (index > 4)
                {
                    // MessageBox.Show("hello");

                    for (int i = 0; i < index; i++)
                    {

                        try
                        {
                            if (this.flag)
                            {
                                //MessageBox.Show("comm with arm");
                                this.sel_count = index;
                                this.selected_buffer[i] = this.send_buffer[i];
                                this.comport.Write(this.send_buffer, i, 1);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else
                {
                    this.get_lock = false;
                    if (this.send_lock)
                    {
                        for (int j = 0; j < this.send_buffer1.Length; j++)
                        {
                            try
                            {
                                this.comport.Write(this.send_buffer1, j, 1);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
            catch (TimeoutException)
            {
            }
            catch (Exception)
            {
            }
            this.sw.Stop();
        }

    }
}