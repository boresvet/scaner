using System;
using static System.Math;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sick_test
{
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
    public class borderPoint{
        public int YpointCoordinate;
        public int XpointCoordinate;

        public int oldYpointCoordinate;
        public int oldXpointCoordinate;

        public int startYpointCoordinate;
        public int startXpointCoordinate;


        //X - горизонтальное расположение в скане
        //Y - шкала времени
        public borderPoint(int _YpointCoordinate, int _XpointCoordinate){
            YpointCoordinate = _YpointCoordinate;
            XpointCoordinate = _XpointCoordinate;

            oldYpointCoordinate = _YpointCoordinate;
            oldXpointCoordinate = _XpointCoordinate;

            startYpointCoordinate = _YpointCoordinate;
            startXpointCoordinate = _XpointCoordinate;            
        }




        //Внешняя проверка на то, является ли точка крайней правой
        public bool ispointright(SuperScan[] input){
            return rightpoint(input)==0;
        }
        //Внешняя проверка на то, является ли точка крайней левой
        public bool ispointleft(SuperScan[] input){
            return leftpoint(input)==0;
        }





        public bool isitstartpoint(){
            return ((startYpointCoordinate == YpointCoordinate)&&(startXpointCoordinate == XpointCoordinate));
        }

        public DateTime time(SuperScan[] input){
            return input[YpointCoordinate].Time;
        }


        private void upindex(SuperScan[] input){
            if(input.Length-YpointCoordinate>1){
                YpointCoordinate++;
            }
        }
        private void downindex(SuperScan[] input){
            if(YpointCoordinate>0){
                YpointCoordinate--;
            }
        }
        private void leftindex(SuperScan[] input){
            if(XpointCoordinate>0){
                XpointCoordinate--;
            }
        }
        private void rightindex(SuperScan[] input){
            if(input[0].CarIslandLanes.Length-XpointCoordinate>1){
                XpointCoordinate++;
            }
        }




        private int leftpoint(SuperScan[] input){
            if(XpointCoordinate==0){
                return 0;
            }else{
                return input[YpointCoordinate].CarIslandLanes[XpointCoordinate-1];
            }
        }
        private int rightpoint(SuperScan[] input){
            if(XpointCoordinate==(input[YpointCoordinate].CarIslandLanes.Length-1)){
                return 0;
            }else{
                return input[YpointCoordinate].CarIslandLanes[XpointCoordinate+1];
            }
        }
        private int uppoint(SuperScan[] input){
            if(YpointCoordinate==(input.Length-1)){
                return 0;
            }else{
                return input[YpointCoordinate+1].CarIslandLanes[XpointCoordinate];
            }
        }
        private int downpoint(SuperScan[] input){
            if(YpointCoordinate==0){
                return 0;
            }else{
                return input[YpointCoordinate+1].CarIslandLanes[XpointCoordinate];
            }
        }



        private int leftuppoint(SuperScan[] input){
            if(XpointCoordinate==0){
                return 0;
            }
            if(YpointCoordinate==(input.Length-1)){
                return 0;
            }
            return input[YpointCoordinate+1].CarIslandLanes[XpointCoordinate-1];
        }
        private int rightuppoint(SuperScan[] input){
            if(YpointCoordinate==(input.Length-1)){
                return 0;
            }
            if(XpointCoordinate==(input[YpointCoordinate+1].CarIslandLanes.Length-1)){
                return 0;
            }
            return input[YpointCoordinate+1].CarIslandLanes[XpointCoordinate+1];
        }
        private int leftdownpoint(SuperScan[] input){
            if(XpointCoordinate==0){
                return 0;
            }
            if(YpointCoordinate==0){
                return 0;
            }
            return input[YpointCoordinate-1].CarIslandLanes[XpointCoordinate-1];
        }
        private int rightdownpoint(SuperScan[] input){
            if(YpointCoordinate==(input.Length-1)){
                return 0;
            }
            if(YpointCoordinate==0){
                return 0;
            }
            return input[YpointCoordinate-1].CarIslandLanes[XpointCoordinate+1];
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
        public Directions LeftsPoints(){
            if((oldYpointCoordinate == YpointCoordinate)&&(oldXpointCoordinate < XpointCoordinate)){
                return Directions.Left;
            }

            if((oldYpointCoordinate > YpointCoordinate)&&(oldXpointCoordinate < XpointCoordinate)){
                return Directions.UpLeft;
            }

            if((oldYpointCoordinate < YpointCoordinate)&&(oldXpointCoordinate < XpointCoordinate)){
                return Directions.DownLeft;
            }
            
            return Directions.Error;
        }
        public Directions MediumsPoint(){
            if((oldYpointCoordinate == YpointCoordinate)&&(oldXpointCoordinate == XpointCoordinate)){
                return Directions.ThisPoint;
            }
            if((oldYpointCoordinate > YpointCoordinate)&&(oldXpointCoordinate == XpointCoordinate)){
                return Directions.Up;
            }
            if((oldYpointCoordinate < YpointCoordinate)&&(oldXpointCoordinate == XpointCoordinate)){
                return Directions.Down;
            }
            return Directions.Error;
        }


        public Directions RightsPoints(){
                if((oldYpointCoordinate == YpointCoordinate)&&(oldXpointCoordinate > XpointCoordinate)){
                    return Directions.Right;
                }
                if((oldYpointCoordinate > YpointCoordinate)&&(oldXpointCoordinate > XpointCoordinate)){
                    return Directions.UpRight;
                }
                if((oldYpointCoordinate < YpointCoordinate)&&(oldXpointCoordinate > XpointCoordinate)){
                    return Directions.DownRight;
                }

                return Directions.Error;
        }
        private Directions whereisoldpoint(){
            if(oldXpointCoordinate<XpointCoordinate){
                return LeftsPoints();
            }
        
            if(oldXpointCoordinate==XpointCoordinate){
                return MediumsPoint();
            }

            if(oldXpointCoordinate>XpointCoordinate){
                return RightsPoints();
            }
            return Directions.Error;
        }




        //Переводит указатель к границе МАШИНЫ
        public void GoToLeftBorder(SuperScan[] input){
            while(leftpoint(input) > 0){
                leftindex(input);
            }
        }
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
        private void InThisPoint(SuperScan[] input){
            if(leftuppoint(input)>0){
                upindex(input);
                leftindex(input);
            }
            if(uppoint(input)>0){
                upindex(input);
            }
            if(rightuppoint(input)>0){
                upindex(input);
                rightindex(input);
            }
            if(rightpoint(input)>0){
                rightindex(input);
            }
            if(rightdownpoint(input)>0){
                downindex(input);
                rightindex(input);
            }
            if(downpoint(input)>0){
                downindex(input);
            }
            if(leftdownpoint(input)>0){
                downindex(input);
                leftindex(input);
            }
            if(leftpoint(input)>0){
                leftindex(input);
            }
        }


        private void InLeftPoint(SuperScan[] input){
            if(leftuppoint(input)>0){
                upindex(input);
                leftindex(input);
            }
            if(uppoint(input)>0){
                upindex(input);
            }
            if(rightuppoint(input)>0){
                upindex(input);
                rightindex(input);
            }
            if(rightpoint(input)>0){
                rightindex(input);
            }
            if(rightdownpoint(input)>0){
                downindex(input);
                rightindex(input);
            }
            if(downpoint(input)>0){
                downindex(input);
            }
            if(leftdownpoint(input)>0){
                downindex(input);
                leftindex(input);
            }
            if(leftpoint(input)>0){
                leftindex(input);
            }
        }

        private void InLeftUpPoint(SuperScan[] input){
            if(uppoint(input)>0){
                upindex(input);
            }
            if(rightuppoint(input)>0){
                upindex(input);
                rightindex(input);
            }
            if(rightpoint(input)>0){
                rightindex(input);
            }
            if(rightdownpoint(input)>0){
                downindex(input);
                rightindex(input);
            }
            if(downpoint(input)>0){
                downindex(input);
            }
            if(leftdownpoint(input)>0){
                downindex(input);
                leftindex(input);
            }
            if(leftpoint(input)>0){
                leftindex(input);
            }
            if(leftuppoint(input)>0){
                upindex(input);
                leftindex(input);
            }
        }


        private void InUpPoint(SuperScan[] input){
            if(rightuppoint(input)>0){
                upindex(input);
                rightindex(input);
            }
            if(rightpoint(input)>0){
                rightindex(input);
            }
            if(rightdownpoint(input)>0){
                downindex(input);
                rightindex(input);
            }
            if(downpoint(input)>0){
                downindex(input);
            }
            if(leftdownpoint(input)>0){
                downindex(input);
                leftindex(input);
            }
            if(leftpoint(input)>0){
                leftindex(input);
            }
            if(leftuppoint(input)>0){
                upindex(input);
                leftindex(input);
            }
            if(uppoint(input)>0){
                upindex(input);
            }
        }

        private void InRightUpPoint(SuperScan[] input){
            if(rightpoint(input)>0){
                rightindex(input);
            }
            if(rightdownpoint(input)>0){
                downindex(input);
                rightindex(input);
            }
            if(downpoint(input)>0){
                downindex(input);
            }
            if(leftdownpoint(input)>0){
                downindex(input);
                leftindex(input);
            }
            if(leftpoint(input)>0){
                leftindex(input);
            }
            if(leftuppoint(input)>0){
                upindex(input);
                leftindex(input);
            }
            if(uppoint(input)>0){
                upindex(input);
            }
            if(rightuppoint(input)>0){
                upindex(input);
                rightindex(input);
            }
        }

        private void InRightPoint(SuperScan[] input){
            if(rightdownpoint(input)>0){
                downindex(input);
                rightindex(input);
            }
            if(downpoint(input)>0){
                downindex(input);
            }
            if(leftdownpoint(input)>0){
                downindex(input);
                leftindex(input);
            }
            if(leftpoint(input)>0){
                leftindex(input);
            }
            if(leftuppoint(input)>0){
                upindex(input);
                leftindex(input);
            }
            if(uppoint(input)>0){
                upindex(input);
            }
            if(rightuppoint(input)>0){
                upindex(input);
                rightindex(input);
            }
            if(rightpoint(input)>0){
                rightindex(input);
            }
        }

        private void InRightDownPoint(SuperScan[] input){
            if(downpoint(input)>0){
                downindex(input);
            }
            if(leftdownpoint(input)>0){
                downindex(input);
                leftindex(input);
            }
            if(leftpoint(input)>0){
                leftindex(input);
            }
            if(leftuppoint(input)>0){
                upindex(input);
                leftindex(input);
            }
            if(uppoint(input)>0){
                upindex(input);
            }
            if(rightuppoint(input)>0){
                upindex(input);
                rightindex(input);
            }
            if(rightpoint(input)>0){
                rightindex(input);
            }
            if(rightdownpoint(input)>0){
                downindex(input);
                rightindex(input);
            }
        }

        private void InDownPoint(SuperScan[] input){
            if(leftdownpoint(input)>0){
                downindex(input);
                leftindex(input);
            }
            if(leftpoint(input)>0){
                leftindex(input);
            }
            if(leftuppoint(input)>0){
                upindex(input);
                leftindex(input);
            }
            if(uppoint(input)>0){
                upindex(input);
            }
            if(rightuppoint(input)>0){
                upindex(input);
                rightindex(input);
            }
            if(rightpoint(input)>0){
                rightindex(input);
            }
            if(rightdownpoint(input)>0){
                downindex(input);
                rightindex(input);
            }
            if(downpoint(input)>0){
                downindex(input);
            }
        }

        private void InLeftDownPoint(SuperScan[] input){
            if(leftpoint(input)>0){
                leftindex(input);
            }
            if(leftuppoint(input)>0){
                upindex(input);
                leftindex(input);
            }
            if(uppoint(input)>0){
                upindex(input);
            }
            if(rightuppoint(input)>0){
                upindex(input);
                rightindex(input);
            }
            if(rightpoint(input)>0){
                rightindex(input);
            }
            if(rightdownpoint(input)>0){
                downindex(input);
                rightindex(input);
            }
            if(downpoint(input)>0){
                downindex(input);
            }
            if(leftdownpoint(input)>0){
                downindex(input);
                leftindex(input);
            }
        }

        public void NextPosition(SuperScan[] input){
            var oldp = whereisoldpoint();
            switch (oldp)
            {
                case Directions.ThisPoint:
                    InThisPoint(input);
                    break;
                case Directions.Left:
                    InLeftPoint(input);
                    break;
                case Directions.UpLeft:
                    InLeftUpPoint(input);
                    break;
                case Directions.Up:
                    InUpPoint(input);
                    break;
                case Directions.UpRight:
                    InRightUpPoint(input);
                    break;
                case Directions.Right:
                    InRightPoint(input);
                    break;
                case Directions.DownRight:
                    InRightDownPoint(input);
                    break;
                case Directions.Down:
                    InDownPoint(input);
                    break;
                case Directions.DownLeft:
                    InLeftDownPoint(input);
                    break;
                default:
                    //Console.WriteLine($"");
                    break;
            }
        }
    }
    public class CarArraySize{
        public int leftborder { get; set; }
        public int rightborder { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public int[][] cararray { get; set; }
        public CarArraySize(){
        }
    }
    ///<summary>///Находит границы (упрощённые, только предельные габариты) машинки по одной точке (номер скана, номер точки)
    ///</summary>
    public class islandborders{
        private borderPoint BorderPoint;
        public int starttime, startposition;
        public int leftborder,rightborder, upborder, downborder;
        public int[] leftindexborders;
        public int[] rightindexborders;
        ///<summary>///Находит границы машины по 1й точке
        ///</summary>
        private void installpoint(){
            if(BorderPoint.XpointCoordinate > rightborder){
                rightborder = BorderPoint.XpointCoordinate;
            }
            if(BorderPoint.XpointCoordinate < leftborder){
                leftborder = BorderPoint.XpointCoordinate;
            }
            if(BorderPoint.YpointCoordinate > upborder){
                upborder = BorderPoint.YpointCoordinate;
            }
            if(BorderPoint.YpointCoordinate < downborder){
                downborder = BorderPoint.YpointCoordinate;
            }
        }

        //Записывает текущую точку в массивы границ машинки
        private void installindex(SuperScan[] input){
            if(BorderPoint.ispointleft(input)){
                if(leftindexborders[BorderPoint.YpointCoordinate - downborder]==0){
                    leftindexborders[BorderPoint.YpointCoordinate - downborder] = BorderPoint.XpointCoordinate;
                }else{
                    if(leftindexborders[BorderPoint.YpointCoordinate - downborder] > BorderPoint.XpointCoordinate){
                        leftindexborders[BorderPoint.YpointCoordinate - downborder] = BorderPoint.XpointCoordinate;
                    }
                }
            }
            if(BorderPoint.ispointright(input)){
                if(rightindexborders[BorderPoint.YpointCoordinate - downborder]==0){
                    rightindexborders[BorderPoint.YpointCoordinate - downborder] = BorderPoint.XpointCoordinate;
                }else{
                    if(rightindexborders[BorderPoint.YpointCoordinate - downborder] < BorderPoint.XpointCoordinate){
                        rightindexborders[BorderPoint.YpointCoordinate - downborder] = BorderPoint.XpointCoordinate;
                    }
                }
            }
        }
        //При объявлении на автомате находятся граничные значения машины по времени/координатам
        public islandborders(int time, int position, SuperScan[] input){
            startposition = position;
            starttime = time;
            BorderPoint = new borderPoint(time, position);
            leftborder = position;
            rightborder = position;
            upborder = time; 
            downborder = time;
            BorderPoint.GoToLeftBorder(input);
            BorderPoint.NextPosition(input);
            installpoint();

            while(BorderPoint.isitstartpoint()!=false){
                BorderPoint.NextPosition(input);
                installpoint();
            }



            //Определение границ машинки на сканах
            leftindexborders = new int[upborder-downborder+1];
            rightindexborders = new int[upborder-downborder+1];

            BorderPoint.NextPosition(input);
            installindex(input);
            while(BorderPoint.isitstartpoint()!=false){
                BorderPoint.NextPosition(input);
                installindex(input);
            }
        }


        public void remoovecar(SuperScan[] input){
            for(int i = 0; i < (upborder-downborder+1); i++){
                for(int j = leftindexborders[i]; j < rightindexborders[i]; j++){
                    input[i].CarIslandLanes[j] = 0;
                }
            }
        }
    }


    public class IslandSeach{
        public List<CarArraySize> CarsArray;

        public IslandSeach(config config){
            CarsArray = new List<CarArraySize>();
        }
    }
}
