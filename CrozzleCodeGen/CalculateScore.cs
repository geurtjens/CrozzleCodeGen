using System;
namespace CrozzleCodeGen
{
	public class CalculateScore
	{
		public CalculateScore()
		{
		}

        public static List<string> GetHorizontals(List<string> combinations)
        {
            var result = new List<string>();
            foreach (var combination in combinations)
            {
                if (combination.ToLower().StartsWith("left"))
                    result.Add("left");
                else if (combination.ToLower().StartsWith("right"))
                    result.Add("right");
                else if (combination.ToLower().StartsWith("middlex"))
                    result.Add("middlex");
                else if (combination.ToLower().StartsWith("outerx"))
                    result.Add("outerx");
            }
            return result;
        }


        public static string CalculatePattern(List<string> combinations, int interlockWidth, int interlockHeight, string indent)
        {
            var horizontals = GetHorizontals(combinations);

            


            var result = indent + "var pattern = \"\"\n";
            int i = 0;

            foreach (var item in horizontals)
            {
                i += 1;
                if (item == "left")
                    result += indent + "pattern += " + GetLeft(interlockWidth,i) + "\n";
                    
                else if (item == "right")
                    result += indent + "pattern += " + GetRight(interlockWidth, i) + "\n";

                else if (item.StartsWith("middlex"))
                    result += indent + "pattern += " + GetMiddle(interlockWidth, i) + "\n";

                else if (item.StartsWith("outerx"))
                    result += indent + "pattern += " + GetOuter(interlockWidth, i) + "\n";

            }
            result += "\n\n";

            // We may as well add some more in here at the end

            result += indent + "let score = ScoreCalculator.WordScore(word: pattern) + stride * 10\n\n";


            result += indent + "if score >= minScore {\n\n";
            return result;
        }

        public static string GetLeft(int interlockWidth, int i)
        {
            var result = "";

            for (int v = interlockWidth - 1; v >= 0; v--)
            {
                if (result != "")
                    result += " + ";
                result += "String(W.End[left" + i + "][" + v + "])";
            }
            return result;
        }

        public static string GetRight(int interlockWidth, int i)
        {
            var result = "";
            for (int v = 0; v < interlockWidth; v++)
            {
                if (result != "")
                    result += " + ";
                result += "String(W.Start[right" + i + "][" + v + "])";
            }
            return result;
        }

        public static string GetMiddle(int interlockWidth, int i)
        {
            var result = "";
            for (int v = 0; v < interlockWidth; v++)
            {
                if (result != "")
                    result += " + ";
                result += "String(W.Start[middlex" + i + "][" + v + "])";
            }
            return result;
        }

        public static string GetOuter(int interlockWidth, int i)
        {
            var result = "";
            for (int v = 0; v < interlockWidth; v++)
            {
                if (result != "")
                    result += " + ";
                result += "String(W.Start[outerx" + i + "][outerx" + i + "Pos + " + v + "])";
            }
            return result;
        }
    }
}

