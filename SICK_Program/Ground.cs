using BSICK.Sensors.LMS1xx;

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
        public void UpdateGround(double[] newRawData){
            //var ret = new double[Step];
            for (int i = 0; i < Step; i++){
                if(RawGroundData[i] < newRawData[i]){
                    RawGroundData[i] = newRawData[i];
                }
            }
            GroundScan.pointsArray = convertor.MakePoint(RawGroundData);
            //return ret;
        }
        public double[] MyGround(double[] newRawData){
            UpdateGround(newRawData);
            return RawGroundData;
        }
        public void InitGround(double[] rawData){
            GroundScan.pointsArray = convertor.MakePoint(rawData);
            RawGroundData = rawData;
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