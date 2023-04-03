using System.Text.Json;


namespace Sick_test
{

    public class Tests
    {
        int[] CarArray;
        config config;
        IslandSeach scans;
        int? point1, point2;
        double? point3, point4;
        line[] testlinestexture;
        line[] scanerrays;
        TimeBuffer times;
        TimeBuffer times1;
        bool reallines;
        CircularBuffer<Scanint> MyConcurrentQueue;
        CircularBuffer<Scanint> MyConcurrentQueue1;
        //CircularBuffer<int[]> MyINTConcurrentQueue;


        [SetUp]
        public void Setup()
        {

            var laserray = new line();
            laserray.createLine(new PointXYint(){X = 5500, Y = 4500}, new PointXYint(){X = 4839, Y = 3749});
            var cartexture = new line();
            cartexture.createLine(new PointXYint(){X = 2050, Y = 1600}, new PointXYint(){X = 2150, Y = 100});
            var roadtexture = new line();
            roadtexture.createLine(new PointXYint(){X = -100, Y = 100}, new PointXYint(){X = 2400, Y = 100});
            var scannerpoint = new PointXYint(){X = 5500, Y = 4500}; 
            var tyu = laserray.firstPointIntersectionSegment(scannerpoint, laserray, new line[]{cartexture, roadtexture});
            reallines = (tyu.Y == 652); 


            //Тесты математики(типовое пересечение линий)
            var line1 = new line();
            line1.createLine(new PointXYint(){X = 0, Y = 10}, new PointXYint(){X = 10, Y = 10});
            var line2 = new line();
            line2.createLine(new PointXY(){X = 0, Y = 10}, new PointXY(){X = 10, Y = 10});
            var ray1 = new line();
            ray1.createLine(new PointXYint(){X = 0, Y = 0}, new PointXYint(){X = 10, Y = 20});
            point1 = line1.DistancetopointSegment(new PointXYint(){X = 0, Y = 0}, ray1, line1);
            var ray2 = new line();
            ray2.createLine(new PointXYint(){X = 0, Y = 0}, new PointXYint(){X = 20, Y = 10});
            point2 = line1.DistancetopointSegment(new PointXYint(){X = 0, Y = 0}, ray2, line1);
            var ray3 = new line();
            ray3.createLine(new PointXY(){X = 0, Y = 0}, new PointXY(){X = 10, Y = 20});
            point3 = line2.DistancetopointSegment(new PointXY(){X = 0, Y = 0}, ray3, line2);
            var ray4 = new line();
            ray4.createLine(new PointXY(){X = 0, Y = 0}, new PointXY(){X = 20, Y = 10});
            point4 = line2.DistancetopointSegment(new PointXY(){X = 0.0, Y = 0.0}, ray4, line2);
            //Console.WriteLine(point2);

            //Тест на первое пересечение


            //Тесты логики(генерации машинок)
            //Создание тестовой машинки
                        string ReadFile= File.ReadAllText("../../../../SICK_Program/config.json");

                        //Console.WriteLine(ReadFile);
                        config = JsonSerializer.Deserialize<config>(ReadFile);


                        var inputs = new AllInput(config);

                        var Inputs = inputs.GetInputTheard(0);
                        var step = (int)((Inputs.scaner.Settings.EndAngle-Inputs.scaner.Settings.StartAngle)/Inputs.scaner.Settings.Resolution);
                        var lms = new TestGenerator(config, 0, 30); 
                        var lms1 = new TestGenerator(config, 0, 3000); //На маленьком промежутке не создаст машин

                        var Conv = new SpetialConvertorint(-5 + Inputs.scaner.Transformations.CorrectionAngle, 185+Inputs.scaner.Transformations.CorrectionAngle, step);
                        //объявление конвертера, переводящего координаты из радиальной системы в ХУ
                        
                        Inputs.ErrorEvent.Reset();

                        var translator = new translator(new PointXYint(){X = Inputs.scaner.Transformations.HorisontalOffset, Y = Inputs.scaner.Transformations.Height});
                        //Объявление транслятора для переноса координат из системы сканера в систему координат дороги
    


                    var res = lms.createscan();
                    var Scan = new Scanint{
                        pointsArray = translator.Translate(Conv.MakePoint(res)),
                        time = DateTime.Now
                    };


            var pointsfilter = new Filter(config);
            var ConcatScanInterface = new ScanConcat(config);
            TimeBuffer times = new TimeBuffer(config.SortSettings.BufferTimesLength, config.SortSettings.Buffers);

            //Создание массива столбцов, каждый столбец - содержит именно точки, которые в него попадают
            var roadColumnsCount = (int)((config.RoadSettings.RightLimit - config.RoadSettings.LeftLimit)/config.RoadSettings.Step);
            var pointsSortTable = new PointXYint[roadColumnsCount][];
            for(var i = 0; i < pointsSortTable.Length; i++){
                pointsSortTable[i] = new PointXYint[0];
            }
            Scanint res1;
            Scanint ret = new Scanint();

            var Slicer = new ScanColumnArray(config);



                    for(int i = 0; i < config.SortSettings.BufferTimesLength; i++){
                        Scan.time = DateTime.Now;
                        Scan.pointsArray = translator.Translate(Array.FindAll(Conv.MakePoint(res), point => (point.X!=0)&(point.Y!=0)));
                        Inputs.AddScan(Scan);


                        res1 = Inputs.GetLastScan();
                        ret.time = res1.time;
                        ConcatScanInterface.Add(res1.pointsArray);
                ret.pointsArray = ConcatScanInterface.Dequeue();
                pointsSortTable = Slicer.Dequeue();
                var FilteredPoints = pointsfilter.CarPoints(pointsSortTable);
                var CarArray = AddAllIslandLane(FilteredPoints);

                        times.AddSuperScan(new SuperScan(CarArray, pointsSortTable, ret.time));

                    }





        }


        private static int[] AddAllIslandLane(int[] input){
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
        public static int[] AddLineIsland(int[] input, int startpoint, int endpoint){

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
        [Test]
        public void TRUE()
        {
            Assert.Pass();
        }
        [Test]
        public void FALSE()
        {
            Assert.IsTrue(false);
        }

        /*
        В тесте ошибка
        [Test]
        public void EDIT()
        {
            Assert.IsTrue(scans.CarsArray.Count>=0);
        }*/




        [Test]
        public void Maths()
        {
            Assert.IsTrue((point1==11)&&(point2==null)&&((point3>=11.18)&&(point3<=11.1804))&&(point4==null)&&reallines);
        }
        //Тесты математики. Предыдущий - кооперация всех этих в один большой, чтобы галочка была только одна
        
    }
}

