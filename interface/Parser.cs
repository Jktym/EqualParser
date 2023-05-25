using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace @interface
{
    public static class Parser
    {
        private static List<ParseErrors> errors = new List<ParseErrors>();
        public static List<ParseErrors> GetErrors() { return errors; }
        private static List<Token> tokens;
        private static int currentTokenIndex;
        private static bool hasId;
        private static int state;
        private static bool hasEr;
        private static void Error(string msg)
        {
            if (IsNextEmpty())
                errors.Add(new ParseErrors(msg, currentTokenIndex));
            else
                errors.Add(new ParseErrors(msg, currentTokenIndex, next().GetValue()));
        }
        public static void ClearErrorList() { errors.Clear(); ParseErrors.SetLastId(); }

        private static Token next()
        {
            if (currentTokenIndex < tokens.Count && tokens[currentTokenIndex] != null)
                return tokens[currentTokenIndex];
            else return null;
        }
        private static void nextIndex()
        {
            if (currentTokenIndex < tokens.Count)
                currentTokenIndex++;
        }
        private static bool IsNextEmpty()
        {
            return next() == null;
        }
        public static void Parse()
        {
            tokens = Token.GetTokens();
            currentTokenIndex = 0;
            state = 0;
            hasId = false;
            hasEr = false;
            try
            {
                Parsing();
                if (currentTokenIndex >= tokens.Count && !(next() == null))
                    Error("Нежиданный символ в коде.");
            }
            catch { return; }
        }

        private static void Parsing()
        {
            if (next().GetTokenType() == TokenType.Оператор_присваивания)
            {
                Error("Ожидался идентификатор");
            }
            else
            {
                while (currentTokenIndex < tokens.Count && (!Check(TokenType.Оператор_присваивания)))
                {
                    if (hasId && !hasEr)
                    { Error("Слева может быть только идентификатор в 1 экземлпяре"); hasEr = true; }
                    if (Check(TokenType.Идентификатор))
                        Identifier();
                    else if (IsLitter())
                    {
                        Expression();
                        if (!hasId && !hasEr)
                        { Error("Слева может быть только 1 идентификатор"); hasEr = true; }
                    }
                }
            }
            Equal();
            Expression();
        }

        private static void Equal()
        {
            if (!Check(TokenType.Оператор_присваивания))
                Error("Ожидался оператор \"=\"");
            else
                state = 1;
            nextIndex();
        }

        // Идентификатор -> Б {Б | Ц}
        private static void Identifier()
        {
            if(!hasId)
                hasId = true;
            ExpectedTokenCompare(TokenType.Идентификатор);
        }
        // Выражение -> T {+ T} {- T}
        private static void Expression()
        {
            Term();
            while (currentTokenIndex < tokens.Count && (Check(TokenType.Plus) || Check(TokenType.Minus)))
            {
                nextIndex();
                Term();
            }
        }
        private static void Term()
        {
            Operand();
            while (currentTokenIndex < tokens.Count && (Check(TokenType.Mult) || Check(TokenType.Div) || Check(TokenType.Degree)))
            {
                nextIndex();
                Operand();
            }
            if (!IsNextEmpty() && next().GetTokenType() == TokenType.Число_без_знака)
            {
                Error("Пропущен знак между числами");
                nextIndex();
            }
        }

        private static void Operand()
        {
            if (state == 0)
            {
                nextIndex();
                return; 
            }
            if (Check(TokenType.BracketL))
            {
                nextIndex();
                Expression();
                if (!Check(TokenType.BracketR))
                {
                    Error("Ожидался символ закрывающей скобки )");
                    if (currentTokenIndex + 1 < tokens.Count)
                        nextIndex();
                }
                if (currentTokenIndex + 1 <= tokens.Count)
                    nextIndex();
            }
            else if (Check(TokenType.Идентификатор))
                Identifier();
            else if (Check(TokenType.Число_без_знака))
                nextIndex();
            else if(Check(TokenType.Invalid))
            {
                Error("Неизвестный символ");
                nextIndex();
            }    
            else
            {
                Error("Ожидался идентификатор или выражение");
                nextIndex();
            }
        }

        static private void ExpectedTokenCompare(TokenType expectedToken)
        {
            if (currentTokenIndex < tokens.Count)
            {
                if (next().GetTokenType() == expectedToken)
                    nextIndex();
                else
                {
                    Error($"Ожидалась лексема {Convert.ToString(expectedToken)}, получена {Convert.ToString(next().GetTokenType())}");
                    nextIndex();
                }
            }
        }

        private static bool Check(TokenType expectedToken)
        {
            if (currentTokenIndex >= tokens.Count)
                return false;
            return next().GetTokenType() == expectedToken;
        }
        private static bool CheckNext(TokenType expectedToken)
        {
            if(currentTokenIndex >= tokens.Count)
                return false;
            return tokens[currentTokenIndex+1].GetTokenType() == expectedToken;
        }

        private static bool IsLitter()
        {
            return (next().GetTokenType() == TokenType.Число_без_знака 
                    || next().GetTokenType() == TokenType.Plus 
                    || next().GetTokenType() == TokenType.Minus
                    || next().GetTokenType() ==TokenType.Degree || next().GetTokenType() == TokenType.Mult 
                    || next().GetTokenType() == TokenType.BracketL 
                    || next().GetTokenType() == TokenType.BracketR);
        }
    }
}
