namespace Sick_test
{
    class translator
    {
        PointXY ZeroPoint;
        PointXYint ZeroPointint;

        public translator(PointXY zeropoint)
        {
            ZeroPoint = zeropoint;
        }
        public translator(PointXYint zeropoint)
        {
            ZeroPointint = zeropoint;
        }
        public PointXY[] Translate(PointXY[] oldArray){
            var newScan = new PointXY[oldArray.Length];
            oldArray.CopyTo(newScan, 0);
            for(int i = 0; i<newScan.Length; i++){
                newScan[i].X = newScan[i].X + ZeroPoint.X;
                newScan[i].Y = newScan[i].Y + ZeroPoint.Y;
            }
            return newScan;
        }
        public PointXYint[] Translate(PointXYint[] oldArray){
            var newScan = new PointXYint[oldArray.Length];
            oldArray.CopyTo(newScan, 0);
            for(int i = 0; i<newScan.Length; i++){
                newScan[i].X = newScan[i].X + ZeroPointint.X;
                newScan[i].Y = newScan[i].Y + ZeroPointint.Y;
            }
            return newScan;
        }
    }
}