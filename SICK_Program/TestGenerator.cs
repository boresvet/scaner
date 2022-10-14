using System;
using System.Text;
using static System.Math;
using System.Threading;
using System.Threading.Tasks;
namespace Sick_test
{
    public class TestGenerator{
        public config Config;
        public int scanernumber;
        public int lanes;
        public int carheight = 1500;
        public Scanner scaner;
        line[] ray;
        public line[][] cars;
        public int[][] laneswithcarArray;
        public int[][] laneswithoutcarArray;




        public PointXYint[] RoadPoints;
        public PointXYint[] RoadPointWithCars;

        public TestGenerator(config config, int _scanernumber){
            Config = config;
            scanernumber = _scanernumber;


            scaner = config.Scanners[scanernumber];
            var scanerpoint = new PointXYint(){X = scaner.Transformations.HorisontalOffset, Y = scaner.Transformations.Height}
            ray = new line[(int)((scaner.Settings.EndAngle-scaner.Settings.StartAngle)/scaner.Settings.Resolution)];
            for(int i = 0; i < (int)((scaner.Settings.EndAngle-scaner.Settings.StartAngle)/scaner.Settings.Resolution); i++){
                ray[i] = new line();
                ray[i].createLine(new PointXYint(){X = scaner.Transformations.HorisontalOffset, Y = scaner.Transformations.Height}, scaner.Transformations.CorrectionAngle);
            }//Создание "лучей" сканера


            cars = CarGenerator(config);


            //laneswithcarArray = new int[Config.RoadSettings.Lanes.Length][];
            //laneswithoutcarArray = new int[Config.RoadSettings.Lanes.Length][];




            /*line[] AllRoadLines = 
            for(int i = 0; i<Config.RoadSettings.Lanes.Length; i++){
                RoadPoints[i] = ray[i].firstPointIntersectionSegment(scanerpoint, ray[i], AllRoadLines);
            }*/
            //Не дописана логика создания точек машин

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
    }
}
