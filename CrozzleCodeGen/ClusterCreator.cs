using System;
using System.Numerics;

namespace CrozzleCodeGen
{
	public class ClusterCreator
	{
		public ClusterCreator()
		{
		}


		
		



        public static string Execute(List<string> combinations, int interlockWidth, int interlockHeight, string structureName)
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

			var containsOuter = false;

			var patternHorizontal = "";
			foreach(var item in leftright)
			{
                if (patternHorizontal != "")
                    patternHorizontal += ", ";

				if (item.StartsWith("left"))
					patternHorizontal += ".leading";
				else if (item.StartsWith("right"))
					patternHorizontal += ".trailing";
				else if (item.StartsWith("middle"))
					patternHorizontal += ".middle";
				else if (item.StartsWith("outer"))
				{
                    patternHorizontal += ".outer";
					containsOuter = true;
                }
					
			}

            var patternVertical = "";
            foreach (var item in updown)
            {
                if (patternVertical != "")
                    patternVertical += ", ";

                if (item.StartsWith("up"))
                    patternVertical += ".leading";
                else if (item.StartsWith("down"))
                    patternVertical += ".trailing";
                else if (item.StartsWith("middle"))
                    patternVertical += ".middle";
                else if (item.StartsWith("outer"))
				{
                    patternVertical += ".outer";
					containsOuter = true;
                }
                    
            }


            var result = "";

            var positions = PatternUtilities.CalculatePositions(combinations);



			result += GetHeader(combinations, interlockWidth: interlockWidth, interlockHeight: interlockHeight, structureName, containsOuter);

			result += GetBody(positions, interlockWidth, interlockHeight);

			string indent = IndentCalculator.Indent(combinations.Count - 1);


            //result += CalculateScore.CalculatePattern(positions, interlockWidth, interlockHeight,indent);
            indent += "    ";

            // Need more work for width and height to be ok
            result += ExtraCode(indent, combinations, interlockWidth, interlockHeight, containsOuter);
            

            int outerCount = 0;

			foreach (string pattern in combinations)
			{
				if (pattern.StartsWith("Outer"))
					outerCount += 1;
			}



			result += GetFooter(interlockWidth + interlockHeight, outerCount, patternHorizontal, patternVertical) + "\n}";

			// Now we save it

			return result;
			


			

			
		}
		

		public static string Indent(int indentAmount)
		{
			string result = "            ";

			for(int i=0;i<indentAmount;i++)
			{
				result += "        ";
			}

			return result;
		}


		public static Dictionary<string,int> GetDictionaryThatKeepsTrackOfPosition(List<string> positions, int interlockWidth, int interlockHeight)
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


		public static string GetBody(List<string> positions, int interlockWidth, int interlockHeight)
		{
			var result = "";



			var dictionary = GetDictionaryThatKeepsTrackOfPosition(positions, interlockWidth, interlockHeight);



			var outerCount = 0;

			for(int i=0;i<positions.Count;i++)
			{
				var indent = Indent(i);



				var position = positions[i];

				result += indent + "for " + position + " in 0..<wordCount {\n\n";

				if(position.StartsWith("outery"))
				{
                    indent += "    ";
					// Here is where we put our extra things for outer
					result += indent + "if W.Len[" + position + "] >= interlockHeight + 2 {\n";
                    result += indent + "    let " + position + "Limit = Int(W.Len[" + position + "]) - Int(interlockHeight)\n";
                    result += indent + "    for " + position + "Pos in 1..<" + position + "Limit {\n";
                    
                    outerCount += 1;
                }
                if (position.StartsWith("outeryx"))
                {
                    indent += "    ";
                    // Here is where we put our extra things for outer
                    result += indent + "if W.Len[" + position + "] >= interlockWidth + 2 {\n";
                    result += indent + "    let " + position + "Limit = Int(W.Len[" + position + "]) - Int(interlockWidth)\n";
                    result += indent + "    for " + position + "Pos in 1..<" + position + "Limit {\n";

                    outerCount += 1;
                }




				if (position.StartsWith("middle"))
				{
					result += indent + "    if (W.Len[" + position + "] == interlock";
					if (PatternUtilities.IsUpDown(position))
						result += "Height";
					else
						result += "Width";

				}
				else if (position.StartsWith("outer") && i != 0)
				{
					result += indent + "    if (";
				}
				else
				{
					result += indent + "    if (W.Len[" + position + "] >= interlock";
					if (PatternUtilities.IsUpDown(position))
						result += "Height";
					else
						result += "Width";
				}
					
				
				
				if(i==0)
				{
					result += ") {\n";
				}
				else
				{
					result += " &&\n";




					result += CrossingWords(indent, position, i - 1, positions,interlockWidth,interlockHeight, dictionary);
					result += GetNotEqualTo(indent, position, i-1, positions);
				}
                result += indent + "        //print(\"" + position + ":\\(W.Start[" + position + "])\")\n\n";
            }

			return result;
		}

