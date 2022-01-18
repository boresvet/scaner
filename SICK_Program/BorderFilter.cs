namespace Sick_test
{
    public class BorderFilter{
        public double Height = 10;
        public double Leftborder = -5;
        public double Rigthborder = 5;
        public int BeginGrade;
        public int EndGrade;
        public int Step;
        /*public BorderFilter(){
        }*/
        /*public BorderInit(double leftborder, double lowerbourder, double rigthborder){
            Height = lowerbourder;
            Leftborder = leftborder;
            Rigthborder = rigthborder;
        }*/
        public BorderFilter(double leftborder, double lowerbourder, double rigthborder, int begingrade, int endgrade, int step){
            Height = lowerbourder;
            Leftborder = leftborder;
            Rigthborder = rigthborder;
            BeginGrade = begingrade;
            EndGrade = endgrade;
            Step = step;
        }
        public PointXY[] ScanBorder(PointXY[] Array){
            var oldArray = new PointXY[Array.Length];
            for(int i = 0; i<oldArray.Length; i++){
                if((Array[i].X > Rigthborder)
                ||(Array[i].X<Leftborder)
                ||(Array[i].Y)>Height){
                    oldArray[i].X = 0.0;
                    oldArray[i].Y = 0.0;
                }else{
                    oldArray[i] = Array[i];
                }
            }
            return oldArray;
        }
        public double[] RawBorder(double[] oldArray){
            var Conv = new SpetialConvertor(BeginGrade, EndGrade, Step);
            for(int i = 0; i<Step; i++){
                if((Conv.MakeXOne(oldArray[i], i)<Leftborder)
                ||(Conv.MakeXOne(oldArray[i], i)>Rigthborder)
                ||(Conv.MakeYOne(oldArray[i], i)>Height)){
                    oldArray[i] = 0.0;
                }
            }
            return oldArray;
        }
    }
}
