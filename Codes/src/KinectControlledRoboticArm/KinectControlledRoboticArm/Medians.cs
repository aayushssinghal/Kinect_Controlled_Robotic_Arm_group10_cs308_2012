using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectControlledRoboticArm
{
	/*
		Class: Medians
		class to find the median of several data sets
	*/
    class Medians
    {
        int[][] lastFewValues = new int[7][];
        int currentValue = 0;
        const int sampleSize = 10;
	
        /*
        	Constructor: Medians
        	Initializes and allocates proper amount of memory
        	to each element
        */
        public Medians(int defaultValue = 90)
        {
            for (int i = 0; i < 7; i++)
            {
                lastFewValues[i] = new int[sampleSize];
                for (int j = 0; j < sampleSize; j++) 
                    lastFewValues[i][j] = defaultValue;
            }
        }

		/*
			Function: GetMedian
			Calculates the Median of a set of integers
			
			Parameters:
				sourceNumbers - array of integers
			
			Returns:
				single integer representing median of given set
		*/
        public int GetMedian(int[] sourceNumbers)
        {
            //Framework 2.0 version of this method. there is an easier way in F4        
            if (sourceNumbers == null || sourceNumbers.Length == 0)
                return 0;

            //make sure the list is sorted, but use a new array
            int[] sortedPNumbers = (int[])sourceNumbers.Clone();
            sourceNumbers.CopyTo(sortedPNumbers, 0);
            Array.Sort(sortedPNumbers);

            //get the median
            int size = sortedPNumbers.Length;
            int mid = size / 2;
            int median = (size % 2 != 0) ? (int)sortedPNumbers[mid] : ((int)sortedPNumbers[mid] + (int)sortedPNumbers[mid - 1]) / 2;
            return median;
        }

		/*
			Function: getNextMedian
			Maintains array of last few data values, and 
			finds out median of current set of values
		*/
        public int[] getNextMedian(int[] newData)
        {
            int[] result = new int[7];
            for (int i = 1; i <= 6; i++)
            {
                lastFewValues[i][currentValue] = newData[i];
            }
            currentValue = (currentValue + 1) % sampleSize;
            for (int i = 1; i < 7; i++) 
                result[i] = GetMedian(lastFewValues[i]);

            return result;

        }
    }
}