		public static string CrossedWord(string indent, string position, int positionPos, string nextPosition, int nextPositionPos, int positionStartingPos, int interlockWidth, int interlockHeight)
		{
			var result = "";

			var positionNumber = position.Substring(position.Length - 1, 1);
			var nextPositionNumber = nextPosition.Substring(nextPosition.Length - 1, 1);

			int positionStart = Int32.Parse(positionNumber) - 1;
			int nextPositionStart = Int32.Parse(nextPositionNumber) - 1;

			int positionWidthEnd = interlockWidth - positionStart;
			int positionHeightEnd = interlockHeight - positionStart;
			int nextPositionEndY = interlockWidth - nextPositionStart;
			int nextPositionHeightEnd = interlockHeight - nextPositionStart;

			int startMinus1 = positionStart - 1;

			// what if we minus one number from the other
			int secondParam = positionStart - nextPositionStart;
			if (secondParam < 0)
				secondParam = secondParam * -1;


			if (position.StartsWith("left"))
				result += "W.End[" + position + "][" + positionStartingPos + "]";

			else if (position.StartsWith("right"))
				result += "W.Start[" + position + "][" + positionStartingPos + "]";

			else if (position.StartsWith("middle"))
				result += "W.Start[" + position + "][" + positionStartingPos + "]";

			else if (position.StartsWith("outer"))
				result += "W.Start[" + position + "][" + position + "Pos]";

			else if (position.StartsWith("up"))
				result += "W.End[" + position + "][" + positionStartingPos + "]";

			else if (position.StartsWith("down"))
				result += "W.Start[" + position + "][" + positionStartingPos + "]";


			result += " == ";


			if (nextPosition.StartsWith("down") || nextPosition.StartsWith("right") || nextPosition.StartsWith("middle"))
				result += "W.Start[" + nextPosition + "][" + positionStart + "]";

			else if (nextPosition.StartsWith("left"))
                result += "W.End[" + nextPosition + "][" + (interlockWidth - positionStart - 1) + "]";

			else if (nextPosition.StartsWith("up"))
                result += "W.End[" + nextPosition + "][" + (interlockHeight - positionStart - 1) + "]";

            else if (nextPosition.StartsWith("outer"))
                result += "W.Start[" + nextPosition + "][" + nextPosition + "Pos + " + positionStart + "]";

            result = indent + "        " + result + " &&\n";
			return result;
		}

		//public static string OtherStuffForOuter(string position)
		//{
		//	somehow get this bit working.
		//	var result = "let " + position + "Limit: Int = Int(W.Len[" + position + "] - 2)\n";
		//	result += "for " + position + "Pos in 1..<" + position + "Limit {\n";
		//	return result;
	 //   }


