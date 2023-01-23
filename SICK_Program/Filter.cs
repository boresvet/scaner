using System;

namespace Sick_test
{
    ///<summary>Класс, убирает все точки, находящиеся в слепых зонах дороги. Фильтруются: точки препятствик(отбойники, заграждения, знаки), и точки ошибочные(выходящие за границы дороги)
    ///</summary>
    public class Filter{
        private int RoadLenght;
        private int[] filterMax;
        private int[] filterMin;
        private RoadSetting Settings;
        private int FilteredHeight;
        ///<summary>///Формирует массивы граничных значений для каждого Х
        ///<param name = "roadLenght">Ширина дороги (количество столбцов)</param>
        ///<param name = "settings">Конфигурация дороги</param>
        ///</summary>
        public Filter(int roadLenght, config settings)
		{
            FilteredHeight = settings.FilteredHeight;
            RoadLenght = roadLenght;
            Settings = settings.RoadSettings;
            filterMax = new int[RoadLenght];
            filterMin = new int[RoadLenght];
            for(int i = 0; i<RoadLenght; i++){
                filterMax[i] = settings.RoadSettings.UpLimit;
                filterMin[i] = minFilter(i);
            }
		}
        private int minFilter(int indexofrange){
            foreach(Blind j in Settings.Blinds){
                if(((Settings.LeftLimit+(((Settings.RightLimit-Settings.LeftLimit)/RoadLenght)*indexofrange))>j.Offset)&((Settings.LeftLimit+(((Settings.RightLimit-Settings.LeftLimit)/RoadLenght)*indexofrange))<(j.Offset+j.Width))){
                    return j.Height + FilteredHeight;
                }
            }
            return Settings.DownLimit + FilteredHeight;
        }
        ///<summary>///Возвращает массив столбцов, фильтруя лишние точки
        ///<param name = "array">Массив столбцов</param>
        ///</summary>
		public PointXYint[][] FilterPoints(PointXYint[][] array){
            var retArray = new PointXYint[array.Length][];
			for(int i = 0; i<array.Length; i++){
                retArray[i] = Array.FindAll(array[i], point => (point.Y<filterMax[i])&(point.Y>filterMin[i]));
            }
            return retArray;
		}
        ///<summary>///Возвращает массив максимальной высоты машины. 0 - в этой точке находится земля, -1 - в этом столбце точек нет. 
        ///<param name = "array">Массив столбцов</param>
        ///</summary>
        public int[] CarPoints(PointXYint[][] array){
            var retArray = new int[array.Length];
			for(int i = 0; i<array.Length; i++){
                if(array[i].Length == 0){
                    retArray[i] = -1;
                }else{
                    if(Array.FindAll(array[i], point => (point.Y<filterMax[i])&(point.Y>filterMin[i])).Length == 0){
                        retArray[i] = 0;
                    }else{
                        retArray[i] = maxPoint(Array.FindAll(array[i], point => (point.Y<filterMax[i])&(point.Y>filterMin[i])));
                    }
                }
            }
            return retArray;
        }
        ///<summary>///Возвращает самую верхнюю точкку в столбце
        ///<param name = "array">Столбуц</param>
        ///</summary>
        private int maxPoint(PointXYint[] array){
            var retint = 0;
            foreach(PointXYint j in array){
                if(j.Y > retint){
                    retint = j.Y;
                }
            }
            return retint;
        }
    }
}