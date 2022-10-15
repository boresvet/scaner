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
    public class TestGenerator{
        public config Config;
        public int scanernumber;
        public int lanes;
        public int carheight = 1500;
        public Scanner scaner;
        public line[] ray;//Все линии лучей
        public line[][] cars;//Примитивные модели машин(3 линии)
        public line[] road; //Все линии ДОРОГИ
        public int[][] laneswithcarArray;
        public int[][] laneswithoutcarArray;

        public int[] RoadDistanceWithCars;
        public int[] RoadDistance;

        int iter;
        int period;
        public PointXYint[] RoadPoints;
        public PointXYint[] RoadPointWithCars;

        public TestGenerator(config config, int _scanernumber){
            Config = config;
            scanernumber = _scanernumber;


            scaner = config.Scanners[scanernumber];
            var scanerpoint = new PointXYint(){X = scaner.Transformations.HorisontalOffset, Y = scaner.Transformations.Height};
            ray = new line[(int)((scaner.Settings.EndAngle-scaner.Settings.StartAngle)/scaner.Settings.Resolution)];
            for(int i = 0; i < (int)((scaner.Settings.EndAngle-scaner.Settings.StartAngle)/scaner.Settings.Resolution); i++){
                ray[i] = new line();
                ray[i].createLine(new PointXYint(){X = scaner.Transformations.HorisontalOffset, Y = scaner.Transformations.Height}, scaner.Transformations.CorrectionAngle);
            }//Создание "лучей" сканера


            cars = CarGenerator(config);//Генерация линий примитивных моделей машинок
            var _cars = testCarGenerator(config);//Все линии машинок в одном массиве


            laneswithcarArray = new int[Config.RoadSettings.Lanes.Length*2+1][];//Объявление глобальных "хранилок" точек
            laneswithoutcarArray = new int[Config.RoadSettings.Lanes.Length*2+1][];



            road = RoadGenerator(config);
            
            for(int i = 0; i<Config.RoadSettings.Lanes.Length; i++){
                RoadPoints[i] = ray[i].firstPointIntersectionSegment(scanerpoint, ray[i], road);
            }
            for(int i = 0; i<Config.RoadSettings.Lanes.Length; i++){
                RoadPointWithCars[i] = ray[i].firstPointIntersectionSegment(scanerpoint, ray[i], (new List<line>(_cars)).Concat(road).ToArray());
            }





            for(int i=0;i<RoadPointWithCars.Length; i++){
                RoadDistanceWithCars[i] = (int)Math.Sqrt(((RoadPointWithCars[i].X-scanerpoint.X)*(RoadPointWithCars[i].X-scanerpoint.X))+((RoadPointWithCars[i].Y-scanerpoint.Y)*(RoadPointWithCars[i].Y-scanerpoint.Y)));
            }
            for(int i=0;i<RoadPointWithCars.Length; i++){
                RoadDistance[i] = (int)Math.Sqrt(((RoadPoints[i].X-scanerpoint.X)*(RoadPoints[i].X-scanerpoint.X))+((RoadPoints[i].Y-scanerpoint.Y)*(RoadPoints[i].Y-scanerpoint.Y)));
            }

            iter = 0;
        }

        public int[] createscan(){
            if(period>= iter*2){
                return RoadDistanceWithCars;
            }else{
                return RoadDistance;
            }
        }
        private line[] RoadGenerator(config config){
            var ret = new line[config.RoadSettings.Lanes.Length+1+config.RoadSettings.Blinds.Length*3];
            ret[0] = new line();
            ret[0].createLine(new PointXYint{X = config.RoadSettings.LeftLimit, Y = config.RoadSettings.DownLimit}, new PointXYint{X = config.RoadSettings.RightLimit, Y = config.RoadSettings.RightLimit});
            for(int i = 0; i<config.RoadSettings.Lanes.Length; i++){
                ret[i+1] = new line();
                ret[i+1].createLine(new PointXYint{X = config.RoadSettings.Lanes[i].Offset, Y = config.RoadSettings.Lanes[i].Height}, new PointXYint{X = (config.RoadSettings.Lanes[i].Offset+config.RoadSettings.Lanes[i].Width), Y = config.RoadSettings.Lanes[i].Height});
            }//Генерация полос


            for(int i = 0; i<config.RoadSettings.Blinds.Length; i+=3){
                ret[i+config.RoadSettings.Lanes.Length] = new line();
                ret[i+config.RoadSettings.Lanes.Length].createLine(new PointXYint{X = config.RoadSettings.Blinds[i].Offset, Y = config.RoadSettings.DownLimit}, new PointXYint{X = config.RoadSettings.Blinds[i].Offset, Y = config.RoadSettings.Blinds[i].Height+config.RoadSettings.DownLimit});

                ret[i+config.RoadSettings.Lanes.Length] = new line();
                ret[i+config.RoadSettings.Lanes.Length].createLine(new PointXYint{X = config.RoadSettings.Blinds[i].Offset+config.RoadSettings.Blinds[i].Width, Y = config.RoadSettings.Blinds[i].Height+config.RoadSettings.DownLimit}, new PointXYint{X = config.RoadSettings.Blinds[i].Offset, Y = config.RoadSettings.Blinds[i].Height+config.RoadSettings.DownLimit});

                ret[i+config.RoadSettings.Lanes.Length] = new line();
                ret[i+config.RoadSettings.Lanes.Length].createLine(new PointXYint{X = config.RoadSettings.Blinds[i].Offset+config.RoadSettings.Blinds[i].Width, Y = config.RoadSettings.Blinds[i].Height+config.RoadSettings.DownLimit}, new PointXYint{X = config.RoadSettings.Blinds[i].Offset+config.RoadSettings.Blinds[i].Width, Y = config.RoadSettings.DownLimit});
            }//Генерирует препятствия в 3 линии
            return ret;
        }
        private line[][] CarGenerator(config config){
            var ret = new line[config.RoadSettings.Lanes.Length][];
            for(int i = 0; i<ret.Length; i++){
                ret[i] = new line[3];
                if(config.RoadSettings.Lanes[i].Width>=2200){
                    var middlepoint = config.RoadSettings.Lanes[i].Offset + (int)(config.RoadSettings.Lanes[i].Width/2);
                    ret[i][0] = new line();
                    ret[i][0].createLine(new PointXYint{X = middlepoint-1000, Y = config.RoadSettings.Lanes[i].Height}, new PointXYint{X = middlepoint-1000, Y = config.RoadSettings.Lanes[i].Height+carheight});

                    ret[i][1] = new line();
                    ret[i][1].createLine(new PointXYint{X = middlepoint-1000, Y = config.RoadSettings.Lanes[i].Height+carheight}, new PointXYint{X = middlepoint+1000, Y = config.RoadSettings.Lanes[i].Height+carheight});

                    ret[i][2] = new line();
                    ret[i][2].createLine(new PointXYint{X = middlepoint+1000, Y = config.RoadSettings.Lanes[i].Height+carheight}, new PointXYint{X = middlepoint+1000, Y = config.RoadSettings.Lanes[i].Height});

                    //Тут создаются текстурные линии машинок
                }else{
                    ret[i][0] = new line();
                    ret[i][0].createLine(new PointXYint{X = config.RoadSettings.Lanes[i].Offset+100, Y = config.RoadSettings.Lanes[i].Height}, new PointXYint{X = config.RoadSettings.Lanes[i].Offset+100, Y = config.RoadSettings.Lanes[i].Height+carheight});

                    ret[i][1] = new line();
                    ret[i][1].createLine(new PointXYint{X = config.RoadSettings.Lanes[i].Offset+100, Y = config.RoadSettings.Lanes[i].Height+carheight}, new PointXYint{X = config.RoadSettings.Lanes[i].Offset-100+config.RoadSettings.Lanes[i].Width, Y = config.RoadSettings.Lanes[i].Height+carheight});

                    ret[i][2] = new line();
                    ret[i][2].createLine(new PointXYint{X = config.RoadSettings.Lanes[i].Offset-100+config.RoadSettings.Lanes[i].Width, Y = config.RoadSettings.Lanes[i].Height+carheight}, new PointXYint{X = config.RoadSettings.Lanes[i].Offset-100+config.RoadSettings.Lanes[i].Width, Y = config.RoadSettings.Lanes[i].Height});

                    //Тут тоже, но в том случае, когда полоса уже чем 2,2 метра
                }
            }
            return ret;
        }
        private line[] testCarGenerator(config config){
            var ret = new line[config.RoadSettings.Lanes.Length*3];
            for(int i = 0; i<ret.Length; i++){
                if(config.RoadSettings.Lanes[i].Width>=2200){
                    var middlepoint = config.RoadSettings.Lanes[i].Offset + (int)(config.RoadSettings.Lanes[i].Width/2);
                    ret[i] = new line();
                    ret[i].createLine(new PointXYint{X = middlepoint-1000, Y = config.RoadSettings.Lanes[i].Height}, new PointXYint{X = middlepoint-1000, Y = config.RoadSettings.Lanes[i].Height+carheight});

                    ret[i+1] = new line();
                    ret[i+1].createLine(new PointXYint{X = middlepoint-1000, Y = config.RoadSettings.Lanes[i].Height+carheight}, new PointXYint{X = middlepoint+1000, Y = config.RoadSettings.Lanes[i].Height+carheight});

                    ret[i+2] = new line();
                    ret[i+2].createLine(new PointXYint{X = middlepoint+1000, Y = config.RoadSettings.Lanes[i].Height+carheight}, new PointXYint{X = middlepoint+1000, Y = config.RoadSettings.Lanes[i].Height});

                    //Тут создаются текстурные линии машинок
                }else{
                    ret[i] = new line();
                    ret[i].createLine(new PointXYint{X = config.RoadSettings.Lanes[i].Offset+100, Y = config.RoadSettings.Lanes[i].Height}, new PointXYint{X = config.RoadSettings.Lanes[i].Offset+100, Y = config.RoadSettings.Lanes[i].Height+carheight});

                    ret[i+1] = new line();
                    ret[i+1].createLine(new PointXYint{X = config.RoadSettings.Lanes[i].Offset+100, Y = config.RoadSettings.Lanes[i].Height+carheight}, new PointXYint{X = config.RoadSettings.Lanes[i].Offset-100+config.RoadSettings.Lanes[i].Width, Y = config.RoadSettings.Lanes[i].Height+carheight});

                    ret[i+2] = new line();
                    ret[i+2].createLine(new PointXYint{X = config.RoadSettings.Lanes[i].Offset-100+config.RoadSettings.Lanes[i].Width, Y = config.RoadSettings.Lanes[i].Height+carheight}, new PointXYint{X = config.RoadSettings.Lanes[i].Offset-100+config.RoadSettings.Lanes[i].Width, Y = config.RoadSettings.Lanes[i].Height});

                    //Тут тоже, но в том случае, когда полоса уже чем 2,2 метра
                }
            }
            return ret;
        }
    }
}
