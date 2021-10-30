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
            const int width = 100;
            const int height = 100;
            var AllData = new Vector3[width*height];
            ReturnFileName = fileName;
        }

        public void PointSaveToFile(CircularBuffer<Scan> myCircularBuffer){


            var Scans = myCircularBuffer.ReadPosition();
            int n = 0;
            while(myCircularBuffer.MyLeanth > n){
                Scans = myCircularBuffer.ReadPosition();
                width = Scans.pointsArray.Length;
                for (var w = 0; w < width; w++)
                {
                    var p = new Vector3
                    {
                        X = (float)Scans.pointsArray[w].X,
                        Y = (float)Scans.pointsArray[w].Y,
                        Z = (float)(n*0.01)
                    };
                    AllData[w + width * + n] = p;
                }
            }


            var cloud = new PointCloud(AllData);
            cloud.SaveToPly(ReturnFileName + "test.ply");
        }
    }
}