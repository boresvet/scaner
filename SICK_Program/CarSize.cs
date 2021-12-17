using static System.Math;

namespace Sick_test
{
    class CarSize
    {
        public float groundA;
        public float groundB;
        public Scan carScan;
        public int Step;
        public Scan sampleScan;
        private PointXY[] groundArray;
        public CarSize(int ScanSize, int CarSize)
        {
            carScan = new Scan(ScanSize);
            Step = ScanSize;
            sampleScan = new Scan(ScanSize);
        }

        public PointXY[] MinAndMaxPoint(Scan retScan){
            var retPoints = new PointXY[2];
            retPoints[0] = retScan.pointsArray[0];
            retPoints[1] = retScan.pointsArray[1];
            foreach(PointXY j in retScan.pointsArray){
                if(j.X<retPoints[0].X){
                    retPoints[0] = j;
                }
                if(j.X>retPoints[1].X){
                    retPoints[1] = j;
                }
            }
            return retPoints;
        }
        private bool contrastPoints(PointXY firstPoint, PointXY secondPoint){
            return (firstPoint.X == secondPoint.X)&(firstPoint.Y == secondPoint.Y);
        }
        public Scan CreateCarBorderPoint(Scan carData){
            var retScan = new Scan();
            retScan = carData.copyScan();
            var pointMinMaxXY = MinAndMaxPoint(carData.copyScan());
            var trigMin = -1;
            var trigMax = -1;
            for (int i = 0; i< retScan.pointsArray.Length; i++){
                if(contrastPoints(pointMinMaxXY[0], retScan.pointsArray[i])&(trigMin == -1)){
                    trigMin = i;
                }
                if(contrastPoints(pointMinMaxXY[1], retScan.pointsArray[i])){
                    trigMax = i;
                }
            }
            for (int i = 0; i< retScan.pointsArray.Length; i++){
                if((trigMax!=i)&(trigMin!=i)){
                    retScan.pointsArray[i].X = 0.0;
                    retScan.pointsArray[i].Y = 0.0;
                }
            }
            return retScan;
        }

        public Scan[] CreateCarBorderPoints(Scan[] scanArray){
            var retScanArray = new Scan[scanArray.Length];
            for(int i = 0; i<scanArray.Length; i++){
                retScanArray[i] = CreateCarBorderPoint(scanArray[i]);
            }
            return retScanArray;
        }
        public PointXY[] CreateLeftGroundBorderPoints(Scan[] scanArray, Scan groundscan){
            var point = new PointXY[scanArray.Length];
            for(int J = 0; J<scanArray.Length; J++){
                foreach(PointXY K in scanArray[J].copyScan().pointsArray){
                    if((K.X*K.X>= 0.25 )|(K.Y*K.Y>= 0.25)){
                        point[J] = K;
                        break;
                    }
                }
            }
            return point;
        }

        public PointXY[] CreateRightGroundBorderPoints(Scan[] scanArray, Scan groundscan){
            var point = new PointXY[scanArray.Length];
            var onepoint = new PointXY();
            for(int J = 0; J<scanArray.Length; J++){
                foreach(PointXY K in scanArray[J].copyScan().pointsArray){
                    if((K.X*K.X>= 0.25 )|(K.Y*K.Y>= 0.25)){
                        onepoint = K;
                    }
                }
                point[J] = onepoint;
            }
            return point;
        }
        public line CreateGroundLine(Scan[] scanArray, Scan ground){ //На вход массив сканов ТОЛЬКО с граничныи точками
            var leftBorderPoints = CreateLeftGroundBorderPoints(scanArray, ground.copyScan());
            var rightBorderPoints = CreateRightGroundBorderPoints(scanArray, ground.copyScan());
            var oneline = new line();
            var averrageLine = new line();
            int size = scanArray.Length;
            for(int i = 0; i< size; i++){
                oneline.createLine(leftBorderPoints[i], rightBorderPoints[i]);
                averrageLine.additionLines(oneline, size);
            }
            return averrageLine;
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

        public PointXY CarScanSize(Scan arrayPoints, line groudLine, Scan ground){
            var retPoint = new PointXY();// X = ширина машины в скане, Y = высота максимальная
            var trig = true;

            var xMin = 0.0;
            var xMax = 0.0;
            var yMax = 0.0;
            for(int i = 0; i<arrayPoints.pointsArray.Length; i++){
                if((arrayPoints.pointsArray[i].Y>=0)&trig){
                    var nPoint = new PointXY{X = (ground.pointsArray[i].X) - arrayPoints.pointsArray[i].X,  Y = (ground.pointsArray[i].Y - arrayPoints.pointsArray[i].Y)};
                    xMin = ProectionPointOnLine(nPoint,groudLine).X;
                    xMax = ProectionPointOnLine(nPoint,groudLine).X;
                    yMax = ProectionPointOnLine(nPoint,groudLine).Y;
                    trig = false;
                }
                if((arrayPoints.pointsArray[i].Y>=0)&(!trig)){
                    var nPoint = new PointXY{X = (ground.pointsArray[i].X) - arrayPoints.pointsArray[i].X,  Y = (ground.pointsArray[i].Y - arrayPoints.pointsArray[i].Y)};
                    if(ProectionPointOnLine(nPoint,groudLine).X<xMin){
                        xMin = ProectionPointOnLine(nPoint,groudLine).X;
                    }

                    if(ProectionPointOnLine(nPoint,groudLine).X>xMax){
                        xMax = ProectionPointOnLine(nPoint,groudLine).X;
                    }
                    if(ProectionPointOnLine(nPoint,groudLine).Y>yMax){
                        yMax = ProectionPointOnLine(nPoint,groudLine).Y;
                    }
                }
            }
            retPoint.X = (xMax-xMin);
            retPoint.Y = yMax;
            return retPoint;
        }

        public PointXY RetCarSize(CircularBuffer<Scan> retScan, Scan ground){
            var convertstruct = new Scan[retScan._buffer.Length];
            var scan = new Scan(retScan._buffer[0].pointsArray.Length);
            retScan._buffer.CopyTo(convertstruct,0);
            var groundLine = CreateGroundLine(convertstruct,ground.copyScan());
            var sizeArray = new PointXY[convertstruct.Length];
            var firstSize = sizeArray[0] = CarScanSize(convertstruct[0],groundLine,ground.copyScan());
            var secondSize = new PointXY();
            secondSize = firstSize;
            var threeSize = new PointXY();
            threeSize = firstSize;
            var fourSize = new PointXY();
            fourSize = firstSize;
            var fiveSize = new PointXY();
            fiveSize = firstSize;

            var maxHight = 0.0;
            for(int i = 1; i<convertstruct.Length; i++){
                sizeArray[i] = CarScanSize(convertstruct[i],groundLine,ground.copyScan());
                if(firstSize.X<sizeArray[i].X){
                    fiveSize = fourSize;
                    fourSize = threeSize;
                    threeSize = secondSize;
                    secondSize = firstSize;
                    firstSize = sizeArray[i];
                }
                if(sizeArray[i].Y>maxHight){
                    maxHight = sizeArray[i].Y;
                }
            }
            var size = new PointXY(){X = fiveSize.X, Y = maxHight};
            return size;
        }
    }
}