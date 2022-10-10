using System;

namespace Sick_test
{
    public struct line
    {
        public double A,B;
        public PointXY StartPoint;
        public PointXY EndPoint;
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
            ret.X = ((secondline.B-firstline.B)/(firstline.A-secondline.A));
            ret.Y = (ret.X*firstline.A+firstline.B);
            return ret;
        }
        public PointXY firstPointIntersectionSegment(PointXY startpoint, line Laser, line[] AllTexturesLines){
            var ret = new PointXY();
            int i = 0;
            while((DistancetopointSegment(startpoint, Laser, AllTexturesLines[i])==null)&(i<AllTexturesLines.Length)){
                i++;
            }
            if(DistancetopointSegment(startpoint, Laser, AllTexturesLines[i])==null){
                return new PointXY(){X=0.0, Y=0.0};
            }
            double distance = (double)DistancetopointSegment(startpoint, Laser, AllTexturesLines[i]);
            ret = pointIntersection(Laser, AllTexturesLines[0]);
            foreach(line j in AllTexturesLines){
                if(distance>Distancetopoint(startpoint, Laser, j)){
                    ret = pointIntersection(Laser, j);
                }
            }
            return ret;
        }
        public PointXY firstPointIntersection(PointXY startpoint, line Laser, line[] AllTexturesLines){
            var ret = new PointXY();
            int i = 0;
            while((Distancetopoint(startpoint, Laser, AllTexturesLines[i])==null)&(i<AllTexturesLines.Length)){
                i++;
            }
            if(Distancetopoint(startpoint, Laser, AllTexturesLines[i])==null){
                return new PointXY(){X=0.0, Y=0.0};
            }
            double distance = (double)Distancetopoint(startpoint, Laser, AllTexturesLines[i]);
            ret = pointIntersection(Laser, AllTexturesLines[0]);
            foreach(line j in AllTexturesLines){
                if(distance>Distancetopoint(startpoint, Laser, j)){
                    ret = pointIntersection(Laser, j);
                }
            }
            return ret;
        }
        public double? DistancetopointSegment(PointXY startpoint, line Laser, line TexturesLines){
            var ret = new double();
            ret = 0.0;
            if((Laser.A==TexturesLines.A)&(Laser.B==TexturesLines.B)){
                return null;
            }else{
                var endpoint = pointIntersection(Laser, TexturesLines);
                ret = Math.Sqrt(((startpoint.X-endpoint.X)*(startpoint.X-endpoint.X))+((startpoint.Y-endpoint.Y)*(startpoint.Y-endpoint.Y)));
                if((endpoint.X<=TexturesLines.EndPoint.X)&(endpoint.X>=TexturesLines.StartPoint.X)){
                    return ret;
                }
                
                return null;
            }
        }
        public double? Distancetopoint(PointXY startpoint, line Laser, line TexturesLines){
            var ret = new double();
            ret = 0.0;
            if((Laser.A==TexturesLines.A)&(Laser.B==TexturesLines.B)){
                return null;
            }else{
                var endpoint = pointIntersection(Laser, TexturesLines);
                ret = Math.Sqrt(((startpoint.X-endpoint.X)*(startpoint.X-endpoint.X))+((startpoint.Y-endpoint.Y)*(startpoint.Y-endpoint.Y)));
                return ret;
            }
        }
    }
}