using System;

namespace Sick_test
{
    ///<summary>Круговой буффер для хранения машинок</summary>  
    public class CarCircularBuffer : CircularBuffer<CarArraySize>{
        CircularBuffer<CarArraySize> retCars;
        config config;
        public CarCircularBuffer(int buffersize,  CircularBuffer<CarArraySize> _retCars, config _config):base(buffersize){
            config = _config;
            retCars = _retCars;
        }
        private int NextPosition(int position){
            position+=1;
            if(position >= _buffersize){
                position = 0;
            }
            return position;
            //return (position + 1) % _buffersize;
        }

        ///<summary>Добавляет полученную машинку в буффер</summary>  
        public void AddCar(CarArraySize input){
            var _input = new CarArraySize(){
                leftborder = new int(),
                rightborder  = new int(),
                starttime = new DateTime(),
                endtime = new DateTime(),
                leftindexborders = new int[input.leftindexborders.Length],
                rightindexborders = new int[input.rightindexborders.Length],

                Width = new int(),
                Height = new int()
            };
            _input.leftborder = input.leftborder;
            _input.rightborder = input.rightborder;
            _input.starttime = input.starttime;
            _input.endtime = input.endtime;
            Array.Copy(input.leftindexborders, _input.leftindexborders, _input.leftindexborders.Length);
            Array.Copy(input.leftindexborders, _input.leftindexborders, _input.leftindexborders.Length);
            _input.Width = input.Width;
            _input.Height = input.Height;
            retCars.Enqueue(Dequeue1());
            Enqueue(_input);
        }
        public CarArraySize GiveCar(){
            return retCars.Dequeue();
        }
        ///<summary>Возвращает все имеющиеся машинки</summary>  
        public CarArraySize[] ReadAllBuffer(){
            if(_length == 0){
                return new CarArraySize[0];
            }
            var ret = new CarArraySize[_length];
            int j = 0;
            if(IsFull){
                j = 1;
                ret[0] = _buffer[_tail];
                for(int i = NextPosition(_tail); i != NextPosition(_head); i = NextPosition(i)){
                    ret[j] = _buffer[i];
                    j++;
                }

                return ret;
            }
            for(int i = 0+_tail; i != NextPosition(_head); i = NextPosition(i)){
                ret[j] = _buffer[i];
                j++;
            }
            return ret;
        }
        private bool iscarinthisLane(CarArraySize car, int roadline){
            var laneleftborder = config.RoadSettings.Lanes[roadline].Offset;
            var lanerightborder = config.RoadSettings.Lanes[roadline].Offset+config.RoadSettings.Lanes[roadline].Width;
            return (((laneleftborder<=car.leftborder&&lanerightborder>car.rightborder))||(lanerightborder>=car.rightborder&&laneleftborder<car.rightborder));
        }
        private bool iscarinthisTime(CarArraySize car, DateTime time){
            return ((car.starttime<time&&car.endtime>time));
        }
        ///<summary>Возвращает машинку, при запросе по времени и номеру полосы</summary>  
        public CarArraySize GiveMyCar(DateTime time, int roadline){
            lock(_lock){
                var ret = new CarArraySize();
                foreach(CarArraySize i in _buffer){
                    if(iscarinthisLane(i, roadline)&&iscarinthisTime(i, time)){
                        ret = i.Copy();
                    }
                }

                return ret;
            }
        }

        ///<summary>Возвращает машинку, при запросе по номеру полосы</summary>  
        public CarArraySize GiveMyCar(DateTime time){
            lock(_lock){
                var ret = new CarArraySize();
                foreach(CarArraySize i in _buffer){
                    if(iscarinthisTime(i, time)){
                        ret = i.Copy();
                    }
                }

                return ret;
            }
        }
    }
}