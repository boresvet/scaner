using System;

namespace Sick_test
{
    //Не Фх+Б, а вектор-направление
    public struct line
    {
        //  x - X1     y - Y1
        // -------- = --------
        //    Qx         Qy

        //Где:
        //Qx, Qy - значения вектора направления
        //X1, Y1 - значения начальной точки
        //x, y - переменные
        public double Qx,Qy;
        public int Qxint,Qyint;
        public PointXY StartPoint;
        public PointXYint StartPointint;
        public PointXY EndPoint;
        public PointXYint EndPointint;
        public bool integer;

        public static line CreateLine(PointXY startpoint, double Angle){
            var ret = new line();
            ret.integer = false;
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
            integer = false;
            StartPoint = firstPoint;
            EndPoint = secondPoint;
            Qx = secondPoint.X-firstPoint.X;
            Qy = secondPoint.Y-firstPoint.Y;
        }
        public void createLine(PointXY firstPoint, float Angle){
            integer = false;
            StartPoint = new PointXY();
            StartPoint = firstPoint;
            var endpoint = new PointXY();
            endpoint.X = firstPoint.X + Math.Cos((Angle)*Math.PI/180);
            endpoint.Y = firstPoint.Y + Math.Sin((Angle)*Math.PI/180);
            createLine(firstPoint, endpoint);
        }

        public static line CreateLine(PointXYint startpoint, double Angle){
            var ret = new line();
            ret.integer = true;
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
            integer = true;
            StartPointint = firstPoint;
            EndPointint = secondPoint;
            Qxint = secondPoint.X-firstPoint.X;
            Qyint = secondPoint.Y-firstPoint.Y;
        }
        public void createLine(PointXYint firstPoint, float Angle){
            integer = true;
            StartPointint = new PointXYint();
            StartPointint = firstPoint;
            var endpoint = new PointXYint();
            endpoint.X = (int)(firstPoint.X + 1000*Math.Cos((Angle)*Math.PI/180));
            endpoint.Y = (int)(firstPoint.Y + 1000*Math.Sin((Angle)*Math.PI/180));
            createLine(firstPoint, endpoint);
        }

        public void additionLines(line newline, int ratio){ //добавляет к структуре часть линии в размере 1/коэффециент
            if(integer){
                Qxint = Qxint + (newline.Qxint/ratio);
                Qyint = Qyint + (newline.Qyint/ratio);
                StartPointint.X = StartPointint.X + (newline.StartPointint.X/ratio);
                StartPointint.Y = StartPointint.Y + (newline.StartPointint.Y/ratio);
                EndPointint.X = EndPointint.X + (newline.EndPointint.X/ratio);
                EndPointint.Y = EndPointint.Y + (newline.EndPointint.Y/ratio);
            }else{
                Qx = Qx + (newline.Qx/ratio);
                Qy = Qy + (newline.Qy/ratio);
                StartPoint.X = StartPoint.X + (newline.StartPoint.X/ratio);
                StartPoint.Y = StartPoint.Y + (newline.StartPoint.Y/ratio);
                EndPoint.X = EndPoint.X + (newline.EndPoint.X/ratio);
                EndPoint.Y = EndPoint.Y + (newline.EndPoint.Y/ratio);
            }
        }
        public PointXY pointIntersection(line firstline, line secondline){
            var ret = new PointXY();
            ret.X = ((secondline.B-firstline.B)/(firstline.A-secondline.A));
            ret.Y = (ret.X*firstline.A+firstline.B);
            return ret;
        }
        public PointXYint pointIntersectionint(line firstline, line secondline){
            var ret = new PointXYint();
            ret.X = (int)((secondline.B*1000-firstline.B*1000)/(firstline.A*1000-secondline.A*1000)*1000);
            ret.Y = (int)((ret.X*firstline.A*1000+firstline.B*1000));
            if((ret.Y-((ret.X*secondline.A*1000+secondline.B*1000)))>=1000){
                Console.WriteLine("Косяк");
            }
            return ret;
        }
        public PointXY firstPointIntersectionSegment(PointXY startpoint, line Laser, line[] AllTexturesLines){
            var ret = new PointXY();
            int i = 0;
            while((DistancetopointSegment(startpoint, Laser, AllTexturesLines[i])==null)){
                i++;
                if((i==AllTexturesLines.Length)){
                    return startpoint;
                }
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
        public PointXYint firstPointIntersectionSegment(PointXYint startpoint, line Laser, line[] AllTexturesLines){
            var ret = new PointXYint();
            int i = 0;
            while((DistancetopointSegment(startpoint, Laser, AllTexturesLines[i])==null)){
                i++;
                if((i==AllTexturesLines.Length)){
                    return startpoint;
                }
            }
            double distance = (double)DistancetopointSegment(startpoint, Laser, AllTexturesLines[i]);
            ret = pointIntersectionint(Laser, AllTexturesLines[i]);
            foreach(line j in AllTexturesLines){
                if((distance == 0)&(DistancetopointSegment(startpoint, Laser, j)>0)){
                    distance = (double)DistancetopointSegment(startpoint, Laser, j);
                }
                if((distance>Distancetopoint(startpoint, Laser, j))&(Distancetopoint(startpoint, Laser, j)>=0)){
                    ret = pointIntersectionint(Laser, j);
                    distance = (double)Distancetopoint(startpoint, Laser, j);
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
        public int? DistancetopointSegment(PointXYint startpoint, line Laser, line TexturesLines){
            var ret = new int();
            ret = 0;
            if((Laser.A==TexturesLines.A)){
                return null;
            }else{
                PointXYint endpoint = pointIntersectionint(Laser, TexturesLines);
                ret = (int)Math.Sqrt(((startpoint.X-endpoint.X)*(startpoint.X-endpoint.X))+((startpoint.Y-endpoint.Y)*(startpoint.Y-endpoint.Y)));
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


            }
        }

        public int? Distancetopoint(PointXYint startpoint, line Laser, line TexturesLines){
            var ret = new int();
            ret = 0;
            if((Laser.A-TexturesLines.A)<=0.1){
                return null;
            }else{
                PointXYint endpoint = pointIntersectionint(Laser, TexturesLines);


            }
        }

        public PointXY ProectionPointOnLine(PointXY point, line line){
            var retPoint = new PointXY();
            retPoint.X = (point.X+(line.A*point.X)-(line.A*line.B))/((line.A*line.A)+1);
            retPoint.Y = line.A*retPoint.X + line.B;
            return retPoint;
        }

        public double DistansFromPointToLine(PointXY point, line line){
            var retDistans = new double();
            retDistans = (Abs(point.X*line.A + ((-1)*point.Y)+line.B))/(Sqrt((line.A*line.A)+(line.B*line.B)));
            return retDistans;
        }
    }
}