using System;
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
            

            result += "    static func Execute(w: WordModelSOA, scoreMin: Int, widthMax: Int, heightMax: Int) -> [ShapeModel] {\n\n";

            result += "        let wordCount = w.wordCount\n\n";


            foreach (var combinations in patterns)
            {
                result += "        let " + PatternUtilities.ConcatinateList(combinations) + " = ToShape.from(cluster: " + name + "_" + PatternFinder.GetSegmentName(combinations) + ".Execute(W: w, wordCount: wordCount), wordList: w, scoreMin: scoreMin, widthMax: widthMax, heightMax: heightMax)\n";
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

            result += "        let result = ";
            for(int i=0;i<patterns.Count; i++)
            {
                string patternArray = PatternUtilities.ConcatinateList(patterns[i]);

                if (i > 0)
                    result += " + ";

                result += patternArray;
                 
            }
            result += "\n";
            result += "        return result\n";

            result += "    }\n";
            result += "}";


            return result;
        }
    
	}
}

