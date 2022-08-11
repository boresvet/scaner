    public class Lane{
        public int ID { get; set; }
        public int Offset { get; set; }
        public int Width { get; set; }
    }
    
    public class config{
        public RoadSetting RoadSettings { get; set; }
        public Scanner[] Scanners { get; set; }
    }
    public class RoadSetting{
        public int UpLimit { get; set; }
        public int DownLimit { get; set; }
        public int LeftLimit { get; set; }
        public int RightLimit { get; set; }
        public Lane[] Lanes { get; set; }
    }

    public class Scanner{
        public int ID { get; set; }
        public Connection Connection { get; set; }
        public Settings Settings { get; set; }
        public Transformations Transformations { get; set; }
    }
    public class Connection{
        public string ScannerAddres { get; set; }
        public int ScannerPort { get; set; }
    }
    public class Settings{
        public int Frequency { get; set; }
        public int StartAngle { get; set; }
        public int EndAngle { get; set; }
        public double Resolution { get; set; }
    }
    public class Transformations{
        public int Height { get; set; }
        public int HorisontalOffset { get; set; }
        public int CorrectionAngle { get; set; }

    }