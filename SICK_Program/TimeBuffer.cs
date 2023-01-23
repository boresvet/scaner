using System;
using System.Linq;
using System.Threading;

namespace Sick_test
{
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
        public SuperScan CopyScan(){
            var newScan = new SuperScan();
            newScan.Time = Time;
            Array.Copy(CarIslandLanes, newScan.CarIslandLanes, CarIslandLanes.Length);
            newScan.ScanArray = new PointXYint[ScanArray.Length][];
            for(int i = 0; i<ScanArray.Length; i++){
                newScan.ScanArray[i] = PointXYint.copyScan(ScanArray[i]);
            }
            return newScan;
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
        private CircularBuffer<SuperScan>[] bufferstosearch; 


        int _head;
        int _indexbuffer;
        int _read_indexbuffer;
        SuperScan _input;
        int _tail;
        int _length;
        int _buffersize;
        public Object _lock = new object ();
        public ManualResetEvent ReadEvent;

        public TimeBuffer(int timesize, int buffersToSearchLeangth = 5){
            _length = 0;
            _buffersize = timesize;
            _head = timesize - 1;
            bufferstosearch = new CircularBuffer<SuperScan>[buffersToSearchLeangth];
            for(int i = 0; i < bufferstosearch.Length; i++){
                bufferstosearch[i] = new CircularBuffer<SuperScan>(timesize);
            }
            _indexbuffer = 0;
            ReadEvent = new ManualResetEvent(false);
        }

        private void nextbuffer(){
            Console.WriteLine("Буффер записан");
            _indexbuffer++;
            if(_indexbuffer >= bufferstosearch.Length){
                _indexbuffer = 0;
            }          
        }
        private void nextreadbuffer(){
            Console.WriteLine("Буффер считан");
            _read_indexbuffer++;
            if(_read_indexbuffer >= bufferstosearch.Length){
                _read_indexbuffer = 0;
            }          
        }

        public int MyLeanth{
            get { return _length;}
        }
        public void SaveSuperScan(SuperScan input){
            lock(_lock){
                if(bufferstosearch[_indexbuffer].IsFull){
                    var i = bufferstosearch[_indexbuffer]._buffer[bufferstosearch[_indexbuffer]._buffer.Length-1];//Перенос последнего суперскана из старого буффера в новый
                    nextbuffer();
                    bufferstosearch[_indexbuffer].Enqueue(i);
                }
                if(_indexbuffer != _read_indexbuffer){
                    ReadEvent.Set();
                }
                bufferstosearch[_indexbuffer].Enqueue(input);
            }
        }
        public SuperScan[] ReadFullArray(){
            lock(_lock){
                var ret = bufferstosearch[_read_indexbuffer]._buffer;
                return ret;
            }
        }
        public void RemoveReadArray(){
            bufferstosearch[_read_indexbuffer].CleanBuffer();
            nextreadbuffer();
            ReadEvent.Reset();
        }
        //Как SaveSuperScan, только с копированием
        public void AddSuperScan(SuperScan input){
            _input = new SuperScan(){CarIslandLanes = new int[input.CarIslandLanes.Length], ScanArray = new PointXYint[input.ScanArray.Length][], Time = new DateTime()};
            _input.CarIslandLanes = input.CarIslandLanes;
            _input.Time = input.Time;
            for(int i = 0; i < input.ScanArray.Length; i++){
                _input.ScanArray[i] = input.ScanArray[i].ToArray();
            }
            SaveSuperScan(_input);
        }
        public SuperScan CopySuperScan(SuperScan input){
            var _input = new SuperScan(){CarIslandLanes = new int[input.CarIslandLanes.Length], ScanArray = new PointXYint[input.ScanArray.Length][], Time = new DateTime()};
            _input.CarIslandLanes = input.CarIslandLanes;
            _input.Time = input.Time;
            for(int i = 0; i < input.ScanArray.Length; i++){
                _input.ScanArray[i] = input.ScanArray[i].ToArray();
            }
            return _input;
        }

        public SuperScan readLastScan(){
            return _input;
        }
    }
}
