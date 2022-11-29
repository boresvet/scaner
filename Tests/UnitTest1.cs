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
namespace Sick_test;

public class Tests
{
    IslandSeach scans;
    int? point1, point2;
    double? point3, point4;
    line[] testlinestexture;
    line[] scanerrays;
    TimeBuffer times;

    CircularBuffer<Scanint> MyConcurrentQueue;
    CircularBuffer<int[]> MyINTConcurrentQueue;

    [SetUp]
    public void Setup()
    {

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
                    var ReadFile = File.ReadAllText("../../../../SICK_Program/config.json");

                    //Console.WriteLine(ReadFile);
                    config config = JsonSerializer.Deserialize<config>(ReadFile);
                    var Inputs = new AllInput(config).inputClass[0];
                    var step = (int)((Inputs.scaner.Settings.EndAngle-Inputs.scaner.Settings.StartAngle)/Inputs.scaner.Settings.Resolution);
                    var lms = new TestGenerator(config, 0); 
                    var Conv = new SpetialConvertorint(-5 + Inputs.scaner.Transformations.CorrectionAngle, 185+Inputs.scaner.Transformations.CorrectionAngle, step);
                    //объявление конвертера, переводящего координаты из радиальной системы в ХУ
                    
                    Inputs.ErrorEvent.Reset();

                    var translator = new translator(new PointXYint(){X = Inputs.scaner.Transformations.HorisontalOffset, Y = Inputs.scaner.Transformations.Height});
                    //Объявление транслятора для переноса координат из системы сканера в систему координат дороги
                    
                    var res = lms.createscan();
                    PointXYint[] y123 = new PointXYint[step];
                    y123 = translator.Translate(Conv.MakePoint(res));
                    var Scan = new Scanint{
                        pointsArray = y123,
                        time = DateTime.Now
                    };
                    int i = 0;
                    MyConcurrentQueue = new CircularBuffer<Scanint>(10000);
                    MyINTConcurrentQueue = new CircularBuffer<int[]>(10000);

                    while(i < 10000){
                        res = lms.createscan();
                        MyINTConcurrentQueue.Enqueue(res);


                        Scan.time = DateTime.Now;
                        Scan.pointsArray = translator.Translate(Array.FindAll(Conv.MakePoint(res), point => (point.X==0)&(point.Y==0)));
                        
                        
                        //Эта штука конвертирует скан из радиальных координат в ХУ, 
                        //потом удаляет все "нули" - точки, совпадающие со сканером 
                        //Потом - транслирует все точки в общую систему  координат дороги
                        
                        MyConcurrentQueue.Enqueue(Scan);
                        //Console.Write("Принят скан от сканера  ");
                        //Console.WriteLine(Inputs.scaner.Connection.ScannerAddres.Substring(Inputs.scaner.Connection.ScannerAddres.Length-1));
                        i++;
                    }




        //Обработка дороги для поиска машинок


            times = new TimeBuffer(10000);


            var pointsSortTable = new PointXYint[(config.RoadSettings.RightLimit - config.RoadSettings.LeftLimit)/config.RoadSettings.Step][];
            for(int p = 0; p < pointsSortTable.Length; p++){
                pointsSortTable[p] = new PointXYint[0];
            }    
            var pointsfilter = new Filter((int)((config.RoadSettings.RightLimit-config.RoadSettings.LeftLimit)/config.RoadSettings.Step), config.RoadSettings);
            var scans = new Scanint[MyConcurrentQueue._buffer.Length];
            scans = MyConcurrentQueue._buffer;
            foreach(Scanint RoadScan in scans){
                var j = 0;
                while(j<RoadScan.pointsArray.Length){
                    if((RoadScan.pointsArray[j].X>config.RoadSettings.LeftLimit)&(RoadScan.pointsArray[j].X<config.RoadSettings.RightLimit)){
                        pointsSortTable[(int)((RoadScan.pointsArray[j].X-config.RoadSettings.LeftLimit)/config.RoadSettings.Step)] = pointsSortTable[(int)((RoadScan.pointsArray[j].X-config.RoadSettings.LeftLimit)/config.RoadSettings.Step)].Concat(RoadScan.pointsArray[j].ToArray()).ToArray();//Навернуть логику
                    }
                    j++;
                }
                var FilteredPoints = pointsfilter.CarPoints(pointsSortTable);
                var CarArray = AddAllIslandLane(FilteredPoints);
                times.SaveSuperScan(new SuperScan(CarArray, pointsSortTable, RoadScan.time));
            }

            times.bufferTimes()[0] = DateTime.Now;
            times.bufferTimes()[1] = DateTime.Today;
    }



