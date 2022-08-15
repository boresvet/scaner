using PointCloudTools;
using System;
using System.Numerics;
using System.Globalization;

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

        public void PointSaveToFile(CircularBuffer<Scan> myCircularBuffer, int savelength){
            var convertstruct = new Scan[myCircularBuffer.MyLeanth];
            var i = myCircularBuffer.MyLeanth;
            convertstruct = myCircularBuffer._buffer;
            var Scans = new Scan(convertstruct[0].pointsArray.Length);
            width = Scans.pointsArray.Length;
            int n = savelength;
            var AllData = new Vector3[i*Scans.pointsArray.Length];
            while(i > n){
                /*while(i>(n-savelength)){
                    myCircularBuffer.NextPosition();
                    n++;
                }*/
                Scans = convertstruct[n].copyScan();
                //myCircularBuffer.NextPosition();
                for (var w = 0; w < width; w++)
                {
                    var p = new Vector3
                    {
                        X = (float)Scans.pointsArray[w].X,
                        Y = (float)Scans.pointsArray[w].Y,
                        Z = (float)(n*0.01)
                    };
                    AllData[w + width * n] = p;
                    //Console.WriteLine(p);
                }
                n++;
            }
            //Console.WriteLine(i);
            var cloud = new PointCloud(AllData);
            var retname = (DateTime.Now.ToString("G", DateTimeFormatInfo.InvariantInfo)) + "test" + n.ToString();
            cloud.SaveToPly(retname.Replace('/',':')+".ply", true);
        }
    }
}