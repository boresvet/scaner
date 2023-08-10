using System;

namespace Sick_test
{
    ///<summary>Конвертор из радиальной системы координат в ХУ
    ///</summary>     
    public class SpetialConvertorint{
        public int BeginGrade;//    Углы относительно оси У
        public int EndGrade;
        public int Step; // Кол-во шагов. 

        public double[] RatioSin;

        double[] RatioCos;
        ///<summary>Объявление класса
        ///<param name = "begingrade">начальный угол в градусах</param>
        ///<param name = "endgrade">Конечный угол в градусах</param>
        ///<param name = "step">Количество точек</param>
        ///</summary>  
        public SpetialConvertorint(int begingrade, int endgrade, int step){
            BeginGrade = begingrade;
            EndGrade = endgrade;
            Step = step;
            RatioSin = RatoiGenSin(BeginGrade, EndGrade, Step);
            RatioCos = RatoiGenCos(BeginGrade, EndGrade, Step);//Массивы с коэффециентами для приведения к нормальному виду координат
        }
        
        static public double[] RatoiGenSin(int BeginGrade, int EndGrade, int Step){   //принимает начальное и конечное значение в градусах(с учётом знака), и кол-во шагов
            double[] result;
            result = new double[Step];
            for (int i=0; i<Step; i++){
                result[i] = Math.Sin(((double)i * (( double )(EndGrade - BeginGrade))/(( double )(Step)) + (double)BeginGrade)*Math.PI/180.0);
            }
            return result;
        }
        static public double[] RatoiGenCos(int BeginGrade, int EndGrade, int Step){   //принимает начальное и конечное значение в градусах(с учётом знака), и кол-во шагов
            double[] result;
            result = new double[Step];
            for (int i=0; i<Step; i++){
                result[i] = Math.Cos(((double)i * (( double )(EndGrade - BeginGrade))/(( double )(Step)) + (double)BeginGrade)*Math.PI/180.0);
            }
            return result;
        }
        private int[] MakeX(int[] rpos){
            int[] result;
            result = new int[rpos.Length];
            for (int i=0; i<rpos.Length; i++){
                result[i] = MakeXOne(rpos[i], i);
            }
        return result;
        }
        private int[] MakeY(int[] rpos){
            int[] result;
            result = new int[rpos.Length];
            for (int i=0; i<rpos.Length; i++){
                result[i] = MakeYOne(rpos[i], i);
            }
        return result;
        }
        private int MakeXOne(int rpos, int step){
            var result = (int)(((double)(rpos))*RatioCos[step]);
            return result;}
        private int MakeYOne(int rpos, int step){
            var result = (int)(((double)(rpos))*RatioSin[step]);
            return result;
        }
        private PointXYint MakePointOne(int rpos, int step){
            var result = new PointXYint();
            result.X = MakeXOne(rpos, step);
            result.Y = MakeYOne(rpos, step);
        return result;
        }
        ///<summary>Перевод из радиальной в ХУ координаты
        ///<param name = "rpos"></param>
        ///</summary> 
        public PointXYint[] MakePoint(int[] rpos){
            var result = new PointXYint[Step];
            for (var i= 0; i<Step; i++){
                result[i] = MakePointOne(rpos[i], i);
            }
        return result;
        }
    }
    ///<summary>Конвертор из радиальной системы координат в ХУ
    ///</summary>     
    public class SpetialConvertor{
        public int BeginGrade;//    Углы относительно оси У
        public int EndGrade;
        public int Step; // Кол-во шагов. 

        public double[] RatioSin;

        double[] RatioCos;
        ///<summary>Объявление класса
        ///<param name = "begingrade">начальный угол в градусах</param>
        ///<param name = "endgrade">Конечный угол в градусах</param>
        ///<param name = "step">Количество точек</param>
        ///</summary>  
        public SpetialConvertor(int begingrade, int endgrade, int step){
            BeginGrade = begingrade;
            EndGrade = endgrade;
            Step = step;
            RatioSin = RatoiGenSin(BeginGrade, EndGrade, Step);
            RatioCos = RatoiGenCos(BeginGrade, EndGrade, Step);//Массивы с коэффециентами для приведения к нормальному виду координат
        }
        
        static private double[] RatoiGenSin(int BeginGrade, int EndGrade, int Step){   //принимает начальное и конечное значение в градусах(с учётом знака), и кол-во шагов
            double[] result;
            result = new double[Step];
            for (int i=0; i<Step; i++){
                result[i] = Math.Sin((i * (( double )(EndGrade - BeginGrade))/(( double )(Step)) + BeginGrade)*Math.PI/180);
            }
            return result;
        }
        static private double[] RatoiGenCos(int BeginGrade, int EndGrade, int Step){   //принимает начальное и конечное значение в градусах(с учётом знака), и кол-во шагов
            double[] result;
            result = new double[Step];
            for (int i=0; i<Step; i++){
                result[i] = Math.Cos((i * (( double )(EndGrade - BeginGrade))/(( double )(Step)) + BeginGrade)*Math.PI/180);
            }
            return result;
        }
        private double[] MakeX(double[] rpos){
            double[] result;
            result = new double[rpos.Length];
            for (int i=0; i<rpos.Length; i++){
                result[i] = MakeXOne(rpos[i], i);
            }
        return result;
        }
        private double[] MakeY(double[] rpos){
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
        private PointXY MakePointOne(double rpos, int step){
            var result = new PointXY();
            result.X = MakeXOne(rpos, step);
            result.Y = MakeYOne(rpos, step);
        return result;
        }
        ///<summary>Перевод из радиальной в ХУ координаты
        ///<param name = "rpos"></param>
        ///</summary> 
        public PointXY[] MakePoint(double[] rpos){
            var result = new PointXY[Step];
            for (var i= 0; i<Step; i++){
                result[i] = MakePointOne(rpos[i], i);
            }
        return result;
        }
    }
}
