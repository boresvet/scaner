
    ///<summary>Информация о полосе(номер, положение в пр-ве и ширина)
    ///</summary>
    public class Lane{
        ///<summary>Номер полосы</summary>
        public int ID { get; set; }
        ///<summary>Х координата начала полосы</summary>
        public int Offset { get; set; }
        ///<summary>Ширина полосы</summary>
        public int Width { get; set; }
        public int Height { get; set; }
    }
    ///<summary>Информация о неровности(отбойник, бетонное заграждение, прочие препятствия и мусор, не являющиеся машинами)
    ///</summary>
    public class Blind{
        ///<summary>Номер препятствия</summary>
        public int ID { get; set; }
        ///<summary>Х координата начала препятствия</summary>
        public int Offset { get; set; }
        ///<summary>Ширина полосы</summary>
        public int Width { get; set; }
        ///<summary>Высота полосы</summary>
        public int Height { get; set; }
    }
    ///<summary>Информация о всех сканерфх, дороге, полосах и различных неровностях
    ///</summary>
    public class config{
        ///<summary> Вкл/Выкл тестовый режим(режим генерации псевдоданных со сканеров по введённой дороге)</summary>
        public bool Test { get; set; }
        ///<summary> Всё, что ниже - не машина </summary>

        public int FilteredHeight { get; set; } 
        ///<summary> Глобальный метод поиска машинок(обработки границ и т.д.)</summary>
        public string Method { get; set; }
        ///<summary> Задаёт минимальные размеры машинок (необходимо для фильтрации птичек/веточек/идиотов с дроном и прочих неприятностей)</summary>
        public SortSettings SortSettings { get; set; }
        ///<summary>позволяет описать все особенности дороги</summary>
        public RoadSetting RoadSettings { get; set; }
        ///<summary>описывает все сканеры(их настройки)</summary>
        public Scaner[] Scanners { get; set; }
    }
    ///<summary>Информация о всей дороге, полосах и различных неровностях. 
    ///</summary>
    public class RoadSetting{
        ///<summary>Верхняя граница дороги</summary>
        public int UpLimit { get; set; }
        ///<summary>нижняя граница(самая нижняя точка)</summary>
        public int DownLimit { get; set; }
        ///<summary>левая граница дороги</summary>
        public int LeftLimit { get; set; }
        ///<summary>Правая граница дороги</summary>
        public int RightLimit { get; set; }
        ///<summary>ширина шага определения габаритов машины</summary>
        public int Step { get; set; }
        ///<summary>все полосы дороги</summary>
        public Lane[] Lanes { get; set; }
        ///<summary>все препятствия/неровности</summary>
        public Blind[] Blinds { get; set; }

    }
    ///<summary>Информация о сканере. 
    ///</summary>
    public class Scaner{
        ///<summary>номер сканера</summary>
        public int ID { get; set; }
        ///<summary>сетевая информация о сканере</summary>
        public Connection Connection { get; set; }
        ///<summary>настройки сканирования(частота, начальный/конечный угол, шаг)</summary>
        public Settings Settings { get; set; }
        ///<summary>местоположение сканера относительно дороги</summary>
        public Transformations Transformations { get; set; }
    }
    ///<summary>Сетевая информация о сканере. 
    ///</summary>
    public class Connection{
        ///<summary>IP адрес</summary>
        public string ScannerAddres { get; set; }
        ///<summary>номер порта</summary>
        public int ScannerPort { get; set; }
    }
    ///<summary>настройки сканирования(частота, начальный/конечный угол, шаг) 
    ///</summary>
    public class Settings{
        ///<summary>частота сканирования</summary>
        public int Frequency { get; set; }
        ///<summary>начальный угол</summary>
        public int StartAngle { get; set; }
        ///<summary>конечный угол</summary>
        public int EndAngle { get; set; }
        ///<summary>единичный угол отклонения(шаг)</summary>
        public double Resolution { get; set; }
    }
    ///<summary>местоположение сканера относительно дороги
    ///</summary>
    public class Transformations{
        ///<summary>высота</summary>
        public int Height { get; set; }
        ///<summary>горизонтальная координата</summary>
        public int HorisontalOffset { get; set; }
        ///<summary>угол отклонения от горизонтали</summary>
        public float CorrectionAngle { get; set; }
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