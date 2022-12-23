using System;
namespace CrozzleCodeGen
{
    public class Executor
    {
        public Executor()
        {
        }
        

        public static void Execute(int interlockWidth, int interlockHeight)
        {
            var name = "C" + interlockWidth + "x" + interlockHeight;
            var patterns = PatternFinder.Execute(interlockWidth, interlockHeight);

            Console.WriteLine(name);
            foreach (var pattern in patterns)
            {

                var horizontalList = new List<string>();
                var verticalList = new List<string>();
                foreach (var item in pattern)
                {
                    if (item == "MiddleY" || item == "OuterY" || item == "Up" || item == "Down")
                        verticalList.Add(item);
                    else
                        horizontalList.Add(item);
                }
                var itemName = "";
                foreach (var item in horizontalList)
                    itemName += item[0];
                itemName += "_";
                foreach (var item in verticalList)
                    itemName += item[0];

                Console.WriteLine(itemName);

            }
            Console.WriteLine();


            




            var result = ExecuteCreator.Execute(patterns, interlockWidth, interlockHeight, name);

            //var path = "/Users/michaelgeurtjens/Developer/Batch/Batch/ShapeCalculators/";
        string path = "/Users/michaelgeurtjens/Developer/BatchSwift/BatchSwift/ShapeCalculator/";

        System.IO.File.WriteAllText(path + name + ".swift", result);


            foreach (var combinations in patterns)
            {
                // C3x3_LRL_UDU is an example
                var structureName = name + "_" + PatternFinder.GetSegmentName(combinations);

                var source = ClusterCreator.Execute(combinations, interlockWidth, interlockHeight, structureName);

                var filename = path + structureName + ".swift";
                System.IO.File.WriteAllText(filename, source);
            }
        }
        
    }

}