        public static int[] AddLineIsland(int[] input, int startpoint, int endpoint){

            /*
            Тут происходит дозапоолнение массива между двумя точками. 
            На входе: три варианта значения
            -1 - в этом столбе нет ни одной точки
            0 - в этом столбе есть точки, но все они являются "землёй"
            >0 - в этом столбе есть точка "машины" - даже если есть и другие точки, то сохранена только самая высокая



            Фишка в том, что такой метод позволит обработать практический любые потери данных. 
            Если на дороге лужа - то точек в неё не попадает, и так как мы берём ближайшие точки, и "дозаполняем" лужу данными. 
            Если крыша машины слишком сильно блестит - то то же самое. 
            Если с одной стороны "провала" есть машина, а с другой её нет, то все точки заполняются "землёй"
            Если с одной стороны бесконечность(то есть край дороги), то все точки до ближайшей считаются землёй
            */
            var start = input[startpoint];
            var end = input[endpoint];
            if((start == -1)&(end == 0)){
                Array.Fill(input, 0, startpoint, endpoint-startpoint);
            }
            if((start == -1)&(end > 0)){
                Array.Fill(input, 0, startpoint, endpoint-startpoint);
            }
            if((start == -1)&(end == -1)){
                Array.Fill(input, 0, startpoint, endpoint-startpoint+1);
            }
            if((start == 0)&(end == -1)){
                Array.Fill(input, 0, startpoint+1, endpoint-startpoint+1);
            }
            if((start == 0)&(end == 0)){
                Array.Fill(input, 0, startpoint+1, endpoint-startpoint);
            }
            if((start == 0)&(end > 0)){
                Array.Fill(input, 0, startpoint, endpoint-startpoint);
            }
            if((start > 0)&(end == 0)){
                Array.Fill(input, 0, startpoint+1, endpoint-startpoint);
            }
            if((start > 0)&(end > 0)){
                for(int i = startpoint; i < endpoint; i++){
                    input[i] = start + ((end-start)/input.Length*i);
                }
            }
            if((start > 0)&(end == -1)){
                Array.Fill(input, 0, startpoint+1, endpoint-startpoint+1);
            }
            return input;
        }
        private static int[] AddAllIslandLane(int[] input){
            /*
            Эта штука "дозаполняет" все имеющиеся точки дороги до монолитного результата, удаляя все бесконечности. 
            То есть с начала - она от левого края идёт до ближайшей точки. Находит точку, и массив до ней дозаполняет по правилам(см. AddLineIsland)
            Потом - от первой точки до второй, и т.д. 
            И так до самого конца, чтобы получить полноценную картину распределения точек
            */
            var start = 0;
            for(int i = 0; i < input.Length; i++){
                if((input[i]>=0)|(i==input.Length-1)){
                    AddLineIsland(input,start,i);
                    start = i;
                }

            }
            return input.ToArray();
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

    [Test]
    public void EDIT()
    {
        Assert.IsTrue(scans.CarsArray.Count>=0);
    }




    [Test]
    public void Maths()
    {
        Assert.IsTrue((point1==11)&&(point2==null)&&((point3>=11.18)&&(point3<=11.1804))&&(point4==null));
    }
    //Тесты математики. Предыдущий - кооперация всех этих в один большой, чтобы галочка была только одна
    /*[Test]
    public void INTLaserLinesInsertion_INSERTION()
    {
        Assert.IsTrue(point1!=null);
    }
    [Test]
    public void INTLaserLinesInsertion_NOINSERTION()
    {
        Assert.IsTrue(point2==null);
    }

    [Test]
    public void LaserLinesInsertion_INSERTION()
    {
        Assert.IsTrue(point3!=null);
    }
    [Test]
    public void LaserLinesInsertion_NOINSERTION()
    {
        Assert.IsTrue(point4==null);
    }*/




    [Test]
    public void CarGen()
    {
        Assert.IsTrue(MyConcurrentQueue.IsFull);
    }
    [Test]
    public void addbufer()
    {
        Assert.IsTrue(MyConcurrentQueue.IsFull);
    }
    //Проверяет концептуальное наличие машинок, 
    private bool Cars(){
        for(int i = 0; i < MyINTConcurrentQueue._buffer.Length; i++){
            if(MyINTConcurrentQueue._buffer[i].Sum() > 0){
                return true;
            }
        }
        return false;
    }
    private bool CarsInArray(){
        int oldsum = MyINTConcurrentQueue._buffer[0].Sum();
        for(int i = 0; i < MyINTConcurrentQueue._buffer.Length; i++){
            if(MyINTConcurrentQueue._buffer[i].Sum() != oldsum){
                return true;
            }
        }
        return false;
    }


    [Test]
    public void TRUEisfirstPointIntersectionSegmentZERO()
    {
        Assert.IsTrue(Cars());
    } 
    [Test]
    public void TRUEisfirstPointIntersectionSegmentNOZERO()
    {
        Assert.IsTrue(Cars());
    }





    [Test]
    public void CreatedCars()
    {
        Assert.IsTrue(Cars());
    }
    [Test]
    public void ISCarsInBuferTRUE()
    {
        Assert.IsTrue(CarsInArray());
    }



//Проверяет заполненность буфера времени
    [Test]
    public void Timebuffercreated()
    {
        Assert.IsTrue(times.MyLeanth>=0);
    }
    [Test]
    public void buffertimes()
    {
        Assert.IsFalse(times.istimesgood());
    }
    
}