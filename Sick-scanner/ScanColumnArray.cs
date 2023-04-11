using System;
using System.Linq;

namespace Sick_test
{
	///<summary> Нарезает данные со сканеров в таблицу столбцов
    ///</summary>   
    public class ScanColumnArray{
        private config _config;
        private PointXYint[][] InterFace;
		private int[] leanth;
		private int scanleanth;
        private int index;
        private double invertStep;
		///<summary> Нарезает данные со сканеров в таблицу столбцов </summary>

        public ScanColumnArray(config config){
            _config = config;
            invertStep = 1.0 / config.RoadSettings.Step;
			scanleanth =  (int)((config.RoadSettings.RightLimit - config.RoadSettings.LeftLimit)/config.RoadSettings.Step);
			InterFace = new PointXYint[scanleanth][];
            leanth = new int[scanleanth];
            for(int i = 0; i < InterFace.Length; i++){
                leanth[i] = 0;
                InterFace[i] = new PointXYint[0];
            }
        }
		///<summary> Проверка на то, является ли буфер пустым </summary>
		public bool IsEmpty(int i){
			return leanth[i] == 0;
		}
        private void injectItem(int index, PointXYint point){
            if((index >= InterFace.Length)||(index<0)){
                return;
            }
            if(InterFace[index].Length <= leanth[index]){
                InterFace[index] = InterFace[index].Concat(point.ToArray()).ToArray();
                leanth[index]+=1;
            }else{
                InterFace[index][leanth[index]] = point;
                leanth[index]+=1;
            }

        }
		///<summary> Нарезка данных в столбцы </summary>
        public void Add(PointXYint[] RoadScan){
            for(int i = 0; i < InterFace.Length; i++){
                leanth[i] = 0;
            }
            foreach(PointXYint i in RoadScan){
                index = (int)((double)(i.X - _config.RoadSettings.LeftLimit) * invertStep);
                injectItem(index, i);
            }
		}
		///<summary> Получение копии таблицы с данными </summary>
		public PointXYint[][] Dequeue(){
			var x = new PointXYint[scanleanth][]; 
            for(int i = 0; i < InterFace.Length; i++){
                if(leanth[i]>0){
                    x[i] = new PointXYint[leanth[i]];
                    Array.Copy(InterFace[i], 0, x[i], 0, leanth[i]);
                }else{
                    x[i] = new PointXYint[0];
                }
                leanth[i] = 0;
            }
			return x;
		}

        ///<summary>Сортировка массива точек по оси Х, от начала и до конца
        ///<param name = "array">Массив сортируемых точек</param>
        ///</summary>   
    }
}