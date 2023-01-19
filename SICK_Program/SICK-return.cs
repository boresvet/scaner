using System;
using static System.Math;
namespace Sick_test
{
    public class returns
    {
        public AllInput Inputs;
        public TimeBuffer times;
        public CarBuffer carbuffer;
        public returns()
        {
        }
        public void initreturns(AllInput _Inputs, TimeBuffer _times, CarBuffer _carbuffer){
            Inputs = _Inputs;
            times = _times;
            carbuffer = _carbuffer;
        }
        public Scanint returnScan(int number){
            var ret = new Scanint();
            ret = Inputs.inputClass[number].MyConcurrentQueue.ReadPosition().copyScan();
            return ret;
        }
        public SuperScan returnroad(){
            var ret = new SuperScan();
            ret = times.readLastScan().CopyScan();
            return ret;
        }
    }
}