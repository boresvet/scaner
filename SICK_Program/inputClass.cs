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