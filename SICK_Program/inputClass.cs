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
    public class inputTheard
    {
        public Scanner scaner;
        public CircularBuffer<Scanint> MyConcurrentQueue;
        public ManualResetEvent InputEvent;
        public ManualResetEvent ErrorEvent;
        public inputTheard(Scanner scanner){
            scaner = scanner;
            InputEvent = new ManualResetEvent(false);
            ErrorEvent = new ManualResetEvent(false);
            MyConcurrentQueue = new CircularBuffer<Scanint>(1);
        }
    }
    public class AllInput{
        public inputTheard[] inputClass;
        public ManualResetEvent[] InputEvent;
        public ManualResetEvent[] ErrorEvent;
        public AllInput(config config){
            inputClass = new inputTheard[config.Scanners.Length];
            for(var i = 0; i < config.Scanners.Length; i++){
                inputClass[i] = new inputTheard(config.Scanners[i]);
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

        //TaskInput, AllTaskInput
    }
}