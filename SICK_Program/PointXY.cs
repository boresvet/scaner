namespace Sick_test
{
    public struct PointXY{
        public double X,Y;

        static public PointXY[] copyScan(PointXY[] oldArray){
            var newScan = new PointXY[oldArray.Length];
            oldArray.CopyTo(newScan, 0);
            return newScan;
        }
        public string ToString(){
            return (@"{""X"": " + X.ToString() + @", ""Y"": " + Y.ToString()+"}");
        }
    }


    public struct PointXYint{
        public int X,Y;

        static public PointXY[] copyScan(PointXY[] oldArray){
            var newScan = new PointXY[oldArray.Length];
            oldArray.CopyTo(newScan, 0);
            return newScan;
        }
        public string ToString(){
            return (@"{""X"": " + X.ToString() + @", ""Y"": " + Y.ToString()+"}");
        }
        public PointXYint[] ToArray(){
            var ret = new PointXYint[1];
            ret[0].X = X;
            ret[0].Y = Y;
            return ret;
        }
    }
}