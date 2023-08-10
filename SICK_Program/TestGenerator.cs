using System;
using static System.Math;

namespace SickScanner
{
    public class TestGenerator{
        public ResponseFullConfig Config;
        public int scanernumber;
        public int lanes;
        public int carheight = 1500;
        public Scanner scaner;
        public Sick_test.line[] ray;//Все линии лучей
        public Sick_test.line[][] cars;//Примитивные модели машин(3 линии)
        public Sick_test.line[] road; //Все линии ДОРОГИ
        public int[][] laneswithcarArray;
        public int[][] laneswithoutcarArray;

        public int[] RoadDistanceWithCars;
        public int[] RoadDistance;

        public int iter;
        public int period;
        public Sick_test.PointXYint[] RoadPoints;
        public Sick_test.PointXYint[] RoadPointWithCars;

        public TestGenerator(ResponseFullConfig config, int _scanernumber, int _period){
            period = _period;
            Config = config;
            scanernumber = _scanernumber;


            scaner = config.scanners[scanernumber];
            var scanerpoint = new Sick_test.PointXYint(){X = scaner.transformations.horisontalOffset, Y = scaner.transformations.height};
            ray = new Sick_test.line[Math.Abs((int)((float)(scaner.settings.endAngle-scaner.settings.startAngle)/scaner.settings.resolution))];
            for(int i = 0; i < (int)((scaner.settings.endAngle-scaner.settings.startAngle)/scaner.settings.resolution); i++){
                ray[i] = new Sick_test.line();
                ray[i].createLine(scanerpoint, (scaner.transformations.correctionAngle-(scaner.settings.resolution*i)));
            }//Создание "лучей" сканера


            cars = CarGenerator(config);//Генерация линий примитивных моделей машинок
            var _cars = testCarGenerator(config);//Все линии машинок в одном массиве


            laneswithcarArray = new int[Config.roadSettings.lanes.Length*2+1][];//Объявление глобальных "хранилок" точек
            laneswithoutcarArray = new int[Config.roadSettings.lanes.Length*2+1][];



            road = RoadGenerator(config);
            RoadPoints = new Sick_test.PointXYint[ray.Length];
            for(int i = 0; i<ray.Length; i++){
                RoadPoints[i] = ray[i].firstPointIntersectionSegment(scanerpoint, ray[i], road);
            }
            var cararray = new Sick_test.line[_cars.Length+road.Length];
            Array.Copy(_cars, cararray, _cars.Length);
            Array.Copy(road, 0, cararray, _cars.Length, road.Length);
            RoadPointWithCars = new Sick_test.PointXYint[ray.Length];
            for(int i = 0; i<ray.Length; i++){
                RoadPointWithCars[i] = ray[i].firstPointIntersectionSegment(scanerpoint, ray[i], cararray);
            }




            RoadDistanceWithCars = new int[RoadPointWithCars.Length];
            for(int i = 0; i < RoadPointWithCars.Length; i++){
                RoadDistanceWithCars[i] = (int)Math.Sqrt(((RoadPointWithCars[i].X-scanerpoint.X)*(RoadPointWithCars[i].X-scanerpoint.X))+((RoadPointWithCars[i].Y-scanerpoint.Y)*(RoadPointWithCars[i].Y-scanerpoint.Y)));
            }
            RoadDistance = new int[RoadPoints.Length];
            for(int i = 0; i < RoadPoints.Length; i++){
                RoadDistance[i] = (int)(Math.Sqrt(((RoadPoints[i].X-scanerpoint.X)*(RoadPoints[i].X-scanerpoint.X))+((RoadPoints[i].Y-scanerpoint.Y)*(RoadPoints[i].Y-scanerpoint.Y))));
            }

            iter = 0;
        }
        private void nextPosition(){
            if(iter >= period-1){
                iter = 0;
            }else{
                iter++;
            }
        }

