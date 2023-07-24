using System;
using static System.Math;
using System.IO;
using System.Linq;


namespace Sick_test
{




    public interface Filter{
        ///<summary> алгоритм поиска машинки </summary>
		public PointXYint[][] FilterPoints(PointXYint[][] array);
        public int[] CarPoints(PointXYint[][] array);


    }
    ///<summary>Класс, убирает все точки, находящиеся в слепых зонах дороги. Фильтруются: точки препятствик(отбойники, заграждения, знаки), и точки ошибочные(выходящие за границы дороги)
    ///</summary>
    public class PrimitiveFilter: Filter{
        private int RoadLenght;
        private int[] filterMax;
        private int[] filterMin;
        private RoadSetting Settings;
        private int FilteredHeight;
        ///<summary>///Формирует массивы граничных значений для каждого Х
        ///<param name = "roadLenght">Ширина дороги (количество столбцов)</param>
        ///<param name = "settings">Конфигурация дороги</param>
        ///</summary>
        public PrimitiveFilter(config settings)
		{
            FilteredHeight = settings.FilteredHeight;
            RoadLenght = (int)((settings.RoadSettings.RightLimit-settings.RoadSettings.LeftLimit)/settings.RoadSettings.Step);
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
                    return j.Height;
                }
            }
            foreach(Lane j in Settings.Lanes){
                if(((Settings.LeftLimit+(((Settings.RightLimit-Settings.LeftLimit)/RoadLenght)*indexofrange))>j.Offset)&((Settings.LeftLimit+(((Settings.RightLimit-Settings.LeftLimit)/RoadLenght)*indexofrange))<(j.Offset+j.Width))){
                    return j.Height+FilteredHeight;
                }
            }
            return FilteredHeight;
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

