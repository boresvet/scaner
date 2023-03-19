using System;
using System.Linq;

namespace Sick_test
{
	///<summary>Объявляет скан с массивом указанной длинны
    ///</summary>   
    public class ScanColumnArray{
        private config _config;
        PointXYint[][] InterFace;
		private int[] leanth;
		private int scanleanth;
        private int index;
        private double invertStep;

        public ScanColumnArray(config config){
            _config = config;
            invertStep = 1.0 / config.RoadSettings.Step;
			scanleanth = 0;
			foreach(Scanner scanner in config.Scanners){
				scanleanth += (int)((float)(scanner.Settings.EndAngle-scanner.Settings.StartAngle)/scanner.Settings.Resolution);
			}
			InterFace = new PointXYint[scanleanth][];
            leanth = new int[scanleanth];
            for(int i = 0; i < InterFace.Length; i++){
                leanth[i] = 0;
                InterFace[i] = new PointXYint[0];
            }
        }
		public bool IsEmpty(int i){
			return leanth[i] == 0;
		}
        private void injectItem(int index, PointXYint point){
            if(InterFace[index].Length >= leanth[index]){
                InterFace[index] = InterFace[index].Concat(point.ToArray()).ToArray();
                leanth[index]+=1;
            }else{
                InterFace[index][leanth[index]] = point;
                leanth[index]+=1;
            }

        }
        public void Add(PointXYint[] RoadScan){
            for(int i = 0; i < InterFace.Length; i++){
                leanth[i] = 0;
            }
            foreach(PointXYint i in RoadScan){
                index = (int)((double)(i.X - _config.RoadSettings.LeftLimit) * invertStep);
                injectItem(index, i);
            }
		}
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