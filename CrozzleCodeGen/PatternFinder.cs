using System;
namespace CrozzleCodeGen
{
	public class PatternFinder
	{
		public PatternFinder()
		{
		}

        public static List<List<string>> Execute(int interlockWidth, int interlockHeight)
        {
            var horizontal = ExecuteX(interlockWidth,interlockHeight);

            if(interlockWidth <= 2)
            {
                // We must remove all middle if the other direction is not long enough, like 2 words long, maybe later.
            }



            var vertical = ExecuteY(interlockWidth,interlockHeight);

            var patterns = Merge(horizontal, vertical);

            RemoveDuplicateIfExists(interlockWidth, interlockHeight, patterns);

            return patterns;
        }



		public static List<List<string>> ExecuteX(int interlockWidth, int interlockHeight)
		{
            // First we get all of the left/right and right/left combinations

            



            var result = PatternFinder.AllowableStates(GetAllowableX(), interlockHeight);


            // You can only have middle for the length of the shortest word which is mostly always 3
            if (interlockWidth >= 3)
            {

                // We now include middle
                var horizontalWithMiddleList = PatternFinder.AllowableStates(GetAllowableXWithMiddle(), interlockHeight);

                // We want to eliminate all those from horizontalWithMiddle that do not add value that is could be substituted to be just like the normal horizontal

                var horizontalWithMiddleUnique = ExtractUniquePatterns(horizontalWithMiddleList, "Left", "Right");

                // Then we can add the unique patterns
                foreach (var item in horizontalWithMiddleUnique)
                    result.Add(item);



                //Finally lets see if we can get the outer thing working and capture only those with an outer
               var extendedList = PatternFinder.AllowableStates(GetAllowableX_Extended(), interlockHeight);
                var extendedFinal = new List<List<string>>();
                foreach (var extended in extendedList)
                {
                    if (IsThisOuterUniqueAndValid(extended))
                        extendedFinal.Add(extended);
                }
                foreach (var extendedItem in extendedFinal)
                {
                    result.Add(extendedItem);
                }

            }
            return result;
        }

        public static List<List<string>> ExecuteY(int interlockWidth, int interlockHeight)
        {
            // First we get all of the left/right and right/left combinations

            var result = PatternFinder.AllowableStates(GetAllowableY(), interlockWidth);


            // You can only have middle for the length of the shortest word which is mostly always 3
            if (interlockHeight >= 3)
            {

                // We now include middle
                var verticalWithMiddleList = PatternFinder.AllowableStates(GetAllowableYWithMiddle(), interlockWidth);

                // We want to eliminate all those from horizontalWithMiddle that do not add value that is could be substituted to be just like the normal horizontal

                // This didnt work, extracted wrong ones.
                var verticalWithMiddleUnique = ExtractUniquePatterns(verticalWithMiddleList, "Up", "Down");

                // Then we can add the unique patterns
                foreach (var item in verticalWithMiddleUnique)
                    result.Add(item);


                // Finally lets see if we can get the outer thing working and capture only those with an outer
                var extendedList = PatternFinder.AllowableStates(GetAllowableY_Extended(), interlockWidth);
                var extendedFinal = new List<List<string>>();
                foreach (var extended in extendedList)
                {
                    if (IsThisOuterUniqueAndValid(extended))
                        extendedFinal.Add(extended);
                }
                if (extendedFinal.Count > 0) {
                    Console.WriteLine(extendedFinal);
                }

                foreach (var extendedItem in extendedFinal)
                {
                    result.Add(extendedItem);
                    if(extendedItem.Count > 3)
                    {
                        Console.WriteLine("Interesting");
                    }
                }
            }
            return result;
        }


