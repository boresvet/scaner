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
    public class SuperScan{
        public int[] CarIslandLanes;
        public PointXYint[][] OneSecondScanArray;
        public DateTime Time;
        ///<summary>Функция, сохраняющая в себе скан
        ///</summary>
        public SuperScan(){
        }
        public SuperScan(int[] carIslandLanes, PointXYint[][] scanArray, DateTime time){
            CarIslandLanes = carIslandLanes;
            OneSecondScanArray = scanArray;
            Time = time;
        }
        /*public SuperScan(){
            CarIslandLanes = carIslandLanes;
            ScanArray = scanArray;
            Time = time;
        }*/
    }
    ///<summary>Скан с точками машинок, и с структурой для поиска островов
    ///</summary>  
    /*public class Second{
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
    }*/
    ///<summary>Круговой буфер с машинками
    ///</summary>  
    public class TimeBuffer{
        public SuperScan[] _buffer;
        int _head;
        int _tail;
        int _length;
        int _buffersize;
        public Object _lock = new object ();

        public TimeBuffer(int timesize){
            _length = 0;
            _buffersize = timesize;
            _buffer = new SuperScan[timesize];
            _head = timesize - 1;
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
        private int NextPosition(int position){
            return (position + 1) % _buffersize;
        }
        public void SaveSuperScan(SuperScan input){
            lock(_lock){
                _head = NextPosition(_head);
                _buffer[_head] = input;
                if(IsFull)
                    _tail = NextPosition(_tail);
                else 
                    _length++;
            }
        }
        public DateTime[] bufferTimes(){
            var ret = new DateTime[2]{_buffer[_tail].Time, _buffer[_head].Time};
            return ret;
        }
        public SuperScan[] ReadFullArray(){
            lock(_lock){
                var ret = new SuperScan[_buffer.Length];
                ret = _buffer;
                return ret;
            }
        }
        //Как SaveSuperScan, только с копированием
        public void AddSuperScan(SuperScan input){
            var _input = new SuperScan(){CarIslandLanes = new int[input.CarIslandLanes.Length], OneSecondScanArray = new PointXYint[input.OneSecondScanArray.Length][], Time = new DateTime()};
            _input.CarIslandLanes = input.CarIslandLanes;
            _input.Time = input.Time;
            for(int i = 0; i < input.OneSecondScanArray.Length; i++){
                _input.OneSecondScanArray[i] = input.OneSecondScanArray[i].ToArray();
            }
            SaveSuperScan(_input);
        }
        public bool istimesgood(){
            return _buffer[_head].Time == _buffer[_tail].Time;
        }
        public void createtimeisgood(){
            _buffer[_tail].Time = DateTime.Now;
            _buffer[_head].Time = DateTime.Now;
            _buffer[_tail].Time.AddHours(11);
        }
    }
}
