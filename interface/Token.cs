using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace @interface
{
    public class Token
    {
        private string _value;
        public string GetValue() { return this._value; }
        private TokenType _tokenType;
        public TokenType GetTokenType() { return this._tokenType; }
        private int _index;
        public int GetIndex() { return this._index; }
        private int _posStart;
        public int GetPosStart() { return _posStart; }
        private int _posEnd;
        public int GetPosEnd() { return _posEnd; }
        public static void TokensClear()
        {
            _tokens.Clear();
        }

        private static List<Token> _tokens;
        public static List<Token> GetTokens() { return _tokens; }
        public Token(TokenType tokenType, string value, int index, int posStart, int posEnd)
        {
            if (_tokens == null)
                _tokens = new List<Token>();
            _value = value;
            _tokenType = tokenType;
            _index = index;
            _posStart = posStart;
            _posEnd = posEnd;
            _tokens.Add(this);
        }
    }
}
