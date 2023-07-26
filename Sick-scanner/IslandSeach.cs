using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Sick_test
{

    public interface ISearchAlgoritm{
        ///<summary> алгоритм поиска машинки </summary>
        public carRESULT search(islandborders input, SuperScan[] inputscans, config config);
    }
    public class PrimitiveSearchAlgoritm : ISearchAlgoritm{
        ///<summary> Примитивный алгоритм поиска машинки </summary>
        public carRESULT search(islandborders input, SuperScan[] inputscans, config config){
            var ret = new carRESULT();
            var lenthArray = new int[input.leftindexborders.Length];
            for(int i = 0; i < lenthArray.Length; i++){
                lenthArray[i] = input.rightindexborders[i] - input.leftindexborders[i];
            }
            Array.Sort(lenthArray);//Отсортированный массив ширины машины



            int index = 0;
            var HeightArray = new int[lenthArray.Sum()];
            for(int i = input.downborder; i < input.upborder; i++){
                for(int j = input.leftindexborders[i - input.downborder]; j < input.rightindexborders[i - input.downborder]; j++){
                    HeightArray[index] = inputscans[i].CarIslandLanes[j];
                    index++;
                }
            }
            Array.Sort(HeightArray);//Сортировка массива высот машинки
            if(lenthArray.Length>config.SortSettings.MinLength){
                ret.Starttime = inputscans[input.downborder].Time;
                ret.Endtime = inputscans[input.upborder].Time;
                if(HeightArray.Length == 0){
                    ret.Width = 0;
                    ret.Height = 0;
                    return ret;
                }
                ret.Width = (lenthArray[(int)(lenthArray.Length - (lenthArray.Length*0.05))]) * config.RoadSettings.Step;//Уже в миллиметрах
                ret.Height = HeightArray[(int)(HeightArray.Length - (HeightArray.Length*0.05))];
                //Отбрасываются максимальные точки, т.к. они скорее всего ошибка(та же антенна, + блики)
                
                if(config.SortSettings.MinWigdh>ret.Width){
                    ret.Width = 0;
                    ret.Height = 0;
                }
                return ret;
            }else{
                /*ret.Starttime;
                ret.Endtime;*/
                ret.Width = 0;
                ret.Height = 0;
            }
            return ret;
        }
    }



    /// <summary>
        /// <br>Матрица направления на старую точку: </br>
        /// <br>Х-координата (чем больше, тем выше)</br>
        /// <br>^</br>
        /// <br>|    123</br>
        /// <br>|    804</br>
        /// <br>|    765</br>
        /// <br>+------------>  Время(Если время больше, то значение правее)</br>
        /// <br>Соответственно индексу 5 соответствует точка позднее и с бОльшим Х</br>
        /// </summary>
    public enum Directions{


    Error = -1,
    ThisPoint = 0,
    UpLeft = 1, 
    Up = 2, 
    UpRight = 3,
    Right = 4, 
    DownRight = 5,
    Down = 6,
    DownLeft = 7,
    Left = 8


    }

    /// <summary> Класс взаимодейстивия с граничной точкой </summary>
    public class borderPoint{
        /// <summary> У Координата нашей граничной точки </summary>
        public int CurrentPointY;
        /// <summary> Х Координата нашей граничной точки </summary>
        public int CurrentPointX;

        /// <summary> У Координата предыдущей точки </summary>
        private int PreviousPointY;
        /// <summary> Х Координата предыдущей точки </summary>
        private int PreviousPointX;

        /// <summary> У Координата начальной точки </summary>
        private int StartPointY;
        /// <summary> Х Координата начальной точки </summary>
        private int StartPointX;


        //X - горизонтальное расположение в скане
        //Y - шкала времени
        public borderPoint(int _YpointCoordinate, int _XpointCoordinate){
            CurrentPointY = _YpointCoordinate;
            CurrentPointX = _XpointCoordinate;

            PreviousPointY = _YpointCoordinate;
            PreviousPointX = _XpointCoordinate;

            StartPointY = _YpointCoordinate;
            StartPointX = _XpointCoordinate;            
        }
        /// <summary> Вывод окружения точки в консоль(высоты вокруг точки) </summary>
        public void ConsoleLog(SuperScan[] input){
            Console.WriteLine($"{leftuppoint(input)>0} {uppoint(input)>0} {rightuppoint(input)>0}");
            Console.WriteLine($"{leftpoint(input)>0} {thispoint(input)>0} {rightpoint(input)>0}");
            Console.WriteLine($"{leftdownpoint(input)>0} {downpoint(input)>0} {rightdownpoint(input)>0}");
            Console.WriteLine($"{whereisoldpoint()}");
        }

        /// <summary> Внешняя проверка на то, является ли точка крайней правой </summary>
        public bool ispointright(SuperScan[] input){
            return rightpoint(input)==0;
        }
        /// <summary> Внешняя проверка на то, является ли точка крайней левой </summary>
        public bool ispointleft(SuperScan[] input){
            return leftpoint(input)==0;
        }




        /// <summary> Проверка, является ли текущая точка той, с которой начали нахождение границы острова </summary>
        public bool isitstartpoint(){
            return ((StartPointY == CurrentPointY)&&(StartPointX == CurrentPointX));
        }
        /// <summary> Время, соответствующее точке </summary>
        public DateTime time(SuperScan[] input){
            return input[CurrentPointY].Time;
        }

        /// <summary> Смещение текущей точки на одну клетку вверх </summary>
        private void upindex(SuperScan[] input){
            if(input.Length-CurrentPointY>1){
                CurrentPointY++;
            }
        }
        /// <summary> Смещение текущей точки на одну клетку вниз </summary>
        private void downindex(SuperScan[] input){
            if(CurrentPointY>0){
                CurrentPointY--;
            }
        }
        /// <summary> Смещение текущей точки на одну клетку влево </summary>
        private void leftindex(SuperScan[] input){
            if(CurrentPointX>0){
                CurrentPointX--;
            }
        }
        /// <summary> Смещение текущей точки на одну клетку вправо </summary>
        private void rightindex(SuperScan[] input){
            if(input[0].CarIslandLanes.Length-CurrentPointX>1){
                CurrentPointX++;
            }
        }


        /// <summary> Получение высоты текущей точки </summary>
        private int thispoint(SuperScan[] input){
            return input[CurrentPointY].CarIslandLanes[CurrentPointX];
        }


        /// <summary> Получение высоты точки слева от текущей </summary>
        private int leftpoint(SuperScan[] input){
            if(CurrentPointX==0){
                return 0;
            }else{
                return input[CurrentPointY].CarIslandLanes[CurrentPointX-1];
            }
        }
        /// <summary> Получение высоты точки справа от текущей </summary>
        private int rightpoint(SuperScan[] input){
            if(CurrentPointX==(input[CurrentPointY].CarIslandLanes.Length-1)){
                return 0;
            }else{
                return input[CurrentPointY].CarIslandLanes[CurrentPointX+1];
            }
        }
        /// <summary> Получение высоты точки сверху от текущей </summary>
        private int uppoint(SuperScan[] input){
            if(CurrentPointY==(input.Length-1)){
                return 0;
            }else{
                return input[CurrentPointY+1].CarIslandLanes[CurrentPointX];
            }
        }
        /// <summary> Получение высоты точки снизу от текущей </summary>
        private int downpoint(SuperScan[] input){
            if(CurrentPointY==0){
                return 0;
            }else{
                return input[CurrentPointY-1].CarIslandLanes[CurrentPointX];
            }
        }
        /// <summary> Получение высоты точки слева сверху от текущей </summary>
        private int leftuppoint(SuperScan[] input){
            if(CurrentPointX==0){
                return 0;
            }
            if(CurrentPointY==(input.Length-1)){
                return 0;
            }
            return input[CurrentPointY+1].CarIslandLanes[CurrentPointX-1];
        }
        /// <summary> Получение высоты точки справа сверху от текущей </summary>
        private int rightuppoint(SuperScan[] input){
            if(CurrentPointY==(input.Length-1)){
                return 0;
            }
            if(CurrentPointX==(input[CurrentPointY+1].CarIslandLanes.Length-1)){
                return 0;
            }
            return input[CurrentPointY+1].CarIslandLanes[CurrentPointX+1];
        }
        /// <summary> Получение высоты точки слева снизу от текущей </summary>
        private int leftdownpoint(SuperScan[] input){
            if(CurrentPointX==0){
                return 0;
            }
            if(CurrentPointY==0){
                return 0;
            }
            return input[CurrentPointY-1].CarIslandLanes[CurrentPointX-1];
        }
        /// <summary> Получение высоты точки справа снизу от текущей </summary>
        private int rightdownpoint(SuperScan[] input){
            if(CurrentPointY==(input.Length-1)){
                return 0;
            }
            if(CurrentPointY==0){
                return 0;
            }
            return input[CurrentPointY-1].CarIslandLanes[CurrentPointX+1];
        }


        /*
        Матрица направления на старую точку: 
        Х-координата (чем больше, тем выше)
        ^
        |    123
        |    804
        |    765
        +------------>  Время(Если время больше, то значение правее)
        Соответственно индексу 5 соответствует точка позднее и с бОльшим Х
        */

        /// <summary> Проверка для всех точек слева от текущей, на направление по оси У, на то, являются ли они предыдущими точками </summary>
        public Directions LeftsPoints(){
            if((PreviousPointY == CurrentPointY)&&(PreviousPointX < CurrentPointX)){
                return Directions.Left;
            }

            if((PreviousPointY > CurrentPointY)&&(PreviousPointX < CurrentPointX)){
                return Directions.UpLeft;
            }

            if((PreviousPointY < CurrentPointY)&&(PreviousPointX < CurrentPointX)){
                return Directions.DownLeft;
            }
            
            return Directions.Error;
        }
        /// <summary> Проверка для точек: сверху, снизу, и текущей, являются ли они предыдущими точками </summary>

        public Directions MediumsPoint(){
            if((PreviousPointY == CurrentPointY)&&(PreviousPointX == CurrentPointX)){
                return Directions.ThisPoint;
            }
            if((PreviousPointY > CurrentPointY)&&(PreviousPointX == CurrentPointX)){
                return Directions.Up;
            }
            if((PreviousPointY < CurrentPointY)&&(PreviousPointX == CurrentPointX)){
                return Directions.Down;
            }
            return Directions.Error;
        }

        /// <summary> Проверка для всех точек справа от текущей, на направление по оси У, на то, являются ли они предыдущими точками </summary>
        public Directions RightsPoints(){
                if((PreviousPointY == CurrentPointY)&&(PreviousPointX > CurrentPointX)){
                    return Directions.Right;
                }
                if((PreviousPointY > CurrentPointY)&&(PreviousPointX > CurrentPointX)){
                    return Directions.UpRight;
                }
                if((PreviousPointY < CurrentPointY)&&(PreviousPointX > CurrentPointX)){
                    return Directions.DownRight;
                }

                return Directions.Error;
        }

        /// <summary> Получение направления на старую точку </summary>
        private Directions whereisoldpoint(){
            if(PreviousPointX<CurrentPointX){
                return LeftsPoints();
            }
        
            if(PreviousPointX==CurrentPointX){
                return MediumsPoint();
            }

            if(PreviousPointX>CurrentPointX){
                return RightsPoints();
            }
            return Directions.Error;
        }



        /// <summary> Переводит указатель к левой границе МАШИНЫ</summary>
        public void GoToLeftBorder(SuperScan[] input){
            while(leftpoint(input) > 0){
                leftindex(input);
            }
        }
        /// <summary> Переводит указатель к правой границе МАШИНЫ</summary>
        public void GoToRightBorder(SuperScan[] input){
            while(rightpoint(input) > 0){
                rightindex(input);
            }
        }



        /*
            Смещение круговое для каждой из точек            
        */


        /*
            Стартовая точка, ноль
            Просто идёт в рандомную сторону
        */


        /// <summary> Поиск и переход на следущую точку для стартовой точки </summary>
        private void InThisPoint(SuperScan[] input){
            if((leftuppoint(input)>0)&&(leftpoint(input) == 0)){
                upindex(input);
                leftindex(input);
                return;
            }
            if((uppoint(input)>0)&&(leftuppoint(input) == 0)){
                upindex(input);
                return;
            }
            if((rightuppoint(input)>0)&&(uppoint(input) == 0)){
                upindex(input);
                rightindex(input);
                return;
            }
            if((rightpoint(input)>0)&&(rightuppoint(input) == 0)){
                rightindex(input);
                return;
            }
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            if((downpoint(input)>0)&&(rightdownpoint(input) == 0)){
                downindex(input);
                return;
            }
            if((leftdownpoint(input)>0)&&(downpoint(input) == 0)){
                downindex(input);
                leftindex(input);
                return;
            }
            if((leftpoint(input)>0)&&(leftdownpoint(input) == 0)){
                leftindex(input);
                return;
            }
            //Console.WriteLine("Ошибка");
        }

        /// <summary> Поиск и переход на следущую точку, когда предыдущая точка слева от текущей </summary>
        private void InLeftPoint(SuperScan[] input){
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
            if((uppoint(input)>0)&&((leftuppoint(input) == 0))){
                upindex(input);
                return;
            }
            if((rightuppoint(input)>0)&&((uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
            if((rightpoint(input)>0)&&((rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            if((downpoint(input)>0)&&((rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
            if((leftdownpoint(input)>0)&&((downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0))){
                leftindex(input);
                return;
            }

            Console.WriteLine("Ошибка");
        }
        /// <summary> Поиск и переход на следущую точку, когда предыдущая точка слева сверху от текущей </summary>
        private void InLeftUpPoint(SuperScan[] input){
            if((uppoint(input)>0)&&((leftuppoint(input) == 0))){
                upindex(input);
                return;
            }
            if((rightuppoint(input)>0)&&((uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
            if((rightpoint(input)>0)&&((rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            if((downpoint(input)>0)&&((rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
            if((leftdownpoint(input)>0)&&((downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0))){
                leftindex(input);
                return;
            }
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
            Console.WriteLine("Ошибка");
        }


        /// <summary> Поиск и переход на следущую точку, когда предыдущая точка сверху от текущей </summary>
        private void InUpPoint(SuperScan[] input){
            if((rightuppoint(input)>0)&&((uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
            if((rightpoint(input)>0)&&((rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            if((downpoint(input)>0)&&((rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
            if((leftdownpoint(input)>0)&&((downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0))){
                leftindex(input);
                return;
            }
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
            if((uppoint(input)>0)&&((leftuppoint(input) == 0))){
                upindex(input);
                return;
            }
            Console.WriteLine("Ошибка");
        }

        /// <summary> Поиск и переход на следущую точку, когда предыдущая точка справа сверху от текущей </summary>
        private void InRightUpPoint(SuperScan[] input){
            if((rightpoint(input)>0)&&((rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            if((downpoint(input)>0)&&((rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
            if((leftdownpoint(input)>0)&&((downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0))){
                leftindex(input);
                return;
            }
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
            if((uppoint(input)>0)&&((leftuppoint(input) == 0))){
                upindex(input);
                return;
            }
            if((rightuppoint(input)>0)&&((uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
            Console.WriteLine("Ошибка");
        }

        /// <summary> Поиск и переход на следущую точку, когда предыдущая точка справа от текущей </summary>
        private void InRightPoint(SuperScan[] input){
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            if((downpoint(input)>0)&&((rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
            if((leftdownpoint(input)>0)&&((downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0))){
                leftindex(input);
                return;
            }
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
            if((uppoint(input)>0)&&((leftuppoint(input) == 0))){
                upindex(input);
                return;
            }
            if((rightuppoint(input)>0)&&((uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
            if((rightpoint(input)>0)&&((rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
            Console.WriteLine("Ошибка");
        }

        /// <summary> Поиск и переход на следущую точку, когда предыдущая точка справа снизу от текущей </summary>
        private void InRightDownPoint(SuperScan[] input){
            if((downpoint(input)>0)&&((rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
            if((leftdownpoint(input)>0)&&((downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0))){
                leftindex(input);
                return;
            }
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
            if((uppoint(input)>0)&&((leftuppoint(input) == 0))){
                upindex(input);
                return;
            }
            if((rightuppoint(input)>0)&&((uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
            if((rightpoint(input)>0)&&((rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            Console.WriteLine("Ошибка");
        }

        /// <summary> Поиск и переход на следущую точку, когда предыдущая точка снизу от текущей </summary>
        private void InDownPoint(SuperScan[] input){
            if((leftdownpoint(input)>0)&&((downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0))){
                leftindex(input);
                return;
            }
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
            if((uppoint(input)>0)&&((leftuppoint(input) == 0))){
                upindex(input);
                return;
            }
            if((rightuppoint(input)>0)&&((uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
            if((rightpoint(input)>0)&&((rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            if((downpoint(input)>0)&&((rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
            Console.WriteLine("ошибка");
        }

        /// <summary> Поиск и переход на следущую точку, когда предыдущая точка слева снизу от текущей </summary>
        private void InLeftDownPoint(SuperScan[] input){
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0))){
                leftindex(input);
                return;
            }
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
            if((uppoint(input)>0)&&((leftuppoint(input) == 0))){
                upindex(input);
                return;
            }
            if((rightuppoint(input)>0)&&((uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
            if((rightpoint(input)>0)&&((rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            if((downpoint(input)>0)&&((rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
            if((leftdownpoint(input)>0)&&((downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
            Console.WriteLine("Ошибка");
        }

        /// <summary> Установка старой точки в координаты текущей точки </summary>
        public void installoldp(){
            PreviousPointX = CurrentPointX;
            PreviousPointY = CurrentPointY;
        }

        /// <summary> Переводит указатель на следущую точку границы острова </summary>
        public void NextPosition(SuperScan[] input){
            //ConsoleLog(input);
            var oldp = whereisoldpoint();
            installoldp();
            switch (oldp)
            {
                case Directions.ThisPoint:
                    InThisPoint(input);
                    //Console.WriteLine("Эта");
                    break;
                case Directions.Left:
                    InLeftPoint(input);
                    //Console.WriteLine("Левая");
                    break;
                case Directions.UpLeft:
                    InLeftUpPoint(input);
                    //Console.WriteLine("Верхнелевая");
                    break;
                case Directions.Up:
                    InUpPoint(input);
                    //Console.WriteLine("Верхняя");
                    break;
                case Directions.UpRight:
                    InRightUpPoint(input);
                    //Console.WriteLine("Верхнеправая");
                    break;
                case Directions.Right:
                    InRightPoint(input);
                    //Console.WriteLine("Правая");
                    break;
                case Directions.DownRight:
                    InRightDownPoint(input);
                    //Console.WriteLine("Нижнеправая");
                    break;
                case Directions.Down:
                    InDownPoint(input);
                    //Console.WriteLine("Нижняя");
                    break;
                case Directions.DownLeft:
                    InLeftDownPoint(input);
                    //Consolereturn;.WriteLine("Нижнелевая");
                    break;
                default:
                    //Console.WriteLine($"");
                    //Console.WriteLine("дефолт");
                    break;
            }
        }
    }
    /// <summary> Хранит в себе всю информацию о проехавшей машине </summary>

    public class CarArraySize{
        ///<summary> левая граница машинки </summary>
        public int leftborder { get; set; }
        ///<summary> правая граница машинки </summary>
        public int rightborder { get; set; }
        ///<summary> начальное время проезда машинки </summary>
        public DateTime starttime { get; set; }
        ///<summary> конечное время проезда машинки </summary>
        public DateTime endtime { get; set; }
        ///<summary> массив высот машинки </summary>
        public int[][] cararray { get; set; }
        ///<summary> массив индексов левой границы машинуки </summary>
        public int[] leftindexborders { get; set; }
        ///<summary> массив индексов правой границы машинуки </summary>
        public int[] rightindexborders { get; set; }

        ///<summary> Ширина машинки </summary>
        public int Width { get; set; }
        ///<summary> Высота машинки </summary>
        public int Height { get; set; }
        public CarArraySize(){
        }

        ///<summary> Копирование всех данныз о машинке </summary>
        public CarArraySize Copy(){
            var ret = new CarArraySize();
            ret.leftborder = leftborder;
            ret.rightborder = rightborder;
            ret.starttime = starttime;
            ret.endtime = endtime;
            ret.leftindexborders = leftindexborders;
            ret.rightindexborders = rightindexborders;
            ret.Height = Height;
            ret.Width = Width;
            return ret;
        }
    }
    ///<summary> Находит границы машинки по одной точке (номер скана, номер точки)
    ///</summary>
    public class islandborders{
        private borderPoint BorderPoint;
        ///<summary> начальная точка машинки </summary>
        public int starttime, startposition;
        ///<summary> примитивные границы машинки </summary>
        public int leftborder,rightborder, upborder, downborder;
        ///<summary> массив левых границ машинки </summary>
        public int[] leftindexborders;
        ///<summary> массив правых границ машинки </summary>
        public int[] rightindexborders;
        
        ///<summary> Раздвигает примитивные границы машинки по текущей точке </summary>
        private void updatePrimitiveBorder(){
            if(BorderPoint.CurrentPointX > rightborder){
                rightborder = BorderPoint.CurrentPointX;
            }
            if(BorderPoint.CurrentPointX < leftborder){
                leftborder = BorderPoint.CurrentPointX;
            }
            if(BorderPoint.CurrentPointY > upborder){
                upborder = BorderPoint.CurrentPointY;
            }
            if(BorderPoint.CurrentPointY < downborder){
                downborder = BorderPoint.CurrentPointY;
            }
        }

        ///<summary> Записывает текущую точку в массивы границ машинки </summary>
        private void updateBorders(SuperScan[] input){
                if(leftindexborders[BorderPoint.CurrentPointY - downborder]==0){
                    leftindexborders[BorderPoint.CurrentPointY - downborder] = BorderPoint.CurrentPointX;
                }
                if(rightindexborders[BorderPoint.CurrentPointY - downborder]==0){
                    rightindexborders[BorderPoint.CurrentPointY - downborder] = BorderPoint.CurrentPointX;
                }

                if(BorderPoint.ispointleft(input)){
                if(leftindexborders[BorderPoint.CurrentPointY - downborder]==0){
                    leftindexborders[BorderPoint.CurrentPointY - downborder] = BorderPoint.CurrentPointX;
                }else{
                    if(leftindexborders[BorderPoint.CurrentPointY - downborder] > BorderPoint.CurrentPointX){
                        leftindexborders[BorderPoint.CurrentPointY - downborder] = BorderPoint.CurrentPointX;
                    }
                }
            }
            if(BorderPoint.ispointright(input)){
                if(rightindexborders[BorderPoint.CurrentPointY - downborder]==0){
                    rightindexborders[BorderPoint.CurrentPointY - downborder] = BorderPoint.CurrentPointX;
                }else{
                    if(rightindexborders[BorderPoint.CurrentPointY - downborder] < BorderPoint.CurrentPointX){
                        rightindexborders[BorderPoint.CurrentPointY - downborder] = BorderPoint.CurrentPointX;
                    }
                }
            }
        }
        ///<summary> При объявлении на автомате находятся граничные значения машины по времени/координатам </summary>
        public islandborders(int time, int position, SuperScan[] input){
            startposition = position;
            starttime = time;
            BorderPoint = new borderPoint(time, position);
            leftborder = position;
            rightborder = position;
            upborder = time; 
            downborder = time;

            addPrimitiveBorders(input);
            addBorders(input);
        }
        ///<summary> Нахождение примитивных границ машинки </summary>
        private void addPrimitiveBorders(SuperScan[] input){
            BorderPoint.GoToLeftBorder(input);
            BorderPoint.NextPosition(input);
            updatePrimitiveBorder();
            //Console.WriteLine("Начальная пара точек");
            while(BorderPoint.isitstartpoint()==false){
                //Console.WriteLine("Следующая точка");
                BorderPoint.NextPosition(input);
                //Console.WriteLine("Установка границ");
                updatePrimitiveBorder();
            }
            //Console.WriteLine("Пробежка по точкам закончена");

        }
        ///<summary> Нахождение границ машинки </summary>
        private void addBorders(SuperScan[] input){
            //Определение границ машинки на сканах
            leftindexborders = new int[upborder-downborder+1];
            rightindexborders = new int[upborder-downborder+1];

            //Console.WriteLine("Пробежка по точкам 2");
            BorderPoint.NextPosition(input);
            updateBorders(input);
            while(BorderPoint.isitstartpoint()==false){
                BorderPoint.NextPosition(input);
                updateBorders(input);
            }
        }
        ///<summary> Удаление(заполнение землёй) машинки </summary>
        public void remoovecar(SuperScan[] input){
            for(int i = 0; i <= (upborder-downborder); i++){
                for(int j = leftindexborders[i]; j <= rightindexborders[i]; j++){
                    input[i+downborder].CarIslandLanes[j] = 0;
                }
            }
        }


        ///<summary> Сохранение границ машинки </summary>
        public CarArraySize CarBorders(SuperScan[] input, carRESULT resulrtCar, config config){
            var ret = new CarArraySize();
            ret.leftborder = config.RoadSettings.LeftLimit + leftborder*config.RoadSettings.Step;
            ret.rightborder = config.RoadSettings.LeftLimit + rightborder*config.RoadSettings.Step;
            ret.starttime = input[starttime].Time;
            ret.endtime = input[starttime + leftindexborders.Length-1].Time;
            ret.leftindexborders = leftindexborders;
            ret.rightindexborders = rightindexborders;
            ret.Height = resulrtCar.Height;
            ret.Width = resulrtCar.Width;
            return ret;
        }
    }
    ///<summary> Параметры машинки </summary>
    public class carRESULT{
        ///<summary>Время начала машинки </summary>
        public DateTime Starttime { get; set; }
        ///<summary>Время конца машинки</summary>
        public DateTime Endtime { get; set; }
        ///<summary>Ширина машинки</summary>
        public int Width { get; set; }
        ///<summary>Высота машинки</summary>
        public int Height { get; set; }
    }
    ///<summary> Поиск островов(машинок) в облаке точек </summary>
    public class IslandSeach{
        ///<summary> Массив найденных машинок </summary>
        public List<CarArraySize> CarsArray; //Массив всех машинок
        ///<summary> метод поиска машинок </summary>
        public string method; //метод поиска машинки, берётся из конфига
        ///<summary> Минимальная длинна(в сканах) машин </summary>
        public int MinLength;
        ///<summary> Минимальная ширина(в мм) машин </summary>
        public int MinWigdh;
        ///<summary> Алгоритм поиска машинки </summary>
        private ISearchAlgoritm SearchAlgoritm;
        config _config;
        public IslandSeach(config config, Logger logger){
            _config = config;
            CarsArray = new List<CarArraySize>();
            method = config.Method;
            MinLength = config.SortSettings.MinLength;
            MinWigdh = config.SortSettings.MinWigdh;
            switch (method)
            {
                case "primitive":
                    logger.Error($"Установлен режим поиска '{method}'");

                    //Console.WriteLine("Установлен режим поиска 'primitive'");
                    SearchAlgoritm = new PrimitiveSearchAlgoritm();
                    break;
                case "primitiveAutomatic":
                    logger.Error($"Установлен режим поиска '{method}'");

                    //Console.WriteLine("Установлен режим поиска 'primitive'");
                    SearchAlgoritm = new PrimitiveSearchAlgoritm();
                    break;
                default:
                    logger.Error($"Ошибка режима поиска: режим '{method}' не известен. Установлен режим поиска 'primitive'");
                    //Console.WriteLine("Ошибка режима поиска: неизвестный режим. Установлен режим поиска 'primitive'");
                    SearchAlgoritm = new PrimitiveSearchAlgoritm();
                    break;
            }
        }

        ///<summary> Нахождение первой попавшейся точки машинки </summary>
        private PointXYint firstcarpoint(SuperScan[] input){
            var ret = new PointXYint{X = -1, Y = -1};
                int i = 0;
                int j = 0;
                while(i < input.Length){
                    j = 0;
                    while(j < input[i].CarIslandLanes.Length){
                        if(input[i].CarIslandLanes[j]>0){
                            return new PointXYint{X = j, Y = i};
                        }
                        j++;
                    }
                    i++;
                }
            return ret;
        }

        ///<summary> Примитивный метод поиска границ </summary>
        private carRESULT primitive(SuperScan[] input, Action<CarArraySize> AddCarAction, ISearchAlgoritm algoritm, Logger logger){
            var ret = new carRESULT();
            var startpoint = firstcarpoint(input);
            //Console.WriteLine("Установка начальной точки");
            if(startpoint.Y != -1){
                //Console.WriteLine("Пройдена проверка");
                var borders = new islandborders(startpoint.Y, startpoint.X, input);
                //Console.WriteLine("Установка границ");
                ret = algoritm.search(borders, input, _config);
                //Console.WriteLine("Получение размеров машины");
                if((ret.Height == 0)&(ret.Width == 0)){
                    //Console.WriteLine("Опять фигня");
                }else{
                    //Console.WriteLine("Хорошая машинка");
                    //CarsArray.Add(borders.CarBorders(input, ret, _config));
                    AddCarAction(borders.CarBorders(input, ret, _config));
                    //Console.WriteLine("Сохранение машины");
                }
                borders.remoovecar(input);
                //CarsArray.Add(borders.CarBorders(input));
            }else{
                logger.Info("Поиск завершён");

                //Console.WriteLine("Поиск завершён");
                ret.Height = -1;
                ret.Width = -1;
            }

            return ret;
        }

        ///<summary> Команда на поиск по алгоритму, задаваемому в конфиге </summary>
        public List<CarArraySize> Search(SuperScan[] input, Logger logger){
            int oldHeight = 0;
            var car = new carRESULT();
            while(oldHeight != -1){
                /*switch (method)
                {
                    case "primitive":
                        //

                        break;
                    default:
                        Console.WriteLine("Ошибка режима поиска: неизвестный режим. Установлен режим поиска 'primitive'");
                        car = primitive(input, x => CarsArray.Add(x));
                        oldHeight = car.Height;
                        break;
                }*/
                car = primitive(input, x => CarsArray.Add(x), SearchAlgoritm, logger);
                oldHeight = car.Height;
            }

            return CarsArray;
        }
    }
}
