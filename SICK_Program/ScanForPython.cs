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
    public class ScanForPython
    {
        public int PicselFilter = 5;
        public double[][] Filter;
        int trigger = 0;
        int qwerty = 0;
        //public float[][] RavFilter;
        public SpetialConvertor Conv;
        
        public int Step = 286;
        public int BebinGrade, EndGrade;
        public LMS1XX lms;
        private bool Trig = false;
        private double mypixelfilter(double[] input){
            var inputArray = new double[input.Length];
            inputArray = input;
            System.Array.Sort(inputArray);
            return inputArray[input.Length/2];
        }

		public ScanForPython(string addr, int port, int qwe, int rty, int begingrade, int endgrade){
            Conv = new SpetialConvertor(begingrade, endgrade, Step);
            BebinGrade = begingrade;
            EndGrade = endgrade;
            lms = new LMS1XX(addr, 2111, 5000, 5000);
		}

        public bool TryConnectScaner(){
            lms.Connect();
            var accessResult = lms.SetAccessMode();
            var sss = lms.Stop();
            var startResult = lms.Start();
            var runResult = lms.Run();
            var contResult = lms.StartContinuous();
            var res = lms.ScanContinious();
            var Scan1 = new Scan{
                pointsArray = Conv.MakePoint(ConnectionResultDistanceData(res)),
                time = DateTime.Now
            };
            var Filter = new double[5][];
            return true;
        }
        public double[] MedianFilter(double[] arraypoints){
            var newMedianfilter = new double[arraypoints.Length];
            if(trigger<4){
                trigger++;
                for(int i = 0; i < arraypoints.Length; i++){
                    Filter[trigger][i] = new double();
                    Filter[trigger][i] = arraypoints[i];
                }
                qwerty = trigger;
            }else{
                for(int i = 0; i < arraypoints.Length; i++){
                    Filter[qwerty][i] = arraypoints[i];
                }
                qwerty++;
                if(qwerty==5){
                    qwerty=0;
                }
            }
            var constArray = new double[trigger];
            for(int i = 0; i < arraypoints.Length; i++){
                for(int j = 0; i < trigger; i++){
                    constArray[j] = Filter[j][i];
                }
                newMedianfilter[i] = mypixelfilter(constArray);
            }
            return newMedianfilter;
        }
        public double[] ConnectionResultDistanceData(LMDScandataResult res){
            double[] result;
            if (res.DistancesData != null){
                result = res.DistancesData.ToArray();
            }else{
                result = new double[Step];}
            return result;
        }

        /*public bool IsScannerConnect(){
            return IsSocketConnected(lms);
        }*/

        public Scan GiveScan(bool trigger1){
            var Scan1 = new Scan{
                pointsArray = Conv.MakePoint(ConnectionResultDistanceData(lms.ScanContinious())),
                time = DateTime.Now
            };
            if(trigger1){
                Scan1.pointsArray = Conv.MakePoint(MedianFilter(ConnectionResultDistanceData(lms.ScanContinious())));
            }
            return Scan1;
        }

    }
}