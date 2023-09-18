namespace SickScanner{
    public class ResponseFullConfig{
        public RoadSettings roadSettings { get; set; }
        public Scanner[] scanners { get; set; }
        public AnalisConfig AnalisSettings { get; set; } //НОВЫЙ КЛАСС

        public SortSettings sorts { get; set; }

        
        public void SaveConfig(ResponseFullConfig webconfig, config config){
            SaveRoadSettings(webconfig, config);
            config.Test = webconfig.AnalisSettings.test;
            config.Method = webconfig.AnalisSettings.mathsmethod;
            SaveSortsettings(webconfig, config);
            SaveScaners(webconfig, config);
        }

        public void ReadConfig(ResponseFullConfig webconfig, config config){
            if (webconfig == null){
                webconfig.AnalisSettings.test = new bool();
                webconfig.AnalisSettings.mathsmethod = new string(config.Method);
                webconfig.roadSettings = new RoadSettings();
                webconfig.sorts = new SortSettings();
                webconfig.scanners = new Scanner[config.Scanners.Length];

                config.RoadSettings.DownLimit = new int();
                config.RoadSettings.LeftLimit = new int();
                config.RoadSettings.RightLimit = new int();
                config.RoadSettings.Step = new int();
                config.RoadSettings.UpLimit = new int();
                config.RoadSettings.Blinds = new Blind[webconfig.roadSettings.blinds.Length];
                for(int i = 0; i < webconfig.roadSettings.blinds.Length; i++){
                    config.RoadSettings.Blinds[i] = new Blind();
                    config.RoadSettings.Blinds[i].ID = new int();
                    config.RoadSettings.Blinds[i].Offset = new int();
                    config.RoadSettings.Blinds[i].Width = new int();
                    config.RoadSettings.Blinds[i].Height = new int();
                }
                config.RoadSettings.Lanes = new Lane[webconfig.roadSettings.lanes.Length];
                for(int i = 0; i < webconfig.roadSettings.lanes.Length; i++){
                    config.RoadSettings.Lanes[i] = new Lane();
                    config.RoadSettings.Lanes[i].ID = new int();
                    config.RoadSettings.Lanes[i].Offset = new int();
                    config.RoadSettings.Lanes[i].Width = new int();
                    config.RoadSettings.Lanes[i].Height = new int();
                }
            }
            ReadRoadSettings(webconfig, config);
            webconfig.AnalisSettings.test = config.Test;
            webconfig.AnalisSettings.mathsmethod = config.Method;
            ReadSortsettings(webconfig, config);
            ReadScaners(webconfig, config);
        }
        public void AddWebConfig(ResponseFullConfig webconfig, config config){
                webconfig.AnalisSettings = new AnalisConfig();
                webconfig.roadSettings = new RoadSettings();
                webconfig.sorts = new SortSettings();
                webconfig.scanners = new Scanner[config.Scanners.Length];

                webconfig.AnalisSettings.test = new bool();
                webconfig.AnalisSettings.mathsmethod = new string(config.Method);

                webconfig.roadSettings.downLimit = new int();
                webconfig.roadSettings.leftLimit = new int();
                webconfig.roadSettings.rightLimit = new int();
                webconfig.roadSettings.step = new int();
                webconfig.roadSettings.upLimit = new int();
                webconfig.roadSettings.blinds = new Blinds[config.RoadSettings.Blinds.Length];
                for(int i = 0; i < config.RoadSettings.Blinds.Length; i++){
                    webconfig.roadSettings.blinds[i] = new Blinds();
                    webconfig.roadSettings.blinds[i].id = new int();
                    webconfig.roadSettings.blinds[i].offset = new int();
                    webconfig.roadSettings.blinds[i].width = new int();
                    webconfig.roadSettings.blinds[i].height = new int();
                }
                webconfig.roadSettings.lanes = new Lanes[config.RoadSettings.Lanes.Length];
                for(int i = 0; i < config.RoadSettings.Lanes.Length; i++){
                    webconfig.roadSettings.lanes[i] = new Lanes();
                    webconfig.roadSettings.lanes[i].id = new int();
                    webconfig.roadSettings.lanes[i].offset = new int();
                    webconfig.roadSettings.lanes[i].width = new int();
                    webconfig.roadSettings.lanes[i].height = new int();
                }

            webconfig.sorts.Buffers = new int();
            webconfig.sorts.BufferTimesLength = new int();
            webconfig.sorts.MinLength = new int();
            webconfig.sorts.MinWigdh = new int();
            webconfig.sorts.SavedCars = new int();
                
            for(int i = 0; i < webconfig.scanners.Length; i++){
                webconfig.scanners[i] = new Scanner();
                webconfig.scanners[i].id = new int();
                webconfig.scanners[i].connection = new Connection();
                webconfig.scanners[i].connection.address = new string(config.Scanners[i].Connection.ScannerAddres);
                webconfig.scanners[i].connection.port = new int();
                webconfig.scanners[i].settings = new Settings();
                webconfig.scanners[i].settings.endAngle = new int();
                webconfig.scanners[i].settings.startAngle = new int();
                webconfig.scanners[i].settings.resolution = new double();
                webconfig.scanners[i].settings.frequency = new int();
                webconfig.scanners[i].transformations = new Transformation();
                webconfig.scanners[i].transformations.correctionAngle = new int();
                webconfig.scanners[i].transformations.height = new int();
                webconfig.scanners[i].transformations.horisontalOffset = new int();
            }  
            ReadScaners(webconfig, config);
            ReadRoadSettings(webconfig, config);
            webconfig.AnalisSettings.test = config.Test;
            webconfig.AnalisSettings.mathsmethod = config.Method;
            ReadSortsettings(webconfig, config);
        }
        private void SaveRoadSettings(ResponseFullConfig webconfig, config config){
            config.RoadSettings.DownLimit = webconfig.roadSettings.downLimit;
            config.RoadSettings.LeftLimit = webconfig.roadSettings.leftLimit;
            config.RoadSettings.RightLimit = webconfig.roadSettings.rightLimit;
            config.RoadSettings.Step = webconfig.roadSettings.step;
            config.RoadSettings.UpLimit = webconfig.roadSettings.upLimit;
            config.RoadSettings.Blinds = new Blind[webconfig.roadSettings.blinds.Length];
            for(int i = 0; i < webconfig.roadSettings.blinds.Length; i++){
                config.RoadSettings.Blinds[i] = SaveBlind(webconfig.roadSettings.blinds[i]);
            }
            config.RoadSettings.Lanes = new Lane[webconfig.roadSettings.lanes.Length];
            for(int i = 0; i < webconfig.roadSettings.lanes.Length; i++){
                config.RoadSettings.Lanes[i] = SaveLane(webconfig.roadSettings.lanes[i]);
            }
        }
        private void ReadRoadSettings(ResponseFullConfig webconfig, config config){
            webconfig.roadSettings.downLimit = config.RoadSettings.DownLimit;
            webconfig.roadSettings.leftLimit = config.RoadSettings.LeftLimit;
            webconfig.roadSettings.rightLimit = config.RoadSettings.RightLimit;
            webconfig.roadSettings.step = config.RoadSettings.Step;
            webconfig.roadSettings.upLimit = config.RoadSettings.UpLimit;
            for(int i = 0; i < webconfig.roadSettings.blinds.Length; i++){
                webconfig.roadSettings.blinds[i] = ReadBlind(config.RoadSettings.Blinds[i]);
            }
            for(int i = 0; i < webconfig.roadSettings.lanes.Length; i++){
                webconfig.roadSettings.lanes[i] = ReadLane(config.RoadSettings.Lanes[i]);
            }
        }
        private Blind SaveBlind(Blinds input){
            var ret = new Blind();
            ret.ID = input.id;
            ret.Offset = input.offset;
            ret.Width = input.width;
            ret.Height = input.height;
            return ret;
        }
        private Lane SaveLane(Lanes input){
            var ret = new Lane();
            ret.ID = input.id;
            ret.Offset = input.offset;
            ret.Width = input.width;
            ret.Height = input.height;
            return ret;
        }
        private Blinds ReadBlind(Blind input){
            var ret = new Blinds();
            ret.id = input.ID;
            ret.offset = input.Offset;
            ret.width = input.Width;
            ret.height = input.Height;
            return ret;
        }
        private Lanes ReadLane(Lane input){
            var ret = new Lanes();
            ret.id = input.ID;
            ret.offset = input.Offset;
            ret.width = input.Width;
            ret.height = input.Height;
            return ret;
        }
        private void SaveSortsettings(ResponseFullConfig webconfig, config config){
            config.SortSettings.Buffers = webconfig.sorts.Buffers;
            config.SortSettings.BufferTimesLength = webconfig.sorts.BufferTimesLength;
            config.SortSettings.MinLength = webconfig.sorts.MinLength;
            config.SortSettings.MinWigdh = webconfig.sorts.MinWigdh;
            config.SortSettings.SavedCars = webconfig.sorts.SavedCars;
        }
        private void ReadSortsettings(ResponseFullConfig webconfig, config config){
            webconfig.sorts.Buffers = config.SortSettings.Buffers;
            webconfig.sorts.BufferTimesLength = config.SortSettings.BufferTimesLength;
            webconfig.sorts.MinLength = config.SortSettings.MinLength;
            webconfig.sorts.MinWigdh = config.SortSettings.MinWigdh;
            webconfig.sorts.SavedCars = config.SortSettings.SavedCars;
        }
        private void SaveScaners(ResponseFullConfig webconfig, config config){
            config.Scanners = new Scaner[webconfig.scanners.Length];
            for(int i = 0; i < webconfig.scanners.Length; i++){
                config.Scanners[i] = SaveScaner(webconfig.scanners[i]);
            }            
        }
        private Scaner SaveScaner(Scanner webconfig){
            var ret = new Scaner();
            ret.ID = webconfig.id;
            ret.Connection = new global::Connection();
            ret.Connection.ScannerAddres = webconfig.connection.address;
            ret.Connection.ScannerPort = webconfig.connection.port;
            ret.Settings = new global::Settings();
            ret.Settings.EndAngle = webconfig.settings.endAngle;
            ret.Settings.StartAngle = webconfig.settings.startAngle;
            ret.Settings.Resolution = webconfig.settings.resolution;
            ret.Settings.Frequency = webconfig.settings.frequency;
            ret.Transformations = new global::Transformations();
            ret.Transformations.CorrectionAngle = webconfig.transformations.correctionAngle;
            ret.Transformations.Height = webconfig.transformations.height;
            ret.Transformations.HorisontalOffset = webconfig.transformations.horisontalOffset;
            return ret;
        }
        private void ReadScaners(ResponseFullConfig webconfig, config config){
            for(int i = 0; i < webconfig.scanners.Length; i++){
                webconfig.scanners[i] = ReadScaner(config.Scanners[i], webconfig.scanners[i]);
            }            
        }
        private Scanner ReadScaner(Scaner webconfig, Scanner oldscaner){
            var ret = new Scanner();
            ret.id = webconfig.ID;
            ret.connection = new Connection();
            ret.connection.address = webconfig.Connection.ScannerAddres;
            ret.connection.port = webconfig.Connection.ScannerPort;
            ret.settings = new Settings();
            ret.settings.endAngle = webconfig.Settings.EndAngle;
            ret.settings.startAngle = webconfig.Settings.StartAngle;
            ret.settings.resolution = webconfig.Settings.Resolution;
            ret.settings.frequency = webconfig.Settings.Frequency;
            ret.transformations = new Transformation();
            ret.transformations.correctionAngle = webconfig.Transformations.CorrectionAngle;
            ret.transformations.height = webconfig.Transformations.Height;
            ret.transformations.horisontalOffset = webconfig.Transformations.HorisontalOffset;
            ret.transformations.uid = webconfig.ID;
            return ret;
        }

        public bool DeleteScaner(int id){
            for(int i = 0; i < scanners.Length; i++){
                if(scanners[i].id == id){
                    var scans = new Scanner[scanners.Length-1];
                    for(int j = 0; j < scanners.Length; j++){
                        if(i<j){
                            scans[j] = scanners[j];
                        }
                        if(i>j){
                            scans[j-1] = scanners[j];
                        }
                    }
                    scanners = scans;
                    return true;
                }
            }
            return false;
        }
    }

    public class Scanner{
        public int id { get; set; }
        public Settings settings { get; set; }
        public Connection connection { get; set; }
        public Transformation transformations { get; set; }
    }


    public class Settings {
        public int endAngle { get; set; }
        public int frequency { get; set; }
        public double resolution { get; set; }
        public int startAngle { get; set; }
    }

    public class Connection {
        public bool enabled { get; set; }
        public string address { get; set; }
        public int port { get; set; }
        public string uid { get; set; }
    }


    public class Transformation {
        public int uid { get; set; }
        public int horisontalOffset { get; set; }
        public int height { get; set; }
        public float correctionAngle { get; set; }
        public bool Flip { get; set; }

        public Transformation() { }

        public Transformation(Scanner scanner)
        {
            uid = scanner.id;
            height = scanner.transformations.height;
            horisontalOffset = scanner.transformations.horisontalOffset;
            correctionAngle = scanner.transformations.correctionAngle;
            Flip = scanner.transformations.Flip;
        }
    }


    public class  RoadSettings {
        public int downLimit { get; set; }
        public int leftLimit { get; set; }
        public int rightLimit { get; set; }
        public int step { get; set; }
        public int upLimit { get; set; }
        public Blinds[] blinds { get; set; }
        public Lanes[] lanes { get; set; }
    }

    public class Blinds{
        ///<summary>Номер препятствия</summary>
        public int id { get; set; }
        ///<summary>Х координата начала препятствия</summary>
        public int height { get; set; }
        ///<summary>Ширина полосы</summary>
        public int offset { get; set; }
        ///<summary>Высота полосы</summary>
        public int width { get; set; }
    }

    public class Lanes{
        public int id { get; set; }
        public int offset { get; set; }
        public int width { get; set; }
        public int height { get; set; }

    }
    public class SettingsInt {
        public string uid { get; set; }
        public int height { get; set; }
        public int horisontalOffset { get; set; }
        public int correctionAngle { get; set; }
    }


    public class ConnectionSettingsObj {
        public ConnectionSettings road  { get; set; }
    }

    public class ConnectionSettings {
        public FieldsInt correctionAngle  { get; set; }
        public FieldsInt  height  { get; set; }
        public FieldsInt horisontalOffset  { get; set; }
    }

    public class MinMax {
        public int min { get; set; }
        public int max { get; set; }
    }

    public class FieldsInt {
        public int min { get; set; }
        public int max { get; set; }
        public MinMax step { get; set; } 

    }

    public class RoadPageLimit {
        BlindLimit blind { get; set; }
        LaneLimit lane { get; set; }
    }

    public class BlindLimit {
        public FieldsInt height { get; set; }
        public FieldsInt offset { get; set; }
        public FieldsInt width { get; set; }
    }

    public class LaneLimit {
        FieldsInt offset { get; set; }
        FieldsInt width { get; set; }
    }























//Добавить в веб морду:
    public class AnalisConfig{
        public bool test { get; set; } 
        public string mathsmethod { get; set; } //ОПИСАНИЕ         
    }
    
    public class SortSettings{
        ///<summary>Минимальная ширина машины</summary>
        public int MinWigdh { get; set; }
        ///<summary>Минимальная длинна машины</summary>
        public int MinLength { get; set; }
        ///<summary>Количество буферов, отдаваемых алгоритму поиска машин</summary>
        public int Buffers { get; set; }
        ///<summary>Количество буферов, отдаваемых алгоритму поиска машин</summary>
        public int BufferTimesLength { get; set; }
        ///<summary>Количество машин, сохраняемых в буффере </summary>
        public int SavedCars { get; set; }
    }

    public class AlgoritmSelectorType
    {
        public string? Algoritm { get; set; }
    }
}