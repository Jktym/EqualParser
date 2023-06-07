using @interface;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml.Linq;

public static class LexicalAnalyzer
{
    // Словарь для хранения ключевых слов
    private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
    {
        { "var", TokenType.Ключевое_слово },
        { "int", TokenType.Ключевое_слово },
        { "real", TokenType.Ключевое_слово },
        { "begin", TokenType.Ключевое_слово },
        { "end", TokenType.Ключевое_слово }
    };
    // Метод для проверки, является ли символ буквой
    private static bool IsLetter(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    // Метод для проверки, является ли символ цифрой
    private static bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    public static void Tokenize(string input)
    {
        if (Token.GetTokens() != null)
            Token.TokensClear();
        char c;
        int line = 0;
        int pos = 0;
        int count = 0;
        int startPos;
        while (pos < input.Length)
        {
            c = input[pos];
            // Если текущий символ является разделителем, пропускаем его
            if (char.IsWhiteSpace(c))
            {
                // Если это пробел или перевод строки, добавляем соответствующую лексему
                if (c == ' ')
                {
                    new Token(TokenType.Разделитель, "Пробел", count++, pos, pos);
                }
                else if (c == '\n')
                {
                    new Token(TokenType.Конец_строки, "Конец строки", count++, pos, pos);
                    line++;
                }
                else if (c == '\t')
                {
                    new Token(TokenType.Разделитель, "Таб", count++, pos, pos);
                }
                pos++;
                continue;
            }

            // Если текущий символ является оператором присваивания (=)
            if (c == '=')
            {
                new Token(TokenType.Оператор_присваивания, "=", count++, pos, pos);
                pos++;
                continue;
            }
            // Если текущий символ является концом оператора (;)
            if (c == ';')
            {
                new Token(TokenType.Разделитель, ";", count++, pos, pos);
                pos++;
                continue;
            }

            // Если текущий символ является скобкой [(] - открытие скобки или комментария
            if (c == '(')
            {
                startPos = pos;
                if (pos + 1 < input.Length && input[pos+1] == '*')
                {
                    string comment = "(*";
                    pos += 2;
                    while (pos < input.Length && input[pos] != '*')
                    {
                        comment += input[pos];
                        pos++;
                    }
                    if (input[pos] == '*')
                    { comment += "*";
                        if (input[pos+1] == ')')
                        {
                            comment += ")";
                            new Token(TokenType.Комментарий, comment, count++, startPos, pos);
                        }
                    }
                    else
                    {
                        new Token(TokenType.Неизвестный_токен, comment, count++, startPos, pos);
                    }
                }
                else
                {
                    string expr = "(";
                    pos++;
                    // Пока не найдем закрывающую скобку 
                    while (pos < input.Length && input[pos] != ')')
                    {
                        expr += input[pos];
                        pos++;
                    }

                    // Если нашли закрывающую скобку, то добавляем лексему в список
                    if (pos < input.Length && input[pos] == ')')
                    {
                        expr += ")";
                        new Token(TokenType.Выражение, expr, count++, startPos, pos);
                        pos++;
                    }

                    else // Иначе скобка не была закрыта - это ошибка
                    { 
                        new Token(TokenType.Неизвестный_токен, expr, count++, startPos, pos);
                        pos++;
                    }
                }
                continue;
            }

            // Если текущий символ является буквой, то это может быть ключевое слово или идентификатор
            if (IsLetter(c))
            {
                startPos = pos;
                string word = "";
                // Пока текущий символ является буквой или цифрой, добавляем его к слову
                while (pos < input.Length && (IsLetter(input[pos]) || IsDigit(input[pos])))
                {
                    word += input[pos];
                    pos++;
                }

                // Если слово является ключевым, то добавляем лексему в список
                if (keywords.ContainsKey(word))
                {
                    new Token(TokenType.Ключевое_слово, word, count++, startPos, pos);
                }
                else // Иначе это идентификатор
                {
                    new Token(TokenType.Идентификатор, word, count++, startPos, pos);
                }

                continue;
            }

            if (IsDigit(c))
            {
                startPos = pos;
                bool hasPoint = false;
                bool invalid = false;
                string number = "";
                while (pos < input.Length && (IsDigit(input[pos]) || (!hasPoint && (input[pos] == '.' || input[pos] == ','))))
                {
                    if (input[pos] == '.' || input[pos] == ',')
                    { hasPoint = true; if (!IsDigit(input[pos])) invalid = true; }
                    pos++;
                }
                if (invalid)
                    new Token(TokenType.Неизвестный_токен, number , count++, startPos, pos);
                else if(hasPoint)
                    new Token(TokenType.Вещественное_число, number, count++, startPos, pos);
                else
                    new Token(TokenType.Целое_число, number, count++, startPos, pos);
                continue;
            }
            else
            {
                startPos = pos;
                string invalid = "";
                while(pos < input.Length && (!IsDigit(input[pos]) || !IsLetter(input[pos])))
                {
                    invalid += input[pos];
                    pos++;
                }
                new Token(TokenType.Неизвестный_токен, invalid, count++, startPos, pos);
                continue;
            }
        }
    }

}

