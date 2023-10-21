using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections;

namespace Sick_test
{
    ///<summary>Примитивнейший круговой буфер
    ///</summary>  
    public class CircularBuffer<T>{
        protected T[] _buffer;
        protected int _head;
        protected int _tail;
        protected int _length;
        protected int _buffersize;
        protected ManualResetEvent ReadEvent;

        protected Object _lock = new object ();

        public CircularBuffer(int buffersize){
            _buffer = new T[buffersize];
            _buffersize = buffersize;
            _head = buffersize -1;
            ReadEvent = new ManualResetEvent(false);
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
        public T Dequeue(){
            ReadEvent.WaitOne(10000);
            lock(_lock){
                if(IsEmpty)throw new InvalidOperationException("Queue exhaused");
                T dequeved = _buffer[_tail];
                _tail = NextPosition(_tail);
                _length --;
                if(IsEmpty){
                    ReadEvent.Reset();
                }
                return dequeved;
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
                ReadEvent.Set();
            }
        }
        ///<summary>Функция, выдающая первый элемент буфера
        ///</summary>  
        public T ZeroPoint(){
            lock(_lock){
                if(IsEmpty)throw new InvalidOperationException("Queue exhaused");
                else{
                    T dequeved = _buffer[0];
                    return dequeved;
                }
            }
        }
        ///<summary>Функция, заполняющая первый элемент буфера. Нужна в том случае, если буфер используется как потокобезопасный тип данных
        ///<param name = "toadd">Значение, отправляемое в буфер</param>
        ///</summary>  
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
                while((_tail>_length)||(_head>_length)){
                    _head = NextPosition(_head);
                    _tail = NextPosition(_tail);
                }
            }
        }

        public void CleanBuffer(bool trigger = false){ //При наличии триггера буффер очищается не полностью
            lock(_lock){
                if(trigger){
                    T dequeved = Dequeue1();
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

    }

    public class ResizebleArray<T>{
        private T[] objectsList;
        private int counter;
        private int maxObjects;

        public ResizebleArray(int _maxObjects){
            maxObjects = _maxObjects;
            counter = 0;
            objectsList = new T[maxObjects];
        }

        public int getCount()
        {
            return counter ;
        }
        public void Null()
        {
            counter = 0;
        }
        public void Update(int index, T data)
        {
            if(index>counter){
                counter = index;
            }
            if(index < maxObjects){
                objectsList[index] = (data);
            }else{
                Console.WriteLine("Переполнение массива");
            }
        }
        public T Read(int index){
            return objectsList[index];
        }
        public void CopyTo(T[] NewArray){
            if(NewArray.Length < counter){
                NewArray = new T[counter];
            }
            for(int i = 0; i < counter; i++){
                NewArray[i] = objectsList[i];
            }
        }
        public void CopyTo(ResizebleArray<T> NewArray){
            if(NewArray.maxObjects < counter){
                NewArray = new ResizebleArray<T>(maxObjects);
            }
            for(int i = 0; i < counter; i++){
                NewArray.objectsList[i] = objectsList[i];
            }
        }
        public T[] Copy(int len){
            return new ArraySegment<T>(objectsList, 0, len).ToArray();
        }
    }



}
