namespace Sick_test
{
    class translator
    {
        PointXY ZeroPoint;
        public translator(PointXY zeropoint)
        {
            ZeroPoint = zeropoint;
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
    }
}