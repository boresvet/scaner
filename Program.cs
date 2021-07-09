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

            var lms = new LMS1XX("192.168.5.242", 2111, 5000, 5000);

            var Conv = new SpetialConvertor(40, -40, Step);
            lms.Connect();

            Thread.Sleep(100);


            var res = lms.LMDScandata2();

            double[] PolarScanResult = ConnectionResultDistanceData(res);
            double[] Xpos = Conv.MakeX(PolarScanResult);
            double[] Ypos = Conv.MakeY(PolarScanResult);

            foreach(var d in Xpos) {
                Console.Write($"{d}  ");
            }
            Console.WriteLine();
            Console.WriteLine();
            foreach(var d in Ypos) {
                Console.Write($"{d}  ");
            }
            Console.WriteLine();

            //Console.WriteLine(lms.LMDScandata2());

            lms.Disconnect();
        }
    }
}
