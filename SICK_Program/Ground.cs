//using BSICK.Sensors.LMS1xx;
using System;
namespace Sick_test
{
    public class Ground{
        public Scan GroundScan;
        public int Step;
        private SpetialConvertor convertor;
        public double[] RawGroundData;

        public Ground(int size, int begingrade, int endgrade){
            GroundScan = new Scan(size);
            Step = size;
            convertor = new SpetialConvertor(begingrade, endgrade, Step);
        }
        public double[] RawScanConvertor(PointXY[] ScanData){
            var RawData = new double[Step];
            for(int i = 0; i<Step; i++){
                RawData[i] = Math.Sqrt((ScanData[i].X*ScanData[i].X)+(ScanData[i].Y*ScanData[i].Y));
            }
            return RawData;
        }
        /*public void addScan(Scan newScan){
            foreach(i in newScan){
                
            }
        }*/
        /*public Scan MyGround(CircularBuffer<Scan> MyCircularBuffer){
            var scan = new Scan(Step);
            //= MyCircularBuffer.ReadPosition();
            var ArrayPointsIndex = new int[Step];
            var leanth = MyCircularBuffer.MyLeanth;
            for(int j = 1; j<leanth; j++){
            var myscan = MyCircularBuffer.ReadPosition();
                for (int i = 0; i < Step; i++){
                    if(scan.pointsArray[i].Y > myscan.pointsArray[i].Y){
                        scan.pointsArray[i].X = myscan.pointsArray[i].X;
                        scan.pointsArray[i].Y = myscan.pointsArray[i].Y;
                    }
                }
                MyCircularBuffer.NextPosition();
            }
            //Console.WriteLine(ArrayPointsIndex[100]);
            return scan;
        }*/
        /*public Scan MyGround(CircularBuffer<Scan> MyCircularBuffer, Scan Oldscan){
            var scan = new Scan(Step);
            var myscan = MyCircularBuffer.ReadPosition();
            for (int i = 0; i < Step; i++){
                if(Oldscan.pointsArray[i].Y > myscan.pointsArray[i].Y){
                    scan.pointsArray[i].X = myscan.pointsArray[i].X;
                    scan.pointsArray[i].Y = myscan.pointsArray[i].Y;
                } else {
                    scan.pointsArray[i].X = Oldscan.pointsArray[i].X;
                    scan.pointsArray[i].Y = Oldscan.pointsArray[i].Y;
                }
            }
            return scan;
        }*/
        public void UpdateGround(PointXY[] newData, PointXY[] carData){
            //var ret = new double[Step];
            for (int i = 0; i < Step; i++){
                if((Math.Sqrt((newData[i].X*newData[i].X)+(newData[i].Y*newData[i].Y)) >= 0.01)&((carData[i].X*carData[i].X+carData[i].Y*carData[i].Y)<0.5)){
                    GroundScan.pointsArray[i].X = GroundScan.pointsArray[i].X*0.999+newData[i].X*0.001;
                    GroundScan.pointsArray
                    [i].Y = GroundScan.pointsArray[i].Y*0.999+newData[i].Y*0.001;
                }
            }
            GroundScan.pointsArray = convertor.MakePoint(RawGroundData);
            //return ret;
        }
        public void UpdateGround(double[] newRawData){
            //var ret = new double[Step];
            for (int i = 0; i < Step; i++){
                if((int)newRawData[i] >= 0.01){
                    RawGroundData[i] = RawGroundData[i]*0.999+newRawData[i]*0.001;
                }
            }
            GroundScan.pointsArray = convertor.MakePoint(RawGroundData);
            //return ret;
        }
        /*public void UpdateGround(PointXY[] groundscan, PointXY[] datascan){

        }*/
        public double[] MyGround(double[] newRawData){
            UpdateGround(newRawData);
            return RawGroundData;
        }
        public PointXY[] MyGround(){
            return GroundScan.pointsArray;
        }
        public void InitGround(double[] rawData){
            GroundScan.pointsArray = convertor.MakePoint(rawData);
            RawGroundData = rawData;
            //return rawData;
        }
        public void InitGround(PointXY[] ScanData){
            GroundScan.pointsArray = ScanData;
            RawGroundData = RawScanConvertor(ScanData);
            //return rawData;
        }
        /*public PointXY[] MyGround(Scan MyNewScan, PointXY[] MyPointsArray){
            var ret = new Scan(Step);
                for (int i = 0; i < Step; i++){
                if(MyNewScan.pointsArray[i].Y < MyPointsArray[i].Y){
                    ret.pointsArray[i].X = MyPointsArray[i].X;
                    ret.pointsArray[i].Y = MyPointsArray[i].Y;
                } else {
                    ret.pointsArray[i].X = MyNewScan.pointsArray[i].X;
                    ret.pointsArray[i].Y = MyNewScan.pointsArray[i].Y;
                }
            }
            return ret.pointsArray;
        }
        public PointXY[] InitGround(PointXY[] points){
            //var ret = new Scan(Step);
            GroundScan.pointsArray = points;
            return points;
        }*/
    }
}