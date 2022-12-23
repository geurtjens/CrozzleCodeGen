﻿using System;
namespace CrozzleCodeGen
{
    // Creates a function that can call each and every Cluster3x3 pattern for example
    public class ExecuteCreator
	{
		public ExecuteCreator()
		{
		}
        
		public static string Execute(List<List<string>> patterns, int interlockWidth, int interlockHeight, string name)
		{
			string result = "public class " + name + " {\n"; ;
            

            result += "    static func Execute(words: [String], scoreMin: Int, widthMax: Int, heightMax: Int) -> ClusterModel {\n\n";
            result += "        let w = WordModelSOA(words: words)\n\n";
            result += "        let wordCount = W.wordCount\n\n";

            var duplicateFunction = "";
            var duplicateFunctionEnd = "";
            if (interlockWidth == interlockHeight)
            {
                duplicateFunction = "RemoveDuplicates" + interlockWidth + "x" + interlockHeight + "(cluster:";
                duplicateFunctionEnd = ")";
            }


            foreach (var combinations in patterns)
            {
                result += "        let " + PatternUtilities.ConcatinateList(combinations) + " = " + duplicateFunction + name + "_" + PatternFinder.GetSegmentName(combinations) + ".Execute(W: W, wordCount: wordCount, minScore: minScore, maxWidth: maxWidth, maxHeight: maxHeight)" + duplicateFunctionEnd + "\n";
            }



            result += "\n";

            result += "        print(\"Cluster" + interlockWidth + "x" + interlockHeight + "\")\n";
            foreach (var combinations in patterns)
            {
                string concatinatedList = PatternUtilities.ConcatinateList(combinations);


                result += "        if " + concatinatedList + ".count > 0 {\n";
                result += "            print(\"" + concatinatedList.ToUpper() + ": \\(" + concatinatedList + ".count)\")\n";
                result += "        }\n";
            }

            result += "\n";


            // We want to return a list of lists

            result += "        let result: [ShapeArray] = [";
            for(int i=0;i<patterns.Count; i++)
            {
                string patternArray = PatternUtilities.ConcatinateList(patterns[i]);

                if (i > 0)
                    result += ", ";

                result += patternArray;
                 
            }
            result += "]\n\n";
            result += "        return result\n\n";

            result += "    }\n\n";
            result += "}";


            return result;
        }
    
	}
}

