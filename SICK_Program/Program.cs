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
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace SickScanner
{
    class carresponse{
        public DateTime time;
        public int lanenumber;
        public carresponse(){

        }
    }
    class Program
    {
        
        config config;
        ResponseFullConfig webconfig;
        WebScans RetScan;
        static void Main1(){
            var returns = new Sick_test.returns();
            var MainT = Task.Run(() => Sick_test.SickScanners.RunScanners(returns));

        }
        static void AbsolutPause(DateTime pausetime){
            if(pausetime>DateTime.Now.AddMinutes(10)){
                return;
            }else{
                pausetime = DateTime.Now.AddMinutes(10);
            }
        }
        static void Pause(DateTime pausetime){
            if(pausetime>DateTime.Now){
                if(pausetime>DateTime.Now.AddMinutes(10)){
                    return;
                }else{
                    pausetime = DateTime.Now.AddMinutes(10);
                }
            }
        }
        async Task Echo(WebSocket webSocket, ResponseFullConfig webconfig, returns returns, DateTime pausetime)
        {
            var cts = new CancellationTokenSource();    

            var recvBuffer = new byte[1024];
            var recvArraySegment = new ArraySegment<byte>(recvBuffer, 0, recvBuffer.Length);
            var trig = false;
            var pausescan = new WebScans();
            while (webSocket.State == WebSocketState.Open)
            {
                
                RetScan.AddScan(webconfig, trig, returns, config.Test);
                trig = !trig;
                string json;
                if(pausetime>=DateTime.Now){
                    json = JsonSerializer.Serialize(pausescan);
                }else{
                    json = JsonSerializer.Serialize(RetScan);
                    pausescan.Scan = RetScan.readScanData();
                }
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
        async Task SendCar(WebSocket webSocket, returns returns)
        {
            var cts = new CancellationTokenSource();    

            var recvBuffer = new byte[1024];
            var recvArraySegment = new ArraySegment<byte>(recvBuffer, 0, recvBuffer.Length);
            while (webSocket.State == WebSocketState.Open)
            {
                string json;
                json = JsonSerializer.Serialize(returns.carbuffer.LastCar());
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
            var WriteFile = JsonSerializer.Serialize<config>(config, new JsonSerializerOptions{WriteIndented = true});
            File.WriteAllText("config.json", WriteFile);
        }
        public void ReadWebConfig(){
            try
            {
                var ReadFile = File.ReadAllText("WebConfig.json");
                webconfig = JsonSerializer.Deserialize<ResponseFullConfig>(ReadFile);
            }
            catch (Exception){
                (webconfig = new ResponseFullConfig()).AddWebConfig(webconfig, config);
            }
        }
        public void SaveWebConfigToFile(){
            var WriteFile = JsonSerializer.Serialize<ResponseFullConfig>(webconfig);
            File.WriteAllText("WebConfig.json", WriteFile);
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
            var Paused = new DateTime();
            Paused = DateTime.Now;
            Paused.AddMinutes(-15);


            var builder = WebApplication.CreateBuilder();
            builder.Logging.SetMinimumLevel(LogLevel.Trace);

            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            RetScan = new WebScans(config.Scanners.Length);
            var app = builder.Build();

            if (true )
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseDefaultFiles();
            app.UseStaticFiles();
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(2)
            };

            app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Echo(webSocket, webconfig, returns, Paused);
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

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws/CarSocket")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await SendCar(webSocket, returns);
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

            app.MapGet("www/lanes", () =>//
            {
                for (int i = 0; i < config.RoadSettings.Lanes.Length; i++)
                    webconfig.roadSettings.lanes[i].id = i;
                return webconfig.roadSettings.lanes;
            });

            app.MapPost("www/lanes", (Lanes[] lanes) =>//
            {
                for (int i = 0; i < lanes.Length; i++)
                    lanes[i].id = i;
                webconfig.roadSettings.lanes = lanes;
                SaveWebConfigToFile();
                return webconfig.roadSettings.lanes;
            });

            app.MapGet("/www/savewebconfigasconfig", () =>//
            {
                webconfig.SaveConfig(webconfig, config);
                SaveConfigToFile();
                return (new { message = "Ok" });
            });
            app.MapGet("/www/ReadConfigToWebconfig", () =>//
            {
                webconfig.ReadConfig(webconfig, config);
                SaveWebConfigToFile();
                return (new { message = "Ok" });
            });
            app.MapPost("/www/pause", (bool paused) =>//
            {
                Paused = DateTime.Now;
                if(!paused){
                    Paused = DateTime.Now;
                }else{
                    AbsolutPause(Paused);
                }
                return (new {isPaused = (int)Paused.Subtract(DateTime.Now).TotalSeconds==0, duration = (int)Paused.Subtract(DateTime.Now).TotalSeconds});
            });
            app.MapGet("/www/pause", () =>//
            {
                if((int)Paused.Subtract(DateTime.Now).TotalSeconds>0){
                    return (new {isPaused = (int)Paused.Subtract(DateTime.Now).TotalSeconds==0, duration = (int)Paused.Subtract(DateTime.Now).TotalSeconds});
                }
                return (new {isPaused = false, duration = 0});
            });
            app.MapPost("/get_data/cars", (DateTime time) =>
            {
                return returns.carbuffer.GiveMyCar(time);
            });
            app.MapGet("/get_data/road", () =>//
            {
                return (returns.returnRoad());
            });
            app.MapPost("/get_data/scan", (int scanernumber) =>
            {
                return returns.returnScan(scanernumber);
            });
            
            app.MapPost("/get_data/car", (carresponse resp) =>
            {
                return returns.carbuffer.GiveMyCar(resp.time, resp.lanenumber);
            });
            app.MapGet("/www/full_config", () =>//
            {
                return config;
            });
            app.MapPost("/www/full_config", (config cnf) =>
            {
                config = cnf;
                SaveConfigToFile();
                return config;
            });

            app.MapGet("/www/road", () =>//
            {
                return webconfig.roadSettings;
            });
            app.MapPost("/www/road", (RoadSettings cnf) =>
            {
                webconfig.roadSettings = cnf;
                SaveWebConfigToFile();
                Pause(Paused);
                return webconfig.roadSettings;
            });
                /*else if (path == "/www/lanes" && request.Method == "GET")//
                {
                    await ReturnLanes(response);
                }
                else if (path == "/www/lanes" && request.Method == "POST")//
                {
                    await SaveLanes(response, request);
                }*/
            app.MapGet("/www/deadzones", () =>//
            {
                SaveWebConfigToFile();
                return webconfig.roadSettings.blinds;
            });
            app.MapPost("/www/deadzones", (Blinds[] blinds) =>
            {
                for (int i = 0; i < blinds.Length; i++)
                    blinds[i].id = i;
                webconfig.roadSettings.blinds = blinds;
                SaveWebConfigToFile();
                Pause(Paused);
                return webconfig.roadSettings.blinds;
            });
                /*else if (path == "/www/transforms" && request.Method == "GET")//
                {
                    await ReturnTransforms(response);
                }
                else if (path == "/www/transform" && request.Method == "POST")//
                {
                    await SaveTransform(response, request);
                }*/
                
            static bool getbool(bool? input){
                if(input == null){
                    return false;
                }
                return (bool)input;
            }
            //server.AddSwaggerGen(options =>{options.CustomSchemaIds(Transformations => Transformations.ToString());});
            app.MapGet("/www/transforms", () =>//
            {
                if(webconfig.scanners == null){
                    return (Transformation[])webconfig.scanners.Select(x => new Transformation(x)).ToArray();
                }else{
                    return new Transformation[0].ToArray();
                }
                return new Transformation[0].ToArray();
            });
            app.MapPost("/www/transform", (Transformation transform) =>
            {
                var scan = webconfig.scanners.FirstOrDefault(x => x.id.Equals(transform.uid));
                if (scan != null){
                
                if(scan == null){
                    scan.transformations = new Transformation
                    {
                        height = transform.height,
                        horisontalOffset = transform.horisontalOffset,
                        correctionAngle = transform.correctionAngle,
                        Flip = false,
                    };
                }else{
                    if(transform.Flip!=scan.transformations.Flip){
                        var i = scan.settings.startAngle;
                        scan.settings.startAngle = scan.settings.endAngle;
                        scan.settings.endAngle = i;
                    }
                    scan.transformations = new Transformation
                    {
                        height = transform.height,
                        horisontalOffset = transform.horisontalOffset,
                        correctionAngle = transform.correctionAngle,
                        Flip = transform.Flip,
                    };
                }
                }
                SaveWebConfigToFile();
                Pause(Paused);
                return webconfig.scanners.Select(x => new Transformation(x)).ToList();
            });

            app.MapGet("/www/connections", () =>//
            {
                return webconfig.scanners.Select(x => new Connect(x)).ToList();
            });
            app.MapPost("/www/connection", (Connect scaners) =>
            {
                var scan = webconfig.scanners.FirstOrDefault(x => x.id.Equals(scaners.uid));
                if (scan == null)
                    return Results.NotFound();
                scan.connection = new Connection
                {
                    address = scaners.Address,
                    port = scaners.Port.Value,
                    enabled = scaners.Enabled.Value  
                };
                SaveWebConfigToFile();
                Pause(Paused);
                return Results.Ok(webconfig.scanners.Select(x => new Connect(x)).ToList());
            });
            app.MapDelete("/www/connection", (int uid) =>
            {
                var scanner = webconfig.scanners.FirstOrDefault(x => x.id.Equals(uid));
                if (scanner == null)
                    return Results.NotFound();
                webconfig.scanners = webconfig.scanners.Where(x => !x.id.Equals(uid)).ToArray();
                SaveWebConfigToFile();
                return Results.Ok(webconfig.scanners.Select(x => new Connect(x)).ToArray());
            });

                /*else if (path == "/wwwroot" && request.Method == "GET")//
                {
                    await SaveScaner(response, request);
                }*/




            /*app.MapGet("www/connections", () =>
            {
                var result =  config.Scanners.Select(x => new Connect(x)).ToList();

                GetScanner();

                return Results.Ok(result);
            });
            app.MapPost("www/connection", (Connect connection) =>
            {
                var scan = cfg.Scanners.FirstOrDefault(x => x.Uid.Equals(connection.Uid));
                if (scan == null)
                    return Results.NotFound();
                scan.Connection = new Connection
                {
                    ScannerAddres = connection.Address,
                    ScannerPort = connection.Port.Value,
                    Enabled = connection.Enabled.Value  
                };
                ConfigFile.Write(cfg);
                return Results.Ok(webconfig.scanners.ToArray()) ;
            });*/
            app.MapGet("/www/limits/transform", () =>
            {
                return new
                {
                    Road = new
                    {
                        HorisontalOffset = new { Min = -5000, Max = 20000, Step = new { Min = 1, Max = 100 } },
                        Height = new { Min = 3000, Max = 10000, Step = new { Min = 1, Max = 100 } },
                        CorrectionAngle = new { Min = -270.0f, Max = 270.0f, Step = new { Min = 0.01f, Max = 1.0f } }
                    },
                };
            });
            app.MapGet("/www/algoritms", () =>
            {
                return JsonSerializer.Deserialize<algoritmsnames>(File.ReadAllText("algoritmsnames.json"));
            });
            app.MapPost("/www/UseAlgoritm", (string Algoritm) =>
            {
                config.Method = Algoritm;
                return Results.Ok(config.Method);
            });
            app.MapGet("/www/limits/road", () =>
            {
                return new
                {
                    Lane = new
                    {
                        Offset = new { Min = -2000, Max = 20000, Step = new { Min = 1, Max = 100 } },
                        Width = new { Min = 1500, Max = 10000, Step = new { Min = 1, Max = 100 } },
                        Height = new { Min = -2000, Max = 2000, Step = new { Min = 1, Max = 100 } },
                    },

                Blind = new
                {
                    Offset = new { Min = -2000, Max = 20000, Step = new { Min = 1, Max = 100 } },
                    Width = new { Min = 10, Max = 10000, Step = new { Min = 1, Max = 100 } },
                    Height = new { Min = 0, Max = 10000, Step = new { Min = 1, Max = 100 } }
                },

                };
            });

            app.Run();
        }


        // получение всех пользователей

        /*async Task ReturnConfig(HttpResponse response)
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
            /*}
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
            /*}
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Где-то произощёл косяк в конвертации. Возможно конфиг заполнен не до конца" });
            }
        }
        async Task ReturnTransforms(HttpResponse response)
        {
            await response.WriteAsJsonAsync();
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
            /*}
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
            /*}
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
         /*   }
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
        }*/

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