        public static bool IsRepeatingPatternWithMiddle(List<string> pattern)
        {
            if (pattern.Count == 3 && pattern[0].StartsWith("Middle") && pattern[2].StartsWith("Middle"))
                return true;
            else if (pattern.Count > 2 && pattern[pattern.Count - 1].StartsWith("Middle") && pattern[pattern.Count - 2].StartsWith("Outer") == false)
                return true;
            else if (pattern.Count > 2 && pattern[0].StartsWith("Middle") && pattern[1].StartsWith("Outer") == false)
                return true;

            else if (pattern.Count > 3 && pattern[0].StartsWith("Middle") && pattern[2].StartsWith("Middle") && pattern[1] == pattern[3] && pattern[1].StartsWith("Outer") == false)
                return true;
            else if (pattern.Count >= 3 && pattern[pattern.Count - 2].StartsWith("Middle") && pattern[pattern.Count - 3] == pattern[pattern.Count - 1] && pattern[pattern.Count - 1].StartsWith("Outer") == false)
                return true;
            else if (pattern.Count >= 3 && pattern[1].StartsWith("Middle") && pattern[0] == pattern[2] && pattern[0].StartsWith("Outer") == false)
                return true;
            else if (pattern.Count >= 4 && pattern[2].StartsWith("Middle") && pattern[1] == pattern[3] && pattern[1].StartsWith("Outer") == false)
                return true;
            else if (pattern.Count >= 5 && pattern[3].StartsWith("Middle") && pattern[2] == pattern[4] && pattern[2].StartsWith("Outer") == false)
                return true;

            return false;
        }


        public static List<List<string>> ExtractUniquePatterns(List<List<string>> patternList, string a, string b)
        {
            var result = new List<List<string>>();
            foreach (var pattern in patternList)
            {
                if (DetectSimilarPattern(pattern, a, b) == false && DetectSimilarPattern(pattern, b, a) == false)
                {
                    if (IsRepeatingPatternWithMiddle(pattern) == false)
                        result.Add(pattern);

                }
                    

            }
            return result;
        }

        /// <summary>
        /// If it starts with or ends with Middle then the next one to it must be outer otherwise it will be rejected
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns>Returns true if this one should be rejected</returns>
        public static bool IsThisOuterUniqueAndValid(List<string> pattern)
        {
            // We should be asking can a more restricted pattern fit into a more losely specified pattern.
            // Or we make the middle a real thing always and dont allow the left right 




            // It should also reject "Middle","Outer","Middle"
            //if (pattern.Count == 3 && pattern[0].StartsWith("Middle") && pattern[1].StartsWith("Outer") && pattern[2].StartsWith("Middle"))
            //    return false;
            if (pattern.Count > 3 && pattern[0] == pattern[2] && pattern[1].StartsWith("Middle") && pattern[0].StartsWith("Outer") == false)
                return false;
            else if (pattern.Count >= 5 && pattern[3].StartsWith("Middle") && pattern[3] == pattern[4])
                return false;
            else if (pattern.Count >= 3 && pattern[pattern.Count - 2].StartsWith("Middle") && pattern[pattern.Count - 1] == pattern[pattern.Count - 3] && pattern[pattern.Count - 1].StartsWith("Outer") == false)
                return false;
            else if (pattern.Count >= 5 && pattern[1].StartsWith("Middle") && pattern[2] == pattern[3] && pattern[4].StartsWith("Outer") == false)
                return false;
            else if (pattern.Count >= 3 && pattern[0].StartsWith("Outer") == false && pattern[1].StartsWith("Middle") && pattern[1] == pattern[2])
                return false;
            else if (pattern.Count >= 4 && pattern[1].StartsWith("Middle") && pattern[1] == pattern[2] && pattern[2] == pattern[3])
                return false;
            else if (pattern.Count == 4 && pattern[0].StartsWith("Outer") == false && pattern[1].StartsWith("Middle") && pattern[2].StartsWith("Middle") && pattern[3].StartsWith("Outer"))
                return false;
            else if (pattern.Count == 4 && pattern[0].StartsWith("Outer") && pattern[1].StartsWith("Middle") && pattern[2].StartsWith("Middle") && pattern[3].StartsWith("Outer") == false)
                return false;
            // It cannot start with middle without the next one being an outer
            else if (pattern[0].StartsWith("Middle") && pattern[1].StartsWith("Outer") == false)
                return false;
            else if (pattern.Count >= 3 && pattern[pattern.Count - 1].StartsWith("Outer") && pattern[pattern.Count - 2].StartsWith("Middle") && pattern[pattern.Count - 3].StartsWith("Middle"))
                return false;
            // It cannot end with a middle without the previous one being an outer
            else if (pattern[pattern.Count - 1].StartsWith("Middle") && pattern[pattern.Count - 2].StartsWith("Outer") == false)
                return false;

            // Now we check if this outer actually contains an outer
            bool hasOuter = false;
            foreach(var item in pattern)
            {
                if (item.StartsWith("Outer"))
                    hasOuter = true;
            }
            if (hasOuter == false)
                return false;


            // Now we have to check if it has a middle that could easily be a regular one
            for(int i=0;i<pattern.Count-2;i++)
            {
                string x = pattern[0].ToLower();
                string y = pattern[1].ToLower();
                string z = pattern[2].ToLower();

                if (y.StartsWith("Middle") && x == z && x.StartsWith("Outer"))
                {
                    // This means its like Left, Middle, Left which is a duplicate so we can reject it
                    return false;
                }
            }



            return true;

        }

