using System;


namespace Sick_test
{
    public class CarCircularBuffer{
        public CarArraySize[] _buffer;
        public int _head;
        int _tail;
        int _length;
        int _buffersize;
        Object _lock = new object ();

        public CarCircularBuffer(int buffersize){
            _buffer = new CarArraySize[buffersize];
            _buffersize = buffersize;
            _head = buffersize -1;
        }

        public bool IsEmpty{
            get{return _length == 0;}
        }

        public bool IsFull{
            get { return _length == _buffersize;}
        }
        public int MyLeanth{
            get { return _length;}
        }
        public void Dequeue(){
            lock(_lock){
                if(IsEmpty)throw new InvalidOperationException("Queue exhaused");
                _tail = NextPosition(_tail);
                _length --;
            }
        }
        public CarArraySize Dequeue1(){
            lock(_lock){
                if(IsEmpty)throw new InvalidOperationException("Queue exhaused");
                CarArraySize dequeved = _buffer[_tail];
                _tail = NextPosition(_tail);
                _length --;
                return dequeved;
            }
        }

        private int NextPosition(int position){
            return (position + 1) % _buffersize;
        }

        public void Enqueue(CarArraySize toadd){
            lock(_lock){
                _head = NextPosition(_head);
                _buffer[_head] = toadd;
                if(IsFull)
                    _tail = NextPosition(_tail);
                else 
                    _length++;
            }
        }
        ///<summary>Функция, выдающая первый элемент буфера
        ///</summary>  
        public CarArraySize ZeroPoint(){
            lock(_lock){
                if(IsEmpty)throw new InvalidOperationException("Queue exhaused");
                else{
                    CarArraySize dequeved = _buffer[0];
                    return dequeved;
                }
            }
        }
        ///<summary>Функция, заполняющая первый элемент буфера. Нужна в том случае, если буфер используется как потокобезопасный тип данных
        ///<param name = "toadd">Значение, отправляемое в буфер</param>
        ///</summary>  
        public void AddZeroPoint(CarArraySize toadd){
            lock(_lock){
                _buffer[0] = toadd;
                _length = 1; 

            }
        }
        public CarArraySize ReadPosition(){
            lock(_lock){
                if(IsEmpty)throw new InvalidOperationException("Queue exhaused");
                else{
                    CarArraySize dequeved = _buffer[_tail];
                    return dequeved;
                }
            }
        }
        public void NextPosition(){
            lock(_lock){
                _head = NextPosition(_head);
                _tail = NextPosition(_tail);

                while((_tail>_length)||(_head>_length)){
                    _head = NextPosition(_head);
                    _tail = NextPosition(_tail);
                }
            }
        }

        public void CleanBuffer(bool trigger = false){ //При наличии триггера буффер очищается не полностью
            lock(_lock){
                if(trigger){
                    CarArraySize dequeved = Dequeue1();
                    _head = _buffersize -1;
                    _tail = 0;
                    _length = 0;
                    Enqueue(dequeved);
                }else{
                    _head = _buffersize -1;
                    _tail = 0;
                    _length = 0;
                }
            }
        }

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
            Enqueue(_input);
        }
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
    }
}