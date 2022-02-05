namespace Sick_test
{
        public struct Vector{
        public double X,Y;

        static public Vector[] copyArray(Vector[] oldArray){
            var newScan = new Vector[oldArray.Length];
            oldArray.CopyTo(newScan, 0);
            return newScan;
        }
        static public Vector VectorFromPoints(PointXY startpoint, PointXY endPoint){
            var newVector = new Vector();
            newVector.X = endPoint.X - startpoint.X;
            newVector.Y = endPoint.Y - startpoint.Y;
            return newVector;
        }
    }
}