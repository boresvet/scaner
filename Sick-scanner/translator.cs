using System.Collections.Generic;
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
        ///<param name = "input">Точка, которая переносится в начало координат</param>
        ///</summary>   
        public PointXY translate(PointXY input){
            input.X = input.X + ZeroPoint.X;
            input.Y = ZeroPoint.Y - input.Y;
            return input;
        }
        ///<summary>Перенос точки в новую систему
        ///<param name = "input">Точка, которая переносится в начало координат</param>
        ///</summary>  
        public PointXYint translate(PointXYint input){
            input.X = input.X + ZeroPointint.X;
            input.Y = ZeroPointint.Y - input.Y;
            return input;
        }
        ///<summary>Перенос точки в новую систему
        ///<param name = "input">Точка, которая переносится в начало координат</param>
        ///</summary>   
        public void translatev(PointXY input){
            input.X = input.X + ZeroPoint.X;
            input.Y = ZeroPoint.Y - input.Y;
        }
        ///<summary>Перенос точки в новую систему
        ///<param name = "input">Точка, которая переносится в начало координат</param>
        ///</summary>  
        public void translatev(PointXYint input){
            input.X = input.X + ZeroPointint.X;
            input.Y = ZeroPointint.Y - input.Y;
        }
        ///<summary>Перенос массива точек в новую систему
        ///<param name = "oldArray">Массив, который переносится в начало координат</param>
        ///</summary>  
        public PointXY[] Translate(PointXY[] oldArray){
            return oldArray.Select(n => translate(n)).ToArray();
        }
        ///<summary>Перенос массива точек в новую систему
        ///<param name = "oldArray">Массив, который переносится в начало координат</param>
        ///</summary>  
        public List<PointXY> Translate(List<PointXY> oldArray){
            return oldArray.ConvertAll(n => translate(n));
        }
        ///<summary>Перенос массива точек в новую систему
        ///<param name = "oldArray">Массив, который переносится в начало координат</param>
        ///</summary>  
        public PointXYint[] Translate(PointXYint[] oldArray){
            return oldArray.Select(n => translate(n)).ToArray();
        }
        ///<summary>Перенос массива точек в новую систему
        ///<param name = "oldArray">Массив, который переносится в начало координат</param>
        ///</summary>  
        public List<PointXYint> Translate(List<PointXYint> oldArray, List<PointXYint> newArray){
            newArray.Clear();
            oldArray.ForEach(n => newArray.Add(translate(n)));
            return newArray;
        }
    }
}