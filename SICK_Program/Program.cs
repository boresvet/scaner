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
    public class Lane{
        public int ID { get; set; }
        public int Offset { get; set; }
        public int Width { get; set; }
    }
    
    public class config{
        public RoadSetting RoadSettings { get; set; }
        public Scanner[] Scanners { get; set; }
    }
    public class RoadSetting{
        public int UpLimit { get; set; }
        public int DownLimit { get; set; }
        public int LeftLimit { get; set; }
        public int RightLimit { get; set; }
        public Lane[] Lanes { get; set; }
    }

    public class Scanner{
        public int ID { get; set; }
        public Connection Connection { get; set; }
        public Settings Settings { get; set; }
        public Transformations Transformations { get; set; }
    }
    public class Connection{
        public string ScannerAddres { get; set; }
        public int ScannerPort { get; set; }
    }
    public class Settings{
        public int Frequency { get; set; }
        public int StartAngle { get; set; }
        public int EndAngle { get; set; }
        public double Resolution { get; set; }
    }
    public class Transformations{
        public int Height { get; set; }
        public int HorisontalOffset { get; set; }
        public int CorrectionAngle { get; set; }

    }
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
                    Console.WriteLine(ArrayPointsIndex[i]);
                }
            }
            Console.WriteLine(ArrayPointsIndex[100]);
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
        static double[] XConv(PointXY[] mass){
            double[] ret = new double[Step]; 
            for(int i = 0; i<mass.Length; i++){
                ret[i] = (double)mass[i].X;
            }
            return ret;
        }

        static double[] YConv(PointXY[] mass){
            double[] ret = new double[Step]; 
            for(int i = 0; i<mass.Length; i++){
                ret[i] = (double)mass[i].Y;
            }
            return ret;
        }
        static string PointsToString(PointXY[] mass){
            //var massStr = Array.ConvertAll<double, string>(XConv(mass), x => x.ToString());
            //var massStr1 = Array.ConvertAll<double, string>(YConv(mass), x => x.ToString());
            return String.Join(" ", XConv(mass).Select(n => n.ToString())) + " &&& " + String.Join(" ", YConv(mass).Select(n => n.ToString()));
        }

        static void Main(){

                // Read the stream as a string, and write the string to the console.
            ConcurrentQueue<Scanint> MyConcurrentQueue = new ConcurrentQueue<Scanint>();
            CircularBuffer<PointXY[]> MyGround = new CircularBuffer<PointXY[]>(1);
            var InputEvent = new ManualResetEvent(false);
            var ExitEvent = new ManualResetEvent(false);
            
            var ReadFile = File.ReadAllText("config.json");
            Console.WriteLine(ReadFile);
            config config = JsonSerializer.Deserialize<config>(ReadFile);
            var Scaners = config.Scanners.ToArray();
            var InputT1 = Task.Run(() => InputTask(Scaners[0], MyConcurrentQueue, InputEvent, ExitEvent));
            var InputT2 = Task.Run(() => InputTask(Scaners[1], MyConcurrentQueue, InputEvent, ExitEvent));
            var InputT3 = Task.Run(() => InputTask(Scaners[2], MyConcurrentQueue, InputEvent, ExitEvent));
            Console.ReadLine();
            ExitEvent.Set();
            Task.WaitAll(InputT1, InputT2, InputT3);
            Console.WriteLine("Завершено");
            return;
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
            //Console.WriteLine($"UpLimit: {config.RoadSettings.UpLimit}");
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
            //Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
            Console.WriteLine($"DownLimit: {config.RoadSettings.DownLimit}");
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
            var MainT = Task.Run(() => MainTask(MyConcurrentQueue, InputEvent, ExitEvent));
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
        static void Main1()
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
        private static void MainTask(ConcurrentQueue<Scan> MyConcurrentQueue, ManualResetEvent InputEvent, ManualResetEvent ExitEvent)
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
                    MyCircularBuffer.Enqueue1(qwer);
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
                            CarCircularBuffer.Enqueue1(MyCar.copyScan());
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
        private static void InputTask(Scanner scaner, ConcurrentQueue<Scanint> MyConcurrentQueue, ManualResetEvent InputEvent, ManualResetEvent ExitEvent)
        {
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
                Console.Write(scaner.Connection.ScannerAddres.Substring(scaner.Connection.ScannerAddres.Length-1) + "  ");
                Console.WriteLine(res.TimeSinceStartup);
                //Console.WriteLine(res.TimeOfTransmission);
                MyConcurrentQueue.Enqueue(Scan);
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