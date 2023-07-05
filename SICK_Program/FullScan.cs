using Sick_test;

namespace SickScanner
{
    public class FullScan
    {
        public TestGenerator[]? testgen;
        public ScanData[]? Scan { get; set; }

        public FullScan() { }
        public FullScan(int len)
        {
            testgen = new TestGenerator[len];
            Scan = new ScanData[len];
        }
        public FullScan(ResponseFullConfig cfg)
        {
            testgen = cfg.scanners.Select(s => new TestGenerator(cfg, s.id, 2)).ToArray();
            Scan = testgen.Select(s => new ScanData(s.lanes, s.RoadPointWithCars)).ToArray();
        }

        public void AddScan(ResponseFullConfig cfg){
            for(int i = 0; i < testgen.Length; i++){
                testgen[i] = new TestGenerator(cfg, i, 2);
                Scan[i] = new ScanData(i,testgen[i].RoadPointWithCars);
            }
        }
        //public FullScan(int scan_count, int scan_len = 286)
        //{

        //    Scan = Enumerable.Range(0, scan_count)
        //                      .Select(x => new ScanData(x, scan_len, x * scan_len, 4000 + x * 1000))
        //                      .ToArray();
        //}
    }

    public class ScanData
    {
        public int Uid { get; set; }
        public PointXYint[]? Points { get; set; }

        public ScanData() { }
        public ScanData(int id, PointXYint[] points) {
            Uid = id;
            Points = points;
        }

        public ScanData(Scaner scanner)
        {
            Uid = scanner.ID;
            Points = Enumerable.Range(0, 286)
                .Select(x => new PointXYint
                {
                    X = x * 10,
                    Y = scanner.Transformations.Height
                })
                .ToArray();
            Transform(scanner);
        }

        public ScanData(int uid, int len, int offset, int ground)
        {
            Uid = uid;
            Points = Enumerable.Range(0, len)
                               .Select(x => new PointXYint { X = offset + x * 10, Y = ground })
                               .ToArray();

        }

        public void Transform(Scaner scanner)
        {
            double sin, cos;
            (sin, cos) = Math.SinCos(Math.PI * scanner.Transformations.CorrectionAngle / 180.0);
            for (var i = 0; i < Points.Length; i++)
            {
                Points[i].X = (int)(Points[i].X * cos - Points[i].Y * sin);
                Points[i].Y = (int)(Points[i].X * sin + Points[i].Y * cos);
                Points[i].X += scanner.Transformations.HorisontalOffset;
                Points[i].Y -= 6000;
            }
        }

        
    }
}