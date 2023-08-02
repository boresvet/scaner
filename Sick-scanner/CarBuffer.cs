using System.Collections.Generic;
using System;


namespace Sick_test
{
    ///<summary>Буффер для хранения машинок</summary>  
    public class CarBuffer{

        CarCircularBuffer _buffer;
        ///<summary>Размер и количество очередей берётся из конфига</summary>  
        public CarBuffer(config config)
        {
            _buffer = new CarCircularBuffer(config.SortSettings.Buffers, config);
        }
        ///<summary>Добавляет полученные машинки к уже имеющимся в буффере</summary>  
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
        ///<summary>Добавляет полученную машинки в буффер</summary>  
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
        ///<summary>Проверяет, не является ли пара машинок кусками одной и той же</summary>  
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
        ///<summary>Возвращает машинку по заданному времении номеру полосы</summary>  

            public CarArraySize GiveMyCar(DateTime time, int roadline){
                return _buffer.GiveMyCar(time, roadline);
            }
        ///<summary>Возвращает машинку по заданному времени</summary>  

            public CarArraySize GiveMyCar(DateTime time){
                return _buffer.GiveMyCar(time);
            }
    }
}