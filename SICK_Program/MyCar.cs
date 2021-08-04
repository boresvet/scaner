namespace Sick_test
{
    public class MyCar
    {   public Scan CarScan;
        public int Step;
        public MyCar(int size)
        {
            CarScan = new Scan(size);
            Step = size;
        }
        public PointXY[] CreatCarScan(PointXY[] newScan){
            var retArray = new PointXY[Step];

            return retArray;
        }
    }
}