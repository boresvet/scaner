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
namespace Sick_test
{
    public struct PointXY{
        public double X,Y;

    }
    public class Scan{
        public PointXY[] pointsArray; 
        public DateTime time;

        public Scan(int count){
            pointsArray = new PointXY[count];
        }
        public Scan(){

        }
    }
    class Program
    {
        public void CreateImage(CircularBuffer<Scan> MyCircularBuffer, string Filename){
            var img = new Bitmap(MyCircularBuffer.MyLeanth, 1000, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var color = new Color();
            var Scans = MyCircularBuffer.ReadPosition();
            var cnt = 0;
            var n = 0;
            while(MyCircularBuffer.IsEmpty == false){
                Scans = MyCircularBuffer.Dequeve1();
                foreach(PointXY i in Scans.pointsArray){
                    n = PointsHigth(i.Y);
                    color = System.Drawing.Color.FromArgb(n,n,127);
                    if(Abs(i.X)<10){
                        var MyPoint = (((i.X/10.0)*500.0)+500)%1000;
                        img.SetPixel(MyCircularBuffer.MyLeanth,(int)MyPoint,color);
                        //img.SetPixel(0,0,color);
                    }
                    cnt++;
                }
                img.Save(Filename, System.Drawing.Imaging.ImageFormat.Png); 
                img.Dispose();
                Console.WriteLine(PointsHigth(7.7));
            }
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
        static double[] ConnectionResultDistanceData(LMDScandataResult res){
            double[] result;
            if (res.DistancesData != null){
                result = res.DistancesData.ToArray();
            }else{
                result = new double[Step];}
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

        static void Main(string[] args)
        {
            ConcurrentQueue<Scan> MyConcurrentQueue = new ConcurrentQueue<Scan>();
            CircularBuffer<PointXY[]> MyGround = new CircularBuffer<PointXY[]>(1);
            var InputEvent = new ManualResetEvent(false);
            var ExitEvent = new ManualResetEvent(false);
            //var dump = @"asciidump/scan_[--ffff-192.168.5.241]-2111_637563296658652353.bin";
            //var s = new FileStream(dump, FileMode.Open, FileAccess.Read);
            //var r = LMDScandataResult.Parse(s);
            var InputT = Task.Run(() => InputTask("192.168.5.241", MyConcurrentQueue, InputEvent, ExitEvent, MyGround));
            var MainT = Task.Run(() => MainTask(MyConcurrentQueue, InputEvent, ExitEvent, MyGround));
            Console.ReadLine();
            ExitEvent.Set();
            Task.WaitAll(InputT, MainT);
            Console.WriteLine("Завершено");
            return;
            /*for (int i = 0; i < pos.Count(); i++)
            {
                Console.WriteLine($"{addr},  {i + 1}, {qwer[i].X},  {qwer[i].Y} ");
            }*/

            Console.WriteLine();
        }

        private static void MainTask(ConcurrentQueue<Scan> MyConcurrentQueue, ManualResetEvent InputEvent, ManualResetEvent ExitEvent, CircularBuffer<PointXY[]> MyGround)
        {   
            //string writepath = @"/home/dan/Рабочий стол/12345/Sick-test/test.txt";
            //StreamWriter Myfyle = new StreamWriter(writepath, true);
            var GroundScan = new Scan();
            Scan qwer;
            CircularBuffer<Scan> MyCircularBuffer = new CircularBuffer<Scan>(10000);
            //var trigger = true;
            while (true)
            {   
                var Number = WaitHandle.WaitAny(new[] {InputEvent, ExitEvent});
                if(Number == 1) break;
                InputEvent.Reset();
                while(MyConcurrentQueue.TryDequeue(out qwer)){
                    //Console.WriteLine(qwer.time);
                    //Console.WriteLine(MyCircularBuffer.IsEmpty);
                    MyCircularBuffer.Enqueue1(qwer);
                }
                if(MyGround.IsFull){
                    GroundScan.pointsArray = MyGround.ReadPosition();
                }
                //if((MyCircularBuffer.MyLeanth >= 5000)&(MyCircularBuffer.MyLeanth <= 5050)) Console.WriteLine("Ура, пять тысясяч пришло!))");
                /*while(MyCircularBuffer.IsEmpty == false){
                    Myfyle.WriteLine($"{MyCircularBuffer.ReadPosition().time}   {PointsToString(MyCircularBuffer.Dequeve1().pointsArray)}");
                }*/
                //Console.WriteLine("Файл записан");
                if(MyCircularBuffer.MyLeanth%1000 == 0){
                    Console.WriteLine(MyCircularBuffer.MyLeanth);
                }
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


            }
        }
        private static void InputTask(string addr, ConcurrentQueue<Scan> MyConcurrentQueue, ManualResetEvent InputEvent, ManualResetEvent ExitEvent, CircularBuffer<PointXY[]> MyGround)
        {
            var lms = new LMS1XX(addr, 2111, 5000, 5000);
            var Conv = new SpetialConvertor(-5, 185, Step);
            lms.Connect();
            var accessResult = lms.SetAccessMode();
            var sss = lms.Stop();
            var startResult = lms.Start();
            var runResult = lms.Run();
            var contResult = lms.StartContinuous();
            var res = lms.ScanContinious();
            var NewGround = new Ground((int)res.EncoderPosition, (int)res.StartAngle, (int)res.SizeOfSingleAngularStep);
            NewGround.InitGround(ConnectionResultDistanceData(res));
            var Scan = new Scan{
                pointsArray = Conv.MakePoint(ConnectionResultDistanceData(res)),
                time = DateTime.Now
            };
            var oldscan=0;
            while(true){
                if (ExitEvent.WaitOne(0)) {
                    lms.Disconnect();
                    return;
                }
                res = lms.ScanContinious();
                if (oldscan!=res.ScanCounter){ 
                    Console.WriteLine($"{oldscan} {res.ScanCounter} {res.ScanFrequency}");
                }
                if (res.ScanCounter == null){
                    oldscan++;
                }else{
                    oldscan = (int)res.ScanCounter+1;
                }
                /*if(oldscan%1000 == 0){
                    Console.WriteLine("Тысячный скан");
                }*/
                Scan.time = DateTime.Now;
                Scan.pointsArray = Conv.MakePoint(ConnectionResultDistanceData(res));
                MyGround.Enqueue1(Conv.MakePoint(NewGround.MyGround(ConnectionResultDistanceData(res))));
                MyConcurrentQueue.Enqueue(Scan);
                InputEvent.Set();
            }
        }
    }
}