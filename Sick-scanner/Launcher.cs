﻿using BSICK.Sensors.LMS1xx;
using System;
using static System.Math;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using NLog;

namespace Sick_test;
public class SickScanners
{

        public static Scan AverrageCircularBuffer(CircularBuffer<Scan> MyCircularBuffer){
            var scan = new Scan(Step);
            //= MyCircularBuffer.ReadPosition();
            var ArrayPointsIndex = new int[Step];
            var leanth = MyCircularBuffer.MyLeanth;
            for(int j = 1; j<leanth; j++){
            var myscan = MyCircularBuffer.ReadPosition();
                for (int i = 0; i < Step; i++){
                    if((((int)myscan.pointsArray[i].X == 0)&((int)myscan.pointsArray[i].Y == 0))==false){
                        scan.pointsArray[i].X += (myscan.pointsArray[i].X);
                        scan.pointsArray[i].Y += (myscan.pointsArray[i].Y);
                        ArrayPointsIndex[i] += 1;
                    }
                }
                MyCircularBuffer.NextPosition();
            }
            //Console.WriteLine(Step);
            for (int i = 0; i < Step; i++){
                if(ArrayPointsIndex[i]!=0){
                    scan.pointsArray[i].X = scan.pointsArray[i].X/(ArrayPointsIndex[i]+1);
                    scan.pointsArray[i].Y = scan.pointsArray[i].Y/(ArrayPointsIndex[i]+1);
                    //Console.WriteLine(ArrayPointsIndex[i]);
                }
            }
            //Console.WriteLine(ArrayPointsIndex[100]);
            return scan;
        }

