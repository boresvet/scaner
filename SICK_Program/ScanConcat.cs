using System;


namespace Sick_test
{
	///<summary>Склеивает массивы со сканеров
    ///</summary>   
    public class ScanConcat{
		private PointXYint[] InterFace;
		private int leanth;
		///<summary> Склеивает массивы со сканеров </summary>
        public ScanConcat(config config){
			int leanth = 0;
			foreach(Scanner scanner in config.Scanners){
				leanth += (int)((float)(scanner.Settings.EndAngle-scanner.Settings.StartAngle)/scanner.Settings.Resolution);
			}
			InterFace = new PointXYint[leanth];
        }
		///<summary> Проверка на то, является ли буфер пустым </summary>

		public bool IsEmpty(){
			return leanth == 0;
		}
		///<summary> Сохранить данные со сканера </summary>
        public void Add(PointXYint[] array){
			Array.Copy(array, 0, InterFace, leanth, array.Length);
			leanth += array.Length;
		}
		///<summary> Получаем данные со сканеров, очищаем внутренний буфер </summary>
		public PointXYint[] Dequeue(){
			var x = new PointXYint[leanth]; 
			Array.Copy(InterFace, 0, x, 0, leanth);
			leanth = 0;
			return x;
		}
        ///<summary>Сортировка массива точек по оси Х, от начала и до конца
        ///<param name = "array">Массив сортируемых точек</param>
        ///</summary>   
    }
}