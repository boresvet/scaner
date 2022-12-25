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
                return input[YpointCoordinate-1].CarIslandLanes[XpointCoordinate];
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
        }


        private void InLeftPoint(SuperScan[] input){
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
            if((uppoint(input)>0)&&((leftuppoint(input) == 0)||(rightuppoint(input) == 0))){
                upindex(input);
                return;
            }
            if((rightuppoint(input)>0)&&((rightpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
            if((rightpoint(input)>0)&&((rightdownpoint(input) == 0)||(rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            if((downpoint(input)>0)&&((leftdownpoint(input) == 0)||(rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
            if((leftdownpoint(input)>0)&&((leftpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0)||(leftuppoint(input) == 0))){
                leftindex(input);
                return;
            }


        }

        private void InLeftUpPoint(SuperScan[] input){
            if((uppoint(input)>0)&&((leftuppoint(input) == 0)||(rightuppoint(input) == 0))){
                upindex(input);
                return;
            }
            if((rightuppoint(input)>0)&&((rightpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
            if((rightpoint(input)>0)&&((rightdownpoint(input) == 0)||(rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            if((downpoint(input)>0)&&((leftdownpoint(input) == 0)||(rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
            if((leftdownpoint(input)>0)&&((leftpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0)||(leftuppoint(input) == 0))){
                leftindex(input);
                return;
            }
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
        }


        private void InUpPoint(SuperScan[] input){
            if((rightuppoint(input)>0)&&((rightpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
            if((rightpoint(input)>0)&&((rightdownpoint(input) == 0)||(rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            if((downpoint(input)>0)&&((leftdownpoint(input) == 0)||(rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
            if((leftdownpoint(input)>0)&&((leftpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0)||(leftuppoint(input) == 0))){
                leftindex(input);
                return;
            }
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
            if((uppoint(input)>0)&&((leftuppoint(input) == 0)||(rightuppoint(input) == 0))){
                upindex(input);
                return;
            }
        }

        private void InRightUpPoint(SuperScan[] input){
            if((rightpoint(input)>0)&&((rightdownpoint(input) == 0)||(rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            if((downpoint(input)>0)&&((leftdownpoint(input) == 0)||(rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
            if((leftdownpoint(input)>0)&&((leftpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0)||(leftuppoint(input) == 0))){
                leftindex(input);
                return;
            }
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
            if((uppoint(input)>0)&&((leftuppoint(input) == 0)||(rightuppoint(input) == 0))){
                upindex(input);
                return;
            }
            if((rightuppoint(input)>0)&&((rightpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
        }

        private void InRightPoint(SuperScan[] input){
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            if((downpoint(input)>0)&&((leftdownpoint(input) == 0)||(rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
            if((leftdownpoint(input)>0)&&((leftpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0)||(leftuppoint(input) == 0))){
                leftindex(input);
                return;
            }
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
            if((uppoint(input)>0)&&((leftuppoint(input) == 0)||(rightuppoint(input) == 0))){
                upindex(input);
                return;
            }
            if((rightuppoint(input)>0)&&((rightpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
            if((rightpoint(input)>0)&&((rightdownpoint(input) == 0)||(rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
        }

        private void InRightDownPoint(SuperScan[] input){
            if((downpoint(input)>0)&&((leftdownpoint(input) == 0)||(rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
            if((leftdownpoint(input)>0)&&((leftpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0)||(leftuppoint(input) == 0))){
                leftindex(input);
                return;
            }
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
            if((uppoint(input)>0)&&((leftuppoint(input) == 0)||(rightuppoint(input) == 0))){
                upindex(input);
                return;
            }
            if((rightuppoint(input)>0)&&((rightpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
            if((rightpoint(input)>0)&&((rightdownpoint(input) == 0)||(rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
        }

        private void InDownPoint(SuperScan[] input){
            if((leftdownpoint(input)>0)&&((leftpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0)||(leftuppoint(input) == 0))){
                leftindex(input);
                return;
            }
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
            if((uppoint(input)>0)&&((leftuppoint(input) == 0)||(rightuppoint(input) == 0))){
                upindex(input);
                return;
            }
            if((rightuppoint(input)>0)&&((rightpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
            if((rightpoint(input)>0)&&((rightdownpoint(input) == 0)||(rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            if((downpoint(input)>0)&&((leftdownpoint(input) == 0)||(rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
        }

        private void InLeftDownPoint(SuperScan[] input){
            if((leftpoint(input)>0)&&((leftdownpoint(input) == 0)||(leftuppoint(input) == 0))){
                leftindex(input);
                return;
            }
            if((leftuppoint(input)>0)&&((leftpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                leftindex(input);
                return;
            }
            if((uppoint(input)>0)&&((leftuppoint(input) == 0)||(rightuppoint(input) == 0))){
                upindex(input);
                return;
            }
            if((rightuppoint(input)>0)&&((rightpoint(input) == 0)||(uppoint(input) == 0))){
                upindex(input);
                rightindex(input);
                return;
            }
            if((rightpoint(input)>0)&&((rightdownpoint(input) == 0)||(rightuppoint(input) == 0))){
                rightindex(input);
                return;
            }
            if((rightdownpoint(input)>0)&&((rightpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                rightindex(input);
                return;
            }
            if((downpoint(input)>0)&&((leftdownpoint(input) == 0)||(rightdownpoint(input) == 0))){
                downindex(input);
                return;
            }
            if((leftdownpoint(input)>0)&&((leftpoint(input) == 0)||(downpoint(input) == 0))){
                downindex(input);
                leftindex(input);
                return;
            }
        }

        public void installoldp(){
            oldXpointCoordinate = XpointCoordinate;
            oldYpointCoordinate = YpointCoordinate;
        }

        public void NextPosition(SuperScan[] input){
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
                    //Console.WriteLine("Нижнелевая");
                    break;
                default:
                    //Console.WriteLine($"");
                    //Console.WriteLine("дефолт");
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
        public int[] leftindexborders { get; set; }
        public int[] rightindexborders { get; set; }
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
                if(leftindexborders[BorderPoint.YpointCoordinate - downborder]==0){
                    leftindexborders[BorderPoint.YpointCoordinate - downborder] = BorderPoint.XpointCoordinate;
                }
                if(rightindexborders[BorderPoint.YpointCoordinate - downborder]==0){
                    rightindexborders[BorderPoint.YpointCoordinate - downborder] = BorderPoint.XpointCoordinate;
                }

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

            while(BorderPoint.isitstartpoint()==false){
                BorderPoint.NextPosition(input);
                installpoint();
            }



            //Определение границ машинки на сканах
            leftindexborders = new int[upborder-downborder+1];
            rightindexborders = new int[upborder-downborder+1];

            BorderPoint.NextPosition(input);
            installindex(input);
            while(BorderPoint.isitstartpoint()==false){
                BorderPoint.NextPosition(input);
                installindex(input);
            }
        }


        public void remoovecar(SuperScan[] input){
            for(int i = 0; i <= (upborder-downborder); i++){
                for(int j = leftindexborders[i]; j <= rightindexborders[i]; j++){
                    input[i+downborder].CarIslandLanes[j] = 0;
                }
            }
        }


        public CarArraySize CarBorders(SuperScan[] input){
            var ret = new CarArraySize();
            ret.leftborder = leftborder;
            ret.rightborder = rightborder;
            ret.starttime = input[starttime].Time;
            ret.endtime = input[starttime + leftindexborders.Length].Time;
            ret.leftindexborders = leftindexborders;
            ret.rightindexborders = rightindexborders;
            return ret;
        }
    }

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
    public class IslandSeach{
        public List<CarArraySize> CarsArray;
        public string method; //метод поиска машинки, бреётся из конфига
        public int MinLength;
        public int MinWigdh;
        config _config;
        public IslandSeach(config config){
            _config = config;
            CarsArray = new List<CarArraySize>();
            method = config.Method;
            MinLength = config.SortSettings.MinLength;
            MinWigdh = config.SortSettings.MinWigdh;
            switch (method)
            {
                case "primitive":
                    Console.WriteLine("Установлен режим поиска 'primitive'");
                    break;
                default:
                    Console.WriteLine("Ошибка режима поиска: неизвестный режим. Установлен режим поиска 'primitive'");
                    break;
            }
        }

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

        private carRESULT primitivealgoritm(islandborders input, SuperScan[] inputscans, config config){
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



            if(lenthArray.Length>MinLength){



                ret.Starttime = inputscans[input.downborder].Time;
                ret.Endtime = inputscans[input.upborder].Time;
                ret.Width = (lenthArray[(int)(lenthArray.Length - (lenthArray.Length*0.05))]) * config.RoadSettings.Step;//Уже в миллиметрах
                ret.Height = HeightArray[(int)(HeightArray.Length - (HeightArray.Length*0.05))];
                //Отбрасываются максимальные точки, т.к. они скорее всего ошибка(та же антенна, + блики)
                
                if(MinWigdh>ret.Width){
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
        private carRESULT primitive(SuperScan[] input){
            var ret = new carRESULT();
            var startpoint = firstcarpoint(input);

            if(startpoint.Y != -1){
                var borders = new islandborders(startpoint.Y, startpoint.X, input);
                ret = primitivealgoritm(borders, input, _config);
                if((ret.Height == 0)&(ret.Width == 0)){
                    Console.WriteLine("Опять фигня");
                }else{
                    Console.WriteLine("Хорошая машинка");
                    CarsArray.Add(borders.CarBorders(input));
                }
                borders.remoovecar(input);
                //CarsArray.Add(borders.CarBorders(input));
            }else{
                Console.WriteLine("Поиск завершён");
                ret.Height = -1;
                ret.Width = -1;
            }



            return ret;
        }

















        public void Search(SuperScan[] input){
            int oldHeight = 0;
            var car = new carRESULT();
            while(oldHeight != -1){
                switch (method)
                {
                    case "primitive":
                        car = primitive(input);
                        oldHeight = car.Height;
                        break;
                    default:
                        Console.WriteLine("Ошибка режима поиска: неизвестный режим. Установлен режим поиска 'primitive'");
                        break;
                }
            }
        }
    }
}
