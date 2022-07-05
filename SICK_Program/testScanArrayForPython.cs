namespace Sick_test
{
    public class testScanArrayForPython{
        private int[] carlines;
        private int carHigh = 142, carWigth = 182, groundWight = 100; // Задаётся ширина, высота, и просвет между машинами(полосами)
        private float scannerHight = 500;
        private int carLength, LineSize;
        private PointXY[][] carArrays, nocarArrays, groundArrays;
        public int arrayLeanght;
        private PointXY[] rerArray;
        public testScanArrayForPython(int lineSize = 1){
            carlines = new int[lineSize];
            carArrays = new PointXY[lineSize][];
            nocarArrays = new PointXY[lineSize][];
            groundArrays = new PointXY[lineSize+1][];
            rerArray = new PointXY[carWigth*lineSize + groundWight*(lineSize+1)];
            LineSize = lineSize;
		}
        public void CarGenerate(){
            var x = 0; 
            groundArrays[0] = new PointXY[groundWight];
            for(int i = 0; i < groundWight; i++){
                groundArrays[0][i].X = x;
                groundArrays[0][i].Y = scannerHight;
                x++;
            }
            for(int j = 0; j < LineSize; j++){
                nocarArrays[j] = new PointXY[carWigth];
                carArrays[j] = new PointXY[carWigth];
                groundArrays[j+1] = new PointXY[groundWight];
                for(int i = 0; i < groundWight; i++){
                    nocarArrays[j][i].X = x;
                    nocarArrays[j][i].Y = scannerHight;
                    carArrays[j][i].X = x;
                    carArrays[j][i].Y = scannerHight-carHigh;
                    x++;
                }
                for(int i = 0; i < groundWight; i++){
                    groundArrays[j+1][i].X = x;
                    groundArrays[j+1][i].Y = scannerHight;
                    x++;
                }
            }
            arrayLeanght = (carWigth*LineSize + groundWight*(LineSize+1));
        }
        /*public PointXY[] CarScan(){
            var rnd = new Random();
            for(int j = 0; j < LineSize; j++){
                if(carlines[j] >= 0){
                    c = c.Concat(carArrays[j]).ToArray();
                    if(carlines[j] >= 150){
                        carlines[j] = -15;
                    }else{
                        carlines[j]++;
                    }
                }else{
                    var c = c.Concat(nocarArrays[j]).ToArray();
                }
                c = c.Concat(groundArrays[j+1]).ToArray();

                if((carlines[j] >= -5)&&(carlines[j] >= -4)){
                    if(rnd.Next(0, 1000) == 0){
                        carlines[j] = 0;
                    }
                }
            }
            return c;
        }*/
    }
}