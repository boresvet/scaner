using System;

namespace Sick_test
{
    public class CircularBuffer<T>{
        public T[] _buffer;
        int _head;
        int _tail;
        int _length;
        int _buffersize;
        Object _lock = new object ();

        public CircularBuffer(int buffersize){
            _buffer = new T[buffersize];
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
        public T Dequeue1(){
            lock(_lock){
                if(IsEmpty)throw new InvalidOperationException("Queue exhaused");
                T dequeved = _buffer[_tail];
                _tail = NextPosition(_tail);
                _length --;
                return dequeved;
            }
        }

        private int NextPosition(int position){
            return (position + 1) % _buffersize;
        }

        public void Enqueue(T toadd){
            lock(_lock){
                _head = NextPosition(_head);
                _buffer[_head] = toadd;
                if(IsFull)
                    _tail = NextPosition(_tail);
                else 
                    _length++;
            }
        }
        public T ZeroPoint(){
            lock(_lock){
                if(IsEmpty)throw new InvalidOperationException("Queue exhaused");
                else{
                    T dequeved = _buffer[0];
                    return dequeved;
                }
            }
        }
        public void AddZeroPoint(T toadd){
            lock(_lock){
                _buffer[0] = toadd;
                _length = 1; 

            }
        }
        public T ReadPosition(){
            lock(_lock){
                if(IsEmpty)throw new InvalidOperationException("Queue exhaused");
                else{
                    T dequeved = _buffer[_tail];
                    return dequeved;
                }
            }
        }
        public void NextPosition(){
            lock(_lock){
                _head = NextPosition(_head);
                _tail = NextPosition(_tail);
            }
        }

        public void CleanBuffer(bool trigger = false){ //При наличии триггера буффер очищается не полностью
            lock(_lock){
                if(trigger){
                    T dequeved = Dequeue1();
                    _buffer = new T[_buffersize];
                    _head = _buffersize -1;
                    _tail = 0;
                    _length = 0;
                    Enqueue(dequeved);
                }else{
                    _buffer = new T[_buffersize];
                    _head = _buffersize -1;
                    _tail = 0;
                    _length = 0;
                }
            }
        }

    }
}
