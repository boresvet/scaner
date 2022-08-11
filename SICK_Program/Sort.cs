namespace Sick_test
{
    public class Sorts{

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

		public static void HoareSort(PointXYint[] array)
		{
			HoareSort(array, 0, array.Length - 1);
		}

		/*public static void Main()
		{
			int [] array = {3,2,5,7,8,1,9 };
			HoareSort(array);
			foreach (var e in array)
				Console.WriteLine(e);
			Console.ReadKey();
		}*/
    }
}