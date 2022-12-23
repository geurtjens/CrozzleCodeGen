using System;
namespace CrozzleCodeGen
{
	public class IndentCalculator
	{
		public IndentCalculator()
		{
		}


        public static string Indent2(int indentAmount)
        {
            string result = "";

            for (int i = 0; i < indentAmount; i++)
            {
                result += "    ";
            }

            return result;
        }


        public static string Indent(int indentAmount)
        {
            string result = "            ";

            for (int i = 0; i < indentAmount; i++)
            {
                result += "        ";
            }

            return result;
        }
    }
}

