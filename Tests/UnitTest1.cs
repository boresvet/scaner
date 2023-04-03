namespace Sick_test
{

    public class Tests
{
    int[] CarArray;
    config config;
    IslandSeach scans;
    int? point1, point2;
    double? point3, point4;
    line[] testlinestexture;
    line[] scanerrays;
    TimeBuffer times;
    TimeBuffer times1;
    bool reallines;
    CircularBuffer<Scanint> MyConcurrentQueue;
    CircularBuffer<Scanint> MyConcurrentQueue1;
    //CircularBuffer<int[]> MyINTConcurrentQueue;


    [SetUp]
    public void Setup()
    {

        var laserray = new line();
        laserray.createLine(new PointXYint(){X = 5500, Y = 4500}, new PointXYint(){X = 4839, Y = 3749});
        var cartexture = new line();
        cartexture.createLine(new PointXYint(){X = 2050, Y = 1600}, new PointXYint(){X = 2150, Y = 100});
        var roadtexture = new line();
        roadtexture.createLine(new PointXYint(){X = -100, Y = 100}, new PointXYint(){X = 2400, Y = 100});
        var scannerpoint = new PointXYint(){X = 5500, Y = 4500}; 
        var tyu = laserray.firstPointIntersectionSegment(scannerpoint, laserray, new line[]{cartexture, roadtexture});
        reallines = (tyu.Y == 652); 


        //Тесты математики(типовое пересечение линий)
        var line1 = new line();
        line1.createLine(new PointXYint(){X = 0, Y = 10}, new PointXYint(){X = 10, Y = 10});
        var line2 = new line();
        line2.createLine(new PointXY(){X = 0, Y = 10}, new PointXY(){X = 10, Y = 10});
        var ray1 = new line();
        ray1.createLine(new PointXYint(){X = 0, Y = 0}, new PointXYint(){X = 10, Y = 20});
        point1 = line1.DistancetopointSegment(new PointXYint(){X = 0, Y = 0}, ray1, line1);
        var ray2 = new line();
        ray2.createLine(new PointXYint(){X = 0, Y = 0}, new PointXYint(){X = 20, Y = 10});
        point2 = line1.DistancetopointSegment(new PointXYint(){X = 0, Y = 0}, ray2, line1);
        var ray3 = new line();
        ray3.createLine(new PointXY(){X = 0, Y = 0}, new PointXY(){X = 10, Y = 20});
        point3 = line2.DistancetopointSegment(new PointXY(){X = 0, Y = 0}, ray3, line2);
        var ray4 = new line();
        ray4.createLine(new PointXY(){X = 0, Y = 0}, new PointXY(){X = 20, Y = 10});
        point4 = line2.DistancetopointSegment(new PointXY(){X = 0.0, Y = 0.0}, ray4, line2);
        //Console.WriteLine(point2);

        //Тест на первое пересечение


        //Тесты логики(генерации машинок)
        //Создание тестовой машинки
                    var ReadFile= File.ReadAllText("../../../../SICK_Program/config.json");

                    //Console.WriteLine(ReadFile);
                    /*config = JsonSerializer.Deserialize<config>(ReadFile);
                    var Inputs = new AllInput(config).inputClass[0];
                    var step = (int)((Inputs.scaner.Settings.EndAngle-Inputs.scaner.Settings.StartAngle)/Inputs.scaner.Settings.Resolution);
                    var lms = new TestGenerator(config, 0, 30); 
                    var lms1 = new TestGenerator(config, 0, 3000); //На маленьком промежутке не создаст машин

                    var Conv = new SpetialConvertorint(-5 + Inputs.scaner.Transformations.CorrectionAngle, 185+Inputs.scaner.Transformations.CorrectionAngle, step);
                    //объявление конвертера, переводящего координаты из радиальной системы в ХУ
                    
                    Inputs.ErrorEvent.Reset();

                    var translator = new translator(new PointXYint(){X = Inputs.scaner.Transformations.HorisontalOffset, Y = Inputs.scaner.Transformations.Height});
                    //Объявление транслятора для переноса координат из системы сканера в систему координат дороги
*/
    }



        



    [Test]
    public void TRUE()
    {
        Assert.Pass();
    }
    [Test]
    public void FALSE()
    {
        Assert.IsTrue(false);
    }

    /*
    В тесте ошибка
    [Test]
    public void EDIT()
    {
        Assert.IsTrue(scans.CarsArray.Count>=0);
    }*/




    [Test]
    public void Maths()
    {
        Assert.IsTrue((point1==11)&&(point2==null)&&((point3>=11.18)&&(point3<=11.1804))&&(point4==null)&&reallines);
    }
    //Тесты математики. Предыдущий - кооперация всех этих в один большой, чтобы галочка была только одна
    
}}

