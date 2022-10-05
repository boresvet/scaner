using System;

namespace Sick_test
{
    public struct line
    {
        public double A,B;
        //line: y = A*x + B
        public void createLine(PointXY firstPoint, PointXY secondPoint){
            A = (firstPoint.Y-secondPoint.Y)/(firstPoint.X-secondPoint.Y);
            B = (firstPoint.X*A) - firstPoint.Y;
        }
        public void createLine(PointXY firstPoint, float Angle){
            

            var secondPoint = new PointXY(){X = firstPoint.X + 1000*Math.Cos(Angle*Math.PI/180), Y = firstPoint.X + 1000*Math.Sin(Angle*Math.PI/180)};
            A = (firstPoint.Y-secondPoint.Y)/(firstPoint.X-secondPoint.Y);
            B = (firstPoint.X*A) - firstPoint.Y;
        }
        public void additionLines(line newline, int ratio){ //добавляет к структуре часть линии в размере 1/коэффециент
            A = A + (newline.A/ratio);
            B = B + (newline.B/ratio);
        }
        public PointXY pointIntersection(line firstline, line secondline){
            var ret = new PointXY();




            return ret;
        }
    }
}