        public static List<string> HorizontalPatterns(List<string> pattern)
        {
            var result = new List<string>();

            foreach(var item in pattern)
            {
                if (item == "Left" || item == "Right" || item == "MiddleX" || item == "OuterX")
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public static List<string> VerticalPatterns(List<string> pattern)
        {
            var result = new List<string>();

            foreach (var item in pattern)
            {
                if (item == "Up" || item == "Down" || item == "MiddleY" || item == "OuterY")
                {
                    result.Add(item);
                }
            }

            return result;
        }


        public static string GetSegmentName(List<string> combinations)
        {
            var horizontalPatterns = PatternFinder.HorizontalPatterns(combinations);
            var verticalPatterns = PatternFinder.VerticalPatterns(combinations);

            var segment = "";
            foreach (var item in horizontalPatterns)
            {
                segment += item[0];
            }
            segment += "_";
            foreach (var item in verticalPatterns)
            {
                segment += item[0];
            }
            segment = segment.Replace("Y", "").Replace("X", "");
            return segment;
        }
        

        /// <summary>
        /// Does this pattern that contains middle, does it follow an expected sequence or is it unique
        /// If its unique and not just Left, Middle, Left etc then we can use it.  So we should not be able to subtitute a Middle for a Left or Right and get a regular sequence
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="expectedSequence"></param>
        /// <returns></returns>
        public static bool DetectSimilarPattern(List<string> pattern, string a, string b)
        {

            // First we start with whatever is in a, could be Left, Right or Top, Bottom
            var expected = a;
            foreach(var item in pattern)
            {
                if(item == expected || item.StartsWith("Middle"))
                {
                    // this is fine so we continue
                    if (expected == a)
                        expected = b;
                    else
                        expected = a;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public static Dictionary<string, string[]> GetAllowableX()
        {
            var result = new Dictionary<string, string[]>();

            result["Right"] = new string[] { "Left" };
            result["Left"] = new string[] { "Right" };

            return result;
        }

        public static Dictionary<string, string[]> GetAllowableXWithMiddle()
        {
            var result = new Dictionary<string, string[]>();

            // What is should be is that if you are middle then you can go back to what is there prior and you cannot start with middle
            result["Right"] = new string[] { "Left","MiddleX" };
            result["Left"] = new string[] { "Right","MiddleX" };
            result["MiddleX"] = new string[] { "Left", "Right" };

            return result;
        }

        public static Dictionary<string, string[]> GetAllowableYWithMiddle()
        {
            var result = new Dictionary<string, string[]>();

            result["Down"] = new string[] { "Up", "MiddleY" };
            result["Up"] = new string[] { "Down", "MiddleY" };
            result["MiddleY"] = new string[] { "Up", "Down" };

            return result;
        }

        public static Dictionary<string, string[]> GetAllowableY()
        {
            var result = new Dictionary<string, string[]>();

            result["Down"] = new string[] { "Up" };
            result["Up"] = new string[] { "Down" };

            return result;
        }

        public static Dictionary<string, string[]> GetAllowableX_Extended()
		{
			var result = new Dictionary<string, string[]>();

			result["OuterX"] = new string[] { "MiddleX" };
            result["MiddleX"] = new string[] { "Left","Right","MiddleX","OuterX" };
			result["Right"] = new string[] { "Left", "MiddleX" };
			result["Left"] = new string[] { "Right", "MiddleX" };

            return result;
		}

        public static Dictionary<string, string[]> GetAllowableY_Extended()
        {
            var result = new Dictionary<string, string[]>();

            result["OuterY"] = new string[] { "MiddleY" };
            result["MiddleY"] = new string[] { "Up", "Down", "MiddleY", "OuterY" };
            result["Up"] = new string[] { "Down", "MiddleY" };
            result["Down"] = new string[] { "Up", "MiddleY" };

            return result;
        }

        public static List<List<string>> Merge(List<List<string>> listOfListsA, List<List<string>> listOfListsB)
        {
            var result = new List<List<string>>();

            foreach(var listA in listOfListsA)
            {
                foreach(var listB in listOfListsB)
                {
                    int max = Math.Max(listA.Count, listB.Count);
                    var sublist = new List<string>();
                    
                    for(int i =0;i<max;i++) {
                        if (listA.Count < listB.Count)
                        {
                            if (listA.Count >= i+1)
                                sublist.Add(listA[i]);
                            if (listB.Count >= i+1)
                                sublist.Add(listB[i]);
                        }
                        else
                        {
                            if (listB.Count >= i + 1)
                                sublist.Add(listB[i]);
                            if (listA.Count >= i + 1)
                                sublist.Add(listA[i]);
                        }
                    }
                    result.Add(sublist);
                }
            }

            return result;

        }

        

        public static List<List<string>> AllowableStates(Dictionary<string, string[]> allowable, int length)
        {
            // This is something we can try to get what we want
            if (length == 3)
            {

            }



            var workingStack = new Stack<List<string>>();

            foreach(var item in allowable)
            {
                workingStack.Push(new List<string> { item.Key });
            }


            var result = new List<List<string>>();
            while(workingStack.Count > 0)
            {
                List<string> workSet = workingStack.Pop();
                var currentState = workSet.Last();

                string[] futureStates = allowable[currentState];


                foreach(var nextMove in allowable[currentState])
                {
                    List<string> possibleResult = CopyArray(workSet);
                    possibleResult.Add(nextMove);
                    if (possibleResult.Count == length)
                        result.Add(possibleResult);
                    else
                        workingStack.Push(possibleResult);

                }
            }
            return result;

        }

        public static List<string> CopyArray(List<string> items)
        {
            var result = new List<string>();

            foreach(var item in items)
            {
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// When we have a 2x2 or a 3x3 then we do not need the "Down","Left" as its a duplicate of "RL:UD"
        /// </summary>
        /// <param name="interlockWidth"></param>
        /// <param name="interlockHeight"></param>
        /// <param name="patterns"></param>
        public static void RemoveDuplicateIfExists(int interlockWidth, int interlockHeight, List<List<string>> patterns)
        {
            if (interlockWidth == interlockHeight)
            {

                int itemToDelete = -1;
                // We want to remove Left,Down,Right,Up as it produces duplicates
                for (int i = 0; i < patterns.Count; i++)
                {
                    var pattern = patterns[i];
                    if (pattern[0] == "Down" && pattern[1] == "Left")
                    {
                        itemToDelete = i;
                    }
                }
                patterns.RemoveAt(itemToDelete);

            }
        }
    }
}

