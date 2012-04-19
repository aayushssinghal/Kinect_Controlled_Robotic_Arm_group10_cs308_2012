using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

/*
   Class: Plane
   A class that maintains x,y,z components of normal to the plane.
*/
public class Plane
{
    public double
        x,
        y,
        z;
	/*
         Constructor: Plane
         Initializes x,y,z.
      */
    public Plane()
    {
        x = y = z = 1;
    }
	/*
         Constructor: Plane
         Initializes x,y,z.
    */
    public Plane(double xx, double yy, double zz)
    {
        x = xx;
        y = yy;
        z = zz;
    }
	/*
         Constructor: Plane
         Initializes x,y,z.
    */
    public Plane(Joint a, Joint b, Joint c)
    {
        Direction d1 = new Direction(a, b);
        Direction d2 = new Direction(a, c);
        Direction per = Mathematics.crossProduct(d1, d2);
        x = per.x;
        y = per.y;
        z = per.z;
    }

    public Direction planePerpendicular()
    {
        Direction d = new Direction(x, y, z);
        return d;
    }

    bool isValid()
    {
        return (Mathematics.norm(planePerpendicular()) != 0);
    }
}
