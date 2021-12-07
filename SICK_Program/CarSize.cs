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

        public Scan[] CreateCarPoints(Scan[] carData){
            var retScanArray = new Scan[carData.Length];
            var trig = true;
            while(trig){
                trig = false;
                for(int k = 1; k<groundArray.Length; k++){
                    var cnv = new PointXY();
                    if(groundArray[k-1].X<groundArray[k].X){
                        trig = true;
                        cnv = groundArray[k-1];
                        groundArray[k-1] = groundArray[k];
                        groundArray[k] = groundArray[k-1];
                    }
                }
            }
            return retScanArray;
        }
        public int RetCarSize(CircularBuffer<Scan> carData, Scan ground){
            var size = 0;
            var convertstruct = new Scan[carData._buffer.Length];
            var scan = new Scan(carData._buffer[0].pointsArray.Length);
            var leftPointsArray = new PointXY[carData._buffer.Length];
            var rightPointsArray = new PointXY[carData._buffer.Length];
            carData._buffer.CopyTo(convertstruct,0);

            
            for(int i = 0; i<convertstruct.Length; i++){
                scan = convertstruct[i].copyScan();
                var leftPoint = new PointXY();
                var rightPoint = new PointXY();
                var leftPointNumber = 0;
                var rightPointNumber = 0;
                for(int j = 0; j< convertstruct[i].pointsArray.Length;j++){
                    if((convertstruct[i].pointsArray[j].X*convertstruct[i].pointsArray[j].X + convertstruct[i].pointsArray[j].Y*convertstruct[i].pointsArray[j].Y)>0.0144){ //проверка, что точка не является нулём
                        if(((convertstruct[i].pointsArray[j].X)<leftPoint.X)|(j == 0)){//Нахождение граничной точки
                            leftPointNumber = j;
                            leftPoint = convertstruct[i].pointsArray[j];
                        }
                        if(((convertstruct[i].pointsArray[j].X)>rightPoint.X)|(j == 0)){//Нахождение граничной точки
                            rightPointNumber = j;
                            rightPoint = convertstruct[i].pointsArray[j];
                        }
                    //Пока дальше не придумал
                    }
                }




            }
            return size;
        }
    }
}