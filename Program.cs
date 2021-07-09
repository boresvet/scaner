using BSICK.Sensors.LMS1xx;
using System;
using static System.Math;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sick_test
{
    class PointXY{
        public double X,Y;
    }
    class Scan{
        public PointXY[] pointsArray; 
        public DateTime time;
    }
    class Program
    {
        const int Step = 286; //Количество шагов
        static double[] ConnectionResultDistanceData(LMDScandataResult res){
            double[] result;
            result = new double[Step];
            //var i = 0;
            if (res.DistancesData != null){
                /*foreach(var d in res.DistancesData) {
                    result[i] = d;
                    i++;
                }*/
                //Array.Copy(res.DistancesData, result, res.DistancesData.Count);
                result = res.DistancesData.ToArray();
            }
            Console.WriteLine("Сканирование завершено");
            return result;
        }

        static void Main(string[] args)
        {
            //var dump = @"asciidump/scan_[--ffff-192.168.5.241]-2111_637563296658652353.bin";
            //var s = new FileStream(dump, FileMode.Open, FileAccess.Read);
            //var r = LMDScandataResult.Parse(s);
            var t=Task.Run(()=>NewMethod("192.168.5.242"));
            var y=Task.Run(()=>NewMethod("192.168.5.241"));
            Task.WaitAll(t, y);
        }

        public static void NewMethod(string addr)
        {
            var lms = new LMS1XX(addr, 2111, 5000, 5000);

            var Conv = new SpetialConvertor(40, -40, Step);
            lms.Connect();

            Thread.Sleep(100);

            var res = lms.LMDScandata2();

            double[] PolarScanResult = ConnectionResultDistanceData(res);
            PointXY[] pos = Conv.MakePoint(PolarScanResult);

            for (int i = 0; i < pos.Count(); i++)
            {
                Console.WriteLine($"{addr},  {i + 1}, {pos[i].X},  {pos[i].Y} ");
            }
            Console.WriteLine();

            //Console.WriteLine(lms.LMDScandata2());

            lms.Disconnect();
        }
    }
}