		public static string CrossingWords(string indent, string position, int pos, List<string> positions, int interlockWidth, int interlockHeight, Dictionary<string,int> dictionary)
		{
			var result = "";

			bool positionIsUpDown = PatternUtilities.IsUpDown(position);

			var positionStartingPos = 0;
			var positionIncrementor = 1;
			if (position.StartsWith("left"))
			{
				positionStartingPos = interlockWidth - 1;
				positionIncrementor = -1;
			}
			else if (position.StartsWith("up"))
			{
				positionStartingPos = interlockHeight - 1;
				positionIncrementor = -1;
			}

			for (int i = 0; i <= pos; i++)
			{
				string nextPosition = positions[i];
				bool nextIsUpDown = PatternUtilities.IsUpDown(nextPosition);

				if (positionIsUpDown != nextIsUpDown) {
					// We are crossing this word
					result += CrossedWord(indent, position, pos, nextPosition, i, positionStartingPos,interlockWidth,interlockHeight);
					positionStartingPos += positionIncrementor;

				}
			}


            return result;
		}
		public static string GetNotEqualTo(string indent, string position, int pos, List<string> positions)
		{
			var result = "";





			for(int i=pos;i>=0;i--)
			{
				result += indent + "        W.Id[" + position + "] != W.Id[" + positions[i] + "]";
				if (i == 0)
					result += ") {\n";
				else
					result += " &&\n";
			}

			return result;
		}

		

		

		
			


		public static List<string> GetMaxFromStartElements(List<string> combinations)
		{
            var result = new List<string>();
            foreach (var item in combinations)
            {
                if (item.StartsWith("left") || item.StartsWith("up"))
                    result.Add("W.Len[" + item + "]");
                else if (item.StartsWith("outer"))
                    result.Add("UInt8(" + item + "Pos)");
				else if (item.StartsWith("middle"))
				{
					// we will not include this
				}
            }
			return result;
        }


		/// <summary>
		/// We get a list of variables that stick out to the right or bottom of the cluster
		/// Or they might even be an outer and stick out to the right or bottom
		/// We want to convert this into code that can be then used for calculating the width or height of the shape
		/// So we convert the names of the variables into code that can be later used to calculate the maximum
		/// </summary>
		/// <param name="combinations"></param>
		/// <returns></returns>
        public static List<string> GetMaxFromEndElements(List<string> combinations)
        {
            var result = new List<string>();
            foreach (var item in combinations)
            {
                if (item.StartsWith("right") || item.StartsWith("down"))
                    result.Add("W.Len[" + item + "]");
                else if (item.StartsWith("outer"))
                    result.Add("W.Len[" + item + "] - UInt8(" + item + "Pos)");
                else if (item.StartsWith("middle"))
                {
                    // we will not include this
                }
            }
            return result;
        }

        public static string GetMaxFromStart(List<string> combinations)
		{
			var valueList = GetMaxFromStartElements(combinations);

			// Can we perform an algorithmic and so simpler way of showing all the max or just stick with the simple one, nah simple one



			// We have to deal with outer combinations also which means it would be specific to it being from the start and from the end
			var result = GetMax2(valueList);

			return result;
        }
        public static string GetMaxFromEnd(List<string> combinations)
        {
            var valueList = GetMaxFromEndElements(combinations);
            // We have to deal with outer combinations also which means it would be specific to it being from the start and from the end
            var result = GetMax2(valueList);

            return result;
        }


        public static string GetMax2(List<string> combinations)
        {




            if (combinations.Count == 0)
                Console.WriteLine("We should not be doing this one");
            if (combinations.Count == 1)
                return combinations[0];
            else if (combinations.Count == 2)
                return "max(" + combinations[0] + "," + combinations[1] + ")";
            else if (combinations.Count == 3)
                return "max(max(" + combinations[0] + "," + combinations[1] + ")," + combinations[2] + ")";
            else if (combinations.Count == 4)
                return "max(max(" + combinations[0] + "," + combinations[1] + "), max(" + combinations[2] + "," + combinations[3] + "))";
            else if (combinations.Count == 5)
                return "max(max(max(" + combinations[0] + "," + combinations[1] + "), max(" + combinations[2] + "," + combinations[3] + "))," + combinations[4] + ")";
            else if (combinations.Count == 6)
                return "max(max(max(" + combinations[0] + "," + combinations[1] + "),max(" + combinations[2] + "," + combinations[3] + ")),max(" + combinations[4] + "," + combinations[5] + "))";
            else
                return "THIS IS RATHER LARGE";
        }

