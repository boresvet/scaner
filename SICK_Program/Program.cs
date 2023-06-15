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
        List<Person> users = new List<Person> 
        { 
            new() { Id = Guid.NewGuid().ToString(), Name = "Tom", Age = 37 },
            new() { Id = Guid.NewGuid().ToString(), Name = "Bob", Age = 41 },
            new() { Id = Guid.NewGuid().ToString(), Name = "Sam", Age = 24 }
        };
        config config;
        ResponseFullConfig webconfig;
        static void Main1(){
            var returns = new Sick_test.returns();
            var MainT = Task.Run(() => Sick_test.SickScanners.RunScanners(returns));

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









            var prog = new Program();
            prog.ReadConfig();
            prog.ReadWebConfig();
            prog.server(Returns);
        }

        void server(returns returns){
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.Run(async (context) =>
            {
                var response = context.Response;
                var request = context.Request;
                var path = request.Path;
                //string expressionForNumber = "^/api/users/([0-9]+)$";   // если id представляет число
                // 2e752824-1657-4c7f-844b-6ec2e168e99c
                string expressionForGuid = @"^/api/users/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";
                if (path == "/api/users" && request.Method=="GET")
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
                }







                else if (path == "/www/configname" && request.Method == "GET")//
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
                else if (path == "www/full_config" && request.Method == "GET")//
                {
                    await ReturnConfig(response);
                }
                else if (path == "www/full_config" && request.Method == "POST")//
                {
                    await WriteWebConfigToFile(response, request);
                }
                else if (path == "www/road" && request.Method == "GET")//
                {
                    await ReturnRoad(response);
                }
                else if (path == "www/road" && request.Method == "POST")//
                {
                    await SaveRoad(response, request);
                }
                else if (path == "www/lanes" && request.Method == "GET")//
                {
                    await ReturnLanes(response);
                }
                else if (path == "www/lanes" && request.Method == "POST")//
                {
                    await SaveLanes(response, request);
                }
                else if (path == "www/deadzones" && request.Method == "GET")//
                {
                    await ReturnDeadzones(response);
                }
                else if (path == "www/deadzones" && request.Method == "POST")//
                {
                    await SaveDeadzones(response, request);
                }
                else if (path == "www/transforms" && request.Method == "GET")//
                {
                    await ReturnTransforms(response);
                }
                else if (path == "www/transform" && request.Method == "POST")//
                {
                    await SaveTransform(response, request);
                }
                
                
                
                else if (path == "www/transforms" && request.Method == "GET")//
                {
                    await ReturnTransforms(response);
                }
                else if (path == "www/transform" && request.Method == "POST")//
                {
                    await SaveTransform(response, request);
                }
                else if (path == "www/connection" && request.Method == "DELETE")//
                {
                    var name = await request.ReadFromJsonAsync<int>();
                    webconfig.DeleteScaner(name);
                }
                else if (path == "www/get_scan" && request.Method == "GET")//
                {
                    await ReturnScan(response, returns);
                }
                else if (path == "www/add_connection" && request.Method == "POST")//
                {
                    await SaveScaner(response, request);
                }
            });
            app.Run();
        }


        // получение всех пользователей
        async Task GetAllPeople(HttpResponse response)
        {
            await response.WriteAsJsonAsync(users);
        }
        // получение одного пользователя по id
        async Task GetPerson(string? id, HttpResponse response)
        {
            // получаем пользователя по id
            Person? user = users.FirstOrDefault((u) => u.Id == id);
            // если пользователь найден, отправляем его
            if (user != null)
                await response.WriteAsJsonAsync(user);
            // если не найден, отправляем статусный код и сообщение об ошибке
            else
            {
                response.StatusCode = 404;
                await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
            }
        }
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
            await response.WriteAsJsonAsync(webconfig.roadSettings.lanes);
        }
        async Task SaveLanes(HttpResponse response, HttpRequest request)
        {
            try
            {
                var cnf = await request.ReadFromJsonAsync<Lanes[]>();
                webconfig.roadSettings.lanes = cnf;
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
            await response.WriteAsJsonAsync(webconfig.roadSettings.blinds);
        }
        async Task SaveDeadzones(HttpResponse response, HttpRequest request)
        {
            try
            {
                var cnf = await request.ReadFromJsonAsync<Blinds[]>();
                webconfig.roadSettings.blinds = cnf;
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
        async Task DeletePerson(string? id, HttpResponse response)
        {
            // получаем пользователя по id
            Person? user = users.FirstOrDefault((u) => u.Id == id);
            // если пользователь найден, удаляем его
            if (user != null)
            {
                users.Remove(user);
                await response.WriteAsJsonAsync(user);
            }
            // если не найден, отправляем статусный код и сообщение об ошибке
            else
            {
                response.StatusCode = 404;
                await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
            }
        }

        async Task CreatePerson(HttpResponse response, HttpRequest request)
        {
            try
            {
                // получаем данные пользователя
                var user = await request.ReadFromJsonAsync<Person>();
                if (user != null)
                {
                    // устанавливаем id для нового пользователя
                    user.Id = Guid.NewGuid().ToString();
                    // добавляем пользователя в список
                    users.Add(user);
                    await response.WriteAsJsonAsync(user);
                }
                else
                {
                    throw new Exception("Некорректные данные");
                }
            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
            }
        }

        async Task UpdatePerson(HttpResponse response, HttpRequest request)
        {
            try
            {
                // получаем данные пользователя
                Person? userData = await request.ReadFromJsonAsync<Person>();
                if (userData != null)
                {
                    // получаем пользователя по id
                    var user = users.FirstOrDefault(u => u.Id == userData.Id);
                    // если пользователь найден, изменяем его данные и отправляем обратно клиенту
                    if (user != null)
                    {
                        user.Age = userData.Age;
                        user.Name = userData.Name;
                        await response.WriteAsJsonAsync(user);
                    }
                    else
                    {
                        response.StatusCode = 404;
                        await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
                    }
                }
                else
                {
                    throw new Exception("Некорректные данные");
                }
            }
            catch (Exception)
            {
                response.StatusCode = 400;
                await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
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
        public class Person
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public int Age { get; set; }
        }
    }
}