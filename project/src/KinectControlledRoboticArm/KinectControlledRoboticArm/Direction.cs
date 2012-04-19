using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

/*
   Class: Direction
   Vector Class, provides function for calculating dot products, 
   cross products and angles between vectors.
*/

public class Direction
{
    public double x, y, z;
    public double cosx, cosy, cosz;

    public bool isCosineEqual(double a, double b)
    {
        return (Math.Abs(a - b) < 0.1); // TODO
    }

    public Direction(Joint i, Joint j)
    {
        createDirection(
            j.Position.X - i.Position.X,
            j.Position.Y - i.Position.Y,
            j.Position.Z - i.Position.Z);
    }

    public Direction(double x = 0, double y = 0, double z = 0)
    {
        createDirection(x, y, z);
    }

    public void createDirection(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;

        double s = Math.Sqrt(x * x + y * y + z * z);
        if (s == 0)
        {
            cosx = cosy = cosz = 1;
            return;
        }
        cosx = x / s;
        cosy = y / s;
        cosz = z / s;
    }

    // Returns angle between two lines
    public Double lineAngle(Direction line)
    {
        return Mathematics.angleBetweenLines(this, line);
    }

    public bool isHorizontal()
    {
        return (isCosineEqual(cosy, 0) && isCosineEqual(cosz, 0) && isCosineEqual(cosx, 1));
    }

    public bool isVertical()
    {
        return (isCosineEqual(cosy, 1) && isCosineEqual(cosz, 0) && isCosineEqual(cosx, 0));
    }

    public bool isPerpendicular()
    {
        return (isCosineEqual(cosy, 0) && isCosineEqual(cosz, 1) && isCosineEqual(cosx, 0));
    }

    public bool isStraightLine()
    {
        return (isCosineEqual(cosx, 0) && isCosineEqual(cosy, 0) && isCosineEqual(cosz, 0));
    }
}
