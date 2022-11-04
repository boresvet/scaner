using System;

namespace Sick_test
{
    public struct lineXY
    {
        public double A,B;
        public PointXY StartPoint;
        public PointXYint StartPointint;
        bool X,Y;
        public PointXY EndPoint;
        public PointXYint EndPointint;

        public static lineXY CreatelineXY(PointXY startpoint, double Angle){
            var ret = new lineXY();
            ret.StartPoint = new PointXY();
            ret.StartPoint = startpoint;
            var endpoint = new PointXY();
            endpoint.X = startpoint.X + Math.Cos((Angle)*Math.PI/180);
            endpoint.Y = startpoint.Y + Math.Sin((Angle)*Math.PI/180);
            ret.createlineXY(startpoint, endpoint);
            return ret;
        }
        //lineXY: y = A*x + B
        public void createlineXY(PointXY firstPoint, PointXY secondPoint){
            StartPoint = firstPoint;
            EndPoint = secondPoint;
            A = (firstPoint.Y-secondPoint.Y)/(firstPoint.X-secondPoint.Y);
            B = (firstPoint.X*A) - firstPoint.Y;
            X = ((secondPoint.X-firstPoint.X)>=0);
            Y = ((secondPoint.Y-firstPoint.Y)>=0);
        }
        public void createlineXY(PointXY firstPoint, float Angle){
            StartPoint = new PointXY();
            StartPoint = firstPoint;
            var endpoint = new PointXY();
            endpoint.X = firstPoint.X + Math.Cos((Angle)*Math.PI/180);
            endpoint.Y = firstPoint.Y + Math.Sin((Angle)*Math.PI/180);
            createlineXY(firstPoint, endpoint);
            /*var secondPoint = new PointXY(){X = firstPoint.X + 1000*Math.Cos(Angle*Math.PI/180), Y = firstPoint.X + 1000*Math.Sin(Angle*Math.PI/180)};
            A = (firstPoint.Y-secondPoint.Y)/(firstPoint.X-secondPoint.Y);
            B = (firstPoint.X*A) - firstPoint.Y;*/
        }

        public static lineXY CreatelineXY(PointXYint startpoint, double Angle){
            var ret = new lineXY();
            ret.StartPointint = new PointXYint();
            ret.StartPointint = startpoint;
            var endpoint = new PointXYint();
            endpoint.X = (int)(startpoint.X + 1000*Math.Cos((Angle)*Math.PI/180));
            endpoint.Y = (int)(startpoint.Y + 1000*Math.Sin((Angle)*Math.PI/180));
            ret.createlineXY(startpoint, endpoint);
            return ret;
        }
        //lineXY: y = A*x + B
        public void createlineXY(PointXYint firstPoint, PointXYint secondPoint){
            StartPointint = firstPoint;
            EndPointint = secondPoint;
            A = (((double)firstPoint.Y*1000-(double)secondPoint.Y*1000))/((double)((double)firstPoint.X*1000-(double)secondPoint.Y*1000));
            B = firstPoint.Y - (firstPoint.X*A);
            X = ((secondPoint.X-firstPoint.X)>=0);
            Y = ((secondPoint.Y-firstPoint.Y)>=0);
        }
        public void createlineXY(PointXYint firstPoint, float Angle){
            StartPointint = new PointXYint();
            StartPointint = firstPoint;
            var endpoint = new PointXYint();
            endpoint.X = (int)(firstPoint.X + 1000*Math.Cos((Angle)*Math.PI/180));
            endpoint.Y = (int)(firstPoint.Y + 1000*Math.Sin((Angle)*Math.PI/180));
            createlineXY(firstPoint, endpoint);
            /*var secondPoint = new PointXY(){X = firstPoint.X + 1000*Math.Cos(Angle*Math.PI/180), Y = firstPoint.X + 1000*Math.Sin(Angle*Math.PI/180)};
            A = (firstPoint.Y-secondPoint.Y)/(firstPoint.X-secondPoint.Y);
            B = (firstPoint.X*A) - firstPoint.Y;*/
        }

