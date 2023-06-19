using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;
using System.Text;

using System.Text.Json;
using System.Text.Json.Serialization;
using Sick_test;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SickScanner
{

    class Program
    {

        config config;
        ResponseFullConfig webconfig;
        static void Main1(){
            var returns = new Sick_test.returns();
            var MainT = Task.Run(() => Sick_test.SickScanners.RunScanners(returns));

        }
        async Task Echo(WebSocket webSocket, returns returns)
        {
            var cts = new CancellationTokenSource();    

            var recvBuffer = new byte[1024];
            var recvArraySegment = new ArraySegment<byte>(recvBuffer, 0, recvBuffer.Length);
            while (webSocket.State == WebSocketState.Open)
            {
                
                var json = JsonSerializer.Serialize(returns.times.readLastScan());
                var buffer = Encoding.UTF8.GetBytes(json);
                ///var tmp = await webSocket.ReceiveAsync(recvArraySegment, CancellationToken.None);
                try
                {
                    var recvTask = webSocket.ReceiveAsync(recvArraySegment, cts.Token);
                    var sendTask = webSocket.SendAsync(
                        new ArraySegment<byte>(buffer, 0, buffer.Length),
                        WebSocketMessageType.Text,
                        true,
                        cts.Token);
                    
                    var result = Task.WaitAny(new[] { recvTask, sendTask });
                    if (result == 1)
                    {
                        await Task.Delay(500);
                        continue;
                    }
                    var status = webSocket.CloseStatus;
                    if (status.HasValue)
                    {
                        cts.Cancel();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    cts.Cancel();
                    return;
                }
                
            }
        }
        public void ReadConfig(){
            var ReadFile = File.ReadAllText("config.json");
            config = JsonSerializer.Deserialize<config>(ReadFile);
        }
        public void SaveConfigToFile(){
            var WriteFile = JsonSerializer.Serialize<config>(config);
            File.WriteAllText("config.json", WriteFile);
        }
        public void ReadWebConfig(){
            try
            {
                var ReadFile = File.ReadAllText($"Configs/{config.WebConfigsName}.json");
                webconfig = JsonSerializer.Deserialize<ResponseFullConfig>(ReadFile);
            }
            catch (Exception){
                (webconfig = new ResponseFullConfig()).AddWebConfig(webconfig, config);
            }
        }
        public void SaveWebConfigToFile(){
            var WriteFile = JsonSerializer.Serialize<ResponseFullConfig>(webconfig);
            File.WriteAllText($"Configs/{config.WebConfigsName}.json", WriteFile);
        }
        static void Main(){
            var Returns = new returns();
            var MainT = Task.Run(() => Sick_test.SickScanners.RunScanners(Returns));





            var prog = new Program();
            prog.ReadConfig();
            prog.ReadWebConfig();
            prog.server(Returns);
        }

        void server(returns returns){
            var builder = WebApplication.CreateBuilder();
            builder.Logging.SetMinimumLevel(LogLevel.Trace);

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/www/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Echo(webSocket, returns);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    }
                }
                else
                {
                    await next(context);
                }

            });

            app.Run(async (context) =>
            {
                var response = context.Response;
                var request = context.Request;
                var path = request.Path;
                //string expressionForNumber = "^/api/users/([0-9]+)$";   // если id представляет число
                // 2e752824-1657-4c7f-844b-6ec2e168e99c
                //string expressionForGuid = @"^/api/users/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";
                /*if (path == "/api/users" && request.Method=="GET")
                {
                    await GetAllPeople(response); 
                }
                else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "GET")
                {
                    // получаем id из адреса url
                    string? id = path.Value?.Split("/")[3];
                    await GetPerson(id, response);
                }
                else if (path == "/api/users" && request.Method == "POST")
                {
                    await CreatePerson(response, request);
                }
                else if (path == "/api/users" && request.Method == "PUT")
                {
                    await UpdatePerson(response, request);
                }
                else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "DELETE")
                {
                    string? id = path.Value?.Split("/")[3];
                    await DeletePerson(id, response);
                }*/






                /*if (path == "/www/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Echo(webSocket, returns);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    }
                }*/
                /*else */if (path == "/www/configname" && request.Method == "GET")//
                {
                    await ReadWebConfigName(response);
                }
                else if (path == "/www/savewebconfigasconfig" && request.Method == "GET")//
                {
                    await SaveWebConfigAsConfig(response);
                }
                else if (path == "/www/configname" && request.Method == "POST")//
                {
                    await WriteConfigName(response, request);
                }
                else if (path == "/www/full_config" && request.Method == "GET")//
                {
                    await ReturnConfig(response);
                }
                else if (path == "/www/full_config" && request.Method == "POST")//
                {
                    await WriteWebConfigToFile(response, request);
                }
                else if (path == "/www/road" && request.Method == "GET")//
                {
                    await ReturnRoad(response);
                }
                else if (path == "/www/road" && request.Method == "POST")//
                {
                    await SaveRoad(response, request);
                }
                else if (path == "/www/lanes" && request.Method == "GET")//
                {
                    await ReturnLanes(response);
                }
                else if (path == "/www/lanes" && request.Method == "POST")//
                {
                    await SaveLanes(response, request);
                }
                else if (path == "/www/deadzones" && request.Method == "GET")//
                {
                    await ReturnDeadzones(response);
                }
                else if (path == "/www/deadzones" && request.Method == "POST")//
                {
                    await SaveDeadzones(response, request);
                }
                /*else if (path == "/www/transforms" && request.Method == "GET")//
                {
                    await ReturnTransforms(response);
                }
                else if (path == "/www/transform" && request.Method == "POST")//
                {
                    await SaveTransform(response, request);
                }*/
                
                
                
                else if (path == "/www/transforms" && request.Method == "GET")//
                {
                    await ReturnTransforms(response);
                }
                else if (path == "/www/transform" && request.Method == "POST")//
                {
                    await SaveTransform(response, request);
                }
                else if (path == "/www/connection" && request.Method == "POST")//
                {
                    await GetScanner(response, request);
                }
                else if (path == "/www/connections" && request.Method == "GET")//
                {
                    await ReturnScanners(response);
                }
                else if (path == "/www/connection" && request.Method == "DELETE")//
                {
                    var name = await request.ReadFromJsonAsync<int>();
                    webconfig.DeleteScaner(name);
                }
                else if (path == "/www/get_scan" && request.Method == "GET")//
                {
                    await ReturnScan(response, returns);
                }
                else if (path == "/www/add_connection" && request.Method == "POST")//
                {
                    await SaveScaner(response, request);
                }
                /*else if (path == "/wwwroot" && request.Method == "GET")//
                {
                    await SaveScaner(response, request);
                }*/
            });
            app.Run();
        }


        // получение всех пользователей

        async Task ReadWebConfigName(HttpResponse response)
        {
            await response.WriteAsJsonAsync(config.WebConfigsName);
        }
        async Task SaveWebConfigAsConfig(HttpResponse response)
        {
            try
            {
                webconfig.SaveConfig(webconfig, config);
                SaveConfigToFile();
            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Где-то произощёл косяк в конвертации. Возможно конфиг заполнен не до конца" });
            }
        }
        async Task WriteConfigName(HttpResponse response, HttpRequest request)
        {
            try
            {
                // получаем название конфига
                var name = await request.ReadFromJsonAsync<string>();
                if (name != null)
                {
                    // устанавливаем название конфига
                    SaveWebConfigName(name);
                    await response.WriteAsJsonAsync(new { message = "Конфиг установлен" });
                }
                else
                {
                    throw new Exception("Назвать конфигурацию пустой строкой - гениально, я считаю. Но увы, Великий Рандом, величайший из богов Хаоса с этим не согласен");
                }
            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
            }
        }
        public void SaveWebConfigName(string name){
            config.WebConfigsName = name;
        }
        async Task ReturnConfig(HttpResponse response)
        {
            await response.WriteAsJsonAsync(webconfig);
        }
        async Task WriteWebConfigToFile(HttpResponse response, HttpRequest request)
        {
            try
            {
                var cnf = await request.ReadFromJsonAsync<ResponseFullConfig>();
                webconfig = cnf;
                SaveWebConfigToFile();
            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Где-то произощёл косяк в конвертации. Возможно конфиг заполнен не до конца" });
            }
        }

        async Task ReturnRoad(HttpResponse response)
        {
            await response.WriteAsJsonAsync(webconfig.roadSettings);
        }
        async Task SaveRoad(HttpResponse response, HttpRequest request)
        {
            try
            {
                var cnf = await request.ReadFromJsonAsync<RoadSettings>();
                webconfig.roadSettings = cnf;
            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Где-то произощёл косяк в конвертации. Возможно конфиг заполнен не до конца" });
            }
        }
        async Task ReturnLanes(HttpResponse response)
        {
            for (int i = 0; i < webconfig.roadSettings.lanes.Length; i++)
                webconfig.roadSettings.lanes[i].id = i;
            await response.WriteAsJsonAsync(webconfig.roadSettings.lanes);
        }
        async Task SaveLanes(HttpResponse response, HttpRequest request)
        {
            try
            {
                var lanes = await request.ReadFromJsonAsync<Lanes[]>();
                for (int i = 0; i < lanes.Length; i++)
                    lanes[i].id = i;
                webconfig.roadSettings.lanes = lanes;
                response.WriteAsJsonAsync(webconfig.roadSettings.lanes);
                /*for (int i = 0; i < cnf.Length; i++)
                {
                    webconfig.roadSettings.lanes[i].id = i;
                }*/
            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Где-то произощёл косяк в конвертации. Возможно конфиг заполнен не до конца" });
            }
        }
        async Task ReturnDeadzones(HttpResponse response)
        {
            for (int i = 0; i < webconfig.roadSettings.blinds.Length; i++)
                webconfig.roadSettings.blinds[i].id = i;
            await response.WriteAsJsonAsync(webconfig.roadSettings.blinds);
        }
        async Task SaveDeadzones(HttpResponse response, HttpRequest request)
        {
            try
            {
                var blinds = await request.ReadFromJsonAsync<Blinds[]>();
                for (int i = 0; i < blinds.Length; i++)
                    blinds[i].id = i;
                webconfig.roadSettings.blinds = blinds;
                response.WriteAsJsonAsync(webconfig.roadSettings.blinds);
                /*for (int i = 0; i < cnf.Length; i++)
                {
                    webconfig.roadSettings.lanes[i].id = i;
                }*/
            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Где-то произощёл косяк в конвертации. Возможно конфиг заполнен не до конца" });
            }
        }
        async Task ReturnTransforms(HttpResponse response)
        {
            await response.WriteAsJsonAsync(webconfig.scanners.Select(x => new Transformations(x)).ToList());
        }
        async Task SaveTransform(HttpResponse response, HttpRequest request)
        {
            try
            {
                var transform = await request.ReadFromJsonAsync<Transformations>();
                var scan = webconfig.scanners.FirstOrDefault(x => x.id.Equals(transform.id));
                if (scan != null){
                scan.transformations = new Transformations
                {
                    height = transform.height,
                    horisontalOffset = transform.horisontalOffset,
                    correctionAngle = transform.correctionAngle
                };
                response.StatusCode = 200;
                }
                
                /*for (int i = 0; i < cnf.Length; i++)
                {
                    webconfig.roadSettings.lanes[i].id = i;
                }*/
            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Где-то произощёл косяк в конвертации. Возможно конфиг заполнен не до конца" });
            }
        }
        async Task SaveScaner(HttpResponse response, HttpRequest request)
        {
            try
            {
                var newScanner = await request.ReadFromJsonAsync<Scanner>();
                webconfig.scanners = webconfig.scanners.Append(newScanner).ToArray();

                
                /*for (int i = 0; i < cnf.Length; i++)d
                {
                    webconfig.roadSettings.lanes[i].id = i;
                }*/
            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Где-то произощёл косяк в конвертации. Возможно конфиг заполнен не до конца" });
            }
        }
        async Task ReturnScan(HttpResponse response, returns returns)
        {
            await response.WriteAsJsonAsync(returns.returnRoad());
        }

        async Task ReturnScanners(HttpResponse response)
        {
            await response.WriteAsJsonAsync(webconfig.scanners.Select(x => new Connect(x)).ToList());
        }
        async Task GetScanner(HttpResponse response, HttpRequest request)
        {
            try
            {
                var scaners = await request.ReadFromJsonAsync<Connect>();

                var scan = webconfig.scanners.FirstOrDefault(x => x.id.Equals(scaners.uid));
                if (scan == null)
                    response.StatusCode = 400;
                scan.connection = new Connection
                {
                    address = scaners.Address,
                    port = scaners.Port.Value,
                    enabled = scaners.Enabled.Value  
                };
                await response.WriteAsJsonAsync(webconfig.scanners.Select(x => new Connect(x)).ToArray());

                /*for (int i = 0; i < cnf.Length; i++)d
                {
                    webconfig.roadSettings.lanes[i].id = i;
                }*/
            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Где-то произощёл косяк в конвертации. Возможно конфиг заполнен не до конца" });
            }
        }
        async Task SaveConfig(ResponseFullConfig webconfig, HttpResponse response, HttpRequest request)
        {
            try
            {
                var ReadFile = File.ReadAllText("config.json");
                config config = JsonSerializer.Deserialize<config>(ReadFile);



                try{
                    File.WriteAllText("config.json", JsonSerializer.Serialize(config));
                }catch (Exception)
                {
                    response.StatusCode = 400;
                    await response.WriteAsJsonAsync(new { message = "Некорректные данные конфигурации" });
                }

            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
            }
        }

        public class Connect
        {
            public int uid  { get; set; }
            public string? Address { get; set; }
            public int? Port { get; set; }
            public bool? Enabled { get; set; }

            public Connect() { }
            public Connect(Scanner scanner)
            {
                uid = scanner.id;
                Address = scanner.connection.address;
                Port = scanner.connection.port;
                Enabled = scanner.connection.enabled;

            }
        }

    }
}