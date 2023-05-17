using ConsoleLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using Xamarin.Forms;
using System.Numerics;

namespace ConsoleLibrary.ConsoleExtensions
{
    public static partial class ConsoleIOExtension
    {
        #region Константы
        private const string PatternArrayCountInfoMessage = "Необходимо ввести {0} значения";
        private const string PatternErrorMessageSizeArray = "Введено {0} элементов, а необходимо {1}";
        private const string PatternErrorMessageParseItemArray = "Ошибка при разборе значения {0}i -> {1}, сообщение: {2}";
        #endregion
        #region Вспомогательные генераторы
        private static GenerateMessagesResult<T[]> GetGeneratorResultArrayMessage<T>(GenerateMessagesResultUseTitle<T[]> generateMessage, string title)
        {
            return (v) => generateMessage?.Invoke(title, v) ?? string.Format(PatternDefaultCreateResultMassages, title, string.Join(", ", v));
        }
        private static string GeneratorNumberArrayHelpMessage<T>(string title, T? startRange, T? endRange, int? count, IEnumerable<string> messages = null)
            where T : struct
        {
            if (messages == null) messages = Enumerable.Empty<string>();
            if (count != null) messages = messages.Append(string.Format(PatternArrayCountInfoMessage, count));
            messages = AppendNumberIntervalInfoToMassage(startRange, endRange, messages);
            return GeneratorHelpMessage(title, messages);
        }
        #endregion
        #region Базовые расширения ввода
        public static Task<T[]> ReadArrayFromText<T>(this IConsole console, string title, GetArrayFromStringValues<T> getResult, int? count = null, GenerateMessagesResult<T[]> getResultMassage = null, OptionReadValue options = OptionReadValue.None, Keyboard keyboard = null, char separator = ' ', IEnumerable<string> defaultsValue = null, CancellationToken? token = null)
        {
            return console.ReadValueFromText(title, (t) =>
            {
                if (string.IsNullOrEmpty(t)) return ResultValue<T[]>.CreateError(ErrorMessageStringIsEmpty);
                if (count != null)
                {
                    int currentCount = t.Count(x => x == separator) + 1;
                    if (currentCount != count) return ResultValue<T[]>.CreateError(string.Format(PatternErrorMessageSizeArray, currentCount, count));
                }
                return getResult.Invoke(t.Split(separator));
            }, getResultMassage, options, keyboard, defaultsValue == null ? string.Empty : string.Join(separator.ToString(), defaultsValue), token);
        }
        public static Task<T[]> ReadArrayNumeric<T>(IConsole console, string title, TryParseNumeric<T> fParse, int? count, T? startRange, T? endRange, T? minValue, T? maxValue, GenerateMessagesResultUseTitle<T[]> getResultMassage, OptionReadValue options, char separator, IEnumerable<T> defaultsValue, CancellationToken? token = null)
        where T : struct, IComparable
        {
            return console.ReadArrayFromText
                (
                    GeneratorNumberArrayHelpMessage(title, startRange, endRange, count),
                    (lines) =>
                    {
                        T[] result = new T[lines.Length];
                        for (int i = 0; i < result.Length; i++)
                        {
                            var res = ParseNumber(lines[i], startRange ?? minValue, endRange ?? maxValue, fParse);
                            if (!res.Validate.State) return ResultValue<T[]>.CreateError(string.Format(PatternErrorMessageParseItemArray, i, lines[i], res.Validate.MessageError));
                            result[i] = res.Value;
                        }
                        return ResultValue<T[]>.CreateResult(result);
                    },
                    count,
                    GetGeneratorResultArrayMessage(getResultMassage, title),
                    options,
                    Keyboard.Text,
                    separator,
                    defaultsValue?.Select(x => x.ToString()),
                    token
                );
        }
        #endregion
        #region Чтение массива чисел
        public static Task<long[]> ReadArrayLong(this IConsole console, string title, long? startRange = null, long? endRange = null, int? count = null, GenerateMessagesResultUseTitle<long[]> getResultMassage = null, OptionReadValue options = OptionReadValue.None, char separator = ' ', IEnumerable<long> defaultsValue = null, CancellationToken? token = null)
        {
            return ReadArrayNumeric(console, title, long.TryParse, count, startRange, endRange, long.MinValue, long.MaxValue, getResultMassage, options, separator, defaultsValue, token);
        }
        public static Task<double[]> ReadArrayDouble(this IConsole console, string title, double? startRange = null, double? endRange = null, int? count = null, GenerateMessagesResultUseTitle<double[]> getResultMassage = null, OptionReadValue options = OptionReadValue.None, char separator = ' ', IEnumerable<double> defaultsValue = null, CancellationToken? token = null)
        {
            return ReadArrayNumeric(console, title, double.TryParse, count, startRange, endRange, double.MinValue, double.MaxValue, getResultMassage, options, separator, defaultsValue, token);
        }
        public static Task<int[]> ReadArrayInt(this IConsole console, string title, int? startRange = null, int? endRange = null, int? count = null, GenerateMessagesResultUseTitle<int[]> getResultMassage = null, OptionReadValue options = OptionReadValue.None, char separator = ' ', IEnumerable<int> defaultsValue = null, CancellationToken? token = null)
        {
            return ReadArrayNumeric(console, title, int.TryParse, count, startRange, endRange, int.MinValue, int.MaxValue, getResultMassage, options, separator, defaultsValue, token);
        }
        public static Task<float[]> ReadArrayFloat(this IConsole console, string title, float? startRange = null, float? endRange = null, int? count = null, GenerateMessagesResultUseTitle<float[]> getResultMassage = null, OptionReadValue options = OptionReadValue.None, char separator = ' ', IEnumerable<float> defaultsValue = null, CancellationToken? token = null)
        {
            return ReadArrayNumeric(console, title, float.TryParse, count, startRange, endRange, float.MinValue, float.MaxValue, getResultMassage, options, separator, defaultsValue, token);
        }
        public static Task<BigInteger[]> ReadArrayBigInteger(this IConsole console, string title, BigInteger? startRange = null, BigInteger? endRange = null, int? count = null, GenerateMessagesResultUseTitle<BigInteger[]> getResultMassage = null, OptionReadValue options = OptionReadValue.None, char separator = ' ', IEnumerable<BigInteger> defaultsValue = null, CancellationToken? token = null)
        {
            return ReadArrayNumeric(console, title, BigInteger.TryParse, count, startRange, endRange, null, null, getResultMassage, options, separator, defaultsValue, token);
        }
        #endregion
    }
}
