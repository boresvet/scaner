using Sick_test;

namespace SickScanner
{
    public class WebScans
    {
        public TestGenerator[]? testgen;
        public SpetialConvertorint[]? Conv;
        public translator[]? translator;
        public ScanData[]? Scan { get; set; }

        public WebScans() { }
        public WebScans(int len)
        {
            testgen = new TestGenerator[len];
            Scan = new ScanData[len];
            Conv = new SpetialConvertorint[len];
            translator = new translator[len];
        }
        public WebScans(ResponseFullConfig cfg)
        {
            testgen = cfg.scanners.Select(s => new TestGenerator(cfg, s.id, 2)).ToArray();
            Scan = testgen.Select(s => new ScanData(s.lanes, s.RoadPointWithCars)).ToArray();
            Conv = cfg.scanners.Select(s => new SpetialConvertorint(s.settings.startAngle + s.transformations.correctionAngle, s.settings.endAngle + s.transformations.correctionAngle, Math.Abs((int)((s.settings.endAngle-s.settings.startAngle)/s.settings.resolution)))).ToArray();
            translator = cfg.scanners.Select(s => new translator(new PointXYint(){X = s.transformations.horisontalOffset, Y = s.transformations.height})).ToArray();
        }

        public void AddScan(ResponseFullConfig cfg, bool trig, returns returns, bool Test){

/*          WSocketTime = DateTime.Now;
            if(oldWSocketTime.AddSeconds(10)>WSocketTime){
                for(int i = 0; i < testgen.Length; i++){
                    testgen[i] = new TestGenerator(cfg, i, 2);
                    Scan[i] = new ScanData(i,testgen[i].RoadPointWithCars);
                }
            }else{
                for(int i = 0; i < testgen.Length; i++){
                    testgen[i] = new TestGenerator(cfg, i, 2);
                    Scan[i] = new ScanData(i,testgen[i].RoadPoints);
                }
                oldWSocketTime = DateTime.Now;
            }
*/
            if(Test){
                if(trig){
                    for(int i = 0; i < testgen.Length; i++){
                        testgen[i] = new TestGenerator(cfg, i, 2);
                        Scan[i] = new ScanData(i,testgen[i].RoadPointWithCars);
                    }
                }else{
                    for(int i = 0; i < testgen.Length; i++){
                        testgen[i] = new TestGenerator(cfg, i, 2);
                        Scan[i] = new ScanData(i,testgen[i].RoadPoints);
                    }
                }
            }
            else{
                for(int i = 0; i < cfg.scanners.Length; i++){
                    Conv[i] = new SpetialConvertorint(cfg.scanners[i].settings.startAngle + cfg.scanners[i].transformations.correctionAngle, cfg.scanners[i].settings.endAngle + cfg.scanners[i].transformations.correctionAngle, Math.Abs((int)((cfg.scanners[i].settings.endAngle-cfg.scanners[i].settings.startAngle)/cfg.scanners[i].settings.resolution)));
                    translator[i] = new translator(new PointXYint(){X = cfg.scanners[i].transformations.horisontalOffset, Y = cfg.scanners[i].transformations.height});
                    Scan[i] = new ScanData(i,translator[i].Translate(Conv[i].MakePoint(returns.Inputs.ReadRawScan(i))));
                }
            }
        }
        public ScanData[]? readScanData(){
            return Scan.ToArray();
        }
        //public WebScans(int scan_count, int scan_len = 286)
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