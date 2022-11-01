namespace Sick_test;

public class Tests
{
    IslandSeach scans;
    [SetUp]
    public void Setup()
    {
        scans = new IslandSeach();
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

}