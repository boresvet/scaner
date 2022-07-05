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
    class Program
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

        static void Main(){
            //ConcurrentQueue<Scan> MyConcurrentQueue = new ConcurrentQueue<Scan>();
            //CircularBuffer<PointXY[]> MyGround = new CircularBuffer<PointXY[]>(1);
            //var InputEvent = new ManualResetEvent(false);
            //var ExitEvent = new ManualResetEvent(false);
            //var dump = @"asciidump/scan_[--ffff-192.168.5.241]-2111_637563296658652353.bin";
            //var s = new FileStream(dump, FileMode.Open, FileAccess.Read);
            //var r = LMDScandataResult.Parse(s);
            //var InputT = Task.Run(() => InputTask("192.168.43.241", MyConcurrentQueue, InputEvent, ExitEvent));
            //var MainT = Task.Run(() => MainTask(MyConcurrentQueue, InputEvent, ExitEvent));
            //Console.ReadLine();
            //ExitEvent.Set();
            //Task.WaitAll(InputT, MainT);
            //Console.WriteLine("Завершено");
            //return;
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
            //var lms12334 = new ScanForPython("192.168.0.250", 2111, 5000, 5000, -5, 185);
            var GAGAGAGA = new testScanArrayForPython(10);
            GAGAGAGA.CarGenerate();
            var Scan = new PointXY[GAGAGAGA.arrayLeanght];
            //Scan = GAGAGAGA.CarScan();
            Console.WriteLine("АААА");
        }
    }
}