        public void additionlineXYs(lineXY newlineXY, int ratio){ //добавляет к структуре часть линии в размере 1/коэффециент
            A = A + (newlineXY.A/ratio);
            B = B + (newlineXY.B/ratio);
        }
        public PointXY pointIntersection(lineXY firstlineXY, lineXY secondlineXY){
            var ret = new PointXY();
            ret.X = ((secondlineXY.B-firstlineXY.B)/(firstlineXY.A-secondlineXY.A));
            ret.Y = (ret.X*firstlineXY.A+firstlineXY.B);
            return ret;
        }
        public PointXYint pointIntersectionint(lineXY firstlineXY, lineXY secondlineXY){
            var ret = new PointXYint();
            ret.X = (int)((secondlineXY.B*1000-firstlineXY.B*1000)/(firstlineXY.A*1000-secondlineXY.A*1000)*1000);
            ret.Y = (int)((ret.X*firstlineXY.A*1000+firstlineXY.B*1000));
            if((ret.Y-((ret.X*secondlineXY.A*1000+secondlineXY.B*1000)))>=1000){
                Console.WriteLine("Косяк");
            }
            return ret;
        }
        public PointXY firstPointIntersectionSegment(PointXY startpoint, lineXY Laser, lineXY[] AllTextureslineXYs){
            var ret = new PointXY();
            int i = 0;
            while((DistancetopointSegment(startpoint, Laser, AllTextureslineXYs[i])==null)){
                i++;
                if((i==AllTextureslineXYs.Length)){
                    return startpoint;
                }
            }
            double distance = (double)DistancetopointSegment(startpoint, Laser, AllTextureslineXYs[i]);
            ret = pointIntersection(Laser, AllTextureslineXYs[0]);
            foreach(lineXY j in AllTextureslineXYs){
                if(distance>Distancetopoint(startpoint, Laser, j)){
                    ret = pointIntersection(Laser, j);
                }
            }
            return ret;
        }
        public PointXYint firstPointIntersectionSegment(PointXYint startpoint, lineXY Laser, lineXY[] AllTextureslineXYs){
            var ret = new PointXYint();
            int i = 0;
            while((DistancetopointSegment(startpoint, Laser, AllTextureslineXYs[i])==null)){
                i++;
                if((i==AllTextureslineXYs.Length)){
                    return startpoint;
                }
            }
            double distance = (double)DistancetopointSegment(startpoint, Laser, AllTextureslineXYs[i]);
            ret = pointIntersectionint(Laser, AllTextureslineXYs[i]);
            foreach(lineXY j in AllTextureslineXYs){
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
        public PointXY firstPointIntersection(PointXY startpoint, lineXY Laser, lineXY[] AllTextureslineXYs){
            var ret = new PointXY();
            int i = 0;
            while((Distancetopoint(startpoint, Laser, AllTextureslineXYs[i])==null)&(i<AllTextureslineXYs.Length)){
                i++;
            }
            if(Distancetopoint(startpoint, Laser, AllTextureslineXYs[i])==null){
                return new PointXY(){X=0.0, Y=0.0};
            }
            double distance = (double)Distancetopoint(startpoint, Laser, AllTextureslineXYs[i]);
            ret = pointIntersection(Laser, AllTextureslineXYs[0]);
            foreach(lineXY j in AllTextureslineXYs){
                if(distance>Distancetopoint(startpoint, Laser, j)){
                    ret = pointIntersection(Laser, j);
                }
            }
            return ret;
        }
        public double? DistancetopointSegment(PointXY startpoint, lineXY Laser, lineXY TextureslineXYs){
            var ret = new double();
            ret = 0.0;
            if((Laser.A==TextureslineXYs.A)&(Laser.B==TextureslineXYs.B)){
                return null;
            }else{
                var endpoint = pointIntersection(Laser, TextureslineXYs);
                ret = Math.Sqrt(((startpoint.X-endpoint.X)*(startpoint.X-endpoint.X))+((startpoint.Y-endpoint.Y)*(startpoint.Y-endpoint.Y)));
                if((endpoint.X<=TextureslineXYs.EndPoint.X)&(endpoint.X>=TextureslineXYs.StartPoint.X)){
                    return ret;
                }
                
                return null;
            }
        }
        public int? DistancetopointSegment(PointXYint startpoint, lineXY Laser, lineXY TextureslineXYs){
            var ret = new int();
            ret = 0;
            if((Laser.A==TextureslineXYs.A)){
                return null;
            }else{
                PointXYint endpoint = pointIntersectionint(Laser, TextureslineXYs);
                ret = (int)Math.Sqrt(((startpoint.X-endpoint.X)*(startpoint.X-endpoint.X))+((startpoint.Y-endpoint.Y)*(startpoint.Y-endpoint.Y)));
                if((endpoint.X<=TextureslineXYs.EndPoint.X)&(endpoint.X>=TextureslineXYs.StartPoint.X)){
                    return ret;
                }
                
                return null;
            }
        }
        public double? Distancetopoint(PointXY startpoint, lineXY Laser, lineXY TextureslineXYs){
            var ret = new double();
            ret = 0.0;
            if((Laser.A==TextureslineXYs.A)&(Laser.B==TextureslineXYs.B)){
                return null;
            }else{
                var endpoint = pointIntersection(Laser, TextureslineXYs);
                if(((endpoint.X-startpoint.X)>=0 == Laser.X)&((endpoint.Y-startpoint.Y)>=0 == Laser.Y)){
                    ret = Math.Sqrt(((startpoint.X-endpoint.X)*(startpoint.X-endpoint.X))+((startpoint.Y-endpoint.Y)*(startpoint.Y-endpoint.Y)));
                    return ret;
                }else{
                    return null;
                }

            }
        }

        public int? Distancetopoint(PointXYint startpoint, lineXY Laser, lineXY TextureslineXYs){
            var ret = new int();
            ret = 0;
            if((Laser.A-TextureslineXYs.A)<=0.1){
                return null;
            }else{
                PointXYint endpoint = pointIntersectionint(Laser, TextureslineXYs);
                if(((endpoint.X-startpoint.X)>=0 == Laser.X)&((endpoint.Y-startpoint.Y)>=0 == Laser.Y)){
                    ret = (int)Math.Sqrt(((startpoint.X-endpoint.X)*(startpoint.X-endpoint.X))+((startpoint.Y-endpoint.Y)*(startpoint.Y-endpoint.Y)));
                    return ret;
                }else{
                    return null;
                }

            }
        }
    }
}