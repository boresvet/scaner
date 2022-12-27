using static System.Math;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Sick_test
{
    public class CarBuffer{

        CarCircularBuffer _buffer;
        ///<summary>Объявление точки, в которую переносится система координат
        ///<param name = "zeropoint">Точка, в которую переносится начало координат</param>
        ///</summary>   
        public CarBuffer(int size)
        {
            _buffer = new CarCircularBuffer(size);
        }

        public void UpdateCars(List<CarArraySize> input){
            var oldcar = _buffer;
            if(input.Count == 0){
                return;
            }
            var carbuffer = _buffer.ReadAllBuffer();
            for(int i = 0; i < input.Count; i++)
            {
                if (_buffer.IsEmpty)
                {
                    _buffer.AddCar(input[i]);
                    continue;
                }
                WriteCarToBuffer(input, i, carbuffer);
            }
        }

        private void WriteCarToBuffer(List<CarArraySize> input, int i, CarArraySize[] carbuffer)
        {
            foreach (CarArraySize carfrombuffer in carbuffer)
            {
                if (IsItThisCar(carfrombuffer, input[i]))
                {
                    return;
                }
            }
            _buffer.AddCar(input[i]);
        }

        public bool IsItThisCar(CarArraySize CarFromBuffer, CarArraySize NewCar){
            if(((CarFromBuffer.leftindexborders[CarFromBuffer.leftindexborders.Length-1]==NewCar.leftindexborders[0])||(CarFromBuffer.rightindexborders[CarFromBuffer.rightindexborders.Length-1]==NewCar.rightindexborders[0]))&&(CarFromBuffer.endtime == NewCar.starttime)){
                CarFromBuffer.endtime = NewCar.endtime;
                if(NewCar.Height > CarFromBuffer.Height){
                    CarFromBuffer.Height = NewCar.Height;
                }
                if(NewCar.Width > CarFromBuffer.Width){
                    CarFromBuffer.Width = NewCar.Width;
                }
                return true;
            }else{
                return false;
            }
            return false;
        }
    }
}