using BSICK.Sensors.LMS1xx;
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

        private int whereisoldpoint(){
            if(oldXpointCoordinate<XpointCoordinate){
                if((oldsecondIndexInBuffer==secondIndexInBuffer)&(milisecondIndexInSecond==oldmilisecondIndexInSecond)){
                    return 8;
                }
                if(((oldsecondIndexInBuffer==secondIndexInBuffer)&(milisecondIndexInSecond==oldmilisecondIndexInSecond+1))|((oldsecondIndexInBuffer+1==secondIndexInBuffer)&(secondIndexInBuffer==0))){
                    return 1;
                }
                if(((oldsecondIndexInBuffer==secondIndexInBuffer)&(milisecondIndexInSecond==oldmilisecondIndexInSecond-1))|((oldsecondIndexInBuffer-1==secondIndexInBuffer)&(oldsecondIndexInBuffer==0))){
                    return 7;
                }
                return-1;
            }
            if(oldXpointCoordinate==XpointCoordinate){
                if((oldsecondIndexInBuffer==secondIndexInBuffer)&(milisecondIndexInSecond==oldmilisecondIndexInSecond)){
                    return 0;
                }
                if(((oldsecondIndexInBuffer==secondIndexInBuffer)&(milisecondIndexInSecond==oldmilisecondIndexInSecond+1))|((oldsecondIndexInBuffer+1==secondIndexInBuffer)&(secondIndexInBuffer==0))){
                    return 2;
                }
                if(((oldsecondIndexInBuffer==secondIndexInBuffer)&(milisecondIndexInSecond==oldmilisecondIndexInSecond-1))|((oldsecondIndexInBuffer-1==secondIndexInBuffer)&(oldsecondIndexInBuffer==0))){
                    return 6;
                }

                return -1;

            }

            if(oldXpointCoordinate>XpointCoordinate){
                if((oldsecondIndexInBuffer==secondIndexInBuffer)&(milisecondIndexInSecond==oldmilisecondIndexInSecond)){
                    return 4;
                }
                if(((oldsecondIndexInBuffer==secondIndexInBuffer)&(milisecondIndexInSecond==oldmilisecondIndexInSecond+1))|((oldsecondIndexInBuffer+1==secondIndexInBuffer)&(secondIndexInBuffer==0))){
                    return 3;
                }
                if(((oldsecondIndexInBuffer==secondIndexInBuffer)&(milisecondIndexInSecond==oldmilisecondIndexInSecond-1))|((oldsecondIndexInBuffer-1==secondIndexInBuffer)&(oldsecondIndexInBuffer==0))){
                    return 5;
                }

                return -1;
            }
            return -1;
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




        public void NextPosition(Second[] input){
            var oldp = whereisoldpoint();
            switch (oldp)
            {
                case 0:
                    if(leftuppoint(input)>0){
                        upindex(input);
                        leftindex(input);
                        break;
                    }
                    if(uppoint(input)>0){
                        upindex(input);
                        break;
                    }
                    if(rightuppoint(input)>0){
                        upindex(input);
                        rightindex(input);
                        break;
                    }
                    if(rightpoint(input)>0){
                        rightindex(input);
                        break;
                    }
                    if(rightdownpoint(input)>0){
                        downindex(input);
                        rightindex(input);
                        break;
                    }
                    if(downpoint(input)>0){
                        downindex(input);
                        break;
                    }
                    if(leftdownpoint(input)>0){
                        downindex(input);
                        leftindex(input);
                        break;
                    }
                    break;
                case 8:
                    if(leftuppoint(input)>0){
                        upindex(input);
                        leftindex(input);
                        break;
                    }
                    if(uppoint(input)>0){
                        upindex(input);
                        break;
                    }
                    if(rightuppoint(input)>0){
                        upindex(input);
                        rightindex(input);
                        break;
                    }
                    if(rightpoint(input)>0){
                        rightindex(input);
                        break;
                    }
                    if(rightdownpoint(input)>0){
                        downindex(input);
                        rightindex(input);
                        break;
                    }
                    if(downpoint(input)>0){
                        downindex(input);
                        break;
                    }
                    if(leftdownpoint(input)>0){
                        downindex(input);
                        leftindex(input);
                        break;
                    }
                    if(leftpoint(input)>0){
                        leftindex(input);
                        break;
                    }
                    break;


                case 1:
                    if(uppoint(input)>0){
                        upindex(input);
                        break;
                    }
                    if(rightuppoint(input)>0){
                        upindex(input);
                        rightindex(input);
                        break;
                    }
                    if(rightpoint(input)>0){
                        rightindex(input);
                        break;
                    }
                    if(rightdownpoint(input)>0){
                        downindex(input);
                        rightindex(input);
                        break;
                    }
                    if(downpoint(input)>0){
                        downindex(input);
                        break;
                    }
                    if(leftdownpoint(input)>0){
                        downindex(input);
                        leftindex(input);
                        break;
                    }
                    if(leftpoint(input)>0){
                        leftindex(input);
                        break;
                    }
                    if(leftuppoint(input)>0){
                        upindex(input);
                        leftindex(input);
                        break;
                    }
                    break;
                case 2:
                    if(rightuppoint(input)>0){
                        upindex(input);
                        rightindex(input);
                        break;
                    }
                    if(rightpoint(input)>0){
                        rightindex(input);
                        break;
                    }
                    if(rightdownpoint(input)>0){
                        downindex(input);
                        rightindex(input);
                        break;
                    }
                    if(downpoint(input)>0){
                        downindex(input);
                        break;
                    }
                    if(leftdownpoint(input)>0){
                        downindex(input);
                        leftindex(input);
                        break;
                    }
                    if(leftpoint(input)>0){
                        leftindex(input);
                        break;
                    }
                    if(leftuppoint(input)>0){
                        upindex(input);
                        leftindex(input);
                        break;
                    }
                    if(uppoint(input)>0){
                        upindex(input);
                        break;
                    }
                    break;

                case 3:
                    if(rightpoint(input)>0){
                        rightindex(input);
                        break;
                    }
                    if(rightdownpoint(input)>0){
                        downindex(input);
                        rightindex(input);
                        break;
                    }
                    if(downpoint(input)>0){
                        downindex(input);
                        break;
                    }
                    if(leftdownpoint(input)>0){
                        downindex(input);
                        leftindex(input);
                        break;
                    }
                    if(leftpoint(input)>0){
                        leftindex(input);
                        break;
                    }
                    if(leftuppoint(input)>0){
                        upindex(input);
                        leftindex(input);
                        break;
                    }
                    if(uppoint(input)>0){
                        upindex(input);
                        break;
                    }
                    if(rightuppoint(input)>0){
                        upindex(input);
                        rightindex(input);
                        break;
                    }
                    break;



                case 4:
                    if(rightdownpoint(input)>0){
                        downindex(input);
                        rightindex(input);
                        break;
                    }
                    if(downpoint(input)>0){
                        downindex(input);
                        break;
                    }
                    if(leftdownpoint(input)>0){
                        downindex(input);
                        leftindex(input);
                        break;
                    }
                    if(leftpoint(input)>0){
                        leftindex(input);
                        break;
                    }
                    if(leftuppoint(input)>0){
                        upindex(input);
                        leftindex(input);
                        break;
                    }
                    if(uppoint(input)>0){
                        upindex(input);
                        break;
                    }
                    if(rightuppoint(input)>0){
                        upindex(input);
                        rightindex(input);
                        break;
                    }
                    if(rightpoint(input)>0){
                        rightindex(input);
                        break;
                    }
                    break;


                case 5:
                    if(downpoint(input)>0){
                        downindex(input);
                        break;
                    }
                    if(leftdownpoint(input)>0){
                        downindex(input);
                        leftindex(input);
                        break;
                    }
                    if(leftpoint(input)>0){
                        leftindex(input);
                        break;
                    }
                    if(leftuppoint(input)>0){
                        upindex(input);
                        leftindex(input);
                        break;
                    }
                    if(uppoint(input)>0){
                        upindex(input);
                        break;
                    }
                    if(rightuppoint(input)>0){
                        upindex(input);
                        rightindex(input);
                        break;
                    }
                    if(rightpoint(input)>0){
                        rightindex(input);
                        break;
                    }
                    if(rightdownpoint(input)>0){
                        downindex(input);
                        rightindex(input);
                        break;
                    }
                    break;

                case 6:
                    if(leftdownpoint(input)>0){
                        downindex(input);
                        leftindex(input);
                        break;
                    }
                    if(leftpoint(input)>0){
                        leftindex(input);
                        break;
                    }
                    if(leftuppoint(input)>0){
                        upindex(input);
                        leftindex(input);
                        break;
                    }
                    if(uppoint(input)>0){
                        upindex(input);
                        break;
                    }
                    if(rightuppoint(input)>0){
                        upindex(input);
                        rightindex(input);
                        break;
                    }
                    if(rightpoint(input)>0){
                        rightindex(input);
                        break;
                    }
                    if(rightdownpoint(input)>0){
                        downindex(input);
                        rightindex(input);
                        break;
                    }
                    if(downpoint(input)>0){
                        downindex(input);
                        break;
                    }
                    break;

                case 7:
                    if(leftdownpoint(input)>0){
                        downindex(input);
                        leftindex(input);
                        break;
                    }
                    if(leftpoint(input)>0){
                        leftindex(input);
                        break;
                    }
                    if(leftuppoint(input)>0){
                        upindex(input);
                        leftindex(input);
                        break;
                    }
                    if(uppoint(input)>0){
                        upindex(input);
                        break;
                    }
                    if(rightuppoint(input)>0){
                        upindex(input);
                        rightindex(input);
                        break;
                    }
                    if(rightpoint(input)>0){
                        rightindex(input);
                        break;
                    }
                    if(rightdownpoint(input)>0){
                        downindex(input);
                        rightindex(input);
                        break;
                    }
                    if(downpoint(input)>0){
                        downindex(input);
                        break;
                    }
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
                CarsArray.Add(island.CarBorders(input, carpoint));
                island.ClearCar(input, CarsArray[CarsArray.Count-1]);
            }
        }

    }
}
