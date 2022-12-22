using BSICK.Sensors.LMS1xx;
using System;
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
    ///<summary>Скан с точками машинок, и с структурой для поиска островов
    ///</summary>  
    public class SuperScan{
        public int[] CarIslandLanes;
        public PointXYint[][] ScanArray;
        public DateTime Time;
        ///<summary>Функция, сохраняющая в себе скан
        ///</summary>
        public SuperScan(){
        }
        public SuperScan(int[] carIslandLanes, PointXYint[][] scanArray, DateTime time){
            CarIslandLanes = carIslandLanes;
            ScanArray = scanArray;
            Time = time;
        }
        /*public SuperScan(){
            CarIslandLanes = carIslandLanes;
            ScanArray = scanArray;
            Time = time;
        }*/
    }
    public class Second{
        public List<SuperScan> secondArray;
        ///<summary>Функция, сохраняющая в себе скан
        ///</summary>  
        public Second(){
            secondArray = new List<SuperScan>();
        }
        public void AddSuperScan(SuperScan input){
            SuperScan _input = new SuperScan();
            _input = input;
            secondArray.Add(_input);
        }
        public DateTime NowBufferTime(){
            if(secondArray.Count==0){
                return DateTime.Now;
            }else{
                return secondArray[0].Time;
            }
        }
    }
    ///<summary>Круговой буфер с машинками
    ///</summary>  
    public class TimeBuffer1{
        public Second[] _buffer;
        int _head;
        int _tail;
        int _length;
        int _buffersize;
        Object _lock = new object ();

        public TimeBuffer1(int timesize){
            _length = 0;
            _buffer = new Second[timesize];
            _buffersize = timesize;
            _head = timesize -1;
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
        public Second Dequeue1(){
            lock(_lock){
                if(IsEmpty)throw new InvalidOperationException("Queue exhaused");
                Second dequeved = _buffer[_tail];
                _tail = NextPosition(_tail);
                _length --;
                return dequeved;
            }
        }

        private int NextPosition(int position){
            return (position + 1) % _buffersize;
        }

        public void Enqueue(Second toadd){
            lock(_lock){
                _head = NextPosition(_head);
                _buffer[_head] = toadd;
                if(IsFull)
                    _tail = NextPosition(_tail);
                else 
                    _length++;
            }
        }
        public void Enqueue(SuperScan toadd){
            lock(_lock){
                _head = NextPosition(_head);
                _buffer[_head] = new Second(){secondArray = new List<SuperScan>(){toadd}}; 
                if(IsFull)
                    _tail = NextPosition(_tail);
                else 
                    _length++;
            }
        }
        public void SaveSuperScan(SuperScan toadd){
            lock(_lock){
                if(_length==0){
                    _head = NextPosition(_head);
                    _buffer[_head] = new Second(){secondArray = new List<SuperScan>(){toadd}}; 
                    _length++;
                }else{
                    if(_buffer[_head].NowBufferTime().Second==toadd.Time.Second){
                        _buffer[_head].AddSuperScan(toadd);
                    }else{
                        _head = NextPosition(_head);
                        SuperScan _toadd = new SuperScan();
                        _toadd = toadd;
                        _buffer[_head] = new Second(){secondArray = new List<SuperScan>(){_toadd}}; 
                        if(IsFull)
                            _tail = NextPosition(_tail);
                        else 
                            _length++;
                    }
                }
            }
        }
        public Second ReadPosition(){
            lock(_lock){
                if(IsEmpty)throw new InvalidOperationException("Queue exhaused");
                else{
                    Second dequeved = _buffer[_tail];
                    return dequeved;
                }
            }
        }
        public Second[] ReadFullArray(){
            lock(_lock){
                var ret = new Second[_buffer.Length];
                ret = _buffer;
                return ret;
            }
        }
        public DateTime[] bufferTimes(){
            var ret = new DateTime[2]{_buffer[_tail].secondArray[0].Time, _buffer[_head].secondArray[0].Time};
            return ret;
        }
        public void NextPosition(){
            lock(_lock){
                _head = NextPosition(_head);
                _tail = NextPosition(_tail);
            }
        }

        //Тестовая штука, проверяет, чтобы случайно время во всех сканах не было одним и тем же
        public bool istimesgood(){
            return(_buffer[_head].secondArray[0].Time == _buffer[_tail].secondArray[0].Time);
        }
        public void createtimeisgood(){
            _buffer[_tail].secondArray[0].Time = DateTime.Now;
            _buffer[_head].secondArray[0].Time = DateTime.Now;
            _buffer[_tail].secondArray[0].Time.AddHours(11);
        }
    }
}
