using System;
namespace CrozzleCodeGen
{
	public class ClusterCreatorUtilities
	{
		public ClusterCreatorUtilities()
		{
		}

        /// <summary>
        /// Given the pattern of combinations we add the variable names that store the words
        /// </summary>
        /// <param name="combinations">A list of parameters that can be both horizontal and vertical</param>
        /// <returns></returns>
        public static (List<string>, List<string>) ConvertToNames(List<string> combinations)
        {
            var updown = new List<string>();
            var leftright = new List<string>();
            foreach (var item in combinations)
            {
                if (item == "Left" || item == "Right" || item == "OuterX" || item == "MiddleX")
                    leftright.Add(item.ToLower() + (leftright.Count + 1));
                else if (item == "Up" || item == "Down" || item == "OuterY" || item == "MiddleY")
                    updown.Add(item.ToLower() + (updown.Count + 1));
            }
            return (leftright, updown);
        }

        /// <summary>
		/// Given the combinations, how many Outer keywords are there
		/// Used to work out bracket counts at the end of the generated procedure
		/// </summary>
		/// <param name="combinations"></param>
		/// <returns></returns>
		public static int CountOuter(List<string> combinations)
        {
            int outerCount = 0;

            foreach (string pattern in combinations)
                if (pattern.StartsWith("Outer"))
                    outerCount += 1;
            return outerCount;
        }

        public static string GetPatternVertical(List<string> updown)
        {
            var result = "";
            foreach (var item in updown)
            {
                if (result != "")
                    result += ", ";

                if (item.StartsWith("up"))
                    result += ".leading";
                else if (item.StartsWith("down"))
                    result += ".trailing";
                else if (item.StartsWith("middle"))
                    result += ".middle";
                else if (item.StartsWith("outer"))
                    result += ".outer";
            }
            return "[" + result + "]";
        }

        /// <summary>
        /// We create the text that makes an array of word orientations in the horizontal dimension
        /// </summary>
        /// <param name="leftright">An array of items that are all in the horizontal</param>
        /// <returns></returns>
        public static string GetPatternHorizontal(List<string> leftright)
        {
            var result = "";
            foreach (var item in leftright)
            {
                if (result != "")
                    result += ", ";

                if (item.StartsWith("left"))
                    result += ".leading";
                else if (item.StartsWith("right"))
                    result += ".trailing";
                else if (item.StartsWith("middle"))
                    result += ".middle";
                else if (item.StartsWith("outer"))
                    result += ".outer";
            }
            return "[" + result + "]";
        }

        /// <summary>
		/// Makes a comma separated list of words
		/// first with the horizontal and then with the vertical words
		/// These words are word position variables like [left1, right2, left3, up1, down2]
		/// </summary>
		/// <param name="leftRight"></param>
		/// <param name="upDown"></param>
		/// <returns></returns>
        public static string WordsToAdd(List<String> leftRight, List<String> upDown)
        {
            string result = "";
            foreach (var item in leftRight)
            {
                if (result != "")
                    result += ", ";
                result += item;
            }
            foreach (var item in upDown)
            {
                if (result != "")
                    result += ", ";
                result += item;
            }
            return "[" + result + "]";
        }

        // We also have to add a bracket for however many outers there are
        public static string GetBrackets(string indent)
        {
            int bracketCount = indent.Length / 4;



            var result = "";
            for (int i = bracketCount - 3; i >= 0; i--)
            {
                var indentType = IndentCalculator.Indent2(i);
                result += indentType + "        }\n";
            }
            return result;
        }

        public static Dictionary<string, int> GetDictionaryThatKeepsTrackOfPosition(List<string> positions, int interlockWidth, int interlockHeight)
        {
            var dictionary = new Dictionary<string, int>();
            foreach (var position in positions)
            {
                if (position.StartsWith("left"))
                    dictionary.Add(position, interlockWidth - 1);
                else if (position.StartsWith("right"))
                    dictionary.Add(position, 0);
                else if (position.StartsWith("middlex"))
                    dictionary.Add(position, 0);
                else if (position.StartsWith("outerx"))
                    dictionary.Add(position, 0);
                else if (position.StartsWith("up"))
                    dictionary.Add(position, interlockHeight - 1);
                else if (position.StartsWith("down"))
                    dictionary.Add(position, 0);
                else if (position.StartsWith("middley"))
                    dictionary.Add(position, 0);
                else if (position.StartsWith("outery"))
                    dictionary.Add(position, 0);
            }
            return dictionary;
        }
    }
}

