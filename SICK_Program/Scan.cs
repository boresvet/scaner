using System;

namespace Sick_test
{
    ///<summary>///Описывает один скан, как массив точек и время, ему соответствующее
    ///</summary>
    public class Scan{
        public PointXY[] pointsArray; 
        public DateTime time;
        ///<summary>///Объявляет скан с массивом указанной длинны
        ///<param name = "count">Длина массива точек</param>
        ///</summary>    
        public Scan(int count){
            pointsArray = new PointXY[count];
        }
        ///<summary>///Объявляет пустой скан
        ///</summary>   
        public Scan(){
        }
        ///<summary>///Возвращает копию скана
        ///</summary>   
        public Scan copyScan(){
            var newScan = new Scan(pointsArray.Length);
            newScan.time = time;
            Array.Copy(pointsArray, newScan.pointsArray, pointsArray.Length);
            return newScan;
        }
    }
    ///<summary>///Описывает один скан, как массив точек и время, ему соответствующее
    ///</summary>
    public class Scanint{
        public PointXYint[] pointsArray; 
        public DateTime time;
        ///<summary>///Объявляет скан с массивом указанной длинны
        ///<param name = "count">Длина массива точек</param>
        ///</summary>    
        public Scanint(int count){
            pointsArray = new PointXYint[count];
        }
        ///<summary>///Объявляет пустой скан
        ///</summary>   
        public Scanint(){
        }
        ///<summary>///Возвращает копию скана
        ///</summary>   
        public Scanint copyScan(){
            var newScan = new Scanint(pointsArray.Length);
            newScan.time = time;
            Array.Copy(pointsArray, newScan.pointsArray, pointsArray.Length);
            return newScan;
        }
    }
}
