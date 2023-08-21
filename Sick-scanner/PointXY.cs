namespace Sick_test
{
    ///<summary>///Структура, содержит Х и У координаты точки
    ///</summary>
    public struct PointXY{
        public double X,Y;
        ///<summary>///Возвращает копию массива точек
        ///<param name = "oldArray">Старый массив, который нужно скопировать</param>
        ///</summary>
        static public PointXY[] copyScan(PointXY[] oldArray){
            var newScan = new PointXY[oldArray.Length];
            oldArray.CopyTo(newScan, 0);
            return newScan;
        }
        ///<summary>///Возвращает json строку, описыващую точку
        ///<param name = "array">Столбуц</param>
        ///</summary>        
        public override string ToString(){
            return (@"{""X"": " + X.ToString() + @", ""Y"": " + Y.ToString()+"}");
        }
        ///<summary>///Возвращает массив длинной 1, содержащий эту точку
        ///</summary>
        public PointXY[] ToArray(){
            var ret = new PointXY[1];
            ret[0].X = X;
            ret[0].Y = Y;
            return ret;
        }
    }

    ///<summary>///Структура, содержит Х и У координаты точки
    ///</summary>
    public struct PointXYint{
        public int X { get; set; }
        public int Y { get; set; }
        ///<summary>///Возвращает копию массива точек
        ///<param name = "oldArray">Старый массив, который нужно скопировать</param>
        ///</summary>
        static public PointXYint[] copyScan(PointXYint[] oldArray){
            var newScan = new PointXYint[oldArray.Length];
            oldArray.CopyTo(newScan, 0);
            return newScan;
        }
        ///<summary>///Возвращает json строку, описыващую точку
        ///<param name = "array">Столбуц</param>
        ///</summary>      
        public override string ToString(){
            return (@"{""X"": " + X.ToString() + @", ""Y"": " + Y.ToString()+"}");
        }
        ///<summary>///Возвращает массив длинной 1, содержащий эту точку
        ///</summary>
        public PointXYint[] ToArray(){
            var ret = new PointXYint[1];
            ret[0].X = X;
            ret[0].Y = Y;
            return ret;
        }
    }
}