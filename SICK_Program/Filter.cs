using System;
using System.Collections.Generic;

namespace Sick_test
{
    public class Filter{
        private int RoadLenght;
        private int[] filterMax;
        private int[] filterMin;
        private RoadSetting Settings;
        public Filter(int roadLenght, RoadSetting settings)
		{
            RoadLenght = roadLenght;
            Settings = settings;
            filterMax = new int[RoadLenght];
            filterMin = new int[RoadLenght];
            for(int i = 0; i<RoadLenght; i++){
                filterMax[i] = settings.UpLimit;
                filterMin[i] = minFilter(i);
            }
		}
        private int minFilter(int indexofrange){
            foreach(Blind j in Settings.Blinds){
                if(((Settings.LeftLimit+(((Settings.RightLimit-Settings.LeftLimit)/RoadLenght)*indexofrange))>j.Offset)&((Settings.LeftLimit+(((Settings.RightLimit-Settings.LeftLimit)/RoadLenght)*indexofrange))<(j.Offset+j.Width))){
                    return j.Height;
                }
            }
            return Settings.DownLimit;
        }

		public PointXYint[][] FilterPoints(PointXYint[][] array){
            var retArray = new PointXYint[array.Length][];
			for(int i = 0; i<array.Length; i++){
                retArray[i] = Array.FindAll(array[i], point => (point.Y<filterMax[i])&(point.Y>filterMin[i]));
            }
            return retArray;
		}
        public int[] CarPoints(PointXYint[][] array){
            var retArray = new int[array.Length];
			for(int i = 0; i<array.Length; i++){
                if(array[i].Length == 0){
                    retArray[i] = -1;
                }else{
                    if(Array.FindAll(array[i], point => (point.Y<filterMax[i])&(point.Y>filterMin[i])).Length == 0){
                        retArray[i] = 0;
                    }else{
                        retArray[i] = maxPoint(Array.FindAll(array[i], point => (point.Y<filterMax[i])&(point.Y>filterMin[i])));
                    }
                }
            }
            return retArray;
        }
        private int maxPoint(PointXYint[] array){
            var retint = 0;
            foreach(PointXYint j in array){
                if(j.Y > retint){
                    retint = j.Y;
                }
            }
            return retint;
        }
    }
}