  //      public static string GetMax(List<string> combinations)
		//{

			


		//	if (combinations.Count == 0)
		//		Console.WriteLine("We should not be doing this one");
		//	if (combinations.Count == 1)`
		//		return "W.Len[" + combinations[0] + "]";
		//	else if (combinations.Count == 2)
		//		return "max(W.Len[" + combinations[0] + "],W.Len[" + combinations[1] + "])";
		//	else if (combinations.Count == 3)
		//		return "max(max(W.Len[" + combinations[0] + "],W.Len[" + combinations[1] + "]),W.Len[" + combinations[2] + "])";
  //          else if (combinations.Count == 4)
  //              return "max(max(W.Len[" + combinations[0] + "],W.Len[" + combinations[1] + "]),max(W.Len[" + combinations[2] + "],W.Len[" + combinations[3] + "]))";
  //          else if (combinations.Count == 5)
  //              return "max(max(max(W.Len[" + combinations[0] + "], W.Len[" + combinations[1] + "]), max(W.Len[" + combinations[2] + "], W.Len[" + combinations[3] + "])), W.Len[" + combinations[4] + "])";
		//	else if (combinations.Count == 6)
  //              return "max(max(max(W.Len[" + combinations[0] + "], W.Len[" + combinations[1] + "]), max(W.Len[" + combinations[2] + "], W.Len[" + combinations[3] + "])), max(W.Len[" + combinations[4] + "], W.Len" + combinations[5] + "]))";
  //          else
  //              return "THIS IS RATHER LARGE";
		//}

		public static string CalculateWidth(List<string> combinations, int interlockWidth)
		{
			var result = "";

			var leftCombinations = new List<string>();
            var rightCombinations = new List<string>();

			foreach(var combination in combinations)
			{
				
				if (combination == "Left")
					leftCombinations.Add(combination.ToLower());
				else if (combination == "Right")
					rightCombinations.Add(combination.ToLower());
				else if (combination == "OuterX")
				{
					leftCombinations.Add(combination.ToLower());
					rightCombinations.Add(combination.ToLower());
				}
			}


			if (leftCombinations.Count > 0 && rightCombinations.Count > 0)
				result += GetMaxFromStart(leftCombinations) + " + " + GetMaxFromEnd(rightCombinations);
			else if (leftCombinations.Count > 0)
				result += GetMaxFromStart(leftCombinations);
			else if (rightCombinations.Count > 0)
				result += GetMaxFromEnd(rightCombinations);
			else
				Console.WriteLine("Interesting problem here");



			if (interlockWidth == 3)
				result += " - 1";



			

            return result;
		}
        public static string CalculateHeight(List<string> combinations, int interlockHeight)
        {
            var result = "";

            var upCombinations = new List<string>();
            var downCombinations = new List<string>();

            foreach (var combination in combinations)
            {
				if (combination == "Up")
					upCombinations.Add("up");
				else if (combination == "Down")
					downCombinations.Add("down");
				else if (combination == "OuterY")
				{
					upCombinations.Add("outery"); // This is not exactly right actually because its a different thing than max
					downCombinations.Add("outery");
				}


					
            }
			if (upCombinations.Count > 0 && downCombinations.Count > 0)
				result += GetMaxFromStart(upCombinations) + " + " + GetMaxFromEnd(downCombinations);
			else if (upCombinations.Count > 0)
				result += GetMaxFromStart(upCombinations);
			else if (downCombinations.Count > 0)
				result += GetMaxFromEnd(downCombinations);
			else
				Console.Write("ERROR HERE")
;
			int extra = interlockHeight - 2;

            if (interlockHeight > 0)
                result += " - " + extra;

            return result;

        }
		public static string ExtraCode(string indent, List<string> combinations, int interlockWidth, int interlockHeight, bool includesOuter)
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


