namespace Sick_test
{
	///<summary>Объявляет скан с массивом указанной длинны
    ///</summary>   
    public class Sorts{
        ///<summary>Сортировка массива точек по оси Х, от начальной и до конечной точки
        ///<param name = "array">Массив сортируемых точек</param>
        ///<param name = "start">Начальный индекс</param>
        ///<param name = "end">Конечный индекс</param>
        ///</summary>   
        public static void HoareSort(PointXYint[] array, int start, int end)
		{
			if (end == start) return;
			var pivot = array[end];
			var storeIndex = start;
			for (int i = start; i <= end - 1; i++)
				if (array[i].X <= pivot.X)
				{
					var t = array[i];
					array[i] = array[storeIndex];
					array[storeIndex] = t;
					storeIndex++;
				}

			var n = array[storeIndex];
			array[storeIndex] = array[end];
			array[end] = n;
			if (storeIndex > start) HoareSort(array, start, storeIndex - 1);
			if (storeIndex < end) HoareSort(array, storeIndex + 1, end);
		}
        ///<summary>Сортировка массива точек по оси Х, от начала и до конца
        ///<param name = "array">Массив сортируемых точек</param>
        ///</summary>   
		public static void HoareSort(PointXYint[] array)
		{
			if (array.Length == 0){
				return;
			}
			HoareSort(array, 0, array.Length - 1);
		}
    }
}