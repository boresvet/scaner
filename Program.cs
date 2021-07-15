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

namespace Sick_test
{
    public class PointXY{
        public double X,Y;
    }
    public class Scan{
        public PointXY[] pointsArray; 
        public DateTime time;
    }
    class Program
    {
        const int Step = 286; //Количество шагов
        static double[] ConnectionResultDistanceData(LMDScandataResult res){
            double[] result;
            result = new double[Step];
            if (res.DistancesData != null){
                result = res.DistancesData.ToArray();
            }
            return result;
        }

        static void Main(string[] args)
        {
            ConcurrentQueue<Scan> MyConcurrentQueue = new ConcurrentQueue<Scan>();
            var InputEvent = new ManualResetEvent(false);
            var ExitEvent = new ManualResetEvent(false);
            //var dump = @"asciidump/scan_[--ffff-192.168.5.241]-2111_637563296658652353.bin";
            //var s = new FileStream(dump, FileMode.Open, FileAccess.Read);
            //var r = LMDScandataResult.Parse(s);
            var InputT = Task.Run(() => InputTask("192.168.5.242", MyConcurrentQueue, InputEvent, ExitEvent));
            var MainT = Task.Run(() => MainTask(MyConcurrentQueue, InputEvent, ExitEvent));
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

        private static void MainTask(ConcurrentQueue<Scan> MyConcurrentQueue, ManualResetEvent InputEvent, ManualResetEvent ExitEvent)
        {
            Scan qwer;
            while (true)
            {
                var Number = WaitHandle.WaitAny(new[] {InputEvent, ExitEvent});
                if(Number == 1) break;
                InputEvent.Reset();
                while(MyConcurrentQueue.TryDequeue(out qwer)){
                Console.WriteLine(qwer.time);
                }
            }
        }

        public static void InputTask(string addr, ConcurrentQueue<Scan> MyConcurrentQueue, ManualResetEvent InputEvent, ManualResetEvent ExitEvent)
        {
            var lms = new LMS1XX(addr, 2111, 5000, 5000);
            var Conv = new SpetialConvertor(40, -40, Step);
            lms.Connect();
            var ContResult = lms.StartContinuous();            //var res = lms.LMDScandata2();
            //Thread.Sleep(100);
            var res = lms.ScanContinious();
            while(true){
                //Thread.Sleep(10000);
                if (ExitEvent.WaitOne(0)) return;
                res = lms.ScanContinious();
                /*CircularBuffer<PointXY[]> MyCircularBuffer = new CircularBuffer<PointXY[]>(10000);
                Console.WriteLine(MyCircularBuffer.IsEmpty);
                MyCircularBuffer.Enqueue1(pos);*/
                var Scan = new Scan{
                    pointsArray = Conv.MakePoint(ConnectionResultDistanceData(res)), time = DateTime.Now
                };
                MyConcurrentQueue.Enqueue(Scan);
                //Console.WriteLine("Скан записан");
                InputEvent.Set();
                /*for (int i = 0; i < pos.Count(); i++)
                {   

                    Console.WriteLine($"{addr},  {i + 1}, {MyCircularBuffer.ReadPosition()[i].X},  {MyCircularBuffer.ReadPosition()[i].Y} ");
                }*/
            }
            lms.Disconnect();
        }
    }
}