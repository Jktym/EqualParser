using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public enum ErrorType
{
    IdFirstSymb,
    WaitedId,
    WaitedIdOrExp,
    WaitedEqual,
    InvalidSymb,
    EqualMiss,
    InvalidToken
}

namespace @interface
{
    public class ParseErrors
    {
        private int _id { get; }
        private int _tokenStartPos { get; }
        private int _tokenEndPos { get; }

        private string _msg;
        private static int _lastId { get; set; }
        public static void SetLastId() { _lastId = 0; }
        private static string _tokenValue;
        public ParseErrors(ErrorType errorType, int tokenPos)
        {
            if (_lastId == 0)
                _id = _lastId;
            else
                _id = ++_lastId;
            _tokenStartPos = tokenPos;
            _msg = Msg(errorType);

        }

        public ParseErrors(string msg, int tokenStartPos, string value)
        {
            _id = ++_lastId;
            _msg = msg;
            _tokenStartPos = tokenStartPos;
            _tokenValue = value;
        }
        public ParseErrors(string msg, int tokenStartPos)
        {
            _id = ++_lastId;
            _msg = msg;
            _tokenStartPos = tokenStartPos;
        }
        private string Msg(ErrorType errorType)
        {
            switch (errorType)
            {
                case ErrorType.IdFirstSymb:
                    return "Идентификатор должен начинаться с буквы";
                case ErrorType.WaitedId:
                    return "Ожидался идентификатор";
                case ErrorType.InvalidSymb:
                    return "Неизвестный символ";
                case ErrorType.WaitedIdOrExp:
                    return "Ожидался идентификатор или выражение";
                case ErrorType.EqualMiss:
                    return "Пропущен оператор присваивания";
                case ErrorType.WaitedEqual:
                    return "Ожидался оператор присваивания";
                case ErrorType.InvalidToken:
                    return "Некорректный токен";
                default:
                    return "";
            }
        }
        public static string ErrorsOut()
        {
            int i = 0;
            List<ParseErrors> errors = Parser.GetErrors();
            string errorsOut = "";
            while (i < errors.Count)
            {
                if(_tokenValue == null)
                    errorsOut += $"{errors[i]._id}. Ошибка в позиции {errors[i]._tokenStartPos + 1}: {errors[i]._msg}\n";
                else
                    errorsOut += $"{errors[i]._id}. Ошибка в позиции {errors[i]._tokenStartPos + 1}: {errors[i]._msg}\t({_tokenValue})\n";
                i++;
            }
            return errorsOut;
        }
    }
}
