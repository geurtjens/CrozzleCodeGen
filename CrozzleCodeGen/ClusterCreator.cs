using System;
using System.Numerics;

namespace CrozzleCodeGen
{
	public class ClusterCreator
	{
		public ClusterCreator()
		{
		}

        

		

        // This is what creates the entire pattern searching algorithm to achieve a C3x3_LRL_UDU for example
        public static string Execute(List<string> combinations, int interlockWidth, int interlockHeight, string structureName)
		{
			var (leftright, updown) = ClusterCreatorUtilities.ConvertToNames(combinations);


			var patternHorizontal = ClusterCreatorUtilities.GetPatternHorizontal(leftright);

			var patternVertical = ClusterCreatorUtilities.GetPatternVertical(updown);

			// When it contains outers then our code must change slightly
			var containsOuter = false;
			if (patternHorizontal.Contains("outer") || patternVertical.Contains("outer"))
				containsOuter = true;


            var positions = PatternUtilities.CalculatePositions(combinations);

            int outerCount = ClusterCreatorUtilities.CountOuter(combinations);

            string indent = "            ";

            var dictionary = ClusterCreatorUtilities.GetDictionaryThatKeepsTrackOfPosition(positions, interlockWidth, interlockHeight);


            var result = "";

			result += GetHeader(combinations, interlockWidth: interlockWidth, interlockHeight: interlockHeight, structureName, containsOuter);


			result += GetBody(positions, interlockWidth, interlockHeight, dictionary, ref indent);

			result += WinningCombinationFound(indent, combinations, interlockWidth, interlockHeight, containsOuter);

			result += ClusterCreatorUtilities.GetBrackets(indent);

			result += returnCluster(patternHorizontal, patternVertical, interlockWidth, interlockHeight);

			result += "\n    }\n}";

            return result;
		}

		


		

		public static string CheckLength(string position, ref string indent, int i)
		{
			string result = "";
			if (position.StartsWith("up") || position.StartsWith("down"))
				result += indent + "if (W.Len[" + position + "] >= interlockHeight";
			else if (position.StartsWith("left") || position.StartsWith("right"))
				result += indent + "if (W.Len[" + position + "] >= interlockWidth";
			else if (position.StartsWith("middlex"))
				result += indent + "if (W.Len[" + position + "] == interlockWidth";
			else if (position.StartsWith("middley"))
				result += indent + "if (W.Len[" + position + "] == interlockHeight";
			else if (position.StartsWith("outery"))
			{
				
				// Here is where we put our extra things for outer
				result += indent + "if W.Len[" + position + "] >= interlockHeight + 2 {\n\n";
				result += indent + "    let " + position + "Limit = Int(W.Len[" + position + "]) - Int(interlockHeight)\n";
				result += indent + "    for " + position + "Pos in 1..<" + position + "Limit {\n\n";

				

				if (i != 0)
				{
                    indent += "    ";
                    result += indent + "    if (";
				}
			}
			else if (position.StartsWith("outerx"))
			{
				
				// Here is where we put our extra things for outer
				result += indent + "if W.Len[" + position + "] >= interlockWidth + 2 {\n\n";
				result += indent + "    let " + position + "Limit = Int(W.Len[" + position + "]) - Int(interlockWidth)\n";
				result += indent + "    for " + position + "Pos in 1..<" + position + "Limit {\n\n";
				
				if (i != 0)
				{
                    indent += "    ";
                    result += indent + "    if (";
				}
			}

			if (position.StartsWith("outer") == false)
			{
				if (i == 0)
					result += ") {\n";
				else
					result += " &&\n";
            }

            return result;
        }


		public static string GetBody(
			List<string> positions,
			int interlockWidth,
			int interlockHeight,
			Dictionary<string, int> dictionary,
			ref string indent)
		{
			var result = "";
            

            for (int i=0;i<positions.Count;i++)
			{
				
				var position = positions[i];

				result += indent + "for " + position + " in 0..<wordCount {\n\n";
				indent += "    ";
				// Check that the word is long enough to be used in this position, depends on position
				result += CheckLength(position, ref indent, i);

				result += CheckLettersOfThisWordInterlockWithExistingWords(indent, position, i - 1, positions, interlockWidth, interlockHeight, dictionary);

				result += CheckThisWordIsNotSameAsAllOtherWords(indent, position, i - 1, positions);
				
                

				indent += "    ";

				if (position.StartsWith("outer"))
					indent += "    ";


                result += indent + "//print(\"" + position + ":\\(W.Start[" + position + "])\")\n\n";

                //                indent += "    ";
                //            else if (position == "outerx1")
                //                indent += "    ";
                //            else 
                //    indent += "    ";
            }
			return result;
		}

