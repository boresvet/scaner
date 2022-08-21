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
    class translator
    {
        PointXY ZeroPoint;
        PointXYint ZeroPointint;

        public translator(PointXY zeropoint)
        {
            ZeroPoint = zeropoint;
        }
        public translator(PointXYint zeropoint)
        {
            ZeroPointint = zeropoint;
        }
        public PointXY translate(PointXY input){
            input.X = input.X + ZeroPoint.X;
            input.Y = input.Y + ZeroPoint.Y;
            return input;
        }
        public PointXYint translate(PointXYint input){
            input.X = input.X + ZeroPointint.X;
            input.Y = input.Y + ZeroPointint.Y;
            return input;
        }
        public PointXY[] Translate(PointXY[] oldArray){
            return oldArray.Select(n => translate(n)).ToArray();
        }
        public PointXYint[] Translate(PointXYint[] oldArray){
            return oldArray.Select(n => translate(n)).ToArray();
        }
    }
}