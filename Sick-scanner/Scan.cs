using System;
using System.Collections.Generic;
namespace Sick_test
{
    ///<summary>///Описывает один скан, как массив точек и время, ему соответствующее
    ///</summary>
    public class Scan{
        protected List<PointXY> pointsArray; 
        protected DateTime time;
        ///<summary>///Объявляет скан со списком указанной длинны
        ///<param name = "capacity">Ёмкость списка точек</param>
        ///</summary>    
        public Scan(int capacity){
            pointsArray = new List<PointXY>(capacity);
        }
        ///<summary>///Объявляет пустой скан
        ///</summary>   
        public Scan(){
        }
        ///<summary>///Возвращает список точек
        ///</summary>   
        public List<PointXY> PointsArray(){
            return pointsArray;
        }

        public DateTime Time(){
            return time;
        }
        ///<summary>///Сохраняет время             
        ///<param name = "_time">время, которое будет сохранено</param>
        ///</summary>   
        public void WriteTime(DateTime _time){
            time = _time;
        }
        ///<summary>///Сохраняет список точек
        ///<param name = "_pointsarray">Список точек, который будет сохранён/param>
        ///</summary>   
        public void WritePointsArray(List<PointXY> _pointsarray){
            pointsArray.Clear();
            _pointsarray.ForEach((item)=>
            {
                pointsArray.Add(new PointXY(item));
            });
        }
        ///<summary>///Добавляет к текущему списку точки из переданного списка точек
        ///<param name = "_pointsarray">Список точек, который будет сохранён/param>
        ///</summary>   
        public void UpdatePointsArray(List<PointXY> _pointsarray){
            _pointsarray.ForEach((item)=>
            {
                pointsArray.Add(new PointXY(item));
            });
        }
        ///<summary>///Возвращает копию скана
        ///</summary>   
        public Scan copyScan(){
            var newScan = new Scan(pointsArray.Capacity);
            newScan.WriteTime(time);
            newScan.WritePointsArray(pointsArray);
            return newScan;
        }
    }
    ///<summary>///Описывает один скан, как массив точек и время, ему соответствующее
    ///</summary>
    public class Scanint{
        public List<PointXYint> pointsArray; 
        public DateTime time;
        ///<summary>///Объявляет скан со списком указанной длинны
        ///<param name = "capacity">Ёмкость списка точек</param>
        ///</summary>    
        public Scanint(int capacity){
            pointsArray = new List<PointXYint>(capacity);
        }
        ///<summary>///Объявляет пустой скан
        ///</summary>   
        public Scanint(){
        }
        ///<summary>///Возвращает список точек
        ///</summary>   
        public List<PointXYint> PointsArray(){
            return pointsArray;
        }

        public DateTime Time(){
            return time;
        }
        ///<summary>///Сохраняет время             
        ///<param name = "_time">время, которое будет сохранено</param>
        ///</summary>   
        public void WriteTime(DateTime _time){
            time = _time;
        }
        ///<summary>///Сохраняет список точек
        ///<param name = "_pointsarray">Список точек, который будет сохранён/param>
        ///</summary>           ///<summary>///Сохраняет список точек
        ///<param name = "_pointsarray">Список точек, который будет сохранён/param>
        ///</summary>   
        public void WritePointsArray(List<PointXYint> _pointsarray){
            pointsArray.Clear();
            _pointsarray.ForEach((item)=>
            {
                pointsArray.Add(new PointXYint(item));
            });
        }
        ///<summary>///Добавляет к текущему списку точки из переданного списка точек
        ///<param name = "_pointsarray">Список точек, который будет сохранён/param>
        ///</summary>   
        public void UpdatePointsArray(List<PointXYint> _pointsarray){
            _pointsarray.ForEach((item)=>
            {
                pointsArray.Add(new PointXYint(item));
            });
        }
        ///<summary>///Возвращает копию скана
        ///</summary>   
        public Scanint copyScan(){
            var newScan = new Scanint(pointsArray.Capacity);
            newScan.WriteTime(time);
            newScan.WritePointsArray(pointsArray);
            return newScan;
        }
    }
}
