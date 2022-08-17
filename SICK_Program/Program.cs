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
using System.Linq;

namespace Sick_test
{

    class Program
    {
        static void CreateImage(CircularBuffer<Scan> myCircularBuffer, string Filename){
            var img = new Bitmap(myCircularBuffer.MyLeanth, 1000, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var color = new Color();
            var Scans = myCircularBuffer.ReadPosition();
            var cnt = 0;
            var n = 0;
            int j = 0;
            while(myCircularBuffer.MyLeanth > j){
                Scans = myCircularBuffer.ReadPosition();
                foreach(PointXY i in Scans.pointsArray){
                    n = PointsHigth(i.Y);
                    color = System.Drawing.Color.FromArgb(n,n,127);
                    if(Abs(i.X)<10){
                        var MyPoint = (((i.X/10.0)*500.0)+500)%1000;
                        img.SetPixel(myCircularBuffer.MyLeanth,(int)MyPoint,color);
                        //img.SetPixel(0,0,color);
                    }
                cnt++;
                }
                //Console.WriteLine(PointsHigth(7.7));
            }
            img.Save(Filename, System.Drawing.Imaging.ImageFormat.Png); 
            img.Dispose();
        }
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
        static void Main1(){
            for(int p = 0; p<10; p++){

                // Read the stream as a string, and write the string to the console.
            CircularBuffer<PointXY[]> MyGround = new CircularBuffer<PointXY[]>(1);

            //var InputEvent = new ManualResetEvent(false);
                var ExitEvent = new ManualResetEvent(false);
                var RoadEvent = new ManualResetEvent(false);

                var ReadFile = File.ReadAllText("config.json");
                //Console.WriteLine(ReadFile);
                config config = JsonSerializer.Deserialize<config>(ReadFile);
                var Scaners = config.Scanners.ToArray();
                var InputEvent = new ManualResetEvent[Scaners.Length];
                for(var i = 0; i < Scaners.Length; i++){
                    InputEvent[i] = new ManualResetEvent(false);
                }

                var ErrorEvent = new ManualResetEvent[Scaners.Length];
                for(var i = 0; i < Scaners.Length; i++){
                    ErrorEvent[i] = new ManualResetEvent(false);
                }
                var MyConcurrentQueue = new CircularBuffer<Scanint>[Scaners.Length];
                for(var i = 0; i < Scaners.Length; i++){
                    MyConcurrentQueue[i] = new CircularBuffer<Scanint>(1);
                }
                var LaneConcurrentQueue = new CircularBuffer<Scanint>[config.RoadSettings.Lanes.Length];
                for(var i = 0; i < config.RoadSettings.Lanes.Length; i++){
                    LaneConcurrentQueue[i] = new CircularBuffer<Scanint>(1);
                }
                ExitEvent.Reset();
                var InputT1 = Task.Run(() => InputTask(Scaners[0], MyConcurrentQueue[0], InputEvent[0], ErrorEvent[0], ExitEvent));
                var InputT2 = Task.Run(() => InputTask(Scaners[1], MyConcurrentQueue[1], InputEvent[1], ErrorEvent[0], ExitEvent));
                var InputT3 = Task.Run(() => InputTask(Scaners[2], MyConcurrentQueue[2], InputEvent[2], ErrorEvent[0], ExitEvent));
                var MainT = Task.Run(() => TMainT(config, MyConcurrentQueue, InputEvent, ErrorEvent, ExitEvent));
                //var LaneT = Task.Run(() => TLaneT(config, LaneConcurrentQueue[0], RoadEvent, ExitEvent));
                //Console.ReadLine();
                Thread.Sleep(600000);
                ExitEvent.Set();
                Task.WaitAll( MainT, InputT1, InputT2, InputT3);
            }
            Console.WriteLine("Завершено");
            return;
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
            //Console.WriteLine($"UpLimit: {config.RoadSettings.UpLimit}");
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
           // Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
        }


        static void Main(){

                // Read the stream as a string, and write the string to the console.
            CircularBuffer<PointXY[]> MyGround = new CircularBuffer<PointXY[]>(1);

            //var InputEvent = new ManualResetEvent(false);
            var ExitEvent = new ManualResetEvent(false);
            var RoadEvent = new ManualResetEvent(false);

            var ReadFile = File.ReadAllText("config.json");
            //Console.WriteLine(ReadFile);
            config config = JsonSerializer.Deserialize<config>(ReadFile);
            var Scaners = config.Scanners.ToArray();
            var InputEvent = new ManualResetEvent[Scaners.Length];
            for(var i = 0; i < Scaners.Length; i++){
                InputEvent[i] = new ManualResetEvent(false);
            }

            var ErrorEvent = new ManualResetEvent[Scaners.Length];
            for(var i = 0; i < Scaners.Length; i++){
                ErrorEvent[i] = new ManualResetEvent(false);
            }
            var MyConcurrentQueue = new CircularBuffer<Scanint>[Scaners.Length];
            for(var i = 0; i < Scaners.Length; i++){
                MyConcurrentQueue[i] = new CircularBuffer<Scanint>(1);
            }
            var LaneConcurrentQueue = new CircularBuffer<Scanint>[config.RoadSettings.Lanes.Length];
            for(var i = 0; i < config.RoadSettings.Lanes.Length; i++){
                LaneConcurrentQueue[i] = new CircularBuffer<Scanint>(1);
            }
            var InputT1 = Task.Run(() => InputTask(Scaners[0], MyConcurrentQueue[0], InputEvent[0], ErrorEvent[0], ExitEvent));
            var InputT2 = Task.Run(() => InputTask(Scaners[1], MyConcurrentQueue[1], InputEvent[1], ErrorEvent[0], ExitEvent));
            var InputT3 = Task.Run(() => InputTask(Scaners[2], MyConcurrentQueue[2], InputEvent[2], ErrorEvent[0], ExitEvent));
            var MainT = Task.Run(() => TMainT(config, MyConcurrentQueue, InputEvent, ErrorEvent, ExitEvent));
            //var LaneT = Task.Run(() => TLaneT(config, LaneConcurrentQueue[0], RoadEvent, ExitEvent));
            Console.ReadLine();
            ExitEvent.Set();
            Task.WaitAll( MainT, InputT1, InputT2, InputT3);
            Console.WriteLine("Завершено");
            return;
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
            //Console.WriteLine($"UpLimit: {config.RoadSettings.UpLimit}");
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
            Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
        }
        static void TLaneT(config config, CircularBuffer<Scanint> LaneConcurrentQueue, ManualResetEvent RoadEvent, ManualResetEvent ExitEvent){
            //var res = new Scanint(MyConcurrentQueue.);
            while(true){
                if (ExitEvent.WaitOne(0)) {
                    return;
                }
                RoadEvent.WaitOne();
/*                 for(int i = 0; i<MyConcurrentQueue.Length; i++){
                    RoadScan = new Scanint(0);
                    MyConcurrentQueue[i].TryDequeue(out var res);
                    InputEvent[i].Reset();
                    if(RoadScan.pointsArray.Length == o){
                        RoadScan.DateTime = res.DateTime;
                    }
                    RoadScan.pointsArray = RoadScan.pointsArray.Concat().ToArray();
                } */
                //Console.WriteLine("356673");
                RoadEvent.Reset();
            }
        }

        static void TMainT(config config, CircularBuffer<Scanint>[] MyConcurrentQueue, ManualResetEvent[] InputEvent, ManualResetEvent[] ErrorEvent, ManualResetEvent ExitEvent){
            var WorkScanners = new bool[MyConcurrentQueue.Length];
            for(int i = 0; i<MyConcurrentQueue.Length; i++){
                WorkScanners[i] = true;
            }
            var LanesArray = new Scanint[config.RoadSettings.Lanes.Length];
            var RoadScan = new Scanint(0);
            WaitHandle.WaitAll(InputEvent);




                var pointsSortTable = new PointXYint[(config.RoadSettings.RightLimit - config.RoadSettings.LeftLimit)/20][];
                for(var i = 0; i < pointsSortTable.Length; i++){
                    pointsSortTable[i] = new PointXYint[0];
                }






            //var res = new Scanint(MyConcurrentQueue.);
            while(true){

                if (ExitEvent.WaitOne(0)) {
                    /*int o = 0;
                    int len = 0;
                    var groundTable = new PointXYint[pointsSortTable.Length];

                    //saveTableToFile(pointsSortTable);
                    for(int l = 0; l<pointsSortTable.Length; l++){
                        o = o+pointsSortTable[l].Length;
                    }
                    var 
                    for(int l = 0; l<pointsSortTable.Length; l++){
                        if(pointsSortTable[l].Length == 0){
                            //Console.Write("Пустой столбец ");
                            //Console.WriteLine(l);
                        }else{
                            if(o/pointsSortTable.Length<pointsSortTable[l].Length){
                            //Console.Write("Заполнен столбец ");
                            //Console.Write(l);
                            //Console.Write(" Значений: ");
                            //Console.WriteLine(pointsSortTable[l].Length);

                            len++;
                            }
                        }
                        o = o+pointsSortTable[l].Length;
                    }
                    Console.Write("Всего столбцов ");
                    Console.WriteLine(pointsSortTable.Length);
                    Console.Write("среднее арифм по столбцам ");
                    Console.WriteLine(o/pointsSortTable.Length);
                    Console.Write("Итого столбцов: ");
                    Console.WriteLine(len);*/
                    
                    return;
                }
                if(WaitHandle.WaitAny(ErrorEvent, 0)!=0) {
                    for(int i = 0; i<MyConcurrentQueue.Length; i++){
                        if(ErrorEvent[i].WaitOne(0)){
                            WorkScanners[i] = false;
                        }
                    }
                }
                RoadScan = new Scanint(0);
                WaitHandle.WaitAny(InputEvent);
                WaitHandle.WaitAll(InputEvent, 50);
                for(int i = 0; i<MyConcurrentQueue.Length; i++){
                    /*if(InputEvent[i].WaitOne(0)){
                    }else{
                        Console.Write("Пропал скан ");
                        Console.WriteLine(i+1);
                    }*/
                    var res = MyConcurrentQueue[i].ZeroPoint();
                    InputEvent[i].Reset();
                    if(RoadScan.pointsArray.Length == 0){
                        RoadScan.time = res.time;
                    }
                    RoadScan.pointsArray = RoadScan.pointsArray.Concat(res.pointsArray).ToArray();
                }
                Sorts.HoareSort(RoadScan.pointsArray);




                int j = 0;
                while(j<RoadScan.pointsArray.Length){
                    if((RoadScan.pointsArray[j].X>config.RoadSettings.LeftLimit)&(RoadScan.pointsArray[j].X<config.RoadSettings.RightLimit)){
                        pointsSortTable[(int)((RoadScan.pointsArray[j].X-config.RoadSettings.LeftLimit)/20)] = pointsSortTable[(int)((RoadScan.pointsArray[j].X-config.RoadSettings.LeftLimit)/20)].Concat(RoadScan.pointsArray[j].ToArray()).ToArray();
                    }
                    j++;
                }
                //LanesArray = LaneGen(RoadScan, config.RoadSettings.Lanes);
                //Сделать дороги
                /*for(int i = 0; i<LaneConcurrentQueue.Length; i++){
                    LaneConcurrentQueue[i].AddZeroPoint(LanesArray[i]);
                }
                RoadEvent.Set();*/




                Console.WriteLine("Обработан скан дороги");
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
        static void TestInputT(Scanner scaner, ConcurrentQueue<Scanint> MyConcurrentQueue, ManualResetEvent InputEvent, ManualResetEvent ExitEvent, ManualResetEvent ErrorEvent)
        {
            try{
            var step = (int)((scaner.Settings.EndAngle-scaner.Settings.StartAngle)/scaner.Settings.Resolution);
            var res = new int[step];            
            var translator = new translator(new PointXYint{X = scaner.Transformations.HorisontalOffset, Y = scaner.Transformations.Height});
            var testgen = new TestGenerator(step,  5, -5, 10, -5+scaner.Transformations.CorrectionAngle, 185+scaner.Transformations.CorrectionAngle);
            var Conv = new SpetialConvertorint(-5+scaner.Transformations.CorrectionAngle, 185+scaner.Transformations.CorrectionAngle, Step);
            var Scan = new Scanint{
                pointsArray = translator.Translate(Conv.MakePoint(res)),
                time = DateTime.Now
            };
            while(true){
                if (ExitEvent.WaitOne(0)) {
                    //lms.Disconnect();
                    return;
                }
                res = testgen.RawScanIntGen();

                /*if(oldscannumber%1000 == 0){
                    Console.WriteLine("Тысячный скан");
                }*/
                Scan.time = DateTime.Now;
                Scan.pointsArray = Conv.MakePoint(res);
                //Console.WriteLine(Scan.time);
                MyConcurrentQueue.Enqueue(Scan);
                InputEvent.Set();
                }
            }

            catch{
                ErrorEvent.Set();
                return;
            }
        }
        private static bool EmptyBuffer(CircularBuffer<Scanint>[] MyConcurrentQueue){
            var ret = true;
            foreach (CircularBuffer<Scanint> i in MyConcurrentQueue){
                ret = ret&i.IsEmpty;
            }
            return ret;
        }
        private static void MainTask(config config, CircularBuffer<Scanint>[] MyConcurrentQueue, ManualResetEvent[] InputEvent, ManualResetEvent RoadEvent, ManualResetEvent[] ErrorEvent, ManualResetEvent ExitEvent)
        {   
            //Task.Delay(100000);

            
            var GroundScan = new Scan();
            Scanint qwer;
            var ScansArray = new Scanint[MyConcurrentQueue.Length];
            for(var i = 0; i<MyConcurrentQueue.Length; i++){
                ScansArray[i] = new Scanint(286);
            }
            WaitHandle.WaitAll(InputEvent, 50);
            var WorkScanners = new bool[MyConcurrentQueue.Length];
            for(int i = 0; i<MyConcurrentQueue.Length; i++){
                WorkScanners[i] = true;
            }
            //var timespan = New TimeSpan(5000, 5000, 5000);
            //CircularBuffer<Scanint> MyCircularBuffer = new CircularBuffer<Scanint>(10000);
            while (true){
                
                WaitHandle.WaitAll(InputEvent, 50);
                //var Number = WaitHandle.WaitAny(new[] {InputEvent, ExitEvent});
                //while(EmptyBuffer(MyConcurrentQueue));
                if(WaitHandle.WaitAny(ErrorEvent, 0)!=0) {
                    for(int i = 0; i<config.Scanners.Length; i++){
                        if(ErrorEvent[i].WaitOne(0)){
                            WorkScanners[i] = false;
                        }
                    }
                }
                if(WaitHandle.WaitAny(InputEvent, 0)!=0) {
                    for(int i = 0; i<config.Scanners.Length; i++){
                        var res = MyConcurrentQueue[i].ZeroPoint();
                    }
                }
                //Console.WriteLine(ScansArray[2].time);
                //MyCircularBuffer.Enqueue(qwer);
                /*if(MyCircularBuffer.IsEmpty){
                ground.InitGround(ground.RawScanConvertor(qwer.pointsArray));
                }*/                    //Console.WriteLine(MyCircularBuffer.MyLeanth);
                foreach (ManualResetEvent i in InputEvent){
                    i.Reset();
                }
            }
        }
        static void Main2(){
            ConcurrentQueue<Scan> MyConcurrentQueue = new ConcurrentQueue<Scan>();
            CircularBuffer<PointXY[]> MyGround = new CircularBuffer<PointXY[]>(1);
            var InputEvent = new ManualResetEvent(false);
            var ExitEvent = new ManualResetEvent(false);
            //var dump = @"asciidump/scan_[--ffff-192.168.5.241]-2111_637563296658652353.bin";
            //var s = new FileStream(dump, FileMode.Open, FileAccess.Read);
            //var r = LMDScandataResult.Parse(s);
            //var InputT = Task.Run(() => InputTask("192.168.43.241", MyConcurrentQueue, InputEvent, ExitEvent));
            //var MainT = Task.Run(() => MainTask(MyConcurrentQueue, InputEvent, ExitEvent));
            Console.ReadLine();
            ExitEvent.Set();
            //Task.WaitAll(InputT, MainT);PointXYint
            Console.WriteLine("Завершено");
            return;
            /*for (int i = 0; i < pos.Count(); i++)
            {
                Console.WriteLine($"{addr},  {i + 1}, {qwer[i].X},  {qwer[i].Y} ");
            }*/
            /*var translatePoint = new PointXY();
            translatePoint.X = 17;
            translatePo}int.Y = 4;
            var TestGen = new TestGenerator(Step, 5, -5, 10, -5, 185);
            var RawScan = TestGen.ScanGen();
            var Translator = new translator(translatePoint);
            var scan = Translator.Translate(RawScan);
            Console.WriteLine();*/
        }

        static void Main12()
        {
            var yutu = new Scan(546);
            var TestGen = new TestGenerator(286, 5, -5, 10, -5, 185);
            var RawScan = TestGen.RawScanGen();
            int Step = 286;
            var Conv = new SpetialConvertor(-5, 185, Step);
            var car = new MyCar(286);
            var ground = new Ground(Step, -5, 185);
            var Scan = new Scan{
                pointsArray = Conv.MakePoint(RawScan),
                time = DateTime.Now
            };
            ground.InitGround(RawScan);
            var reterror = false;
            for(int i = 0; i<10000000; i++){
                RawScan = TestGen.RawScanGen();
                /*if (oldscannumber!RawScanGene($"{oldscannumber} {res.ScanCounter} {res.ScanFrequency}");
                }
                if (res.ScanCounter == null){
                    oldscannumber++;
                }else{
                    oldscannumber = (int)res.ScanCounter+1;
                }*/
                /*if(oldscannumber%1000 == 0){
                    Console.WriteLine("Тысячный скан");
                }*/
                Scan.time = DateTime.Now;
                Scan.pointsArray = Conv.MakePoint(RawScan);
                var MyCar = car.CreatCarScan(Scan.pointsArray, ground.MyGround());
                ground.UpdateGround(Scan.pointsArray, MyCar);
                var mycar = car.SeeCar(MyCar);
                var testcar = (Scan.pointsArray[159].Y <= 8.5);
                if(mycar != testcar){
                    reterror = true;
                    Console.WriteLine($"Номер скана:{i}, полученное значение:{mycar}, значение 159й точки:{ground.MyGround()[159].Y}");
                }
                if(mycar){
                    //reterror = true;
                    Console.WriteLine($"Номер скана:{i}, полученное значение:{mycar}, значение 159й точки:{ground.MyGround()[159].Y}");
                }
            }
            /*if(reterror){
                Assert.Fail("Усёплохо");
            }else{
                Assert.Pass("Усёхорошо");
            }*/
        }

        private static void MainTask1(ConcurrentQueue<Scan> MyConcurrentQueue, ManualResetEvent InputEvent, ManualResetEvent ExitEvent)
        {   
            var trigger = true;
            var mycarleanth = 0;
            //string writepath = @"/home/dan/Рабочиqwerй стол/12345/Sick-test/test.txt";
            //StreamWriter Myfyle = new StreamWriter(writepath, true);
            var GroundScan = new Scan();
            Scan qwer;
            var Save = new PointSave("1234");
            var car = new MyCar(Step);
            var MyCar = new Scan(286);
            var ground = new Ground(Step, -5, 185);
            CircularBuffer<Scan> MyCircularBuffer = new CircularBuffer<Scan>(10000);
            CircularBuffer<Scan> CarCircularBuffer = new CircularBuffer<Scan>(10000);
            //var trigger = true;
            var boololdcar = false;
            var boolcar = false;
            //string picture = "test1.png";
            var Size = new CarSize();
            bool trig = true;
            while (true)
            {   
                var Number = WaitHandle.WaitAny(new[] {InputEvent, ExitEvent});
                if(Number == 1) break;
                InputEvent.Reset();
                while(MyConcurrentQueue.IsEmpty);
                var sortArray = new Scan[MyConcurrentQueue.Count];
                for(int i = 0;i<sortArray.Length; i++){
                    //Console.WriteLine(qwer.time);
                    //Console.WriteLine(MyCircularBuffer.IsEmpty);
                    MyConcurrentQueue.TryDequeue(out qwer);
                    MyCircularBuffer.Enqueue(qwer);
                    /*if(MyCircularBuffer.IsEmpty){
                        ground.InitGround(ground.RawScanConvertor(qwer.pointsArray));
                    }*/
                    sortArray[i] = qwer;
                    //Console.WriteLine(MyCircularBuffer.MyLeanth);
                }
                sortArray.Reverse();
                for(int i = 0;i<sortArray.Length; i++){
                    var qwe = sortArray[i];
                    if((!ground.InitedGround)&(MyCircularBuffer.MyLeanth>=1000)){
                        ground.InitGround(MyCircularBuffer);
                        Console.WriteLine("Успешная инициализация");
                    }
                    //Console.WriteLine($" Настоящее значение:{qwe.pointsArray[159].Y}, по");
                    if((ground.InitedGround)&(MyCircularBuffer.MyLeanth>=0)){
                        MyCar.pointsArray = car.CreatCarScan(qwe.pointsArray, ground.MyGround());
                        ground.UpdateGround(qwe.pointsArray, MyCar.pointsArray);
                        if(car.SeeCar(MyCar.pointsArray)){
                            CarCircularBuffer.Enqueue(MyCar.copyScan());
                            trig = false;
                        }else{
                            if(trig){
                            CarCircularBuffer.CleanBuffer();
                            } else {
                                Console.WriteLine($" Поймана машинка: {MyCircularBuffer.ReadPosition().time}");
                                Save.PointSaveToFile(CarCircularBuffer, mycarleanth);
                                var size = Size.RetCarSize(CarCircularBuffer, ground.GroundScan.copyScan());
                                Console.WriteLine($" Размеры: Ширина {size.X}, Высота: {size.Y}");
                                CarCircularBuffer.CleanBuffer();
                                trig = true;
                            };
                        }
                    }
                }
                //if((MyCircularBuffer.MyLeanth >= 5000)&(MyCircularBuffer.MyLeanth <= 5050)) Console.WriteLine("Ура, пять тысясяч пришло!))");
                /*while(MyCircularBuffer.IsEmpty == false){
                    Myfyle.WriteLine($"{MyCircularBuffer.ReadPosition().time}   {PointsToString(MyCircularBuffer.Dequeve1().pointsArray)}");
                }*/
                //Console.WriteLine("Файл записан");
                /*if(ground.InitedGround){
                    /*if(car.SeeCar(CarCircularBuffer.ReadPosition().pointsArray) != (MyCircularBuffer.ReadPosition().pointsArray[159].Y <= 8.5)){
                        Console.WriteLine($" Настоящее значение:{CarCircularBuffer.ReadPosition().pointsArray[159].Y >= 0.1}, полученное значение:{car.SeeCar(CarCircularBuffer.ReadPosition().pointsArray)}, значение 159й точки Земли:{ground.MyGround()[159].Y}, значение 159й точки Машины:{CarCircularBuffer.ReadPosition().pointsArray[159].Y}, Значение исходника {MyCircularBuffer.ReadPosition().pointsArray[159].Y}");
                    }else{
                        if(car.SeeCar(CarCircularBuffer.ReadPosition().pointsArray)){
                            //Console.WriteLine("Успех!");
                        }
                    }*/
                    /*boolcar = car.SeeCar(CarCircularBuffer.ReadPosition().pointsArray);
                    if((boolcar)&(boololdcar == false)){
                        Console.WriteLine($" Поймана машинка: {MyCircularBuffer.ReadPosition().time}");
                        if(trigger){
                            Save.PointSaveToFile(CarCircularBuffer, mycarleanth);
                            var size = Size.RetCarSize(CarCircularBuffer, ground.GroundScan.copyScan());
                            Console.WriteLine($" Размеры: Ширина {size.X}, Высота: {size.Y}");
                        }
                        //trigger = false;
                    }
                    if(boolcar == true){
                        mycarleanth +=1;
                    }else{
                        mycarleanth = 0;
                    }
                    boololdcar = boolcar;
                    /*if(MyCircularBuffer.MyLeanth%1000 == 0){
                        Console.WriteLine(MyCircularBuffer.MyLeanth);
                    }*/
                    /*if((MyCircularBuffer.MyLeanth >=10000)&(trigger)){
                        CreateImage(MyCircularBuffer, "test.png");
                        trigger = false;
                    }*/  //Создание картинок
                    //var i = 0;
                    /*if((MyCircularBuffer.MyLeanth >=10000)&(trigger)){
                        Console.WriteLine("------");
                        var Averrage = AverrageCircularBuffer(MyCircularBuffer);
                        trigger = false;
                        Console.WriteLine(Averrage.pointsArray[34].X);
                        Console.WriteLine(MyCircularBuffer.MyLeanth);
                    }*/

                //}
            }
        }
        private static void InputTask(Scanner scaner, CircularBuffer<Scanint> MyConcurrentQueue, ManualResetEvent InputEvent, ManualResetEvent ErrorEvent, ManualResetEvent ExitEvent){
            try{
                var step = (int)((scaner.Settings.EndAngle-scaner.Settings.StartAngle)/scaner.Settings.Resolution);
                //step = 286;
                var lms = new LMS1XX(scaner.Connection.ScannerAddres, scaner.Connection.ScannerPort, 5000, 5000);
                var Conv = new SpetialConvertorint(-5 + scaner.Transformations.CorrectionAngle, 185+scaner.Transformations.CorrectionAngle, step);
                lms.Connect();
                var translator = new translator(new PointXYint(){X = scaner.Transformations.HorisontalOffset, Y = scaner.Transformations.Height});
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
                while(true){
                    if (ExitEvent.WaitOne(0)) {
                        lms.Disconnect();
                        return;
                    }
                    res = lms.ScanContinious();
                    if (oldscannumber!=res.ScanCounter){ 
                        Console.WriteLine($"{oldscannumber} {res.ScanCounter} {res.ScanFrequency}");
                    }
                    if (res.ScanCounter == null){
                        oldscannumber++;
                    }else{
                        oldscannumber = (int)res.ScanCounter+1;
                    }
                    /*if(oldscannumber%1000 == 0){
                        Console.WriteLine("Тысячный скан");
                    }*/
                    Scan.time = DateTime.Now;
                    Scan.pointsArray = Conv.MakePoint(ConnectionResultDistanceDataint(res));
                    //Console.Write(scaner.Connection.ScannerAddres.Substring(scaner.Connection.ScannerAddres.Length-1) + "  ");
                    //Console.WriteLine(res.TimeSinceStartup);
                    //Console.WriteLine(res.TimeOfTransmission);
                    MyConcurrentQueue.AddZeroPoint(Scan);
                    InputEvent.Set();
                    //Console.Write("Принят скан от сканера  ");
                    //Console.WriteLine(scaner.Connection.ScannerAddres.Substring(scaner.Connection.ScannerAddres.Length-1));
                }
            }
            catch{
                ErrorEvent.Set();
                InputEvent.Set();
            }
        }
        private static void InputTask1(string addr, ConcurrentQueue<Scan> MyConcurrentQueue, ManualResetEvent InputEvent, ManualResetEvent ExitEvent)
        {
            var endgrade = 185;
            var begingrade = -5;
            /*var lms = new LMS1XX(addr, 2111, 5000, 5000);*/
            //Conv = new SpetialConvertor(-5, 185, Step);
            /*lms.Connect();
            var accessResult = lms.SetAccessMode();
            var sss = lms.Stop();
            var startResult = lms.Start();
            var runResult = lms.Run();
            var contResult = lms.StartContinuous();*/
            var TestGen = new TestGenerator(Step, 5, -5, 10, begingrade, endgrade);
            var RawScan = TestGen.RawScanGen();
            Thread.Sleep(10);
            var Conv = new SpetialConvertor(begingrade, endgrade, Step);
            //NewGround.InitGround(ConnectionResultDistanceData(res));
            var Scan = new Scan{
                pointsArray = Conv.MakePoint(RawScan),
                time = DateTime.Now
            };
            while(true){
                if (ExitEvent.WaitOne(0)) {
                    //lms.Disconnect();
                    return;
                }
                RawScan = TestGen.RawScanGen();
                /*if (oldscannumber!=res.ScanCounter){ 
                    Console.WriteLine($"{oldscannumber} {res.ScanCounter} {res.ScanFrequency}");
                }
                if (res.ScanCounter == null){
                    oldscannumber++;
                }else{
                    oldscannumber = (int)res.ScanCounter+1;
                }*/
                /*if(oldscannumber%1000 == 0){
                    Console.WriteLine("Тысячный скан");
                }*/
                Scan.time = DateTime.Now;
                Thread.Sleep(10);
                Scan.pointsArray = Conv.MakePoint(RawScan);
                MyConcurrentQueue.Enqueue(Scan.copyScan());
                InputEvent.Set();
            }
        }
    }
}