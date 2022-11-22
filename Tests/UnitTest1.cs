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
    CircularBuffer<Scanint> MyConcurrentQueue;
    CircularBuffer<int[]> MyINTConcurrentQueue;

    [SetUp]
    public void Setup()
    {

        //Тесты математики(типовое пересечение линий)
        scans = new IslandSeach();
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



        //Тесты логики(генерации машинок)
        //Создание тестовой машинки
                    var ReadFile = File.ReadAllText("../../../../SICK_Program/config.json");

                    //Console.WriteLine(ReadFile);
                    config config = JsonSerializer.Deserialize<config>(ReadFile);
                    var Inputs = new AllInput(config).inputClass[0];
                    var step = (int)((Inputs.scaner.Settings.EndAngle-Inputs.scaner.Settings.StartAngle)/Inputs.scaner.Settings.Resolution);
                    var lms = new TestGenerator(config, 1); 
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
                        /*if (oldscannumber!=res.ScanCounter){ 
                            Console.WriteLine($"{oldscannumber} {res.ScanCounter} {res.ScanFrequency}");
                        }
                        if (res.ScanCounter == null){
                            oldscannumber++;
                        }else{
                            oldscannumber = (int)res.ScanCounter+1;
                        }*/
                        /*if(oldscannumber%1000 == 0){
                            Console.WriteLine("Тысячный скан");
                        }*/


                        Scan.time = DateTime.Now;
                        Scan.pointsArray = translator.Translate(Array.FindAll(Conv.MakePoint(res), point => (point.X==0)&(point.Y==0)));
                        /*
                        Эта штука конвертирует скан из радиальных координат в ХУ, 
                        потом удаляет все "нули" - точки, совпадающие со сканером 
                        Потом - транслирует все точки в общую систему  координат дороги
                        */
                        MyConcurrentQueue.Enqueue(Scan);
                        //Console.Write("Принят скан от сканера  ");
                        //Console.WriteLine(Inputs.scaner.Connection.ScannerAddres.Substring(Inputs.scaner.Connection.ScannerAddres.Length-1));
                        i++;
                    }
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
    }
    //[Test]
    /*public void CarGen()
    {
        Assert.IsTrue(scans.CarsArray.Count>=0);
    }*/
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
    public void CreatedCars()
    {
        Assert.IsTrue(Cars());
    } 
    [Test]
    public void ISCarsInBuferTRUE()
    {
        Assert.IsTrue(CarsInArray());
    } 
}