		public static string CrossedWord(string position, int positionPos, string nextPosition, int nextPositionPos, int positionStartingPos, int interlockWidth, int interlockHeight)
		{
			var result = "";
			
			// trailing interlock - the interlock is at the end
			if (position.StartsWith("left") || position.StartsWith("up"))
				result += "W.End[" + position + "][" + positionStartingPos + "]";

			// leading interlock - the interlock is at the beginning
            else if (position.StartsWith("down") || position.StartsWith("right") || position.StartsWith("middle"))
				result += "W.Start[" + position + "][" + positionStartingPos + "]";

			// outer is like a leading interlock except it starts a little to the right of the start
            else if (position.StartsWith("outer"))
				result += "W.Start[" + position + "][" + position + "Pos + " + positionStartingPos + "]";

			result += " == ";

            // We strip the positionNumber from the end of the word
            var positionNumber = position.Substring(position.Length - 1, 1);
            var nextPositionNumber = nextPosition.Substring(nextPosition.Length - 1, 1);

            // Convert the number into an actual integer
            int positionStart = Int32.Parse(positionNumber) - 1;
            int nextPositionStart = Int32.Parse(nextPositionNumber) - 1;


            if (nextPosition.StartsWith("down") || nextPosition.StartsWith("right") || nextPosition.StartsWith("middle"))
				result += "W.Start[" + nextPosition + "][" + positionStart + "]";

			else if (nextPosition.StartsWith("left"))
                result += "W.End[" + nextPosition + "][" + (interlockWidth - positionStart - 1) + "]";

			else if (nextPosition.StartsWith("up"))
                result += "W.End[" + nextPosition + "][" + (interlockHeight - positionStart - 1) + "]";

            else if (nextPosition.StartsWith("outer"))
                result += "W.Start[" + nextPosition + "][" + nextPosition + "Pos + " + positionStart + "]";

            result = result + " &&\n";
			return result;
		}

		
		/// <summary>
		/// When placing a word we must check that it can interlock with words already in the grid
		/// </summary>
		/// <param name="indent"></param>
		/// <param name="position"></param>
		/// <param name="pos"></param>
		/// <param name="positions"></param>
		/// <param name="interlockWidth"></param>
		/// <param name="interlockHeight"></param>
		/// <param name="dictionary"></param>
		/// <returns></returns>
		public static string CheckLettersOfThisWordInterlockWithExistingWords(string indent, string position, int pos, List<string> positions, int interlockWidth, int interlockHeight, Dictionary<string,int> dictionary)
		{
			var result = "";

			bool positionIsUpDown = PatternUtilities.IsUpDown(position);

			var positionStartingPos = 0;
			var positionIncrementor = 1;
			// if the interlock is trailing
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

			
			int interlockCount = 0;

			for (int i = 0; i <= pos; i++)
			{
				string nextPosition = positions[i];
				bool nextIsUpDown = PatternUtilities.IsUpDown(nextPosition);

				// We only cross over the shapes if they are perpendicular to each other, one is horizontal and one is vertical
				if (positionIsUpDown != nextIsUpDown) {
                    interlockCount += 1;

					
					if (!(position.StartsWith("outer") && interlockCount == 1))
					{
                        // When the word is outer then we do not check its length at this point rather we check it previously
                        // and so all the other kinds expect we have to add indent but for outerx or outery we dont

                        result += indent + "    ";
					}
					result += CrossedWord(position, pos, nextPosition, i, positionStartingPos,interlockWidth,interlockHeight);
					positionStartingPos += positionIncrementor;

				}
			}


            return result;
		}


