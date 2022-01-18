namespace Sick_test
{
    public struct PointXY{
        public double X,Y;

        static public PointXY[] copyScan(PointXY[] oldArray){
            var newScan = new PointXY[oldArray.Length];
            oldArray.CopyTo(newScan, 0);
            return newScan;
        }
    }
}