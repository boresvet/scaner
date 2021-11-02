using PointCloudTools;
using System;
using System.Numerics;

namespace Sick_test
{
    public class PointSave
    {
        public int width;
        public int height;
        private Vector3[] AllData;
        private string ReturnFileName;
        public PointSave(string fileName){
            int width = 286;
            int height = 10000;
            var AllData = new Vector3[width*height];
            ReturnFileName = fileName;
        }

        public void PointSaveToFile(CircularBuffer<Scan> myCircularBuffer){

            var i = myCircularBuffer.MyLeanth;
            var Scans = myCircularBuffer.ReadPosition();
            int n = 0;
            var AllData = new Vector3[i*Scans.pointsArray.Length];
            while(i > n){
                Scans = myCircularBuffer.ReadPosition();
                myCircularBuffer.NextPosition();
                width = Scans.pointsArray.Length;
                for (var w = 0; w < width; w++)
                {
                    var p = new Vector3
                    {
                        X = (float)Scans.pointsArray[w].X,
                        Y = (float)Scans.pointsArray[w].Y,
                        Z = 10f*(float)(n*0.01)
                    };
                    AllData[w + width * n] = p;
                    Console.WriteLine("");
                }
                n++;
            }


            var cloud = new PointCloud(AllData);
            cloud.SaveToPly(ReturnFileName + "test.ply");
        }
    }
}