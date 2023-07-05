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
        FullScan RetScan;
        static void Main1(){
            var returns = new Sick_test.returns();
            var MainT = Task.Run(() => Sick_test.SickScanners.RunScanners(returns));

        }

        async Task Echo(WebSocket webSocket, ResponseFullConfig webconfig)
        {
            var cts = new CancellationTokenSource();    

            var recvBuffer = new byte[1024];
            var recvArraySegment = new ArraySegment<byte>(recvBuffer, 0, recvBuffer.Length);
            while (webSocket.State == WebSocketState.Open)
            {
                RetScan.AddScan(webconfig);
                var json = JsonSerializer.Serialize(RetScan);
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
            RetScan = new FullScan(config.Scanners.Length);
            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(2)
            };

            app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/www/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Echo(webSocket, webconfig);
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
                    config.RoadSettings.Lanes[i].ID = i;
                return config.RoadSettings.Lanes;
            });

            app.MapPost("www/lanes", (Lane[] lanes) =>//
            {
                for (int i = 0; i < lanes.Length; i++)
                    lanes[i].ID = i;
                config.RoadSettings.Lanes = lanes;
                return config.RoadSettings.Lanes;
            });

            app.MapGet("/www/configname", () =>//
            {
                return config.WebConfigsName;
            });
            app.MapGet("/www/savewebconfigasconfig", () =>//
            {
                webconfig.SaveConfig(webconfig, config);
                SaveConfigToFile();
                return (new { message = "Ok" });
            });
            app.MapPost("/www/configname", (string name) =>//
            {
                if (name != null)
                {
                    // устанавливаем название конфига
                    SaveWebConfigName(name);
                    return (new { message = "Конфиг установлен" });
                }
                else
                {
                    return (new { message = ("Назвать конфигурацию пустой строкой - гениально, я считаю. Но увы, Великий Рандом, величайший из богов Хаоса с этим не согласен" ) });
                }
            });
            app.MapGet("/www/full_config", () =>//
            {
                return webconfig.roadSettings;
            });
            app.MapPost("/www/full_config", (ResponseFullConfig cnf) =>
            {
                webconfig = cnf;
                SaveWebConfigToFile();
                return webconfig.roadSettings;
            });

            app.MapGet("/www/road", () =>//
            {
                return webconfig.roadSettings;
            });
            app.MapPost("/www/road", (RoadSettings cnf) =>
            {
                webconfig.roadSettings = cnf;
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
                return webconfig.roadSettings.blinds;
            });
            app.MapPost("/www/deadzones", (Blinds[] blinds) =>
            {
                for (int i = 0; i < blinds.Length; i++)
                    blinds[i].id = i;
                webconfig.roadSettings.blinds = blinds;
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
                
                

            app.MapGet("/www/transforms", () =>//
            {
                return webconfig.scanners.Select(x => new Transformations(x)).ToList();
            });
            app.MapPost("/www/transform", (Transformations transform) =>
            {
                var scan = webconfig.scanners.FirstOrDefault(x => x.id.Equals(transform.id));
                if (scan != null){
                scan.transformations = new Transformations
                {
                    height = transform.height,
                    horisontalOffset = transform.horisontalOffset,
                    correctionAngle = transform.correctionAngle
                };
                }
                return webconfig.scanners.Select(x => new Transformations(x)).ToList();
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
                RetScan = new FullScan(webconfig);

                return Results.Ok(webconfig.scanners.Select(x => new Connect(x)).ToList());
            });
            app.MapDelete("/www/connection", (int uid) =>
            {
                var scanner = webconfig.scanners.FirstOrDefault(x => x.id.Equals(uid));
                if (scanner == null)
                    return Results.NotFound();
                webconfig.scanners = webconfig.scanners.Where(x => !x.id.Equals(uid)).ToArray();
                RetScan = new FullScan(webconfig);
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
                        HorisontalOffset = new { Min = -2000, Max = 20000, Step = new { Min = 1, Max = 100 } },
                        Height = new { Min = 4000, Max = 10000, Step = new { Min = 1, Max = 100 } },
                        CorrectionAngle = new { Min = -45, Max = 45, Step = new { Min = 1, Max = 10 } }
                    },
                };
            });

            app.MapGet("/www/limits/road", () =>
            {
                return new
                {
                    Lane = new
                    {
                        Offset = new { Min = -2000, Max = 20000, Step = new { Min = 1, Max = 100 } },
                        Width = new { Min = 4000, Max = 10000, Step = new { Min = 1, Max = 100 } }
                    },

                    Blind = new
                    {
                        Offset = new { Min = -2000, Max = 20000, Step = new { Min = 1, Max = 100 } },
                        Width = new { Min = 4000, Max = 10000, Step = new { Min = 1, Max = 100 } },
                        Height = new { Min = 0, Max = 10000, Step = new { Min = 1, Max = 100 } }
                    },

                };
            });

            app.Run();
        }


        // получение всех пользователей

        async Task SaveWebConfigAsConfig(HttpResponse response)
        {

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
            public string uid  { get; set; }
            public string? Address { get; set; }
            public int? Port { get; set; }
            public bool? Enabled { get; set; }

            public Connect() { }
            public Connect(Scanner scanner)
            {
                uid = scanner.id.ToString();
                Address = scanner.connection.address;
                Port = scanner.connection.port;
                Enabled = scanner.connection.enabled;

            }
        }

    }
}