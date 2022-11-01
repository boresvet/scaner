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
        public int secondIndexInBuffer;
        public int milisecondIndexInSecond;
        public int XpointCoordinate;

        public int oldsecondIndexInBuffer;
        public int oldmilisecondIndexInSecond;
        public int oldXpointCoordinate;

        public int startsecondIndexInBuffer;
        public int startmilisecondIndexInSecond;
        public int startXpointCoordinate;

        public borderPoint(int _secondIndexInBuffer, int _milisecondIndexInSecond, int _XpointCoordinate){
            secondIndexInBuffer = _secondIndexInBuffer;
            milisecondIndexInSecond = _milisecondIndexInSecond;
            XpointCoordinate = _XpointCoordinate;

            oldsecondIndexInBuffer = _secondIndexInBuffer;
            oldmilisecondIndexInSecond = _milisecondIndexInSecond;
            oldXpointCoordinate = _XpointCoordinate;

            startsecondIndexInBuffer = _secondIndexInBuffer;
            startmilisecondIndexInSecond = _milisecondIndexInSecond;
            startXpointCoordinate = _XpointCoordinate;            
        }

        public bool isitstartpoint(){
            return ((startsecondIndexInBuffer==secondIndexInBuffer)&(startmilisecondIndexInSecond==milisecondIndexInSecond)&(startXpointCoordinate==XpointCoordinate));
        }

        public DateTime time(Second[] input){
            return input[secondIndexInBuffer].secondArray[milisecondIndexInSecond].Time;
        }


        private void upindex(Second[] input){
            if(input[secondIndexInBuffer].secondArray.Count>milisecondIndexInSecond+1){
                milisecondIndexInSecond++;
            }else{
                secondIndexInBuffer++;
                milisecondIndexInSecond = 0;
            }
        }
        private void downindex(Second[] input){
            if(milisecondIndexInSecond>0){
                milisecondIndexInSecond--;
            }else{
                secondIndexInBuffer--;
                milisecondIndexInSecond = input[secondIndexInBuffer].secondArray.Count-1;
            }
        }
        private void leftindex(Second[] input){
            if(XpointCoordinate>0){
                XpointCoordinate--;
            }
        }
        private void rightindex(Second[] input){
            if(XpointCoordinate<(input[secondIndexInBuffer].secondArray[milisecondIndexInSecond].CarIslandLanes.Length-1)){
                XpointCoordinate++;
            }
        }




        private int leftpoint(Second[] input){
            if(XpointCoordinate==0){
                return 0;
            }else{
                return input[secondIndexInBuffer].secondArray[milisecondIndexInSecond].CarIslandLanes[XpointCoordinate-1];
            }
        }
        private int rightpoint(Second[] input){
            if(XpointCoordinate==(input[secondIndexInBuffer].secondArray[milisecondIndexInSecond].CarIslandLanes.Length-1)){
                return 0;
            }else{
                return input[secondIndexInBuffer].secondArray[milisecondIndexInSecond].CarIslandLanes[XpointCoordinate+1];
            }
        }
        private int uppoint(Second[] input){
            if((input[secondIndexInBuffer].secondArray.Count==(milisecondIndexInSecond+1))&(input.Length == secondIndexInBuffer+1)){
                return 0;
            }
            if(input[secondIndexInBuffer].secondArray.Count>(milisecondIndexInSecond+1)){
                return input[secondIndexInBuffer].secondArray[milisecondIndexInSecond+1].CarIslandLanes[XpointCoordinate];
            }else{
                return input[secondIndexInBuffer+1].secondArray[0].CarIslandLanes[XpointCoordinate];
            }
        }
        private int downpoint(Second[] input){
            if((milisecondIndexInSecond == 0)&(secondIndexInBuffer == 0)){
                return 0;
            }
            if(milisecondIndexInSecond>0){
                return input[secondIndexInBuffer].secondArray[milisecondIndexInSecond-1].CarIslandLanes[XpointCoordinate];
            }else{
                return input[secondIndexInBuffer-1].secondArray[input[secondIndexInBuffer-1].secondArray.Count-1].CarIslandLanes[XpointCoordinate];
            }
        }



        private int leftuppoint(Second[] input){
            if(XpointCoordinate==0){
                return 0;
            }
            if((input[secondIndexInBuffer].secondArray.Count==(milisecondIndexInSecond+1))&(input.Length == secondIndexInBuffer+1)){
                return 0;
            }
            if(input[secondIndexInBuffer].secondArray.Count>(milisecondIndexInSecond+1)){
                return input[secondIndexInBuffer].secondArray[milisecondIndexInSecond+1].CarIslandLanes[XpointCoordinate-1];
            }else{
                return input[secondIndexInBuffer+1].secondArray[0].CarIslandLanes[XpointCoordinate-1];
            }
        }
        private int rightuppoint(Second[] input){
            if(XpointCoordinate==(input[secondIndexInBuffer].secondArray[milisecondIndexInSecond].CarIslandLanes.Length-1)){
                return 0;
            }
            if((input[secondIndexInBuffer].secondArray.Count==(milisecondIndexInSecond+1))&(input.Length == secondIndexInBuffer+1)){
                return 0;
            }
            if(input[secondIndexInBuffer].secondArray.Count>(milisecondIndexInSecond+1)){
                return input[secondIndexInBuffer].secondArray[milisecondIndexInSecond+1].CarIslandLanes[XpointCoordinate+1];
            }else{
                return input[secondIndexInBuffer+1].secondArray[0].CarIslandLanes[XpointCoordinate+1];
            }
        }
        private int leftdownpoint(Second[] input){
            if(XpointCoordinate==0){
                return 0;
            }
            if((milisecondIndexInSecond==0)&(secondIndexInBuffer==0)){
                return 0;
            }
            if(milisecondIndexInSecond>0){
                return input[secondIndexInBuffer].secondArray[milisecondIndexInSecond-1].CarIslandLanes[XpointCoordinate-1];
            }else{
                return input[secondIndexInBuffer-1].secondArray[input[secondIndexInBuffer-1].secondArray.Count-1].CarIslandLanes[XpointCoordinate-1];
            }
        }
        private int rightdownpoint(Second[] input){
            if(XpointCoordinate==(input[secondIndexInBuffer].secondArray[milisecondIndexInSecond].CarIslandLanes.Length-1)){
                return 0;
            }
            if((milisecondIndexInSecond==0)&(secondIndexInBuffer==0)){
                return 0;
            }
            if(milisecondIndexInSecond>0){
                return input[secondIndexInBuffer].secondArray[milisecondIndexInSecond-1].CarIslandLanes[XpointCoordinate+1];
            }else{
                return input[secondIndexInBuffer-1].secondArray[input[secondIndexInBuffer-1].secondArray.Count-1].CarIslandLanes[XpointCoordinate+1];
            }
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
            if((oldsecondIndexInBuffer==secondIndexInBuffer)&&(milisecondIndexInSecond==oldmilisecondIndexInSecond)){
                return Directions.Left;
            }

            if(((oldsecondIndexInBuffer==secondIndexInBuffer)&&(milisecondIndexInSecond==oldmilisecondIndexInSecond+1))|((oldsecondIndexInBuffer+1==secondIndexInBuffer)&&(secondIndexInBuffer==0))){
                return Directions.UpLeft;
            }

            if(((oldsecondIndexInBuffer==secondIndexInBuffer)&&(milisecondIndexInSecond==oldmilisecondIndexInSecond-1))|((oldsecondIndexInBuffer-1==secondIndexInBuffer)&&(oldsecondIndexInBuffer==0))){
                return Directions.DownLeft;
            }
            
            return Directions.Error;
        }
        public Directions MediumsPoint(){
            if((oldsecondIndexInBuffer==secondIndexInBuffer)&&(milisecondIndexInSecond==oldmilisecondIndexInSecond)){
                return Directions.ThisPoint;
            }
            if(((oldsecondIndexInBuffer==secondIndexInBuffer)&&(milisecondIndexInSecond==oldmilisecondIndexInSecond+1))|((oldsecondIndexInBuffer+1==secondIndexInBuffer)&&(secondIndexInBuffer==0))){
                return Directions.Up;
            }
            if(((oldsecondIndexInBuffer==secondIndexInBuffer)&&(milisecondIndexInSecond==oldmilisecondIndexInSecond-1))|((oldsecondIndexInBuffer-1==secondIndexInBuffer)&&(oldsecondIndexInBuffer==0))){
                return Directions.Down;
            }
            return Directions.Error;
        }


        public Directions RightsPoints(){
                if((oldsecondIndexInBuffer==secondIndexInBuffer)&&(milisecondIndexInSecond==oldmilisecondIndexInSecond)){
                    return Directions.Right;
                }
                if(((oldsecondIndexInBuffer==secondIndexInBuffer)&&(milisecondIndexInSecond==oldmilisecondIndexInSecond+1))|((oldsecondIndexInBuffer+1==secondIndexInBuffer)&&(secondIndexInBuffer==0))){
                    return Directions.UpRight;
                }
                if(((oldsecondIndexInBuffer==secondIndexInBuffer)&&(milisecondIndexInSecond==oldmilisecondIndexInSecond-1))|((oldsecondIndexInBuffer-1==secondIndexInBuffer)&&(oldsecondIndexInBuffer==0))){
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





        public void GoToLeftBorder(Second[] input){
            while(leftpoint(input) > 0){
                leftindex(input);
            }
        }
        public void GoToRightBorder(Second[] input){
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
        private void InThisPoint(Second[] input){
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


        private void InLeftPoint(Second[] input){
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

        private void InLeftUpPoint(Second[] input){
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


        private void InUpPoint(Second[] input){
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

        private void InRightUpPoint(Second[] input){
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

        private void InRightPoint(Second[] input){
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

        private void InRightDownPoint(Second[] input){
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

        private void InDownPoint(Second[] input){
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

        private void InLeftDownPoint(Second[] input){
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

        public void NextPosition(Second[] input){
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
    ///<summary>///Описывает один скан, как массив точек и время, ему соответствующее
    ///</summary>
    public class islandborders{
        private borderPoint BorderPoint;
        ///<summary>///Находит границы машины по 1й точке
        ///</summary>
        public islandborders(){}

        ///<summary>///Объявляет пустой скан
        ///</summary>
        public CarArraySize CarBorders(Second[] input, PointXYint inputCarPoint){
            for(int i = 0; i<input[inputCarPoint.X].secondArray[inputCarPoint.Y].CarIslandLanes.Length; i++){
                if(input[inputCarPoint.X].secondArray[inputCarPoint.Y].CarIslandLanes[i]>0){
                    BorderPoint = new borderPoint(inputCarPoint.X, inputCarPoint.Y, i);
                    break;
                }
            }
            BorderPoint.GoToLeftBorder(input);
            var ret = new CarArraySize();
            ret.starttime = BorderPoint.time(input);
            ret.leftborder = BorderPoint.XpointCoordinate;
            ret.rightborder = BorderPoint.XpointCoordinate;
            BorderPoint.NextPosition(input);
            ret.endtime = BorderPoint.time(input);
            while(BorderPoint.isitstartpoint()==false){
                BorderPoint.NextPosition(input);
                if(BorderPoint.time(input)>ret.endtime){
                    ret.endtime = BorderPoint.time(input);
                }
                if(BorderPoint.time(input)<ret.starttime){
                    ret.starttime = BorderPoint.time(input);
                }
                if(BorderPoint.XpointCoordinate<ret.leftborder){
                    ret.leftborder = BorderPoint.XpointCoordinate;
                }
                if(BorderPoint.XpointCoordinate>ret.rightborder){
                    ret.rightborder = BorderPoint.XpointCoordinate;
                }
            }
            return ret;
        }

        public void AddCar(Second[] input, CarArraySize inputCar){
            //Тут ничего пока нет
        }
        public void ClearCar(Second[] input, CarArraySize inputCar){
            for(int i = 0; i<input.Length; i++){
                for(int j = 0; j<input[i].secondArray.Count; j++){
                    if((inputCar.starttime<=input[i].secondArray[j].Time)&(inputCar.endtime>=input[i].secondArray[j].Time)){
                        Array.Fill(input[i].secondArray[j].CarIslandLanes, 0, inputCar.leftborder, inputCar.rightborder);
                    }
                }
            }
        }



    }
    ///<summary>///Находит все машинки в заданнои промежутке времени
    ///</summary>
    public class IslandSeach{
        public List<CarArraySize> CarsArray;

        public PointXYint[] pointsArray; 
        public DateTime time;
        ///<summary>///Находит все машинки в заданнои промежутке времени
        ///</summary>    
        public IslandSeach(){
            CarsArray = new List<CarArraySize>();
        }

        public PointXYint CarPoint(DateTime second, Second[] input){
            for(int i = 0; i<input.Length; i++){
                if((second.Minute == input[i].secondArray[0].Time.Minute)&(second.Second == input[i].secondArray[0].Time.Second)){
                    for(int j = 0; j<input[i].secondArray.Count; j++){
                        if(Array.FindAll(input[i].secondArray[j].CarIslandLanes, point => (point > 0)).Length >= 3){
                            return new PointXYint(){ X = i, Y = j};
                        }
                    }
                }
            }
 
            return new PointXYint(){ X = -1, Y = -1};
        }
            //Тут пока ничего нет
        public void CarArrays(DateTime second, Second[] input){
            var carpoint = CarPoint(second, input);
            var island = new islandborders();
            while(carpoint.X!=-1){
                carpoint = CarPoint(second, input);
                CarsArray.Add(island.CarBorders(input, carpoint));
                island.ClearCar(input, CarsArray[CarsArray.Count-1]);
            }
        }

    }
}
