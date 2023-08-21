using System.Linq;
namespace Sick_test
{
    ///<summary>Перевод начала координат в точку ХУ
    ///</summary>   
    public class translator
    {
        PointXY ZeroPoint;
        PointXYint ZeroPointint;
        ///<summary>Объявление точки, в которую переносится система координат
        ///<param name = "zeropoint">Точка, в которую переносится начало координат</param>
        ///</summary>   
        translator(PointXY zeropoint)
        {
            ZeroPoint = zeropoint;
        }
        ///<summary>Объявление точки, в которую переносится система координат
        ///<param name = "zeropoint">Точка, в которую переносится начало координат</param>
        ///</summary>   
        public translator(PointXYint zeropoint)
        {
            ZeroPointint = zeropoint;
        }
        ///<summary>Перенос точки в новую систему
        ///<param name = "zeropoint">Точка, которая переносится в начало координат</param>
        ///</summary>   
        public PointXY translate(PointXY input){
            input.X = input.X + ZeroPoint.X;
            input.Y = ZeroPoint.Y - input.Y;
            return input;
        }
        ///<summary>Перенос точки в новую систему
        ///<param name = "zeropoint">Точка, которая переносится в начало координат</param>
        ///</summary>  
        public PointXYint translate(PointXYint input){
            input.X = input.X + ZeroPointint.X;
            input.Y = ZeroPointint.Y - input.Y;
            return input;
        }
        ///<summary>Перенос массива точек в новую систему
        ///<param name = "zeropoint">Массив, который переносится в начало координат</param>
        ///</summary>  
        public PointXY[] Translate(PointXY[] oldArray){
            return oldArray.Select(n => translate(n)).ToArray();
        }
        ///<summary>Перенос массива точек в новую систему
        ///<param name = "zeropoint">Массив, который переносится в начало координат</param>
        ///</summary>  
        public PointXYint[] Translate(PointXYint[] oldArray){
            return oldArray.Select(n => translate(n)).ToArray();
        }
    }
}