using System.Linq;
using System.Threading;
namespace Sick_test
{
    ///<summary> Класс, содержащий данные о сканере для одного из потоков. 
    ///</summary>
    public class inputTheard
    {
        public int id;
        public Scaner scaner;
        private CircularBuffer<Scanint> MyConcurrentQueue;
        private CircularBuffer<int[]> RawConcurrentQueue;
        public ManualResetEvent InputEvent;
        public ManualResetEvent ErrorEvent;
        
        
        ///<summary>Создаёт класс, описывающий все данные, передаваемые в потоки
        ///</summary>
        ///<param name = "scanner">Конфигурация сканера, тип <paramref scanner="Scanner"/>Scadner</paramref>r</param>
        public inputTheard(Scaner scanner){
            id = scanner.ID;
            scaner = scanner;
            InputEvent = new ManualResetEvent(false);
            ErrorEvent = new ManualResetEvent(false);
            MyConcurrentQueue = new CircularBuffer<Scanint>(1);
            RawConcurrentQueue = new CircularBuffer<int[]>(1);
        }

        public Scanint GetLastScan(){
            return MyConcurrentQueue.ZeroPoint();
        }
        public void AddScan(Scanint scan){
            MyConcurrentQueue.AddZeroPoint(scan);
        }


        public int[] GetRawScan(){
            return RawConcurrentQueue.ZeroPoint();
        }
        public void AddRawScan(int[] scan){
            RawConcurrentQueue.AddZeroPoint(scan);
        }
    }
    ///<summary> Класс содержит все данные, передаваемые в поток и из потока. Так же из массива описаний потоков создаётся массив тригеров, что позволяет не тратить каждый раз время на извлечение триггеров из массива классов.
    ///</summary>
    public class AllInput{
        private bool[] ScannerDataReady;
        private inputTheard[] inputThreads;
        private ManualResetEvent[] InputEvent;
        private ManualResetEvent[] ErrorEvent;
        ///<summary>
        ///</summary>
        ///<param name = "config">Конфигурации всех сканеров, тип <paramref config="config"/>config</paramref> позволяют обработать сразу все сканеры в одном потоке.</param>
        public AllInput(config config){
            inputThreads = new inputTheard[config.Scanners.Length];
            ScannerDataReady = new bool[config.Scanners.Length];
            for(var i = 0; i < config.Scanners.Length; i++){
                inputThreads[i] = new inputTheard(config.Scanners[i]);
            }
            InputEvent = InputClInput();
            ErrorEvent = InputClError();
        }
        private ManualResetEvent[] InputClInput(){
            return(inputThreads.Select(n => n.InputEvent).ToArray());
        }
        private ManualResetEvent[] InputClError(){
            return(inputThreads.Select(n => n.ErrorEvent).ToArray());
        }

        public void TestScanners(){
            for(int i = 0; i < ErrorEvent.Length; i++)
            {
                ScannerDataReady[i] = (ErrorEvent[i].WaitOne(0) == false);
            }
        }
        public bool IsScannerReady(int i){
            return ScannerDataReady[i];
        }

        public Scanint ReadLastScan(int i){
            return inputThreads[i].GetLastScan();
        }
        public int[] ReadRawScan(int i){
            return inputThreads[i].GetRawScan();
        }
        public int ReadScanerID(int i){
            return inputThreads[i].id;
        }
        public Scanint GetLastScan(int i){
            var ret = inputThreads[i].GetLastScan();
            InputEvent[i].Reset();
            return ret;
        }

        public void WaitAnyData(){
            WaitHandle.WaitAny(InputEvent, 500);
        }
        public bool WaitAllData(){
            return WaitHandle.WaitAll(InputEvent, 50);
        }
        
        public inputTheard GetInputTheard(int i){
            return inputThreads[i];
        }

        //TaskInput, AllTaskInput
    }
}