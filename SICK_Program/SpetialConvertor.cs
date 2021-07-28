using System;

namespace Sick_test
{
    class SpetialConvertor {
        public int BeginGrade;//    Углы относительно оси У
        public int EndGrade;
        public int Step; // Кол-во шагов. 

        public double[] RatioSin;

        double[] RatioCos;

        public SpetialConvertor(int begingrade, int endgrade, int step){
            BeginGrade = begingrade;
            EndGrade = endgrade;
            Step = step;
            RatioSin = RatoiGenSin(BeginGrade, EndGrade, Step);
            RatioCos = RatoiGenCos(BeginGrade, EndGrade, Step);//Массивы с коэффециентами для приведения к нормальному виду координат
        }
        
        static double[] RatoiGenSin(int BeginGrade, int EndGrade, int Step){   //принимает начальное и конечное значение в градусах(с учётом знака), и кол-во шагов
            double[] result;
            result = new double[Step];
            for (int i=0; i<Step; i++){
                result[i] = Math.Sin((i * (( double )(EndGrade - BeginGrade))/(( double )(Step)) + BeginGrade)*Math.PI/180);
            }
            return result;
        }
        static double[] RatoiGenCos(int BeginGrade, int EndGrade, int Step){   //принимает начальное и конечное значение в градусах(с учётом знака), и кол-во шагов
            double[] result;
            result = new double[Step];
            for (int i=0; i<Step; i++){
                result[i] = Math.Cos((i * (( double )(EndGrade - BeginGrade))/(( double )(Step)) + BeginGrade)*Math.PI/180);
            }
            return result;
        }
        public double[] MakeX(double[] rpos){
            double[] result;
            result = new double[rpos.Length];
            for (int i=0; i<rpos.Length; i++){
                result[i] = MakeXOne(rpos[i], i);
            }
        return result;
        }
        public double[] MakeY(double[] rpos){
            double[] result;
            result = new double[rpos.Length];
            for (int i=0; i<rpos.Length; i++){
                result[i] = MakeYOne(rpos[i], i);
            }
        return result;
        }
        public double MakeXOne(double rpos, int step){
            var result = rpos*RatioCos[step];
        return result;}
        public double MakeYOne(double rpos, int step){
            var result = rpos*RatioSin[step];
        return result;
        }
        public PointXY MakePointOne(double rpos, int step){
            var result = new PointXY();
            result.X = MakeXOne(rpos, step);
            result.Y = MakeYOne(rpos, step);
        return result;
        }
        public PointXY[] MakePoint(double[] rpos){
            var result = new PointXY[Step];
            for (var i= 0; i<Step; i++){
                result[i] = MakePointOne(rpos[i], i);
            }
        return result;
        }
    }
}
