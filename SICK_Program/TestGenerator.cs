using System;
using System.Text;
using static System.Math;
using System.Threading;
using System.Threading.Tasks;
namespace Sick_test
{
    public class TestGenerator{
        config Config;
        int scannumber;
        public TestGenerator(config config, int _scannumber){
            Config = config;
            scannumber = _scannumber;
        }
    }
}
