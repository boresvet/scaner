using Sick_test;


namespace SickScanner
{

    class Program
    {
        static void Main(){
            var returns = new Sick_test.returns();
            Sick_test.SickScanners.RunScanners(returns);
        }
    }
}