using System;

namespace Sick_test
{
    public struct line
    {
        public double A,B;
        public PointXY StartPoint;
        public PointXYint StartPointint;
        bool X,Y;
        public PointXY EndPoint;
        public PointXYint EndPointint;

        public static line CreateLine(PointXY startpoint, double Angle){
            var ret = new line();
            ret.StartPoint = new PointXY();
            ret.StartPoint = startpoint;
            var endpoint = new PointXY();
            endpoint.X = startpoint.X + Math.Cos((Angle)*Math.PI/180);
            endpoint.Y = startpoint.Y + Math.Sin((Angle)*Math.PI/180);
            ret.createLine(startpoint, endpoint);
            return ret;
        }
        //line: y = A*x + B
        public void createLine(PointXY firstPoint, PointXY secondPoint){
            StartPoint = firstPoint;
            EndPoint = secondPoint;
            A = (firstPoint.Y-secondPoint.Y)/(firstPoint.X-secondPoint.Y);
            B = (firstPoint.X*A) - firstPoint.Y;
            X = ((secondPoint.X-firstPoint.X)>=0);
            Y = ((secondPoint.Y-firstPoint.Y)>=0);
        }
        public void createLine(PointXY firstPoint, float Angle){
            StartPoint = new PointXY();
            StartPoint = firstPoint;
            var endpoint = new PointXY();
            endpoint.X = firstPoint.X + Math.Cos((Angle)*Math.PI/180);
            endpoint.Y = firstPoint.Y + Math.Sin((Angle)*Math.PI/180);
            createLine(firstPoint, endpoint);
            /*var secondPoint = new PointXY(){X = firstPoint.X + 1000*Math.Cos(Angle*Math.PI/180), Y = firstPoint.X + 1000*Math.Sin(Angle*Math.PI/180)};
            A = (firstPoint.Y-secondPoint.Y)/(firstPoint.X-secondPoint.Y);
            B = (firstPoint.X*A) - firstPoint.Y;*/
        }

        public static line CreateLine(PointXYint startpoint, double Angle){
            var ret = new line();
            ret.StartPointint = new PointXYint();
            ret.StartPointint = startpoint;
            var endpoint = new PointXYint();
            endpoint.X = (int)(startpoint.X + 1000*Math.Cos((Angle)*Math.PI/180));
            endpoint.Y = (int)(startpoint.Y + 1000*Math.Sin((Angle)*Math.PI/180));
            ret.createLine(startpoint, endpoint);
            return ret;
        }
        //line: y = A*x + B
        public void createLine(PointXYint firstPoint, PointXYint secondPoint){
            StartPointint = firstPoint;
            EndPointint = secondPoint;
            A = ((double)(firstPoint.Y-secondPoint.Y))/((double)(firstPoint.X-secondPoint.Y));
            B = (firstPoint.X*A) - firstPoint.Y;
            X = ((secondPoint.X-firstPoint.X)>=0);
            Y = ((secondPoint.Y-firstPoint.Y)>=0);
        }
        public void createLine(PointXYint firstPoint, float Angle){
            StartPointint = new PointXYint();
            StartPointint = firstPoint;
            var endpoint = new PointXYint();
            endpoint.X = (int)(firstPoint.X + 1000*Math.Cos((Angle)*Math.PI/180));
            endpoint.Y = (int)(firstPoint.Y + 1000*Math.Sin((Angle)*Math.PI/180));
            createLine(firstPoint, endpoint);
            /*var secondPoint = new PointXY(){X = firstPoint.X + 1000*Math.Cos(Angle*Math.PI/180), Y = firstPoint.X + 1000*Math.Sin(Angle*Math.PI/180)};
            A = (firstPoint.Y-secondPoint.Y)/(firstPoint.X-secondPoint.Y);
            B = (firstPoint.X*A) - firstPoint.Y;*/
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
                if(((endpoint.X-startpoint.X)>=0 == Laser.X)&((endpoint.Y-startpoint.Y)>=0 == Laser.Y)){

                }
                ret = Math.Sqrt(((startpoint.X-endpoint.X)*(startpoint.X-endpoint.X))+((startpoint.Y-endpoint.Y)*(startpoint.Y-endpoint.Y)));
                return ret;
            }
        }
    }
}