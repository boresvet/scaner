using System;

namespace Sick_test
{
    public class Scan{
        public PointXY[] pointsArray; 
        public DateTime time;

        public Scan(int count){
            pointsArray = new PointXY[count];
        }
        public Scan(){
        }
        public Scan copyScan(){
            var newScan = new Scan(pointsArray.Length);
            newScan.time = time;
            Array.Copy(pointsArray, newScan.pointsArray, pointsArray.Length);
            return newScan;
        }
    }
}