        public static int PointsHigth(double pointY){
            var result = new Int32();
            if(pointY>10.0){
                return 1;
            }else{
                result = 127+((int)(12.8*pointY));
                return result;
            }
        }
        const int Step = 286; //Количество шагов
        /*static double[] ConnectionResultDistanceData(LMDScandataResult res){
            double[] result;
            if (res.DistancesData != null){
                result = res.DistancesData.ToArray();
            }else{
                result = new double[Step];}
            return result;
        }*/
        static int[] ConnectionResultDistanceDataint(LMDScandataResult res){
            int[] result;
            if (res.DistancesData != null){
                result = res.DistancesData.ToArray();
            }else{
                result = new int[Step];}
            return result;
        }
        static string PointsToString(PointXYint[] mass){
            //var massStr = Array.ConvertAll<double, string>(XConv(mass), x => x.ToString());
            //var massStr1 = Array.ConvertAll<double, string>(YConv(mass), x => x.ToString());
            return (@"""PointsMass"":[" + String.Join(", ", mass.Select(n => n.ToString())) + "]");
        }
        static string PointsToString(PointXY[] mass){
            //var massStr = Array.ConvertAll<double, string>(XConv(mass), x => x.ToString());
            //var massStr1 = Array.ConvertAll<double, string>(YConv(mass), x => x.ToString());
            return (@"""PointsMass"":[" + String.Join(", ", mass.Select(n => n.ToString())) + "]");
        }
        static void saveTableToFile(PointXYint[][] mass){
            string writepath = $"/home/dan/Рабочий стол/Sick-test/test.json";
            StreamWriter Myfyle = new StreamWriter(writepath, true);
            Myfyle.WriteLine(@"{""PointsXY"":[" + String.Join(", ", mass.Select(n => PointsToString(n))) + "]}");
        }


        public static void RunScanners(returns returns){

                /*var line1 = new line();
                line1.createLine(new PointXYint(){X = 0, Y = 10}, new PointXYint(){X = 10, Y = 10});
                var ray3 = new line();
                ray3.createLine(new PointXY(){X = 0, Y = 0}, new PointXY(){X = 10, Y = 20});
                Console.WriteLine(line1.DistancetopointSegment(new PointXY(){X = 0, Y = 0}, ray3, line1));
                
                Тесты для линий если усё пропало
                */
                // Read the stream as a string, and write the string to the console.

            //var InputEvent = new ManualResetEvent(false);
            var ExitEvent = new ManualResetEvent(false);
            //var RoadEvent = new ManualResetEvent(false);

            var ReadFile = File.ReadAllText("config.json");
            //Console.WriteLine(ReadFile);
            config config = JsonSerializer.Deserialize<config>(ReadFile);
            var Scaners = config.Scanners.ToArray();
            TimeBuffer times = new TimeBuffer(config.SortSettings.BufferTimesLength, returns.logger, config.SortSettings.Buffers);


            var Inputs = new AllInput(config);
            Task[] InputT;
            if(config.Test){
                InputT = Enumerable.Range(0, config.Scanners.Length).Select(y => Task.Run(() => TestInputTask(config, Inputs.GetInputTheard(y), ExitEvent, y, returns.logger))).ToArray();
            }else{
                InputT = Enumerable.Range(0, config.Scanners.Length).Select(y => Task.Run(() => InputTask(Inputs.GetInputTheard(y), ExitEvent, returns.logger))).ToArray();
            }
            //for(int y = 0; y<Scaners.Length; y++){
            //InputT[y] = Task.Run(() => InputTask(Scaners[y], MyConcurrentQueue[y], InputEvent[y], ErrorEvent[y], ExitEvent));
            //var InputT2 = Task.Run(() => InputTask(Scaners[1], MyConcurrentQueue[1], InputEvent[1], ErrorEvent[0], ExitEvent));
            //}
            var MainT = Task.Run(() => TMainT(config, Inputs, times, ExitEvent, returns.logger));
            var carbuffer = new CarBuffer(config);
            var ServerT = Task.Run(() => TServerT(times, config, carbuffer, ExitEvent, returns.logger));
            //var LaneT = Task.Run(() => TLaneT(config, LaneConcurrentQueue[0], RoadEvent, ExitEvent));
            returns.initreturns(Inputs, times, carbuffer, config);
            Console.ReadLine();
            ExitEvent.Set();
            Task.WaitAll(InputT.Concat(new [] {MainT, ServerT}).ToArray());
            returns.logger.Info("Завершено");
            return;
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
            //Console.WriteLine($"UpLimit: {config.RoadSettings.UpLimit}");
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
        }

        static void TServerT(TimeBuffer times, config config, CarBuffer carbuffer, ManualResetEvent ExitEvent, Logger logger){
            while(true){
                if (ExitEvent.WaitOne(0)) {
                    return;
                }
                try
                {
                    /*while(times.IsFull == false){
                        Thread.Sleep(1000);
                    }*/                  

                    times.ReadEvent.WaitOne();

                    SuperScan[] _buffer = times.ReadFullArray();
                    logger.Info("Считан массив");
                    //Console.WriteLine("Считан массив");
                    var seach = new IslandSeach(config, logger);
                    logger.Info("Созданы острова");
                    //Console.WriteLine("Созданы острова");
                    seach.Search(_buffer, logger);
                    logger.Info("Произведён поиск");
                    //Console.WriteLine("Произведён поиск");
                    //var cars = seach.CarsArray;
                    carbuffer.UpdateCars(seach.CarsArray); //Сохранение машинок в буффер с машинками
                    logger.Info("Сохранено");
                    //Console.WriteLine("Сохранено");
                    times.RemoveReadArray();
                    logger.Info("Буффер очищен");
                    //Console.WriteLine("Буффер очищен");
                    logger.Info($"Найдено {seach.CarsArray.Count} машинок");
                    Console.WriteLine($"Найдено {seach.CarsArray.Count} машинок");



                }
                catch(Exception ex)
                {
                    logger.Fatal($"{ex}");
                    //Console.WriteLine(ex);
                }
            }
        }

        static void TMainT(config config, AllInput Inputs, TimeBuffer times, ManualResetEvent ExitEvent, Logger logger){
            try{
            //Объявление массива с датчиками рабочести сканеров. 
            //var ScannerDataReady = Enumerable.Range(0,config.Scanners.Length).Select(x => true).ToArray();
            /*for(int i = 0; i<config.Scanners.Length; i++){
                WorkScanners[i] = true;
            }*/


            //var LanesArray = new Scanint[config.RoadSettings.Lanes.Length];
            Scanint RoadScan;// = new Scanint(0);
            Inputs.WaitAnyData();
            Inputs.WaitAllData();
            Filter pointsfilter;
            var method = config.Method;
            switch (method)
            {
                case "primitive":
                    logger.Info($"Установлен режим поиска '{method}'");

                    //Console.WriteLine("Установлен режим поиска 'primitive'");
                    pointsfilter = new PrimitiveFilter(config);
                    break;
                case "primitiveAutomatic":
                    logger.Info($"Установлен режим поиска '{method}'");

                    //Console.WriteLine("Установлен режим поиска 'primitive'");
                    pointsfilter = new AutomaticPrimitiveFilter(config);
                    break;
                default:
                    logger.Error($"Ошибка режима поиска: режим '{method}' не известен. Установлен режим поиска 'primitive'");
                    //Console.WriteLine("Ошибка режима поиска: неизвестный режим. Установлен режим поиска 'primitive'");
                    pointsfilter = new PrimitiveFilter(config);
                    break;
            }
            var ConcatScanInterface = new ScanConcat(config);

            //Создание массива столбцов, каждый столбец - содержит именно точки, которые в него попадают
            var roadColumnsCount = (int)((config.RoadSettings.RightLimit - config.RoadSettings.LeftLimit)/config.RoadSettings.Step);
            var pointsSortTable = new PointXYint[roadColumnsCount][];
            for(var i = 0; i < pointsSortTable.Length; i++){
                pointsSortTable[i] = new PointXYint[0];
            }

            var Slicer = new ScanColumnArray(config);

            //Штука, куда складываются все точки дороги. 
            //Точки распределяются по массивам, как по вертикальным столбцам шириной в "config.RoadSettings.Step"
            //Это позволяет, вычислив машину в этих вот столбцах, использовать на них стандартный генератор островов. 
            //Соответственно, достаточно собрать генератор островов, и профит)


            //var res = new Scanint(MyConcurrentQueue.);
            while(true)
            {

                if (ExitEvent.WaitOne(0))
                {
                    return;
                }
                //Console.WriteLine("Ждём");

                logger.Debug("Старт ожидания даных от сканеров");

                Inputs.WaitAnyData();
                Inputs.WaitAllData();

                logger.Debug("Завершено ожидание данных от сканеров");

                //Console.WriteLine("Дождались");

                //Проверка работоспособности сканеров(упал ли поток со сканерами)
                Inputs.TestScanners();

                RoadScan = new Scanint(0);
                for (int i = 0; i < config.Scanners.Length; i++)
                {
                    /*if(InputEvent[i].WaitOne(0)){
                    }else{
                        Console.Write("Пропал скан ");
                        Console.WriteLine(i+1);
                    }*/

                    //Проверка на ошибку в сканере
                    try{
                    if (Inputs.IsScannerReady(i)) 
                    {
                        var res = Inputs.GetLastScan(i);
                        if (ConcatScanInterface.IsEmpty())
                        {
                            RoadScan.time = res.time;
                        }
                        ConcatScanInterface.Add(res.pointsArray);
                    }
                    }
                    catch(Exception ex){

                    logger.Error($"Упал сканер {config.Scanners[i].ID}, ошибка {ex}");
                    }
                }
                logger.Debug("Смешная нарезка точек в столбцы");

                RoadScan.pointsArray = ConcatScanInterface.Dequeue();
                Slicer.Add(RoadScan.pointsArray);//Смешная нарезка в столбцы
                //SliceByColumns(config, RoadScan, pointsSortTable);
                pointsSortTable = Slicer.Dequeue();
                var CarArray = pointsfilter.CarPoints(pointsSortTable);//Дорисовка машины (недостающих столбцов)
                //LanesArray = LaneGen(RoadScan, config.RoadSettings.Lanes);
                //Сделать дороги
                /*for(int i = 0; i<LaneConcurrentQueue.Length; i++){
                    LaneConcurrentQueue[i].AddZeroPoint(LanesArray[i]);
                }
                RoadEvent.Set();*/
                logger.Debug("Сохранение обработанных данных в буфер по времени");
                times.AddSuperScan(new SuperScan(CarArray, pointsSortTable, RoadScan.time));
                //Console.WriteLine($"Обработан скан дороги, всего {Array.FindAll(CarArray, point => (point != 0)).Length} столбцов с машинками");
                logger.Debug($"Обработан скан дороги, всего {Array.FindAll(CarArray, point => (point != 0)).Length} столбцов с машинками");
            }
            }catch(Exception ex){
                logger.Error($"Ошибка в потоке обработки {ex}");
                //Console.WriteLine("Ошибка в потоке обработки");
            }
        }


        //Нарезка данных в колонны
        private static void SliceByColumns(config config, Scanint RoadScan, PointXYint[][] pointsSortTable)
        {
            for (var i = 0; i < pointsSortTable.Length; i++)
            {
                pointsSortTable[i] = new PointXYint[0];
            }
            
            Sorts.HoareSort(RoadScan.pointsArray);//Сортировка точек в скане по ширине дороги

            var invertStep = 1.0 / config.RoadSettings.Step;
            int j = 0;
            while (j < RoadScan.pointsArray.Length)
            {
                if ((RoadScan.pointsArray[j].X > config.RoadSettings.LeftLimit) & (RoadScan.pointsArray[j].X < config.RoadSettings.RightLimit))
                {
                    //Вычисляет номер столбца, в который нужно добавить точку
                    var index = (int)((double)(RoadScan.pointsArray[j].X - config.RoadSettings.LeftLimit) * invertStep);
                    //Добавление точки в столбец
                    pointsSortTable[index] = pointsSortTable[index].Concat(RoadScan.pointsArray[j].ToArray()).ToArray();
                }
                j++;
            }
        }




        //с разбиением на полосы
        /*static void TMainT(config config, CircularBuffer<Scanint>[] MyConcurrentQueue, CircularBuffer<Scanint>[] LaneConcurrentQueue, ManualResetEvent[] InputEvent, ManualResetEvent RoadEvent, ManualResetEvent[] ErrorEvent, ManualResetEvent ExitEvent){
            var WorkScanners = new bool[MyConcurrentQueue.Length];
            for(int i = 0; i<MyConcurrentQueue.Length; i++){
                WorkScanners[i] = true;
            }
            var LanesArray = new Scanint[config.RoadSettings.Lanes.Length];
            var RoadScan = new Scanint(0);
            //var res = new Scanint(MyConcurrentQueue.);
            while(true){
                if (ExitEvent.WaitOne(0)) {
                    return;
                }
                if(WaitHandle.WaitAny(ErrorEvent, 0)!=0) {
                    for(int i = 0; i<MyConcurrentQueue.Length; i++){
                        if(ErrorEvent[i].WaitOne(0)){
                            WorkScanners[i] = false;
                        }
                    }
                }
                WaitHandle.WaitAll(InputEvent);
                RoadScan = new Scanint(0);

                for(int i = 0; i<MyConcurrentQueue.Length; i++){
                    var res = MyConcurrentQueue[i].ZeroPoint();
                    InputEvent[i].Reset();
                    if(RoadScan.pointsArray.Length == 0){
                        RoadScan.time = res.time;
                    }
                    RoadScan.pointsArray = RoadScan.pointsArray.Concat(res.pointsArray).ToArray();
                }
                Sorts.HoareSort(RoadScan.pointsArray);
                LanesArray = LaneGen(RoadScan, config.RoadSettings.Lanes);
                //Сделать дороги
                for(int i = 0; i<LaneConcurrentQueue.Length; i++){
                    LaneConcurrentQueue[i].AddZeroPoint(LanesArray[i]);
                }
                RoadEvent.Set();

            }
        }*/
        public static Scanint[] LaneGen(Scanint Road, Lane[] inputLanes){
            var retArray = new Scanint[inputLanes.Length];
            for(int j = 0; j < inputLanes.Length; j++){
                retArray[j] = new Scanint(0);
                int i = 0;
                var startLane = 0;
                while(Road.pointsArray[i].X<=(inputLanes[j].Offset+inputLanes[j].Width)){
                    if((Road.pointsArray[i].X>=inputLanes[j].Offset)&(startLane==0)){
                        startLane = i;
                    }
                    i++;
                }
                //Console.WriteLine(Road.pointsArray.Take(i).Skip(startLane).ToArray().Length);
                retArray[j].pointsArray = Road.copyScan().pointsArray;
                retArray[j].time = Road.time;
            }
            return retArray;
        }

        private static bool EmptyBuffer(CircularBuffer<Scanint>[] MyConcurrentQueue){
            var ret = true;
            foreach (CircularBuffer<Scanint> i in MyConcurrentQueue){
                ret = ret&i.IsEmpty;
            }
            return ret;
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

        private static void InputTask(inputTheard Inputs, ManualResetEvent ExitEvent, Logger logger){
            while(true){
                logger.Debug($"Начало запуска сканера {Inputs.id}");
                try{
                    var step = (int)((Inputs.scaner.Settings.EndAngle-Inputs.scaner.Settings.StartAngle)/Inputs.scaner.Settings.Resolution);
                    //step = 286;
                    
                    var lms = new LMS1XX(Inputs.scaner.Connection.ScannerAddres, Inputs.scaner.Connection.ScannerPort, 5000, 5000);
                    var Conv = new SpetialConvertorint(Inputs.scaner.Settings.StartAngle + Inputs.scaner.Transformations.CorrectionAngle, Inputs.scaner.Settings.EndAngle+Inputs.scaner.Transformations.CorrectionAngle, step);
                    //объявление конвертера, переводящего координаты из радиальной системы в ХУ
                    
                    logger.Debug($"Подключение к сканеру {Inputs.id}");

                    lms.Connect();
                    Inputs.ErrorEvent.Reset();

                    logger.Debug($"Начало считывания информации о сканере {Inputs.id}, подготовка таблиц перевода в XY систему координат");

                    var translator = new translator(new PointXYint(){X = Inputs.scaner.Transformations.HorisontalOffset, Y = Inputs.scaner.Transformations.Height});
                    //Объявление транслятора для переноса координат из системы сканера в систему координат дороги
                    
                    var accessResult = lms.SetAccessMode();
                    var sss = lms.Stop();
                    var startResult = lms.Start();
                    var runResult = lms.Run();
                    var contResult = lms.StartContinuous();
                    var res = lms.ScanContinious();
                    var Scan = new Scanint{
                        pointsArray = translator.Translate(Conv.MakePoint(ConnectionResultDistanceDataint(res))),
                        time = DateTime.Now
                    };
                    var oldscannumber=0;

                    logger.Debug($"Запись первого пришедшего скана {Inputs.id}");

                    while(true){
                        logger.Debug($"Проверка на ExitEvent {Inputs.id}");
                        if (ExitEvent.WaitOne(0)) {
                            lms.Disconnect();
                            return;
                        }
                        logger.Debug($"Получение значений со сканера {Inputs.id}");
                        res = lms.ScanContinious();
                        if (oldscannumber!=res.ScanCounter){ 
                            logger.Warn($"Потеряны сканы со сканера с {oldscannumber} по {res.ScanCounter}; частота сканера {res.ScanFrequency}");
                            //Console.WriteLine($"{oldscannumber} {res.ScanCounter} {res.ScanFrequency}");
                        }
                        if (res.ScanCounter == null){
                            oldscannumber++;
                        }else{
                            oldscannumber = (int)res.ScanCounter+1;
                        }
                        /*if(oldscannumber%1000 == 0){
                            Console.WriteLine("Тысячный скан");
                        }*/

                        logger.Debug($"Запись времени получения скана со сканера {Inputs.id}");
                        Scan.time = DateTime.Now;
                        logger.Debug($"Обработка данных со сканера {Inputs.id}");
                        Scan.pointsArray = translator.Translate(Array.FindAll(Conv.MakePoint(ConnectionResultDistanceDataint(res)), point => (point.X!=0)||(point.Y!=0)));
                        logger.Debug($"Данные от сканера {Inputs.id} обработаны");

                        /*
                        Эта штука конвертирует скан из радиальных координат в ХУ, 
                        потом удаляет все "нули" - точки, совпадающие со сканером 
                        Потом - транслирует все точки в общую систему  координат дороги
                        */





                        //Console.Write(scaner.Connection.ScannerAddres.Substring(scaner.Connection.ScannerAddres.Length-1) + "  ");
                        //Console.WriteLine(res.TimeSinceStartup);
                        //Console.WriteLine(res.TimeOfTransmission);
                        logger.Debug($"Сохранение сырых данных со сканера {Inputs.id}");
                        Inputs.AddRawScan(ConnectionResultDistanceDataint(res));
                        logger.Debug($"Сохранение сырых данных со сканера {Inputs.id} завершено, сохранение XY точек");
                        Inputs.AddScan(Scan);
                        logger.Debug($"Сохранение XY точек со сканера {Inputs.id} завершено");
                        Inputs.InputEvent.Set();
                        logger.Debug($"Сканер {Inputs.id} завершил работу, Мавр свободен");
                        //Console.Write("Принят скан от сканера  ");
                        //Console.WriteLine(Inputs.scaner.Connection.ScannerAddres.Substring(Inputs.scaner.Connection.ScannerAddres.Length-1));
                    }
                }
                catch{
                    Inputs.ErrorEvent.Set();
                    logger.Warn("Упал поток со сканером");
                    //Inputs.InputEvent.Set();
                }
            }
        }

        private static void TestInputTask(config config, inputTheard Inputs, ManualResetEvent ExitEvent, int scannumber, Logger logger){
            while(true){
                try{
                    var step = (int)((Inputs.scaner.Settings.EndAngle-Inputs.scaner.Settings.StartAngle)/Inputs.scaner.Settings.Resolution);
                    //step = 286;
                    var lms = new TestGenerator(config, scannumber, 30); 
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
                    while(true){
                        if (ExitEvent.WaitOne(10)) {
                            return;
                        }
                        res = lms.createscan();

                        Scan.time = DateTime.Now;
                        Scan.pointsArray = translator.Translate(Array.FindAll(Conv.MakePoint(res), point => (point.X!=0)&(point.Y!=0)));
                        /*
                        Эта штука конвертирует скан из радиальных координат в ХУ, 
                        потом удаляет все "нули" - точки, совпадающие со сканером 
                        Потом - транслирует все точки в общую систему  координат дороги
                        */

                        Inputs.AddScan(Scan);
                        Inputs.InputEvent.Set();
                        //logger.Debug($"Принят скан от сканера {Inputs.scaner.Connection.ScannerAddres.Substring(Inputs.scaner.Connection.ScannerAddres.Length-1)}");
                        //Console.Write("Принят скан от сканера  ");
                        //Console.WriteLine(Inputs.scaner.Connection.ScannerAddres.Substring(Inputs.scaner.Connection.ScannerAddres.Length-1));
                    }
                }
                catch{
                    Inputs.ErrorEvent.Set();
                    Inputs.InputEvent.Set();
                    logger.Warn($"Упал сканер {Inputs.scaner.Connection.ScannerAddres.Substring(Inputs.scaner.Connection.ScannerAddres.Length-1)}");
                }
            }
        }
}