        public int[] createscan(){
            nextPosition();
            //Thread.Sleep(3);
            if(period <= iter*2){
                return RoadDistanceWithCars;
            }else{
                return RoadDistance;
            }
        }
        private Sick_test.line[] RoadGenerator(ResponseFullConfig config){
            var ret = new Sick_test.line[config.roadSettings.lanes.Length+1+config.roadSettings.blinds.Length*3];
            ret[0] = new Sick_test.line();
            ret[0].createLine(new Sick_test.PointXYint{X = config.roadSettings.leftLimit, Y = config.roadSettings.downLimit}, new Sick_test.PointXYint{X = config.roadSettings.rightLimit, Y = config.roadSettings.downLimit});
            for(int i = 0; i<config.roadSettings.lanes.Length; i++){
                ret[i+1] = new Sick_test.line();
                ret[i+1].createLine(new Sick_test.PointXYint{X = config.roadSettings.lanes[i].offset, Y = config.roadSettings.lanes[i].height}, new Sick_test.PointXYint{X = (config.roadSettings.lanes[i].offset+config.roadSettings.lanes[i].width), Y = config.roadSettings.lanes[i].height});
            }//Генерация полос


            for(int i = 0; i<config.roadSettings.blinds.Length; i+=3){
                ret[i+config.roadSettings.lanes.Length+1] = new Sick_test.line();
                ret[i+config.roadSettings.lanes.Length+1].createLine(new Sick_test.PointXYint{X = config.roadSettings.blinds[i].offset, Y = config.roadSettings.downLimit}, new Sick_test.PointXYint{X = config.roadSettings.blinds[i].offset, Y = config.roadSettings.blinds[i].height+config.roadSettings.downLimit});

                ret[i+config.roadSettings.lanes.Length+2] = new Sick_test.line();
                ret[i+config.roadSettings.lanes.Length+2].createLine(new Sick_test.PointXYint{X = config.roadSettings.blinds[i].offset+config.roadSettings.blinds[i].width, Y = config.roadSettings.blinds[i].height+config.roadSettings.downLimit}, new Sick_test.PointXYint{X = config.roadSettings.blinds[i].offset, Y = config.roadSettings.blinds[i].height+config.roadSettings.downLimit});

                ret[i+config.roadSettings.lanes.Length+3] = new Sick_test.line();
                ret[i+config.roadSettings.lanes.Length+3].createLine(new Sick_test.PointXYint{X = config.roadSettings.blinds[i].offset+config.roadSettings.blinds[i].width, Y = config.roadSettings.blinds[i].height+config.roadSettings.downLimit}, new Sick_test.PointXYint{X = config.roadSettings.blinds[i].offset+config.roadSettings.blinds[i].width, Y = config.roadSettings.downLimit});
            }//Генерирует препятствия в 3 линии
            return ret;
        }
        private Sick_test.line[][] CarGenerator(ResponseFullConfig config){
            var ret = new Sick_test.line[config.roadSettings.lanes.Length][];
            for(int i = 0; i<ret.Length; i++){
                ret[i] = new Sick_test.line[3];
                if(config.roadSettings.lanes[i].width>=2200){
                    var middlepoint = config.roadSettings.lanes[i].offset + (int)(config.roadSettings.lanes[i].width/2);
                    ret[i][0] = new Sick_test.line();
                    ret[i][0].createLine(new Sick_test.PointXYint{X = middlepoint-1000, Y = config.roadSettings.lanes[i].height}, new Sick_test.PointXYint{X = middlepoint-1000, Y = config.roadSettings.lanes[i].height+carheight});

                    ret[i][1] = new Sick_test.line();
                    ret[i][1].createLine(new Sick_test.PointXYint{X = middlepoint-1000, Y = config.roadSettings.lanes[i].height+carheight}, new Sick_test.PointXYint{X = middlepoint+1000, Y = config.roadSettings.lanes[i].height+carheight});

                    ret[i][2] = new Sick_test.line();
                    ret[i][2].createLine(new Sick_test.PointXYint{X = middlepoint+1000, Y = config.roadSettings.lanes[i].height+carheight}, new Sick_test.PointXYint{X = middlepoint+1000, Y = config.roadSettings.lanes[i].height});

                    //Тут создаются текстурные линии машинок
                }else{
                    ret[i][0] = new Sick_test.line();
                    ret[i][0].createLine(new Sick_test.PointXYint{X = config.roadSettings.lanes[i].offset+100, Y = config.roadSettings.lanes[i].height}, new Sick_test.PointXYint{X = config.roadSettings.lanes[i].offset+100, Y = config.roadSettings.lanes[i].height+carheight});

                    ret[i][1] = new Sick_test.line();
                    ret[i][1].createLine(new Sick_test.PointXYint{X = config.roadSettings.lanes[i].offset+100, Y = config.roadSettings.lanes[i].height+carheight}, new Sick_test.PointXYint{X = config.roadSettings.lanes[i].offset-100+config.roadSettings.lanes[i].width, Y = config.roadSettings.lanes[i].height+carheight});

                    ret[i][2] = new Sick_test.line();
                    ret[i][2].createLine(new Sick_test.PointXYint{X = config.roadSettings.lanes[i].offset-100+config.roadSettings.lanes[i].width, Y = config.roadSettings.lanes[i].height+carheight}, new Sick_test.PointXYint{X = config.roadSettings.lanes[i].offset-100+config.roadSettings.lanes[i].width, Y = config.roadSettings.lanes[i].height});

                    //Тут тоже, но в том случае, когда полоса уже чем 2,2 метра
                }
            }
            return ret;
        }
        private Sick_test.line[] testCarGenerator(ResponseFullConfig config){
            var ret = new Sick_test.line[config.roadSettings.lanes.Length*3];
            for(int i = 0; i<config.roadSettings.lanes.Length; i++){
                if(config.roadSettings.lanes[i].width>=2200){
                    var middlepoint = config.roadSettings.lanes[i].offset + (int)(config.roadSettings.lanes[i].width/2);
                    ret[i] = new Sick_test.line();
                    ret[i].createLine(new Sick_test.PointXYint{X = middlepoint-1000, Y = config.roadSettings.lanes[i].height}, new Sick_test.PointXYint{X = middlepoint-1000+100, Y = config.roadSettings.lanes[i].height+carheight});

                    ret[i+1] = new Sick_test.line();
                    ret[i+1].createLine(new Sick_test.PointXYint{X = middlepoint-1000+100, Y = config.roadSettings.lanes[i].height+carheight}, new Sick_test.PointXYint{X = middlepoint+1000-100, Y = config.roadSettings.lanes[i].height+carheight});

                    ret[i+2] = new Sick_test.line();
                    ret[i+2].createLine(new Sick_test.PointXYint{X = middlepoint+1000-100, Y = config.roadSettings.lanes[i].height+carheight}, new Sick_test.PointXYint{X = middlepoint+1000, Y = config.roadSettings.lanes[i].height});

                    //Тут создаются текстурные линии машинок
                }else{
                    ret[i] = new Sick_test.line();
                    ret[i].createLine(new Sick_test.PointXYint{X = config.roadSettings.lanes[i].offset+100, Y = config.roadSettings.lanes[i].height}, new Sick_test.PointXYint{X = config.roadSettings.lanes[i].offset+100, Y = config.roadSettings.lanes[i].height+carheight});

                    ret[i+1] = new Sick_test.line();
                    ret[i+1].createLine(new Sick_test.PointXYint{X = config.roadSettings.lanes[i].offset+100, Y = config.roadSettings.lanes[i].height+carheight}, new Sick_test.PointXYint{X = config.roadSettings.lanes[i].offset-100+config.roadSettings.lanes[i].width, Y = config.roadSettings.lanes[i].height+carheight});

                    ret[i+2] = new Sick_test.line();
                    ret[i+2].createLine(new Sick_test.PointXYint{X = config.roadSettings.lanes[i].offset-100+config.roadSettings.lanes[i].height, Y = config.roadSettings.lanes[i].height+carheight}, new Sick_test.PointXYint{X = config.roadSettings.lanes[i].offset-100+config.roadSettings.lanes[i].width, Y = config.roadSettings.lanes[i].height});

                    //Тут тоже, но в том случае, когда полоса уже чем 2,2 метра
                }
            }
            return ret;
        }
    }
}
