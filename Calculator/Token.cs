using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    class Token
    {
        public TokenType type;
        public Associativity assoc;

        public String operand;
        public double value;

        public int precedence;
        public int parameterCount;

        public enum TokenType
        {
            Nothing,
            Variable,
            Number,
            Operator,
            LeftParenth,
            RightParenth
        };

        public enum Associativity
        {
            Left,
            Right
        };

        public Token(TokenType type)
        {
            this.type = type;
        }

        public Token(double number)
        {
            type = TokenType.Number;

            value = number;
        }

        public Token(String operand, Associativity assoc, int precedence, int parameterCount)
        {
            type = TokenType.Operator;

            this.operand = operand;

            this.assoc = assoc;
            this.precedence = precedence;
            this.parameterCount = parameterCount;
        }

        public static Token StringToToken(string str)
        {
            double value;

            if (double.TryParse(str, out value))
            {
                return new Token(value);
            }
            else if (str == "X")
            {
                return new Token(TokenType.Variable);
            }
            else if (str == "π")
            {
                return new Token(Math.PI);
            }
            else
            {
                switch (str)
                {
                    case "(":
                        return new Token(TokenType.LeftParenth);
                    case ")":
                        return new Token(TokenType.RightParenth);
                    case "+":
                        return new Token("+", Associativity.Left, 10, 2);
                    case "-":
                        return new Token("-", Associativity.Left, 10, 2);
                    case "*":
                        return new Token("*", Associativity.Left, 20, 2);
                    case "÷":
                        return new Token("÷", Associativity.Left, 20, 2);
                    case "%":
                        return new Token("%", Associativity.Left, 20, 2);
                    case "√":
                        return new Token("√", Associativity.Right, 30, 1);
                    case "Sin":
                        return new Token("Sin", Associativity.Right, 30, 1);
                    case "Cos":
                        return new Token("Cos", Associativity.Right, 30, 1);
                    case "Tan":
                        return new Token("Tan", Associativity.Right, 30, 1);
                }
            }

            return new Token(TokenType.Nothing);
        }

        public Token Join(Token tok)
        {
            if (type == TokenType.Operator && tok.type == TokenType.Number)
            {
                return Token.StringToToken(operand + tok.value);
            }
            else if (tok.type == TokenType.Operator && type == TokenType.Number)
            {
                return Token.StringToToken(tok.operand + value);
            }

            return null;
        }

        public String GetString()
        {
            return type == TokenType.Number ? value.ToString() : operand;
        }
    }
}