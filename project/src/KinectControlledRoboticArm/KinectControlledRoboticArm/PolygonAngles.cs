using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace KinectControlledRoboticArm
{
	/*
   	Class: PolygonAngles
   	A class for calculating axis 2, 3, 4 angles using two cirlce algorithm.
	*/
    class PolygonAngles
    {
        public double PI = Math.PI;
        private int i=0;
        private double theta,phi;

        /**
  		* Constructor: PolygonAngles

   	    Parameters:

      	t - Distance from robotic arm base to it's hand.
      	prec - Precision in calculating angles.

   		
		*/
		
        public PolygonAngles(double t, double prec = 0.001) {
            double[] r = new double[3];
            r[0] = RoboticArm.Arm1;
            r[1] = RoboticArm.Arm2;
            r[2] = RoboticArm.Arm3;
            CalculateAngles(r,t,prec);

            //Console.WriteLine("theta {0}, phi {1}, distance : {2}", theta, phi, t);
        }

        public double getArm1Arm2Angle() {
            return getTheta();
        }

        public double getTheta() {
            return theta;
        }

        public double getArm1BaseAngle() {
            return phi;
        }

        public double getPhi() {
            return phi;
        }

        double f(double[] r,double t){
            return Mathematics.square(r[0]+r[1]*Math.Cos(t)+r[2]*Math.Cos(2*t))+ Mathematics.square(r[1]*Math.Sin(t)+r[2]*Math.Sin(2*t));
        }

		
        public void CalculateAngles(double[] r, double d, double prec){
            double mint=0,maxt=PI/2,t,v;
            Debug.Assert(d>0, "Skeleton is not captured properly, Hand length is becoming 0 or negative");
            while(true){
                i++;
                t=(mint+maxt)/2;
                v=f(r,t);
                if(Math.Abs(v-d*d) <= prec){
                    theta=t;
                    phi= Math.Acos((r[0]+r[1]* Math.Cos(t)+r[2] * Math.Cos(2*t))/d);
                    return;
                }

                if(v>d*d) mint=t;
                else maxt=t;
                if(maxt <= prec){
                    theta=0;
                    phi=0;
                    return;
                }
                if(mint >= PI/2-prec){
                    theta=t=PI/2;
                    phi= Math.Acos((r[0]+r[1]*Math.Cos(t)+r[2]*Math.Cos(2*t))/d);
                    return;
                }
            }
        }
    }
}

