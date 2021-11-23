namespace Sick_test
{
    class CarSize
    {
        public Scan carScan;
        public int Step;
        public Scan sampleScan;
        public CarSize(int ScanSize, int CarSize)
        {
            carScan = new Scan(ScanSize);
            Step = ScanSize;
            sampleScan = new Scan(ScanSize);
        }

        public int RetCarSize(CircularBuffer<Scan> carData){
            var size = 0;
            var convertstruct = carData._buffer;
            var scan = new Scan(convertstruct[0].pointsArray.Length);
            carData._buffer.CopyTo(convertstruct,0);
            for(int i = 0; i<convertstruct.Length; i++){
                scan = convertstruct[i].copyScan();
                foreach(PointXY j in convertstruct[i].pointsArray){
                    //Пока дальше не придумал

                }




            }
            return size;
        }
    }
}