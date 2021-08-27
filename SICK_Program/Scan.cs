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
    }
}