            return AddAllIslandLane(retArray);
        }
        public static int[] AddAllIslandLane(int[] input){
            /*
            Эта штука "дозаполняет" все имеющиеся точки дороги до монолитного результата, удаляя все бесконечности. 
            То есть с начала - она от левого края идёт до ближайшей точки. Находит точку, и массив до ней дозаполняет по правилам(см. AddLineIsland)
            Потом - от первой точки до второй, и т.д. 
            И так до самого конца, чтобы получить полноценную картину распределения точек
            */
            var start = 0;
            for(int i = 0; i < input.Length; i++){
                if((input[i]>=0)|(i==input.Length-1)){
                    AddLineIsland(input,start,i);
                    start = i;
                }

            }
            return input.ToArray();
        }


        private static int[] AddLineIsland(int[] input, int startpoint, int endpoint){

            /*
            Тут происходит дозапоолнение массива между двумя точками. 
            На входе: три варианта значения
            -1 - в этом столбе нет ни одной точки
            0 - в этом столбе есть точки, но все они являются "землёй"
            >0 - в этом столбе есть точка "машины" - даже если есть и другие точки, то сохранена только самая высокая



            Фишка в том, что такой метод позволит обработать практический любые потери данных. 
            Если на дороге лужа - то точек в неё не попадает, и так как мы берём ближайшие точки, и "дозаполняем" лужу данными. 
            Если крыша машины слишком сильно блестит - то то же самое. 
            Если с одной стороны "провала" есть машина, а с другой её нет, то все точки заполняются "землёй"
            Если с одной стороны бесконечность(то есть край дороги), то все точки до ближайшей считаются землёй
            */
            var start = input[startpoint];
            var end = input[endpoint];
            if((start == -1)&(end == 0)){
                Array.Fill(input, 0, startpoint, endpoint-startpoint);
            }
            if((start == -1)&(end > 0)){
                Array.Fill(input, 0, startpoint, endpoint-startpoint);
            }
            if((start == -1)&(end == -1)){
                Array.Fill(input, 0, startpoint, endpoint-startpoint+1);
            }
            if((start == 0)&(end == -1)){
                Array.Fill(input, 0, startpoint+1, endpoint-startpoint);
            }
            if((start == 0)&(end == 0)){
                Array.Fill(input, 0, startpoint+1, endpoint-startpoint);
            }
            if((start == 0)&(end > 0)){
                Array.Fill(input, 0, startpoint, endpoint-startpoint);
            }
            if((start > 0)&(end == 0)){
                Array.Fill(input, 0, startpoint+1, endpoint-startpoint);
            }
            if((start > 0)&(end > 0)){
                for(int i = startpoint; i < endpoint; i++){
                    input[i] = start + ((end-start)/input.Length*i);
                }
            }
            if((start > 0)&(end == -1)){
                Array.Fill(input, 0, startpoint+1, endpoint-startpoint);
            }
            return input;
        }
        ///<summary>///Возвращает самую верхнюю точкку в столбце
        ///<param name = "array">Столбец</param>
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
    public class AutomaticPrimitiveFilter: Filter{
        private int RoadLenght;
        private int[] filterMax;
        private int[] filterMin;
        private int[] ground;

        private RoadSetting Settings;
        private int FilteredHeight;
        ///<summary>///Формирует массивы граничных значений для каждого Х
        ///<param name = "roadLenght">Ширина дороги (количество столбцов)</param>
        ///<param name = "settings">Конфигурация дороги</param>
        ///</summary>
        public AutomaticPrimitiveFilter(config settings)
		{
            FilteredHeight = settings.FilteredHeight;
            RoadLenght = (int)((settings.RoadSettings.RightLimit-settings.RoadSettings.LeftLimit)/settings.RoadSettings.Step);
            Settings = settings.RoadSettings;
            filterMax = new int[RoadLenght];
            filterMin = new int[RoadLenght];
            for(int i = 0; i<RoadLenght; i++){
                filterMax[i] = settings.RoadSettings.UpLimit;
                filterMin[i] = startGroundFilter(i);
                ground[i] = groundgen(i);
            }
		}
        private int startGroundFilter(int indexofrange){
            foreach(Blind j in Settings.Blinds){
                if(((Settings.LeftLimit+(((Settings.RightLimit-Settings.LeftLimit)/RoadLenght)*indexofrange))>j.Offset)&((Settings.LeftLimit+(((Settings.RightLimit-Settings.LeftLimit)/RoadLenght)*indexofrange))<(j.Offset+j.Width))){
                    return j.Height+FilteredHeight;
                }
            }
            return Settings.DownLimit;
        }
        private int groundgen(int indexofrange){
            foreach(Blind j in Settings.Blinds){
                if(((Settings.LeftLimit+(((Settings.RightLimit-Settings.LeftLimit)/RoadLenght)*indexofrange))>j.Offset)&((Settings.LeftLimit+(((Settings.RightLimit-Settings.LeftLimit)/RoadLenght)*indexofrange))<(j.Offset+j.Width))){
                    return j.Height;
                }
            }
            foreach(Lane j in Settings.Lanes){
                if(((Settings.LeftLimit+(((Settings.RightLimit-Settings.LeftLimit)/RoadLenght)*indexofrange))>j.Offset)&((Settings.LeftLimit+(((Settings.RightLimit-Settings.LeftLimit)/RoadLenght)*indexofrange))<(j.Offset+j.Width))){
                    return j.Height;
                }
            }
            return Settings.DownLimit;
        }
        private bool ispointfiltered(PointXYint point, int i){
            return (point.Y<filterMax[i])&(point.Y>filterMin[i]);
        }
        ///<summary>///Возвращает массив столбцов, фильтруя лишние точки
        ///<param name = "array">Массив столбцов</param>
        ///</summary>
		public PointXYint[][] FilterPoints(PointXYint[][] array){
            var retArray = new PointXYint[array.Length][];
			for(int i = 0; i<array.Length; i++){
                retArray[i] = Array.FindAll(array[i], point => (ispointfiltered(point, i)));
            }
            return retArray;
		}
        ///<summary>///Возвращает массив максимальной высоты машины. 0 - в этой точке находится земля, -1 - в этом столбце точек нет. 
        ///<param name = "array">Массив столбцов</param>
        ///</summary>rewritegroundpiece
        public int[] CarPoints(PointXYint[][] array){
            var retArray = new int[array.Length];
            int oldi = 0;
			for(int i = 0; i<array.Length; i++){
                if(Array.FindAll(array[i], point => (point.Y<filterMax[i])&(point.Y<(ground[i]+FilteredHeight))).Length == 0){
                    //добавление к земле всех пойманных точек с к-том реальности 0.001
                    ground[i] = (int)((float)ground[i]*(1.000-0.001*(Array.FindAll(array[i], point => (point.Y<filterMax[i])&(point.Y<(ground[i]+FilteredHeight))).Length))+((double)Array.FindAll(array[i], point => (point.Y<filterMax[i])&(point.Y<(ground[i]+FilteredHeight))).Sum<PointXYint>(point => (point.Y))));
                    oldi = i;
                }
                if(i != oldi){
                    ground = rewritegroundpiece(ground, oldi, i);
                }
            }

			for(int i = 0; i<array.Length; i++){
                if(array[i].Length == 0){
                    retArray[i] = -1;
                }else{
                if(Array.FindAll(array[i], point => (point.Y<filterMax[i])&(point.Y>(ground[i]+FilteredHeight))).Length == 0){
                        retArray[i] = 0;
                    }else{
                        retArray[i] = maxPoint(Array.FindAll(array[i], point => (point.Y<filterMax[i])&(point.Y>filterMin[i])));
                    }
                }
            }

            return AddAllIslandLane(retArray);
        }
        public static int[] AddAllIslandLane(int[] input){
            /*
            Эта штука "дозаполняет" все имеющиеся точки дороги до монолитного результата, удаляя все бесконечности. 
            То есть с начала - она от левого края идёт до ближайшей точки. Находит точку, и массив до ней дозаполняет по правилам(см. AddLineIsland)
            Потом - от первой точки до второй, и т.д. 
            И так до самого конца, чтобы получить полноценную картину распределения точек
            */
            var start = 0;
            for(int i = 0; i < input.Length; i++){
                if((input[i]>=0)|(i==input.Length-1)){
                    AddLineIsland(input,start,i);
                    start = i;
                }

            }
            return input.ToArray();
        }

        private static int[] rewritegroundpiece(int[] input, int startpoint, int endpoint){
            var start = input[startpoint];
            var end = input[endpoint];
            for(int i = startpoint; i < endpoint; i++){
                input[i] = start + ((end-start)/input.Length*i);
            }
            return input;
        }
        private static int[] AddLineIsland(int[] input, int startpoint, int endpoint){

            /*
            Тут происходит дозапоолнение массива между двумя точками. 
            На входе: три варианта значения
            -1 - в этом столбе нет ни одной точки
            0 - в этом столбе есть точки, но все они являются "землёй"
            >0 - в этом столбе есть точка "машины" - даже если есть и другие точки, то сохранена только самая высокая



            Фишка в том, что такой метод позволит обработать практический любые потери данных. 
            Если на дороге лужа - то точек в неё не попадает, и так как мы берём ближайшие точки, и "дозаполняем" лужу данными. 
            Если крыша машины слишком сильно блестит - то то же самое. 
            Если с одной стороны "провала" есть машина, а с другой её нет, то все точки заполняются "землёй"
            Если с одной стороны бесконечность(то есть край дороги), то все точки до ближайшей считаются землёй
            */
            var start = input[startpoint];
            var end = input[endpoint];
            if((start == -1)&(end == 0)){
                Array.Fill(input, 0, startpoint, endpoint-startpoint);
            }
            if((start == -1)&(end > 0)){
                Array.Fill(input, 0, startpoint, endpoint-startpoint);
            }
            if((start == -1)&(end == -1)){
                Array.Fill(input, 0, startpoint, endpoint-startpoint+1);
            }
            if((start == 0)&(end == -1)){
                Array.Fill(input, 0, startpoint+1, endpoint-startpoint);
            }
            if((start == 0)&(end == 0)){
                Array.Fill(input, 0, startpoint+1, endpoint-startpoint);
            }
            if((start == 0)&(end > 0)){
                Array.Fill(input, 0, startpoint, endpoint-startpoint);
            }
            if((start > 0)&(end == 0)){
                Array.Fill(input, 0, startpoint+1, endpoint-startpoint);
            }
            if((start > 0)&(end > 0)){
                for(int i = startpoint; i < endpoint; i++){
                    input[i] = start + ((end-start)/input.Length*i);
                }
            }
            if((start > 0)&(end == -1)){
                Array.Fill(input, 0, startpoint+1, endpoint-startpoint);
            }
            return input;
        }
        ///<summary>///Возвращает самую верхнюю точкку в столбце
        ///<param name = "array">Столбец</param>
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