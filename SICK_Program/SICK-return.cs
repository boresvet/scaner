using System;
namespace Sick_test
{
    public class returns
    {
        ///<summary>Интерфейс ко всем сканерам</summary>  
        public AllInput Inputs;
        ///<summary>Хранит все сканы дороги</summary>  
        public TimeBuffer times;
        ///<summary>Хранит все сканы машинки</summary>  
        public CarBuffer carbuffer;
        public returns()
        {
        }
        ///<summary>Инициализирует класс, передавая ссылки на интерфейсы между потоками и буфферы</summary>  
        public void initreturns(AllInput _Inputs, TimeBuffer _times, CarBuffer _carbuffer){
            Inputs = _Inputs;
            times = _times;
            carbuffer = _carbuffer;
        }
        ///<summary>Получить последний скан по номеру сканера</summary>  
        public Scanint returnScan(int number){
            var ret = new Scanint();
            ret = Inputs.ReadLastScan(number).copyScan();
            return ret;
        }
        ///<summary>Получить собранный и обработанный скан дороги</summary>  
        public SuperScan returnRoad(){
            var ret = new SuperScan();
            ret = times.readLastScan().CopyScan();
            return ret;
        }
        ///<summary>Получить машинку по заданному времени</summary>  
        public CarArraySize returnCar(DateTime Time, int roadline){
            var ret = new CarArraySize();
            ret = carbuffer.GiveMyCar(Time, roadline);
            return ret;
        }
    }
}