            string result = "";
			//result += indent + "let width = " + CalculateWidth(combinations, interlockWidth) + "\n";
			//result += indent + "let height = " + CalculateHeight(combinations, interlockHeight) + "\n\n";

			//result += indent + "if (width <= maxWidth && height <= maxHeight) || (width <= maxHeight && height <= maxWidth) {\n";
			indent += "    ";
			result += indent + "if phase == 0 {\n";
			result += indent + "    shapeCount += 1\n";
			result += indent + "}\n";
			result += indent + "else {\n";

            if (includesOuter)
            {
                for (var i = 0; i < leftright.Count; i++)
                {
                    if (leftright[i].StartsWith("outerx"))
                    {
                        var newText = indent + "    outerStart[index + " + i + "] = UInt8(" + leftright[i] + "Pos)\n\n";
                        result += newText;
                    }
                }
                for (var i = 0; i < updown.Count; i++)
                {
                    if (updown[i].StartsWith("outery"))
                    {

                        var newText = indent + "    outerStart[index + " + (i + leftright.Count) + "] = UInt8(" + updown[i] + "Pos)\n\n";
                        result += newText;
                    }
                }
            }



            result += indent + "    ClusterHelper.AddWords(index: &index, wordId: &wordId, wordsToAdd: [";



            
			result += WordsToAdd(leftright, updown);


            result += "])\n";
			
            result += "\n";


            return result;

        }

        public static string WordsToAdd(List<String> leftRight, List<String> upDown)
		{
			string result = "";
			foreach(var item in leftRight)
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
			return result;
        }
		
		// This is the part that creates the header
        public static string GetHeader(List<string> combinations, int interlockWidth, int interlockHeight, string structureName, bool containsOuter)
		{
			string result = "public class " + structureName + " {\n   static func Execute";
			//foreach(var combination in combinations)
			//{
			//	result += combination;
			//}

			int stride = interlockWidth + interlockHeight;

			result += "(W: WordModelSOA, wordCount: Int) -> ClusterModel {\n";
			result += "        let interlockWidth = " + interlockWidth + "\n";
            result += "        let interlockHeight = " + interlockHeight + "\n";
			result += "        let stride = interlockWidth + interlockHeight\n";
			result += "        var index = 0 //pointer to where we should put next set of words\n";
			result += "        var shapeCount = 0\n";
			result += "        var outerStart: [UInt8] = []\n";
			result += "        var wordId: [UInt8] = []\n\n";
			result += "        for phase in 0..<2 {\n";
			result += "            if phase == 1 {\n";
			result += "                wordId = Array(repeating: 0, count: stride * shapeCount)\n";
			if (containsOuter == true)
				result += "                outerStart = Array(repeating: 0, count: stride * shapeCount)\n";
			result += "            }\n\n";

            return result;

		}

		// We also have to add a bracket for however many outers there are
        public static string GetFooter(int stride, int outers, string patternHorizontal, string patternVertical)
        {

            var brackets = 1 + stride * 2;

            var result = "";
            for (int i = brackets; i >= 0; i--)
            {
                result += "}";
            }

			for (int i = 0; i < outers; i++)
				result += "}";

			var indent12 = "            ";


			result += "\n        return ClusterModel(\n";
			result += indent12 + "wordId: wordId,\n";
			result += indent12 + "outerStart: outerStart,\n";
			result += indent12 + "patternHorizontal: [" + patternHorizontal +"],\n";
			result += indent12 + "patternVertical: [" + patternVertical + "],\n";
			result += indent12 + "interlockWidth: interlockWidth,\n";
			result += indent12 + "interlockHeight: interlockHeight,\n";
			result += indent12 + "stride: stride,\n";
		    result += indent12 + "size: shapeCount)\n";


			result +=  "    }\n";
            return result;
        }
    }
}

