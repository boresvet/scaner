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


        static void Main(){
                // Read the stream as a string, and write the string to the console.
            CircularBuffer<PointXY[]> MyGround = new CircularBuffer<PointXY[]>(1);
            TimeBuffer times = new TimeBuffer(300);

            //var InputEvent = new ManualResetEvent(false);
            var ExitEvent = new ManualResetEvent(false);
            //var RoadEvent = new ManualResetEvent(false);

            var ReadFile = File.ReadAllText("config.json");
            //Console.WriteLine(ReadFile);
            config config = JsonSerializer.Deserialize<config>(ReadFile);
            var Scaners = config.Scanners.ToArray();

            var Inputs = new AllInput(config);
            Task[] InputT;
            if(config.Test){
                InputT = Enumerable.Range(0, 3).Select(y => Task.Run(() => TestInputTask(config, Inputs.inputClass[y], ExitEvent, y))).ToArray();
            }else{
                InputT = Enumerable.Range(0, 3).Select(y => Task.Run(() => InputTask(Inputs.inputClass[y], ExitEvent))).ToArray();
            }
            //for(int y = 0; y<Scaners.Length; y++){
            //InputT[y] = Task.Run(() => InputTask(Scaners[y], MyConcurrentQueue[y], InputEvent[y], ErrorEvent[y], ExitEvent));
            //var InputT2 = Task.Run(() => InputTask(Scaners[1], MyConcurrentQueue[1], InputEvent[1], ErrorEvent[0], ExitEvent));
            //}
            var MainT = Task.Run(() => TMainT(config, Inputs, times, ExitEvent));
            var ServerT = Task.Run(() => TServerT(times, config, ExitEvent));
            //var LaneT = Task.Run(() => TLaneT(config, LaneConcurrentQueue[0], RoadEvent, ExitEvent));
            Console.ReadLine();
            ExitEvent.Set();
            Task.WaitAll(InputT.Concat(new [] {MainT, ServerT}).ToArray());
            Console.WriteLine("Завершено");
            return;
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
            //Console.WriteLine($"UpLimit: {config.RoadSettings.UpLimit}");
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
        }
        static void TServerT(TimeBuffer times, config config, ManualResetEvent ExitEvent){
            //var res = new Scanint(MyConcurrentQueue.);
            var serv = new Server(new string[] {"127.0.0.0", "9000"});
            while(true){
                if (ExitEvent.WaitOne(0)) {
                    return;
                }
                try
                {
                    serv.ServerLoop(times, config);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    if(serv.response != null)
                    {
                        serv.SendErrorResponse(500, "Internal server error");
                    }
                }
            }
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

        static void TMainT(config config, AllInput Inputs, TimeBuffer times, ManualResetEvent ExitEvent){
            var WorkScanners = new bool[Inputs.inputClass.Length];
            //Объявление массива с датчиками рабочести сканеров. 
            
            for(int i = 0; i<Inputs.inputClass.Length; i++){
                WorkScanners[i] = true;
            }
            //var LanesArray = new Scanint[config.RoadSettings.Lanes.Length];
            Scanint RoadScan;// = new Scanint(0);
            WaitHandle.WaitAll(Inputs.InputEvent);
            var pointsfilter = new Filter((int)((config.RoadSettings.RightLimit-config.RoadSettings.LeftLimit)/config.RoadSettings.Step), config.RoadSettings);




            var pointsSortTable = new PointXYint[(config.RoadSettings.RightLimit - config.RoadSettings.LeftLimit)/config.RoadSettings.Step][];
            for(var i = 0; i < pointsSortTable.Length; i++){
                pointsSortTable[i] = new PointXYint[0];
            }
            //Штука, куда складываются все точки дороги. 
            //Точки распределяются по массивам, как по вертикальным столбцам шириной в "config.RoadSettings.Step"
            //Это позволяет, вычислив машину в этих вот столбцах, использовать на них стандартный генератор островов. 
            //Соответственно, достаточно собрать генератор островов, и профит)


            var trig = false;


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
                if((WaitHandle.WaitAny(Inputs.ErrorEvent, 0)!=0)|(trig)) {
                    for(int i = 0; i<Inputs.ErrorEvent.Length; i++){
                        WorkScanners[i] = (Inputs.ErrorEvent[i].WaitOne(0)==false);
                        trig = true;
                    }
                }
                RoadScan = new Scanint(0);
                WaitHandle.WaitAny(Inputs.InputEvent);
                WaitHandle.WaitAll(Inputs.InputEvent, 50);
                for(int i = 0; i<Inputs.InputEvent.Length; i++){
                    /*if(InputEvent[i].WaitOne(0)){
                    }else{
                        Console.Write("Пропал скан ");
                        Console.WriteLine(i+1);
                    }*/
                    if(WorkScanners[i]){
                        var res = Inputs.inputClass[i].MyConcurrentQueue.ZeroPoint();
                        Inputs.InputEvent[i].Reset();
                        if(RoadScan.pointsArray.Length == 0){
                            RoadScan.time = res.time;
                        }
                        RoadScan.pointsArray = RoadScan.pointsArray.Concat(res.pointsArray).ToArray();
                    }
                }
                Sorts.HoareSort(RoadScan.pointsArray);



                int j = 0;
                while(j<RoadScan.pointsArray.Length){
                    if((RoadScan.pointsArray[j].X>config.RoadSettings.LeftLimit)&(RoadScan.pointsArray[j].X<config.RoadSettings.RightLimit)){
                        pointsSortTable[(int)((RoadScan.pointsArray[j].X-config.RoadSettings.LeftLimit)/config.RoadSettings.Step)] = pointsSortTable[(int)((RoadScan.pointsArray[j].X-config.RoadSettings.LeftLimit)/config.RoadSettings.Step)].Concat(RoadScan.pointsArray[j].ToArray()).ToArray();//Навернуть логику
                    }
                    j++;
                }
                var FilteredPoints = pointsfilter.CarPoints(pointsSortTable);
                var CarArray = AddAllIslandLane(FilteredPoints);
                //LanesArray = LaneGen(RoadScan, config.RoadSettings.Lanes);
                //Сделать дороги
                /*for(int i = 0; i<LaneConcurrentQueue.Length; i++){
                    LaneConcurrentQueue[i].AddZeroPoint(LanesArray[i]);
                }
                RoadEvent.Set();*/
                times.SaveSuperScan(new SuperScan(CarArray, pointsSortTable, RoadScan.time));



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
                Array.Fill(input, 0, startpoint+1, endpoint-startpoint+1);
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
                Array.Fill(input, 0, startpoint+1, endpoint-startpoint+1);
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

        private static void MainTask1(ConcurrentQueue<Scan> MyConcurrentQueue, ManualResetEvent InputEvent, ManualResetEvent ExitEvent)
        {
            var trigger = true;
            var mycarleanth = 0;
            //string writepath = @"/home/dan/Рабочиqwerй стол/12345/Sick-test/test.txt";
            //StreamWriter Myfyle = new StreamWriter(writepath, true);
            var GroundScan = new Scan();
            Scan qwer;
            //var Save = new PointSave("1234");
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
                                //Save.PointSaveToFile(CarCircularBuffer, mycarleanth);
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
        private static void InputTask(inputTheard Inputs, ManualResetEvent ExitEvent){
            while(true){
                try{
                    var step = (int)((Inputs.scaner.Settings.EndAngle-Inputs.scaner.Settings.StartAngle)/Inputs.scaner.Settings.Resolution);
                    //step = 286;
                    
                    var lms = new LMS1XX(Inputs.scaner.Connection.ScannerAddres, Inputs.scaner.Connection.ScannerPort, 5000, 5000);
                    var Conv = new SpetialConvertorint(-5 + Inputs.scaner.Transformations.CorrectionAngle, 185+Inputs.scaner.Transformations.CorrectionAngle, step);
                    //объявление конвертера, переводящего координаты из радиальной системы в ХУ
                    
                    lms.Connect();
                    Inputs.ErrorEvent.Reset();

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
                        Scan.pointsArray = translator.Translate(Array.FindAll(Conv.MakePoint(ConnectionResultDistanceDataint(res)), point => (point.X==0)&(point.Y==0)));
                        /*
                        Эта штука конвертирует скан из радиальных координат в ХУ, 
                        потом удаляет все "нули" - точки, совпадающие со сканером 
                        Потом - транслирует все точки в общую систему  координат дороги
                        */





                        //Console.Write(scaner.Connection.ScannerAddres.Substring(scaner.Connection.ScannerAddres.Length-1) + "  ");
                        //Console.WriteLine(res.TimeSinceStartup);
                        //Console.WriteLine(res.TimeOfTransmission);
                        Inputs.MyConcurrentQueue.AddZeroPoint(Scan);
                        Inputs.InputEvent.Set();
                        Console.Write("Принят скан от сканера  ");
                        Console.WriteLine(Inputs.scaner.Connection.ScannerAddres.Substring(Inputs.scaner.Connection.ScannerAddres.Length-1));
                    }
                }
                catch{
                    Inputs.ErrorEvent.Set();
                    Inputs.InputEvent.Set();
                }
            }
        }

        private static void TestInputTask(config config, inputTheard Inputs, ManualResetEvent ExitEvent, int scannumber){
            while(true){
                try{
                    var step = (int)((Inputs.scaner.Settings.EndAngle-Inputs.scaner.Settings.StartAngle)/Inputs.scaner.Settings.Resolution);
                    //step = 286;
                    var lms = new TestGenerator(config, scannumber); 
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
                    var oldscannumber=0;
                    while(true){
                        if (ExitEvent.WaitOne(100)) {
                            return;
                        }
                        res = lms.createscan();
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
                        Scan.pointsArray = translator.Translate(Array.FindAll(Conv.MakePoint(res), point => (point.X==0)&(point.Y==0)));
                        /*
                        Эта штука конвертирует скан из радиальных координат в ХУ, 
                        потом удаляет все "нули" - точки, совпадающие со сканером 
                        Потом - транслирует все точки в общую систему  координат дороги
                        */

                        Inputs.MyConcurrentQueue.AddZeroPoint(Scan);
                        Inputs.InputEvent.Set();
                        Console.Write("Принят скан от сканера  ");
                        Console.WriteLine(Inputs.scaner.Connection.ScannerAddres.Substring(Inputs.scaner.Connection.ScannerAddres.Length-1));
                    }
                }
                catch{
                    Inputs.ErrorEvent.Set();
                    Inputs.InputEvent.Set();
                }
            }
        }
    }
}