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
        int lanes;
        int[][] laneswithcarArray;
        int [][] laneswithoutcarArray;
        public TestGenerator(config config, int _scannumber){
            Config = config;
            scannumber = _scannumber;


            laneswithcarArray = new int[Config.RoadSettings.Lanes.Length][];
            laneswithoutcarArray = new int[Config.RoadSettings.Lanes.Length][];


            for(int i = 0; i<Config.RoadSettings.Lanes.Length; i++){
                
            }
        }
    }
}
