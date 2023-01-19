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
using System;


namespace Sick_test
{
    public class CarBuffer{

        CarCircularBuffer _buffer;
        ///<summary>Объявление точки, в которую переносится система координат
        ///<param name = "zeropoint">Точка, в которую переносится начало координат</param>
        ///</summary>   
        public CarBuffer(config config)
        {
            _buffer = new CarCircularBuffer(config.SortSettings.Buffers, config);
        }

        public void UpdateCars(List<CarArraySize> input){
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
                WriteCarToBuffer(input[i], carbuffer);
            }
        }

        private void WriteCarToBuffer(CarArraySize input, CarArraySize[] carbuffer)
        {
            foreach (CarArraySize carfrombuffer in carbuffer)
            {
                if (IsItThisCar(carfrombuffer, input))
                {
                    return;
                }
            }
            _buffer.AddCar(input);
        }

        public bool IsItThisCar(CarArraySize CarFromBuffer, CarArraySize NewCar){
            //Если машина из буфера совпадает с свеженайденой машиной, то границы машинф в буффере расширяются
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

            public CarArraySize GiveMyCar(DateTime time, int roadline){
                return _buffer.GiveMyCar(time, roadline);
            }
    }
}