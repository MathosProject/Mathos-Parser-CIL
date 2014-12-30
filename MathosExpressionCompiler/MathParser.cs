/* 
 * Copyright (C) 2012-2014 Artem Los,
 * All rights reserved.
 * 
 * Please see the license file in the project folder,
 * or, goto https://mathosparser.codeplex.com/license.
 * 
 * Please feel free to ask me directly at my email or my website form!
 *  artem@artemlos.net
 *  http://artemlos.net/contact/
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Mathos.ILParser
{
    /// <summary>
    /// This is a mathematical expression parser that allows you to parser a string value,
    /// perform the required calculations, and return a value in form of a decimal.
    /// </summary>
    public class ILMathParser
    {
        /// <summary>
        /// This constructor will add some basic operators, functions, and variables
        /// to the parser. Please note that you are able to change that using
        /// boolean flags
        /// </summary>
        /// <param name="loadPreDefinedOperators">This will load "%", "*", ":", "/", "+", "-", ">", "&lt;", "="</param>
        public ILMathParser(bool loadPreDefinedOperators = true)
        {
            if (loadPreDefinedOperators)
            {

                OperatorList.Add("/"); // division 2
                OperatorList.Add("*"); // multiplication
                OperatorList.Add("-"); // subtraction
                OperatorList.Add("+"); // addition

                OperatorList.Add(">"); // greater than
                OperatorList.Add("<"); // less than
                OperatorList.Add("="); // are equal


                OperatorActionIL.Add("/", "call valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal)");
                OperatorAction.Add("/", (x, y) => x - y);          

                OperatorActionIL.Add("*", "call valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal)");
                OperatorAction.Add("*", (x, y) => x - y);               

                OperatorActionIL.Add("-", "call valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Subtraction(valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal)");
                OperatorAction.Add("-", (x, y) => x - y);

                OperatorActionIL.Add("+", "call valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal)");
                OperatorAction.Add("+", (x, y) => x + y);

                OperatorActionIL.Add(">", "call bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal)\r\n" +
                                            "brtrue.s q@\r\n" +
                                            "ldc.i4.0 \r\n" +
                                            "br.s p@\r\n"
                                           + "q@: ldc.i4.1\r\n"
                                           + "p@: nop\r\n"
                                           + "call valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)");
                OperatorAction.Add(">", (x, y) => x > y ? 1 : 0);

                OperatorActionIL.Add("<", "call bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal)\r\n" +
                                            "brtrue.s q@\r\n" +
                                            "ldc.i4.0 \r\n" +
                                            "br.s p@\r\n"
                                           + "q@: ldc.i4.1\r\n"
                                           + "p@: nop\r\n"
                                           + "call valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)");
                OperatorAction.Add("<", (x, y) => x < y ? 1 : 0);

                OperatorActionIL.Add("=", "call bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal)\r\n" +
                                        "brtrue.s q@\r\n" +
                                        "ldc.i4.0 \r\n" +
                                        "br.s p@\r\n"
                                       + "q@: ldc.i4.1\r\n"
                                       + "p@: nop\r\n"
                                       + "call valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)");
                OperatorAction.Add("=", (x, y) => x == y ? 1 : 0);



                //OperatorListIL.Add("+",);
                // by default, we will load basic arithmetic operators.
                // please note, its possible to do it either inside the constructor,
                //// or outside the class. the lowest value will be executed first!
                //OperatorList.Add("%"); // modulo
                //OperatorList.Add("^"); // to the power of
                //OperatorList.Add(":"); // division 1
                //OperatorList.Add("/"); // division 2
                //OperatorList.Add("*"); // multiplication
                //OperatorList.Add("-"); // subtraction
                //OperatorList.Add("+"); // addition

                //OperatorList.Add(">"); // greater than
                //OperatorList.Add("<"); // less than
                //OperatorList.Add("="); // are equal


                //// when an operator is executed, the parser needs to know how.
                //// this is how you can add your own operators. note, the order
                //// in this list does not matter.
                //_operatorAction.Add("%", (numberA, numberB) => numberA % numberB);
                //_operatorAction.Add("^", (numberA, numberB) => (decimal)Math.Pow((double)numberA, (double)numberB));
                //_operatorAction.Add(":", (numberA, numberB) => numberA / numberB);
                //_operatorAction.Add("/", (numberA, numberB) => numberA / numberB);
                //_operatorAction.Add("*", (numberA, numberB) => numberA * numberB);
                //_operatorAction.Add("+", (numberA, numberB) => numberA + numberB);
                //_operatorAction.Add("-", (numberA, numberB) => numberA - numberB);

                //_operatorAction.Add(">", (numberA, numberB) => numberA > numberB ? 1 : 0);
                //_operatorAction.Add("<", (numberA, numberB) => numberA < numberB ? 1 : 0);
                //_operatorAction.Add("=", (numberA, numberB) => numberA == numberB ? 1 : 0);
            }

        }

        private string _outputIL = "";
        public string OutputIL
        {
            get { return _outputIL; }
            set { _outputIL = value; }
        }

        private int stackCount = 0;

        private List<string> _operatorList = new List<string>();
        public List<string> OperatorList
        {
            get { return _operatorList; }
            set { _operatorList = value; }
        }

        /* THE LOCAL VARIABLES */
        private Dictionary<string,string> _operatorActionIL = new Dictionary<string,string>();
        /// <summary>
        /// All operators should be inside this property.
        /// The first operator is executed first, et cetera.
        /// An operator may only be ONE character.
        /// </summary>
        public Dictionary<string,string> OperatorActionIL
        {
            get { return _operatorActionIL; }
            set { _operatorActionIL = value; }
        }

        
        public Dictionary<string, Func<decimal, decimal,decimal>> _operatorAction = new Dictionary<string, Func<decimal, decimal,decimal>>();

        public  Dictionary<string, Func<decimal, decimal, decimal>> OperatorAction
        {
            get { return _operatorAction; }
            set { _operatorAction = value; }
        }
        

        private Dictionary<string, Func<decimal[], decimal>> _localFunctions = new Dictionary<string, Func<decimal[], decimal>>();
        /// <summary>
        /// All functions that you want to define should be inside this property.
        /// </summary>
        public Dictionary<string, Func<decimal[], decimal>> LocalFunctions
        {
            get { return _localFunctions; }
            set { _localFunctions = value; }
        }

        private List<string> _localVariables = new List<string>();
        /// <summary>
        /// All variables that you want to define should be inside this property.
        /// </summary>
        public List<string> LocalVariables
        {
            get { return _localVariables; }
            set { _localVariables = value; }
        }

        private readonly CultureInfo _cultureInfo = CultureInfo.InvariantCulture;
        /// <summary>
        /// When converting the result from the Parse method or ProgrammaticallyParse method ToString(),
        /// please use this cultur info.
        /// </summary>
        public CultureInfo CultureInfo
        {
            get { return _cultureInfo; }
        }


        /* PARSER FUNCTIONS, PUBLIC */
        /// <summary>
        /// Enter the math expression in form of a string.
        /// </summary>
        /// <param name="mathExpression"></param>
        /// <returns></returns>
        public string Parse(string mathExpression)
        {
            MathParserLogic(Scanner(mathExpression));


            var head = @".assembly extern mscorlib {}

.assembly MathosILParser
{
    .ver 1:0:1:0
}
.module MathosILParser.exe

.namespace Mathos
{
.class public abstract auto ansi sealed beforefieldinit ILFunction extends [mscorlib]System.Object
{
.method static void main() cil managed
{

.maxstack 1
.entrypoint

ldstr " + "\""+ " This executable contains an expression that was parsed with Mathos ILParser \\n The function is stored in the method Parse. \\n More info: mathosproject.com/" + "\" \r\n"  + 
@"call void [mscorlib]System.Console::WriteLine(string)
call string [mscorlib]System.Console::ReadLine()
pop
ret
}" + "\r\n";

            string varNames = "";

            for (int i = 0; i < LocalVariables.Count; i++)
            {
                if(i < LocalVariables.Count-1)
                {
                    varNames += "valuetype [mscorlib]System.Decimal " + LocalVariables[i] + ", ";

                }
                else
                {
                    varNames += "valuetype [mscorlib]System.Decimal " + LocalVariables[i];
                }
            }

            string stack = "";

            for (int i = 0; i < stackCount; i++)
            {
                if (i < stackCount - 1)
                {
                    stack += "valuetype [mscorlib]System.Decimal, ";

                }
                else
                {
                    stack += "valuetype [mscorlib]System.Decimal";
                }
            }

            var body = @".method public hidebysig static valuetype [mscorlib]System.Decimal Parse(" + varNames + @") cil managed
{

.maxstack 5
.locals init(" +stack+") \r\n";

            OutputIL = head + body + OutputIL + "\r\nldloc." + (stackCount-1) + "\r\nret \r\n}\r\n}\r\n}";
            return OutputIL;
            //return MathParserLogic(Scanner(mathExpression));
        }


        /* UNDER THE HOOD - THE CORE OF THE PARSER */
        private List<string> Scanner(string expr)
        {
            // SCANNING THE INPUT STRING AND CONVERT IT INTO TOKENS

            var tokens = new List<string>();
            var vector = "";

            //_expr = _expr.Replace(" ", ""); // remove white space
            expr = expr.Replace("+-", "-"); // some basic arithmetical rules
            expr = expr.Replace("-+", "-");
            expr = expr.Replace("--", "+");

            /* foreach (var item in LocalVariables)
             {
                 // replace the current variables with their value
                 _expr = _expr.Replace(item.Key, "(" + item.Value.ToString(CULTURE_INFO) + ")");
             }*/

            for (var i = 0; i < expr.Length; i++)
            {
                var ch = expr[i];

                if (char.IsWhiteSpace(ch)) { } // could also be used to remove white spaces.
                else if (Char.IsLetter(ch))
                {
                    if (i != 0 && (Char.IsDigit(expr[i - 1]) || Char.IsDigit(expr[i - 1]) || expr[i - 1] == ')'))
                    {
                        tokens.Add("*");
                    }

                    vector = vector + ch;

                    while ((i + 1) < expr.Length && Char.IsLetterOrDigit(expr[i + 1])) // here is it is possible to choose whether you want variables that only contain letters with or without digits.
                    {
                        i++;
                        vector = vector + expr[i];
                    }

                    tokens.Add(vector);
                    vector = "";
                }
                else if (Char.IsDigit(ch))
                {
                    vector = vector + ch;

                    while ((i + 1) < expr.Length && (Char.IsDigit(expr[i + 1]) || expr[i + 1] == '.')) // removed || _expr[i + 1] == ','
                    {
                        i++;
                        vector = vector + expr[i];
                    }

                    tokens.Add(vector);
                    vector = "";
                }
                else if ((i + 1) < expr.Length && (ch == '-' || ch == '+') && Char.IsDigit(expr[i + 1]) && (i == 0 || OperatorList.IndexOf(expr[i - 1].ToString(CultureInfo.InvariantCulture)) != -1 || ((i - 1) > 0 && expr[i - 1] == '(')))
                {
                    // if the above is true, then, the token for that negative number will be "-1", not "-","1".
                    // to sum up, the above will be true if the minus sign is in front of the number, but
                    // at the beginning, for example, -1+2, or, when it is inside the brakets (-1).
                    // NOTE: this works for + sign as well!
                    vector = vector + ch;

                    while ((i + 1) < expr.Length && (Char.IsDigit(expr[i + 1]) || expr[i + 1] == '.')) // removed || _expr[i + 1] == ','
                    {
                        i++;
                        vector = vector + expr[i];
                    }

                    tokens.Add(vector);
                    vector = "";
                }
                else if (ch == '(')
                {
                    if (i != 0 && (Char.IsDigit(expr[i - 1]) || Char.IsDigit(expr[i - 1]) || expr[i - 1] == ')'))
                    {
                        tokens.Add("*");
                        // if we remove this line(above), we would be able to have numbers in function names. however, then we can't parser 3(2+2)
                        tokens.Add("(");
                    }
                    else
                        tokens.Add("(");
                }
                else
                {
                    tokens.Add(ch.ToString());
                }
            }

            return tokens;
            //return MathParserLogic(_tokens);
        }

        private void MathParserLogic(List<string> tokens)
        {
            // CALCULATING THE EXPRESSIONS INSIDE THE BRACKETS
            // IF NEEDED, EXECUTE A FUNCTION

            // Variables replacement
            for (var i = 0; i < tokens.Count; i++)
            {
                if (LocalVariables.Contains(tokens[i]))
                {
                    tokens[i] = "ldarg." + LocalVariables.IndexOf(tokens[i]);//LocalVariables[tokens[i]].ToString(CultureInfo);
                }
            }
            while (tokens.IndexOf("(") != -1)
            {
                // getting data between "(", ")"
                var open = tokens.LastIndexOf("(");
                var close = tokens.IndexOf(")", open); // in case open is -1, i.e. no "(" // , open == 0 ? 0 : open - 1

                if (open >= close)
                {
                    // if there is no closing bracket, throw a new exception
                    throw new ArithmeticException("No closing bracket/parenthesis! tkn: " + open.ToString(CultureInfo.InvariantCulture));
                }

                var roughExpr = new List<string>();

                for (var i = open + 1; i < close; i++)
                {
                    roughExpr.Add(tokens[i]);
                }

                string result = ""; // the temporary result is stored here

                var functioName = tokens[open == 0 ? 0 : open - 1];
                var args = new decimal[0];

                if (LocalFunctions.Keys.Contains(functioName))
                {
                    //if (roughExpr.Contains(","))
                    //{
                    //    // converting all arguments into a decimal array
                    //    for (var i = 0; i < roughExpr.Count; i++)
                    //    {
                    //        var firstCommaOrEndOfExpression = roughExpr.IndexOf(",", i) != -1 ? roughExpr.IndexOf(",", i) : roughExpr.Count;
                    //        var defaultExpr = new List<string>();

                    //        while (i < firstCommaOrEndOfExpression)
                    //        {
                    //            defaultExpr.Add(roughExpr[i]);
                    //            i++;
                    //        }

                    //        // changing the size of the array of arguments
                    //        Array.Resize(ref args, args.Length + 1);

                    //        if (defaultExpr.Count == 0)
                    //        {
                    //            args[args.Length - 1] = 0;
                    //        }
                    //        else
                    //        {
                    //            //args[args.Length - 1] = BasicArithmeticalExpression(defaultExpr);
                    //        }
                    //    }

                    //    // finnaly, passing the arguments to the given function
                    //    result = decimal.Parse(LocalFunctions[functioName](args).ToString(CultureInfo), CultureInfo);
                    //}
                    //else
                    //{
                    //    // but if we only have one argument, then we pass it directly to the function
                    //    //result = decimal.Parse(LocalFunctions[functioName](new[] { BasicArithmeticalExpression(roughExpr) }).ToString(CultureInfo), CultureInfo);
                    //}
                }
                else
                {
                    // if no function is need to execute following expression, pass it
                    // to the "BasicArithmeticalExpression" method.
                    result = BasicArithmeticalExpression(roughExpr);
                }

                // when all the calculations have been done
                // we replace the "opening bracket with the result"
                // and removing the rest.
                tokens[open] = result;
                tokens.RemoveRange(open + 1, close - open);
                if (LocalFunctions.Keys.Contains(functioName))
                {
                    // if we also executed a function, removing
                    // the function name as well.
                    tokens.RemoveAt(open - 1);
                }
            }

            // at this point, we should have replaced all brackets
            // with the appropriate values, so we can simply
            // calculate the expression. it's not so complex
            // any more!
            BasicArithmeticalExpression(tokens);
            //return BasicArithmeticalExpression(tokens);
        }

        private string BasicArithmeticalExpression(List<string> tokens)
        {
            // PERFORMING A BASIC ARITHMETICAL EXPRESSION CALCULATION
            // THIS METHOD CAN ONLY OPERATE WITH NUMBERS AND OPERATORS
            // AND WILL NOT UNDERSTAND ANYTHING BEYOND THAT.

            //switch (tokens.Count)
            //{
            //    case 1:
            //        if(!tokens.Contains("ld"))
            //        {
            //            return decimal.Parse(tokens[0]);
            //        }
            //    case 2:
            //        {
            //            var op = tokens[0];

            //            if (op == "-" || op == "+")
            //                return decimal.Parse((op == "+" ? "" : "-") + tokens[1], CultureInfo);

            //            return OperatorAction[op](0, decimal.Parse(tokens[1], CultureInfo));
            //        }
            //    case 0:
            //        return "";
            //}

            //foreach (var op in OperatorList)
            //{
            //    while (tokens.IndexOf(op) != -1)
            //    {
            //        var opPlace = tokens.IndexOf(op);

            //        var numberA = Convert.ToDecimal(tokens[opPlace - 1], CultureInfo);
            //        var numberB = Convert.ToDecimal(tokens[opPlace + 1], CultureInfo);

            //        var result = OperatorAction[op](numberA, numberB);

            //        tokens[opPlace - 1] = result.ToString(CultureInfo);
            //        tokens.RemoveRange(opPlace, 2);
            //    }
            //}

            foreach (var op in OperatorList)
            {

                while (tokens.IndexOf(op) != -1)
                {
                    var opPlace = tokens.IndexOf(op);

                    if (!(tokens[opPlace - 1].Contains("ld") || tokens[opPlace + 1].Contains("ld")))
                    {
                        //both are numbers

                        var numberA = decimal.Parse(tokens[opPlace - 1]);
                        var numberB = decimal.Parse(tokens[opPlace + 1]);

                        var result = OperatorAction[op](numberA, numberB);
                        DecimalToIL(result);
                        tokens[opPlace - 1] = "ldloc." + (stackCount - 1);
                        tokens.RemoveRange(opPlace, 2);

                    }
                    else if (tokens[opPlace - 1].Contains("ld") && tokens[opPlace + 1].Contains("ld"))
                    {
                        OutputIL += tokens[opPlace - 1] + "\r\n";
                        OutputIL += tokens[opPlace + 1] + "\r\n";
                        OutputIL += OperatorActionIL[op].Replace("@", stackCount.ToString("x2")) + "\r\n";
                        //OutputIL += "call valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal)" + "\n";
                        OutputIL += "stloc." + stackCount + "\r\n";
                        tokens[opPlace - 1] = "ldloc." + (stackCount);
                        tokens.RemoveRange(opPlace, 2);
                        stackCount++;
                    }
                    else
                    {
                        if (tokens[opPlace - 1].Contains("ld"))
                        {
                            var numberB = decimal.Parse(tokens[opPlace + 1]);
                            DecimalToIL(numberB); //stack -1

                            OutputIL += tokens[opPlace - 1] + "\r\n";
                            OutputIL += "ldloc." + (stackCount - 1) + "\r\n";
                            OutputIL += OperatorActionIL[op].Replace("@", stackCount.ToString("x2")) + "\r\n";
                            //OutputIL += "call valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal)" + "\n";
                            OutputIL += "stloc." + stackCount + "\r\n";
                            tokens[opPlace - 1] = "ldloc." + (stackCount);
                            tokens.RemoveRange(opPlace, 2);
                            stackCount++;


                        }
                        else if (tokens[opPlace + 1].Contains("ld"))
                        {
                            var numberA = decimal.Parse(tokens[opPlace - 1]);
                            DecimalToIL(numberA); //stack -1

                            OutputIL += "ldloc." + (stackCount - 1) + "\r\n";
                            OutputIL += tokens[opPlace + 1] + "\r\n";
                            OutputIL += OperatorActionIL[op].Replace("@", stackCount.ToString("x2")) + "\r\n";
                            //OutputIL += "call valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,valuetype [mscorlib]System.Decimal)" + "\n";
                            OutputIL += "stloc." + stackCount + "\r\n";
                            tokens[opPlace - 1] = "ldloc." + (stackCount);
                            tokens.RemoveRange(opPlace, 2);
                            stackCount++;
                        }
                    }

                }
            }

            return tokens[0];

            //return Convert.ToDecimal(tokens[0], CultureInfo);
        }
        private void DecimalToIL(decimal x)
        {
            int[] parts = Decimal.GetBits(x);
            bool sign = (parts[3] & 0x80000000) != 0;

            byte scale = (byte)((parts[3] >> 16) & 0x7F);

            OutputIL += "ldc.i4 " + parts[0] + "\r\n";
            OutputIL += "ldc.i4 " + parts[1] + "\r\n";
            OutputIL += "ldc.i4 " + parts[2] + "\r\n";
            OutputIL += "ldc.i4 " + (sign == true ? 1 : 0) + "\r\n";
            OutputIL += "ldc.i4 " + scale + "\r\n";
            OutputIL += "newobj instance void [mscorlib]System.Decimal::.ctor(int32,int32,int32,bool,uint8) \r\n";
            OutputIL += "stloc." + stackCount + "\r\n"; 


            stackCount++;

        }
    }
}