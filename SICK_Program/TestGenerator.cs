using System;
namespace Sick_test
{
    public class TestGenerator{
        public int groundsize;
        public int Step;
        private int scannumber = 1;
        private int carid = 0;
        private int carhight = 3;
        private int carleanth = 2;
        private PointXY[] EmptyScan;
        private PointXY[] FullScan;
        private double[] RawEmptyScan;
        private double[] RawFullScan;
        public TestGenerator(int size, int leftgrade, int rightgrade, int groundhight, int startgrade, int endgrade){
            Step = size;
            groundsize = groundhight;
            RawEmptyScan = new double[size];
            RawFullScan = new double[size];
            for(int i = 0; i<Step; i++){
                var grade = (double)startgrade+((((double)(endgrade-startgrade)/Step)*(i+1)*Math.PI/180));
                var firstlen = leftgrade/Math.Sin(grade);
                var secondlen = -groundsize/(Math.Cos(grade));
                var thirdlen = rightgrade/(Math.Sin(grade));
                if((firstlen>=0.1)){
                    RawEmptyScan[i] = firstlen;
                }
                if(((secondlen>=0.1)&(secondlen<RawEmptyScan[i]))|((secondlen>=0.1)&(RawEmptyScan[i]<=0.1))){
                    RawEmptyScan[i] = secondlen;
                }
                if(((thirdlen>=0.1)&(thirdlen<RawEmptyScan[i]))|((thirdlen>-0)&(RawEmptyScan[i]<=0.1))){
                    RawEmptyScan[i] = thirdlen;
                }
                firstlen = 0;
                secondlen = 0;
                thirdlen = 0;
            }
            for(int i = 0; i<Step; i++){
                var grade = (double)startgrade+((((double)(endgrade-startgrade)/Step)*(i+1)*Math.PI/180));
                var firstlen = leftgrade/Math.Sin(grade);
                var secondlen = -groundsize/(Math.Cos(grade));
                var thirdlen = rightgrade/(Math.Sin(grade));
                var forthlen = -(groundsize-carhight)/(Math.Cos(grade));
                if((firstlen>=0.1)){
                    RawFullScan[i] = firstlen;
                }
                if(((secondlen>=0.1)&(secondlen<RawFullScan[i]))|((secondlen>=0.1)&(RawFullScan[i]<=0.1))){
                    RawFullScan[i] = secondlen;
                }
                if(((thirdlen>=0.1)&(thirdlen<RawFullScan[i]))|((thirdlen>-0)&(RawFullScan[i]<=0.1))){
                    RawFullScan[i] = thirdlen;
                }
                if((((forthlen>=0.1)&(forthlen<RawFullScan[i]))|((forthlen>-0)&(RawFullScan[i]<=0.1)))&((carleanth/2)>=(forthlen*forthlen-(groundhight-carhight)*(groundhight-carhight)))){
                    RawFullScan[i] = forthlen;
                }
                firstlen = 0;
                secondlen = 0;
                thirdlen = 0;
                forthlen = 0;
            }
            EmptyScan = new PointXY[size];
            FullScan = new PointXY[size];
            Step = size;
        }
        private void numberupdate(){
            scannumber = (scannumber+1)%1000;
        } 
        public double[] RawScanGen(){
            if(scannumber%500 == 0){
                carid = 1;
            }
            if(carid>0){

                carid = (carid+1)%100;
                numberupdate();
                return RawFullScan;
            }else{
                numberupdate();
                return RawEmptyScan;
            }
        }
        public PointXY[] ScanGen(){
            if(scannumber%500 == 0){
                carid = 1;
            }
            if(carid!=0){

                carid = (carid+1)%100;
                numberupdate();
                return FullScan;
            }else{
                numberupdate();
                return EmptyScan;
            }
        }
    }
}
