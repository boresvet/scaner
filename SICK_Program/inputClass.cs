using System.Linq;
using System.Threading;
namespace Sick_test
{
    ///<summary> Класс, содержащий данные о сканере для одного из потоков. 
    ///</summary>
    public class inputTheard
    {
        public Scanner scaner;
        public CircularBuffer<Scanint> MyConcurrentQueue;
        public ManualResetEvent InputEvent;
        public ManualResetEvent ErrorEvent;
        
        
        ///<summary>Создаёт класс, описывающий все данные, передаваемые в потоки
        ///</summary>
        ///<param name = "scanner">Конфигурация сканера, тип <paramref scanner="Scanner"/>Scadner</paramref>r</param>
        public inputTheard(Scanner scanner){
            scaner = scanner;
            InputEvent = new ManualResetEvent(false);
            ErrorEvent = new ManualResetEvent(false);
            MyConcurrentQueue = new CircularBuffer<Scanint>(1);
        }
    }
    ///<summary> Класс содержит все данные, передаваемые в поток и из потока. Так же из массива описаний потоков создаётся массив тригеров, что позволяет не тратить каждый раз время на извлечение триггеров из массива классов.
    ///</summary>
    public class AllInput{
        public inputTheard[] inputClass;
        public ManualResetEvent[] InputEvent;
        public ManualResetEvent[] ErrorEvent;
        ///<summary>
        ///</summary>
        ///<param name = "config">Конфигурации всех сканеров, тип <paramref config="config"/>config</paramref> позволяют обработать сразу все сканеры в одном потоке.</param>
        public AllInput(config config){
            inputClass = new inputTheard[config.Scanners.Length];
            for(var i = 0; i < config.Scanners.Length; i++){
                inputClass[i] = new inputTheard(config.Scanners[i]);
            }
            InputEvent = InputClInput();
            ErrorEvent = InputClError();
        }
        private ManualResetEvent[] InputClInput(){
            return(inputClass.Select(n => n.InputEvent).ToArray());
        }
        private ManualResetEvent[] InputClError(){
            return(inputClass.Select(n => n.ErrorEvent).ToArray());
        }

        //TaskInput, AllTaskInput
    }
}