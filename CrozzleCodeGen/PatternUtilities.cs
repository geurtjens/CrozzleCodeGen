using System;
namespace CrozzleCodeGen
{
	public class PatternUtilities
	{
		public PatternUtilities()
		{
		}

        public static string ConcatinateWords(List<string> combinations)
        {
            var result = "";
            foreach (var combination in combinations)
            {
                result += combination;
            }
            return result;
        }

        public static string ConcatinateList(List<string> combinations)
        {
            var horizontal = new List<string>();
            var vertical = new List<string>();

            foreach (var combination in combinations)
            {
                if (IsUpDown(combination))
                    vertical.Add(combination);
                else
                    horizontal.Add(combination);
            }



            var result = "";
            foreach (var combination in horizontal)
            {
                result += combination[0];
            }
            result += "_";

            foreach (var combination in vertical)
            {
                result += combination[0];
            }

            return result.ToLower();






        }

        // We like to add numbers which is the position of each word in its dimension
        // left1, right2, left3 and up1, down2
        // So we convert the combinations to have the position also, in the variables name
        public static List<string> CalculatePositions(List<string> combinations)
        {
            int leftRight = 1;
            int upDown = 1;
            var result = new List<string>();
            for (int i = 0; i < combinations.Count; i++)
            {
                var combination = combinations[i].ToLower();
                if (combination == "left" || combination == "right" || combination == "middlex" || combination == "outerx")
                {
                    combination += leftRight;
                    leftRight += 1;
                }
                else if (combination == "up" || combination == "down" || combination == "middley" || combination == "outery")
                {
                    combination += upDown;
                    upDown += 1;
                }

                result.Add(combination);
            }
            return result;
        }

        public static bool IsUpDown(string position)
        {
            if (position.ToLower().StartsWith("up") || position.ToLower().StartsWith("down") || position.ToLower().StartsWith("middley") || position.ToLower().StartsWith("outery"))
                return true;
            else
                return false;

        }
        public static int CalculateInterlockHeight(List<string> combinations)
        {
            int result = 0;
            foreach (var item in combinations)
            {
                if (item == "Left" || item == "Right")
                {
                    result += 1;
                }
            }
            return result;
        }
        public static int CalculateInterlockWidth(List<string> combinations)
        {
            int result = 0;
            foreach (var item in combinations)
            {
                if (item == "Up" || item == "Down")
                {
                    result += 1;
                }
            }
            return result;
        }
    }
}

