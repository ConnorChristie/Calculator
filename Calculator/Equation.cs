using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Calculator
{
    class Equation
    {
        private List<Token> tokens = new List<Token>();

        private Stack<Token> tokenStack = new Stack<Token>();
        private List<Token> outputQueue = new List<Token>();

        public String equation { get; set; }

        public EquationHandler equationHandler;

        private String errorString = "";

        public Equation(String equation, EquationHandler equationHandler)
        {
            this.equation = equation;
            this.equationHandler = equationHandler;

            Tokenize();
            EvaluateTokens();
        }

        private void Tokenize()
        {
            equation = equation.Replace("X", " X ");

            equation = equation.Replace("(", " ( ");
            equation = equation.Replace(")", " ) ");

            equation = equation.Replace("+", " + ");
            equation = equation.Replace("-", " - ");

            equation = equation.Replace("*", " * ");
            equation = equation.Replace("÷", " ÷ ");
            equation = equation.Replace("%", " % ");
            equation = equation.Replace("√", " √ ");
            equation = equation.Replace("π", " π ");

            equation = equation.Replace("Sin", " Sin ");
            equation = equation.Replace("Cos", " Cos ");
            equation = equation.Replace("Tan", " Tan ");

            string[] splitString = equation.Replace("  ", " ").Split(' ');

            int leftParenthCount = 0;
            int rightParenthCount = 0;

            for (int i = 0; i < splitString.Length; i++)
            {
                Token token = Token.StringToToken(splitString[i]);

                if (token.type == Token.TokenType.Operator && token.operand == "-")
                {
                    if (i == 0 || (tokens.Count >= 1 && tokens.ElementAt(tokens.Count - 1).type != Token.TokenType.Number))
                    {
                        if (i + 1 < splitString.Length)
                        {
                            tokens.Add(token.Join(Token.StringToToken(splitString[i + 1])));

                            i++;

                            continue;
                        }
                    }
                }
                else if (i != 0 && token.type == Token.TokenType.LeftParenth && tokens[tokens.Count - 1].type == Token.TokenType.Number)
                {
                    tokens.Add(Token.StringToToken("*"));
                }
                else if (i != 0 && token.type == Token.TokenType.Number && tokens[tokens.Count - 1].type == Token.TokenType.RightParenth)
                {
                    tokens.Add(Token.StringToToken("*"));
                }
                else if (i != 0 && token.type == Token.TokenType.Variable && tokens[tokens.Count - 1].type == Token.TokenType.Number)
                {
                    tokens.Add(Token.StringToToken("*"));
                }
                else if (i != 0 && token.type == Token.TokenType.Number && tokens[tokens.Count - 1].type == Token.TokenType.Variable)
                {
                    tokens.Add(Token.StringToToken("*"));
                }

                if (token.type == Token.TokenType.LeftParenth)
                {
                    leftParenthCount++;
                } else if (token.type == Token.TokenType.RightParenth)
                {
                    rightParenthCount++;
                }

                tokens.Add(Token.StringToToken(splitString[i]));
            }

            if (leftParenthCount != rightParenthCount || equation.Contains("□"))
            {
                errorString = "Error";
            }
        }

        private void EvaluateTokens()
        {
            if (errorString != "")
            {
                return;
            }

            tokenStack.Clear();
            outputQueue.Clear();

            foreach (Token token in tokens)
            {
                if (token.type == Token.TokenType.Number || token.type == Token.TokenType.Variable)
                {
                    outputQueue.Add(token);
                }
                else if (token.type == Token.TokenType.Operator)
                {
                    while (tokenStack.Count > 0)
                    {
                        Token tok = tokenStack.Peek();

                        if (tok.precedence >= token.precedence)
                        {
                            outputQueue.Add(tokenStack.Pop());
                        }
                        else
                            break;
                    }

                    tokenStack.Push(token);
                }
                else if (token.type == Token.TokenType.LeftParenth)
                {
                    tokenStack.Push(token);
                }
                else if (token.type == Token.TokenType.RightParenth)
                {
                    while (tokenStack.Count > 0)
                    {
                        Token tok = tokenStack.Pop();

                        if (tok.type != Token.TokenType.LeftParenth)
                        {
                            outputQueue.Add(tok);
                        }
                        else if (tok.type == Token.TokenType.LeftParenth)
                        {
                            break;
                        }
                    }
                }
            }

            while (tokenStack.Count > 0)
            {
                outputQueue.Add(tokenStack.Pop());
            }
        }

        public String EquateTokens(double varX)
        {
            if (errorString != "")
            {
                return errorString;
            }

            Stack<Token> outputStack = new Stack<Token>();

            foreach (Token token in outputQueue)
            {
                if (token.type == Token.TokenType.Variable)
                {
                    if (varX != double.NaN && token.type == Token.TokenType.Variable)
                    {
                        token.value = varX;
                    }
                    else if (varX == double.NaN && token.type == Token.TokenType.Variable)
                    {
                        return "Error";
                    }

                    outputStack.Push(token);
                }
            }

            foreach (Token token in outputQueue)
            {
                if (token.type == Token.TokenType.Number)
                {
                    outputStack.Push(token);
                }
                else if (token.type == Token.TokenType.Operator)
                {
                    double tokenAnswer = 0;

                    Token tok1 = outputStack.Pop();
                    Token tok2 = null;

                    if (token.parameterCount == 1 && token.assoc == Token.Associativity.Left)
                    {
                        if (outputStack.Count == 0)
                        {
                            return "Error";
                        }
                        else
                        {
                            Token temp = outputStack.Pop();

                            outputStack.Push(tok1);

                            tok1 = temp;
                        }
                    }
                    else if (token.parameterCount == 2)
                    {
                        if (outputStack.Count == 0)
                        {
                            return "Error";
                        }
                        else
                        {
                            tok2 = outputStack.Pop();
                        }
                    }

                    double value = 0;

                    switch (token.operand)
                    {
                        case "+":
                            tokenAnswer = tok2.value + tok1.value;

                            break;
                        case "-":
                            tokenAnswer = tok2.value - tok1.value;

                            break;
                        case "*":
                            tokenAnswer = tok2.value * tok1.value;

                            break;
                        case "÷":
                            tokenAnswer = tok2.value / tok1.value;

                            break;
                        case "%":
                            tokenAnswer = tok2.value % tok1.value;

                            break;
                        case "√":
                            tokenAnswer = Math.Sqrt(tok1.value);

                            break;
                        case "Sin":
                            value = equationHandler.isDegrees ? Math.PI * tok1.value / 180 : tok1.value;

                            tokenAnswer = Math.Sin(value);

                            break;
                        case "Cos":
                            value = equationHandler.isDegrees ? Math.PI * tok1.value / 180 : tok1.value;

                            tokenAnswer = Math.Cos(value);

                            break;
                        case "Tan":
                            value = equationHandler.isDegrees ? Math.PI * tok1.value / 180 : tok1.value;

                            tokenAnswer = Math.Tan(value);

                            break;
                    }

                    outputStack.Push(new Token(tokenAnswer));
                }
            }

            if (outputStack.Count > 1)
            {
                return "Error";
            }

            String outputString = "";

            foreach (Token token in outputStack)
            {
                outputString += " " + token.GetString();
            }

            return outputString;
        }

        public string SolveEquation()
        {
            return EquateTokens(double.NaN);
        }

        public string SolveEquation(double varX)
        {
            return EquateTokens(varX);
        }

        public String SolveEquationDouble(double varX)
        {
            return EquateTokens(varX);
        }
    }
}
