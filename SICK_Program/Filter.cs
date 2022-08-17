using System;
using System.Collections.Generic;

namespace Sick_test
{
    public class Filter{
        private int roadLenght;
        private int[] filterMax;
        private int[] filterMin;
        private RoadSetting Settings;
        public Filter(int roadLenght, RoadSetting settings)
		{
            var RoadLenght = roadLenght;
            var Settings = settings;
            var filterMax = new int[RoadLenght];
            for(int i = 0; i<RoadLenght; i++){
                filterMax[i] = settings.UpLimit;
                filterMin[i] = minFilter(i);
            }
		}
        private int minFilter(int indexofrange){
            foreach(Blind j in Settings.Blinds){
                if(((Settings.LeftLimit+(((Settings.RightLimit-Settings.LeftLimit)/roadLenght)*indexofrange))>j.Offset)&((Settings.LeftLimit+(((Settings.RightLimit-Settings.LeftLimit)/roadLenght)*indexofrange))<(j.Offset+j.Width))){
                    return j.Height;
                }
            }
            return Settings.DownLimit;
        }

		public void FilterPoints(PointXYint[][] array){
			for(int i = 0; i<array.Length; i++){
                array[i] = Array.FindAll(array[i], point => (point.Y<filterMax[i])&(point.Y>filterMin[i]));
            }
		}
    }
}