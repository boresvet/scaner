using System;
using System.IO;
using System.Net;
using BSICK.Sensors.LMS1xx;
using static System.Math;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sick_test
{
    class Server
    {
        public HttpListenerResponse response;
        private static HttpListener listener;
        private static string baseFilesystemPath;

        public Server(string[] args)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine(
                    "*** HttpListener requires at least Windows XP SP2 or Windows Server 2003.");
                return;
            }

            /*if(args.Length < 2)
            {
                Console.WriteLine("Basic read-only HTTP file server");
                Console.WriteLine();
                Console.WriteLine("Usage: httpfileserver <base filesystem path> <port>");
                Console.WriteLine("Request format: http://url:port/path/to/file.ext");
                return;
            }*/

            baseFilesystemPath = Path.GetFullPath(args[0]);
            var port = int.Parse(args[1]);

            listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:" + port + "/");
            listener.Start();

            Console.WriteLine("--- Server stated, base path is: " + baseFilesystemPath);
            Console.WriteLine("--- Listening, exit with Ctrl-C");

        }
        private DateTime addDate(DateTime[] input, int[] second){
            var ret = new DateTime();
            if(input[0].Day!=second[0]){
                ret = DateTime.Now;
            }else{
                ret = input[0];
            }
            ret.AddDays(second[0]);
            ret.AddHours(second[1]);
            ret.AddMinutes(second[2]);
            ret.AddSeconds(second[3]);
            return ret;
        }
        private string SerializeCars(List<CarArraySize> cars){
            return JsonSerializer.Serialize<List<CarArraySize>>(cars);
        }
        private string SerializeCarsFromId(int linesID, List<CarArraySize> cars, config config){          
            foreach(CarArraySize i in cars){
                if((i.leftborder)>(config.RoadSettings.Lanes[linesID].Offset+config.RoadSettings.Lanes[linesID].Width)|(i.rightborder)<(config.RoadSettings.Lanes[linesID].Offset)){
                    cars.Remove(i);
                }
            }
            return JsonSerializer.Serialize<List<CarArraySize>>(cars);
        }
        private string SerializeCarsFromCoordinate(int linesPoint, List<CarArraySize> cars){
            foreach(CarArraySize i in cars){
                if((((i.leftborder)>linesPoint)|(i.rightborder<linesPoint))){
                    cars.Remove(i);
                }
            }
            return JsonSerializer.Serialize<List<CarArraySize>>(cars);
        }
        public void ServerLoop(TimeBuffer times, config config)
        {
            while(true)
            {
                var context = listener.GetContext();
                var request = context.Request;

                var mycarsseach = new IslandSeach(config);
                var timesArray = times.ReadFullArray();
                var time = times.bufferTimes();

                //Принимает время, в котором нужно искать машинку из post запроса
                string text;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    text = reader.ReadToEnd();
                }
                //Принимает время парся текстовую строку
                /*
                response = context.Response;
                var TimeData = request.RawUrl.Substring(1);*/






                //Содержит номер типа запроса
                var functionType = Int32.Parse(text.Split('&')[0].Split('=')[1]);
                //var textarray = text.Split('&');
                //Содержит остальные данные времени
                var textarray = new ArraySegment<int>(text.Split('&').Select(n => Int32.Parse(n.Split('=')[1])).ToArray(), 1, 4).ToArray();
                var functionData = text.Split('&').Select(n => Int32.Parse(n.Split('=')[1])).ToArray().Skip(4).ToArray();
                
                //Сделать преобразование времени в формат DataTime!!!
                DateTime second = addDate(time, textarray);











                //Тут будет вызов islandSeach, которая вернёт массив машинок
                //Она СОЗДАСТ массив машинок
                //Его можно забрать из класса
                //mycarsseach.CarArrays(second, timesArray);
                var cars = mycarsseach.CarsArray;
                var jsonret = new string("");



                //Для каждого типа запроса создаёт свой JSON объект данных
                switch (functionType)
                {
                    case 0:
                        jsonret = SerializeCars(cars);
                        break;
                    case 1:
                        jsonret = SerializeCarsFromId(functionData[0], cars, config);
                        break;
                    case 2:
                        jsonret = SerializeCarsFromCoordinate(functionData[0], cars);
                        break;
                    default:
                        jsonret = SerializeCars(cars);
                        break;
                }
                
                
                response.Headers.Set("Content-Type", "text/plain");
                byte[] buffer = Encoding.UTF8.GetBytes(jsonret);
                response.ContentLength64 = buffer.Length;
                using Stream ros = response.OutputStream;
                ros.Write(buffer, 0, buffer.Length);

                response.OutputStream.Close();
                response = null;
                Console.WriteLine(" Ok!");
            }
        }

        public void SendErrorResponse(int statusCode, string statusResponse)
        {
            response.ContentLength64 = 0;
            response.StatusCode = statusCode;
            response.StatusDescription = statusResponse;
            response.OutputStream.Close();
            Console.WriteLine("*** Sent error: {0} {1}", statusCode, statusResponse);
        }
    }
}