		/// <summary>
		/// We want to make sure that the position is not equal to all the other positions
		/// W.Id[right1] != W.Id[up1]
		/// </summary>
		/// <param name="indent"></param>
		/// <param name="position"></param>
		/// <param name="pos"></param>
		/// <param name="positions"></param>
		/// <returns></returns>
		public static string CheckThisWordIsNotSameAsAllOtherWords(string indent, string position, int pos, List<string> positions)
		{
			var result = "";

			for(int i=pos;i>=0;i--)
			{
				result += indent + "    W.Id[" + position + "] != W.Id[" + positions[i] + "]";
				if (i == 0)
					result += ") {\n";
				else
					result += " &&\n";
			}

			return result;
		}


		public static string UpdateOuterVariables(List<String> leftright, List<String> updown, string indent)
		{
			string result = "";
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
			return result;
        }


		/// <summary>
		/// This creates the code for when we have a correct combination and we want to save it
		/// </summary>
		/// <param name="indent"></param>
		/// <param name="combinations"></param>
		/// <param name="interlockWidth"></param>
		/// <param name="interlockHeight"></param>
		/// <param name="includesOuter"></param>
		/// <returns></returns>
		
		public static string WinningCombinationFound(string indent, List<string> combinations, int interlockWidth, int interlockHeight, bool includesOuter)
		{
			var (leftright, updown) = ClusterCreatorUtilities.ConvertToNames(combinations);

            string result = "";
			
			
			result += indent + "if phase == 0 {\n";
			result += indent + "    shapeCount += 1\n";
			result += indent + "}\n";
			result += indent + "else {\n";

            if (includesOuter)
            {
				result += UpdateOuterVariables(leftright, updown, indent);
            }

            result += indent + "    ClusterHelper.AddWords(index: &index, wordId: &wordId, wordsToAdd: ";
			result += ClusterCreatorUtilities.WordsToAdd(leftright, updown);
            result += ")\n";
			result += indent + "}\n";

            return result;

        }

		
		
		// This is the part that creates the header
        public static string GetHeader(List<string> combinations, int interlockWidth, int interlockHeight, string structureName, bool containsOuter)
		{
			string result = "";
			result += "public class " + structureName + " {\n";
			result += "   static func Execute(W: WordModelSOA, wordCount: Int) -> ClusterModel {\n";
			result += "        let interlockWidth = " + interlockWidth + "\n";
            result += "        let interlockHeight = " + interlockHeight + "\n";
			result += "        let stride = interlockWidth + interlockHeight\n";
			result += "        var index = 0 //pointer to where we should put next set of words\n";
			result += "        var shapeCount = 0\n";
			result += "        var outerStart: [UInt8] = []\n";
			result += "        var wordId: [UInt8] = []\n";
			result += "\n";
			result += "        for phase in 0..<2 {\n";
			result += "            if phase == 1 {\n";
			result += "                wordId = Array(repeating: 0, count: stride * shapeCount)\n";
			if (containsOuter == true)
				result += "                outerStart = Array(repeating: 0, count: stride * shapeCount)\n";
			result += "            }\n\n";

            return result;

		}

        public static string returnCluster(string patternHorizontal, string patternVertical, int interlockWidth, int interlockHeight)
		{
            var indent12 = "            ";

			bool removeDuplicates = false;
			if (interlockWidth == interlockHeight)
                removeDuplicates = true;
            
			var result = "\n";
			if (removeDuplicates == true)
                result += "        return RemoveDuplicates.RemoveDuplicates" + interlockWidth + "x" + interlockHeight + "(cluster: ClusterModel(\n";
             else
                result += "        return ClusterModel(\n";
            
            result += indent12 + "wordId: wordId,\n";
            result += indent12 + "outerStart: outerStart,\n";
            result += indent12 + "patternHorizontal: " + patternHorizontal + ",\n";
            result += indent12 + "patternVertical: " + patternVertical + ",\n";
            result += indent12 + "interlockWidth: interlockWidth,\n";
            result += indent12 + "interlockHeight: interlockHeight,\n";
            result += indent12 + "stride: stride,\n";
            result += indent12 + "size: shapeCount)";

			if (removeDuplicates == true)
				result += ")";

			return result;
        }
    }
}