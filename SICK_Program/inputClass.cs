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
    public class AppContext
    {
        public InputTaskContext[] TasksContext;
        public ManualResetEvent[] InputEvents;
        public ManualResetEvent[] ErrorEvents;
        public ManualResetEvent ExitEvent;
        public config Config;

        public AppContext(config cfg)
        {
            ExitEvent = new ManualResetEvent(false);
            Config = cfg;
            TasksContext = Enumerable.Range(0, cfg.Scanners.Length)
                                     .Select(x => new InputTaskContext(
                                         cfg.Scanners[x],ExitEvent))
                                     .ToArray();
            InputEvents = TasksContext.Select(x => x.InputEvent).ToArray();
            ErrorEvents = TasksContext.Select(x => x.ErrorEvent).ToArray();
        }
    }

    public class InputTaskContext
    {
        public Scanner scaner;
        public CircularBuffer<Scanint> MyConcurrentQueue;
        public ManualResetEvent InputEvent;
        public ManualResetEvent ErrorEvent;
        public ManualResetEvent ExitEvent;

        public InputTaskContext(Scanner scanner, ManualResetEvent exitEvent)
        {
            scaner = scanner;
            InputEvent = new ManualResetEvent(false);
            ErrorEvent = new ManualResetEvent(false);
            ExitEvent = exitEvent;
            MyConcurrentQueue = new CircularBuffer<Scanint>(1);
        }
    }
    public class inputClass
    {
        public Scanner scaner;
        public CircularBuffer<Scanint> MyConcurrentQueue;
        public ManualResetEvent InputEvent;
        public ManualResetEvent ErrorEvent;
        public inputClass(Scanner scanner){
            scaner = scanner;
            InputEvent = new ManualResetEvent(false);
            ErrorEvent = new ManualResetEvent(false);
            MyConcurrentQueue = new CircularBuffer<Scanint>(1);
        }
    }
    public class InputClass{
        public inputClass[] inputClass;
        public ManualResetEvent[] InputEvent;
        public ManualResetEvent[] ErrorEvent;
        public InputClass(config config){
            inputClass = new inputClass[config.Scanners.Length];
            for(var i = 0; i < config.Scanners.Length; i++){
                inputClass[i] = new inputClass(config.Scanners[i]);
            }
            InputEvent = InputClInput();
            ErrorEvent = InputClError();
        }
        public ManualResetEvent[] InputClInput(){
            return(inputClass.Select(n => n.InputEvent).ToArray());
        }
        public ManualResetEvent[] InputClError(){
            return(inputClass.Select(n => n.ErrorEvent).ToArray());
        }
    }
}