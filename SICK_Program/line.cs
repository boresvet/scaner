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
        public void createLine(PointXYint firstPoint, double Angle){
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

        private double ordinate(double input){
            var ret = 0.0;
            ret = (Qy*(input - StartPoint.X))/Qx + StartPoint.Y;
            return ret;
        }
        private int ordinate(int input){
            var ret = 0;
            ret = (Qyint*(input - StartPointint.X))/Qxint + StartPointint.Y;
            return ret;
        }
        public PointXY pointIntersection(line firstline, line secondline){
            var ret = new PointXY();
                if((((firstline.Qx == 0.0)&&(!firstline.integer))|((firstline.Qxint == 0.0)&&firstline.integer))&&(((secondline.Qx == 0.0)&&(!secondline.integer))|((secondline.Qxint == 0.0)&&secondline.integer))){
                    if(firstline.StartPoint.X==secondline.StartPoint.X){
                        //В случае, когда обе линии вертикальны, и совпадают - возвращается начальная точка ВТОРОЙ линии
                        ret = secondline.StartPoint;
                        return ret;
                    }else{
                        //Случаев, когда линии вертикальны и не совпадают - стоит не допускать
                    }
                }


                //Когда только одна из линий - вертикальна
                if(((firstline.Qx == 0.0)&&(!firstline.integer))|((firstline.Qxint == 0.0)&&firstline.integer)){
                    ret.X = firstline.StartPoint.X;
                    ret.Y = (secondline.Qx*(firstline.StartPoint.X-firstline.StartPoint.X))/secondline.Qy + secondline.StartPoint.Y;
                    return ret;
                }
                if(((secondline.Qx == 0.0)&&(!secondline.integer))|((secondline.Qxint == 0.0)&&secondline.integer)){
                    ret.X = secondline.StartPoint.X;
                    ret.Y = (firstline.Qx*(secondline.StartPoint.X-secondline.StartPoint.X))/firstline.Qy + firstline.StartPoint.Y;
                    return ret;
                }



                //Когда ни одна из линий не вертикальна:
                
                var K = ((firstline.Qy*secondline.Qx)-(secondline.Qy*firstline.Qx))/(firstline.Qx*secondline.Qx);
                ret.X = (K*firstline.StartPoint.X + secondline.StartPoint.Y - firstline.StartPoint.Y)/K;
                ret.Y = ordinate(ret.X);
            return ret;
        }
        public PointXYint pointIntersectionint(line firstline, line secondline){
            var ret = new PointXYint();
                if((((firstline.Qxint == 0)&&(!firstline.integer))|((firstline.Qxint == 0)&&firstline.integer))&&(((secondline.Qxint == 0)&&(!secondline.integer))|((secondline.Qxint == 0)&&secondline.integer))){
                    if(firstline.StartPoint.X==secondline.StartPoint.X){
                        //В случае, когда обе линии вертикальны, и совпадают - возвращается начальная точка ВТОРОЙ линии
                        ret = secondline.StartPointint;
                        return ret;
                    }else{
                        //Случаев, когда линии вертикальны и не совпадают - стоит не допускать
                    }
                }


                //Когда одна вертикальна, вторая - горизонтальна
                if(((firstline.Qxint == 0)&&(secondline.Qyint == 0))|((firstline.Qyint == 0)&&(secondline.Qxint == 0))){
                    if((firstline.Qxint == 0)&&(secondline.Qyint == 0)){
                        ret.X = firstline.StartPointint.X;
                        ret.Y = secondline.StartPointint.Y;
                    }else{
                        ret.X = secondline.StartPointint.X;
                        ret.Y = firstline.StartPointint.Y;
                    }
                    return ret;
                }

                //Когда только одна из линий - вертикальна
                if(((firstline.Qxint == 0)&&(!firstline.integer))|((firstline.Qxint == 0)&&firstline.integer)){
                    ret.X = firstline.StartPointint.X;
                    ret.Y = (secondline.Qxint*(firstline.StartPointint.X-firstline.StartPointint.X))/secondline.Qyint + secondline.StartPointint.Y;
                    return ret;
                }
                if(((secondline.Qxint == 0)&&(!secondline.integer))|((secondline.Qxint == 0)&&secondline.integer)){
                    ret.X = secondline.StartPointint.X;
                    ret.Y = (firstline.Qxint*(secondline.StartPointint.X-secondline.StartPointint.X))/firstline.Qyint + firstline.StartPointint.Y;
                    return ret;
                }



                //Когда ни одна из линий не вертикальна:
                double K = (double)((firstline.Qyint*secondline.Qxint)-(secondline.Qyint*firstline.Qxint))/(double)(firstline.Qxint*secondline.Qxint);
                ret.X = (int)((K*(double)firstline.StartPointint.X + (double)secondline.StartPointint.Y - (double)firstline.StartPointint.Y)/K);
                ret.Y = ordinate(ret.X);
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
            var ret = new PointXYint(){X = startpoint.X, Y = startpoint.Y};
            //ret = startpoint;
            var points = new int?[AllTexturesLines.Length];
            for(int i = 0; i<points.Length; i++){
                points[i] = DistancetopointSegment(startpoint, Laser, AllTexturesLines[i]);
            }

            var nonullpoints = Array.FindAll(points, point => (point!=null));
            if(nonullpoints.Length>0){
                var distance = mindistance(nonullpoints);
                for(int i = 0; i<points.Length; i++){
                    if(DistancetopointSegment(startpoint, Laser, AllTexturesLines[i])==distance){
                        ret = pointIntersectionint(Laser, AllTexturesLines[i]);
                    }
                }
                return ret;//Возвращает значение конечной точки, когда таковая существует
            }else{
                return ret; //Возвращение начальной точки, если нет пересечений
            }
        }
        public PointXY firstPointIntersection(PointXY startpoint, line Laser, line[] AllTexturesLines){
            var ret = new PointXY(){X = 0.0, Y = 0.0};
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

        private int mindistance(int?[] distance){
            var ret = (int)distance[0];
            for(int i = 0; i<distance.Length; i++){
                if(distance[i]<ret){
                    ret = (int)distance[i];
                }
            }
            return ret;
        }
        public PointXYint firstPointIntersection(PointXYint startpoint, line Laser, line[] AllTexturesLines){
            var ret = new PointXYint(){X = 0, Y = 0};
            ret = startpoint;
            var points = new int?[AllTexturesLines.Length];
            for(int i = 0; i<points.Length; i++){
                points[i] = Distancetopoint(startpoint, Laser, AllTexturesLines[i]);
            }
            var nonullpoints = Array.FindAll(points, point => (point!=null));
            if(nonullpoints.Length>0){
                var distance = mindistance(nonullpoints);
                for(int i = 0; i<points.Length; i++){
                    if(Distancetopoint(startpoint, Laser, AllTexturesLines[i])==distance){
                        ret = pointIntersectionint(Laser, AllTexturesLines[i]);
                    }
                }
                return ret;//Возвращает значение конечной точки, когда таковая существует
            }else{
                return ret; //Возвращение начальной точки, если нет пересечений
            }
        }
        public double? DistancetopointSegment(PointXY startpoint, line Laser, line TexturesLines){
            var ret = new double();
            ret = 0.0;
            if(paralleles(TexturesLines, Laser)){
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
            if(parallelesint(TexturesLines, Laser)){
                return null;
            }else{
                PointXYint endpoint = pointIntersectionint(Laser, TexturesLines);
                ret = (int)Math.Sqrt(((startpoint.X-endpoint.X)*(startpoint.X-endpoint.X))+((startpoint.Y-endpoint.Y)*(startpoint.Y-endpoint.Y)));
                if(((endpoint.X<TexturesLines.EndPointint.X)&(endpoint.X>TexturesLines.StartPointint.X))){
                    return ret;
                }
                
                return null;
            }
        }
        private bool paralleles(line firstlane, line secondlane){
            //Если прямые совпадают - выдаёт нет
            //Если прямые пересекаются (хотя бы в бесконечности) - выдаёт нет
            //Если прямые параллельны - выдаёт да
            if((firstlane.Qx == secondlane.Qx)&(firstlane.Qy==secondlane.Qy)){
                if((firstlane.ordinate(secondlane.StartPoint.X)==secondlane.StartPoint.Y)&&(secondlane.ordinate(firstlane.StartPoint.X)==firstlane.StartPoint.Y)){
                    return false;
                }else{
                    return true;
                }
            }


            //Проверка, чтобы не было различий в нулях функций
            if(((firstlane.Qx==0)&&(secondlane.Qx!=0))|((firstlane.Qx!=0)&&(secondlane.Qx==0))){
                return false;
            }
            if(((firstlane.Qy==0)&&(secondlane.Qy!=0))|((firstlane.Qy!=0)&&(secondlane.Qy==0))){
                return false;
            }
            //Если обе линии параллельны оси абсцисс/ординат, то они параллельны между собой
            if((firstlane.Qx==0.0)|(firstlane.Qy==0.0)){
                return true;
            }


            //Если у функций аналогичны вектора, то эти функции параллельны
            if(firstlane.Qy/firstlane.Qx == secondlane.Qy/secondlane.Qx){
                return true;
            }

            return false;
        }

        private bool parallelesint(line firstlane, line secondlane){
            //Если прямые совпадают - выдаёт нет
            //Если прямые пересекаются (хотя бы в бесконечности) - выдаёт нет
            //Если прямые параллельны - выдаёт да
            if((firstlane.Qxint == secondlane.Qxint)&(firstlane.Qyint==secondlane.Qyint)){
                if((firstlane.ordinate(secondlane.StartPoint.X)==secondlane.StartPoint.Y)&&(secondlane.ordinate(firstlane.StartPoint.X)==firstlane.StartPoint.Y)){
                    return false;
                }else{
                    return true;
                }
            }


            //Проверка, чтобы не было различий в нулях функций
            if(((firstlane.Qxint==0)&&(secondlane.Qxint!=0))|((firstlane.Qxint!=0)&&(secondlane.Qxint==0))){
                return false;
            }
            if(((firstlane.Qyint==0)&&(secondlane.Qyint!=0))|((firstlane.Qyint!=0)&&(secondlane.Qyint==0))){
                return false;
            }
            //Если обе линии параллельны оси абсцисс/ординат, то они параллельны между собой
            if((firstlane.Qxint==0.0)|(firstlane.Qyint==0.0)){
                return true;
            }


            //Если у функций аналогичны вектора, то эти функции параллельны
            if(firstlane.Qyint/firstlane.Qxint == secondlane.Qyint/secondlane.Qxint){
                return true;
            }

            return false;
        }
        public double? Distancetopoint(PointXY startpoint, line Laser, line TexturesLines){
            var ret = new double();
            ret = 0.0;
            if(paralleles(TexturesLines, Laser)){
                return null;
            }else{
                var endpoint = pointIntersection(Laser, TexturesLines);
                ret = Math.Sqrt(endpoint.X*endpoint.X + endpoint.Y*endpoint.Y);
            }
            return ret;
        }





        public int? Distancetopoint(PointXYint startpoint, line Laser, line TexturesLines){
            var ret = new int();
            ret = 0;
            if(parallelesint(TexturesLines, Laser)){
                return null;
            }else{
                PointXYint endpoint = pointIntersectionint(Laser, TexturesLines);
                ret = (int)Math.Sqrt(endpoint.X*endpoint.X + endpoint.Y*endpoint.Y);
            }
            return ret;
        }



        public PointXY ProectionPointOnLine(PointXY point, line line){
            var retPoint = new PointXY();
            var vert = new line();
            vert.Qx = line.Qy;
            vert.Qy = line.Qx;
            vert.StartPoint = point;
            vert.EndPoint = point;
            vert.EndPoint.X = vert.StartPoint.X+vert.Qx;
            vert.EndPoint.Y = vert.StartPoint.Y+vert.Qy;
            retPoint = pointIntersection(line, vert);
            return retPoint;
        }

        public PointXYint ProectionPointOnLine(PointXYint point, line line){
            var retPoint = new PointXYint();
            var vert = new line();
            vert.Qxint = line.Qyint;
            vert.Qyint = line.Qxint;
            vert.StartPointint = point;
            vert.EndPointint = point;
            vert.EndPointint.X = vert.StartPointint.X+vert.Qxint;
            vert.EndPointint.Y = vert.StartPointint.Y+vert.Qyint;
            retPoint = pointIntersectionint(line, vert);
            return retPoint;
        }


        public double DistansFromPointToLine(PointXY point, line line){
            var retDistans = new double();
            var pointproection = ProectionPointOnLine(point, line);
            retDistans = (Math.Abs(((point.X-pointproection.X)*(point.X-pointproection.X)))+((point.Y-pointproection.Y)*(point.Y-pointproection.Y)));
            return retDistans;
        }

        public int DistansFromPointToLine(PointXYint point, line line){
            var retDistans = new int();
            var pointproection = ProectionPointOnLine(point, line);
            retDistans = (int)Math.Abs((((point.X-pointproection.X)*(point.X-pointproection.X)))+((point.Y-pointproection.Y)*(point.Y-pointproection.Y)));
            return retDistans;
        }
    }
}