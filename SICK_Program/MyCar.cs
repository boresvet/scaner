using System;
using static System.Math;
namespace Sick_test
{
    public class MyCar
    {   public Scan CarScan;
        public int Step;
        public Scan sampleScan;
        public MyCar(int size)
        {
            CarScan = new Scan(size);
            Step = size;
            sampleScan = new Scan(size);
        }
        public PointXY[] CreatCarScan(PointXY[] newScan, PointXY[] myGround){
            var retArray = new PointXY[Step];
            for(int i = 0; i <Step; i++){
                if((myGround[i].X-newScan[i].X)*(myGround[i].X-newScan[i].X)
                +(myGround[i].Y-newScan[i].Y)*(myGround[i].Y-newScan[i].Y)>=0.25){
                    retArray[i].X = myGround[i].X-newScan[i].X;
                    retArray[i].Y = myGround[i].Y-newScan[i].Y;
                }else{
                    retArray[i].X = 0;
                    retArray[i].Y = 0;
                }
            }
            return retArray;
        }
        public bool SeeCar(PointXY[] carScan){
            int trigger = 0;
            for(int i = 0; i<carScan.Length; i++){
                if((Math.Sqrt((carScan[i].Y*carScan[i].Y)+(carScan[i].X*carScan[i].X))) >=0.01){
                    trigger++;
                    if(trigger >=10){
                        return true;
                    }
                }
            }
            return false;
            //return sampleScan.pointsArray == carScan;
        }
    }
}