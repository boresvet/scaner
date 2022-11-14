namespace Sick_test;

public class Tests
{
    IslandSeach scans;
    int? point1, point2;
    double? point3, point4;
    [SetUp]
    public void Setup()
    {
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

}