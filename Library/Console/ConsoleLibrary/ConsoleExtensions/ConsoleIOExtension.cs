using ConsoleLibrary.Extensions;
using ConsoleLibrary.Interfaces;
using CSharpMath.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ConsoleLibrary.Tools;

using Xamarin.Forms;

namespace ConsoleLibrary.ConsoleExtensions
{
    public static partial class ConsoleIOExtension
    {
        #region Константы
        public const string PatternDefaultCreateResultMassages = "{0} -> {1}";
        public const string PatternNumberIntervalStartRangeInfoMessage = "Значение не должно быть меньше {0}";
        public const string PatternNumberIntervalEndRangeInfoMessage = "Значение не должно быть больше {0}";
        public const string PatternNumberIntervalStartRangeEndRangeInfoMessage = "Значение должно находиться в интервале от {0} до {1}";

        public const string ErrorMessageStringIsEmpty = "Введена пустая строка";
        public const string PatternErrorMessageIntervalViolation = "Значение [{0}] должно находиться в диапазоне от {1} до {2}";
        public const string PatternErrorParseNumber = "Введённое выражение [{0}] не является числом";
        #endregion
        #region Стили
        public readonly static string InputGridStyle = Styles.CreateNameStyleConsoleLibrary();
        public readonly static string InputDecorationStyle = Styles.CreateNameStyleConsoleLibrary();

        public readonly static string LabelStyle = Styles.CreateNameStyleConsoleLibrary();
        public readonly static string LaTeXStyle = Styles.CreateNameStyleConsoleLibrary();
        public readonly static string TitleFontSize = Styles.CreateNameStyleConsoleLibrary();
        public readonly static string TitleLaTeXFontSize = Styles.CreateNameStyleConsoleLibrary();
        public readonly static string ColorError = Styles.CreateNameStyleConsoleLibrary();

        public readonly static string EditorStyle = Styles.CreateNameStyleConsoleLibrary();
        public readonly static string ButtonWriteLineStyle = Styles.CreateNameStyleConsoleLibrary();

        public readonly static string ButtonAnswerOkStyle = Styles.CreateNameStyleConsoleLibrary();
        public readonly static string ButtonAnswerNoStyle = Styles.CreateNameStyleConsoleLibrary();
        #endregion
        #region Делегаты
        public delegate ResultValue<T> GetValueFromView<T, V>(V view);
        public delegate ResultValue<T> GetValueFromText<T>(string text);
        public delegate ResultValue<T[]> GetArrayFromStringValues<T>(string[] strValues);
        public delegate ResultValidate CheckValidate<T>(T text);
        public delegate string GenerateMessagesResult<T>(T result);
        public delegate string GenerateMessagesResultUseTitle<T>(string title, T result);

        public delegate bool TryParseNumeric<T>(string text, out T value);
        #endregion
        #region Данные
        public struct ResultValidate
        {
            public bool State { get; private set; }
            public string MessageError { get; private set; }

            public static ResultValidate CreateResult() => new ResultValidate()
            {
                State = true,
                MessageError = null
            };
            public static ResultValidate CreateError(string messageError) => new ResultValidate()
            {
                State = false,
                MessageError = messageError
            };
        }
        public struct ResultValue<T>
        {
            public ResultValidate Validate { get; private set; }
            public T Value { get; private set; }

            public static ResultValue<T> CreateResult(T value) => new ResultValue<T>()
            {
                Value = value,
                Validate = ResultValidate.CreateResult()
            };
            public static ResultValue<T> CreateError(string messageError) => new ResultValue<T>()
            {
                Validate = ResultValidate.CreateError(messageError)
            };
            public static ResultValue<T> CreateResult(T value, ResultValidate validate) => new ResultValue<T>()
            {
                Value = value,
                Validate = validate
            };
        }
        public class LabelEditor
        {
            public TypeLabel Type { get; private set; }
            public string Content
            {
                get => GetContent().Result;
                set => SetContent(value);
            }
            private readonly View _view;
            private readonly Func<Action, Task> _uiTheard;


            public async Task<string> GetContent()
            {
                string result = null;
                switch (Type)
                {
                    case TypeLabel.Text: await _uiTheard.Invoke(() => result = (string)_view.GetValue(Label.TextProperty)); return result;
                    case TypeLabel.LaTeX: await _uiTheard.Invoke(() => result = (string)_view.GetValue(TextView.LaTeXProperty)); return result;
                    default:
                    case TypeLabel.Unrecognized: throw new Exception("Тип не распознан");
                }
            }
            public Task SetContent(string value)
            {
                switch (Type)
                {
                    case TypeLabel.Text: return _uiTheard.Invoke(() => _view.SetValue(Label.FormattedTextProperty, FormattedStringExtension.Create(value)));
                    case TypeLabel.LaTeX: return _uiTheard.Invoke(() => _view.SetValue(TextView.LaTeXProperty, value));
                    default:
                    case TypeLabel.Unrecognized: throw new Exception("Тип не распознан или не установлен обьект");
                }
            }

            public LabelEditor(View view, Func<Action, Task> uiTheard)
            {
                _view = view;
                _uiTheard = uiTheard;
                Type = view is Label ? TypeLabel.Text : (view is TextView ? TypeLabel.LaTeX : TypeLabel.Unrecognized);
            }

            public enum TypeLabel : byte
            {
                Unrecognized,
                Text,
                LaTeX
            }
        }
        #endregion
        #region Перечисления
        [Flags]
        public enum TextStyle : byte
        {
            None = 0,
            UseLaTeX = 1,
            IsTitle = 2,
            IsError = 4
        }
        [Flags]
        public enum OptionReadValue : byte
        {
            None = 0,
            UseLaTeX = 1,
            IsLeftResultMessage = 2
        }
        #endregion
        #region Вспомогательные генераторы
        private static GenerateMessagesResult<T> GetGeneratorResultMessage<T>(GenerateMessagesResultUseTitle<T> generateMessage, string title)
        {
            return (v) => generateMessage?.Invoke(title, v) ?? string.Format(PatternDefaultCreateResultMassages, title, v);
        }
        private static string GeneratorHelpMessage(string title, IEnumerable<string> messages = null)
        {
            if (messages == null || !messages.Any()) return title;
            return string.Format("{0}\n{1}", title, string.Join("\n", messages));
        }
        private static IEnumerable<string> AppendNumberIntervalInfoToMassage<T>(T? startRange, T? endRange, IEnumerable<string> messages = null) where T : struct
        {
            if (messages == null) messages = Enumerable.Empty<string>();
            if (startRange == null && endRange != null) return messages.Append(string.Format(PatternNumberIntervalEndRangeInfoMessage, endRange));
            if (startRange != null && endRange == null) return messages.Append(string.Format(PatternNumberIntervalStartRangeInfoMessage, startRange));
            if (startRange != null && endRange != null) return messages.Append(string.Format(PatternNumberIntervalStartRangeEndRangeInfoMessage, startRange, endRange));
            return messages;
        }
        #endregion
        #region Парсеры чисел
        private static ResultValue<T> ParseNumber<T>(string text, T startRange, T endRange, TryParseNumeric<T> fGet) where T : IComparable
        {
            if (string.IsNullOrWhiteSpace(text)) return ResultValue<T>.CreateError(ErrorMessageStringIsEmpty);
            if (fGet.Invoke(text, out T result))
            {
                if (startRange.CompareTo(result) <= 0 && endRange.CompareTo(result) >= 0) return ResultValue<T>.CreateResult(result);
                else return ResultValue<T>.CreateError(string.Format(PatternErrorMessageIntervalViolation, result, startRange, endRange));
            }
            else
            {
                return ResultValue<T>.CreateError(string.Format(PatternErrorParseNumber, text));
            }
        }
        #endregion
        #region Генераторы UI элементов
        public static View CreateLabel(string text, TextStyle textStyle = TextStyle.None)
        {
            View view;
            bool isUseLaTeX = textStyle.HasFlag(TextStyle.UseLaTeX);
            bool isErrorStyle = textStyle.HasFlag(TextStyle.IsError);
            bool isTitle = textStyle.HasFlag(TextStyle.IsTitle);
            if (!isUseLaTeX)
            {
                view = new Label()
                {
                    FormattedText = FormattedStringExtension.Create(text),
                    HorizontalOptions = isTitle ? LayoutOptions.FillAndExpand : LayoutOptions.Start,
                    HorizontalTextAlignment = isTitle ? TextAlignment.Center : TextAlignment.Start
                };
                view.SetDynamicResource(VisualElement.StyleProperty, LabelStyle);
                if (isTitle) view.SetDynamicResource(Label.FontSizeProperty, TitleFontSize);
                if (isErrorStyle) view.SetDynamicResource(Label.TextColorProperty, ColorError);
            }
            else
            {
                view = new TextView()
                {
                    LaTeX = text.Replace("\n", "\\\\"),
                    HorizontalOptions = isTitle ? LayoutOptions.FillAndExpand : LayoutOptions.Start,
                    TextAlignment = isTitle ? CSharpMath.Rendering.FrontEnd.TextAlignment.Center : CSharpMath.Rendering.FrontEnd.TextAlignment.Left,
                    LineStyle = CSharpMath.Atom.LineStyle.Display
                };
                view.SetDynamicResource(VisualElement.StyleProperty, LaTeXStyle);
                if (isTitle) view.SetDynamicResource(TextView.FontSizeProperty, TitleLaTeXFontSize);
                if (isErrorStyle) view.SetDynamicResource(TextView.TextColorProperty, ColorError);
            }
            return view;
        }
        public static Editor CreateEditor(string textDefault = null, Keyboard keyboard = null)
        {
            Editor entry = new Editor()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                AutoSize = EditorAutoSizeOption.TextChanges,
                Keyboard = keyboard ?? Keyboard.Text,
                Text = textDefault ?? string.Empty
            };
            entry.SetDynamicResource(VisualElement.StyleProperty, EditorStyle);
            return entry;
        }
        #endregion
        #region Расширения вывода
        public static Task WriteLine(this IConsole console, string text, TextStyle textStyle = TextStyle.None) => console.AddUIElement(CreateLabel(text, textStyle));
        public static async Task<LabelEditor> WriteLineReturnEditor(this IConsole console, string text, TextStyle textStyle = TextStyle.None)
        {
            var view = CreateLabel(text, textStyle);
            await console.AddUIElement(view);
            return new LabelEditor(view, console.InvockeUITheardAction);
        }
        #endregion
        #region Базовые расширения ввода
        public static async Task<Task<T>> ReadValue<T, V>(this IConsole console, string title, V view, GetValueFromView<T, V> getResult, GenerateMessagesResult<T> getResultMassage = null, OptionReadValue options = OptionReadValue.None, CancellationToken? token = null) where V : View
        {
            if (getResultMassage == null) getResultMassage = (v) => v.ToString();
            ContentView presenter = new ContentView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };
            Button button = new Button()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center
            };
            button.SetDynamicResource(VisualElement.StyleProperty, ButtonWriteLineStyle);
            View labelPlacholder = CreateLabel(title, options.HasFlag(OptionReadValue.UseLaTeX) ? TextStyle.UseLaTeX : TextStyle.None);
            Label labelValidInfo = (Label)CreateLabel("", TextStyle.IsError);
            labelValidInfo.IsEnabled = false;
            TaskCompletionSource<T> taskCompletion = new TaskCompletionSource<T>(token);
            Grid.SetColumn(labelPlacholder, 0);
            Grid.SetColumnSpan(labelPlacholder, 2);
            Grid.SetRow(labelValidInfo, 1);
            Grid.SetColumn(labelValidInfo, 0);
            Grid.SetColumnSpan(labelValidInfo, 2);
            Grid.SetRow(view, 2);
            Grid.SetRow(button, 2);
            Grid.SetColumn(view, 0);
            Grid.SetColumn(button, 1);
            var grid = new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = GridLength.Auto }
                },
                RowDefinitions =
                {
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto }
                },
                Children =
                {
                    labelPlacholder,
                    labelValidInfo,
                    view,
                    button
                },
            };
            grid.SetDynamicResource(VisualElement.StyleProperty, InputGridStyle);
            Frame frame = new Frame() { Content = grid };
            frame.SetDynamicResource(VisualElement.StyleProperty, InputDecorationStyle);
            presenter.Content = frame;
            button.Clicked += (d, dd) =>
            {
                var result = getResult.Invoke(view);
                if (result.Validate.State)
                {
                    var insertNewContent = CreateLabel(getResultMassage.Invoke(result.Value), options.HasFlag(OptionReadValue.UseLaTeX) ? TextStyle.UseLaTeX : TextStyle.None);
                    insertNewContent.HorizontalOptions = options.HasFlag(OptionReadValue.IsLeftResultMessage) ? LayoutOptions.Start : LayoutOptions.End;
                    presenter.Content = insertNewContent;
                    presenter.HorizontalOptions = options.HasFlag(OptionReadValue.IsLeftResultMessage) ? LayoutOptions.Start : LayoutOptions.End;
                    taskCompletion.SetResult(result.Value);
                }
                else
                {
                    labelValidInfo.Text = result.Validate.MessageError;
                    labelValidInfo.IsEnabled = true;
                }
            };
            await console.AddUIElement(presenter);
            return taskCompletion.WaitAsyncResultUseCancellationToken(token);
        }
        public static async Task<T> ReadValueFromText<T>(this IConsole console, string title, GetValueFromText<T> getResult, GenerateMessagesResult<T> getResultMassage = null, OptionReadValue options = OptionReadValue.None, Keyboard keyboard = null, string defaultValue = null, CancellationToken? token = null)
        {
            Editor entry = CreateEditor(defaultValue, keyboard);
            var result = await console.ReadValue(title, entry, (e) => getResult.Invoke(e.Text), getResultMassage, options, token);
            await console.InvockeUITheardAction(() => entry.Focus());
            return await result;
        }
        public static Task<T> ReadNumber<T>(this IConsole console, string title, T? startRange, T? endRange, T minValue, T maxValue, TryParseNumeric<T> fParse, GenerateMessagesResultUseTitle<T> getResultTitle, OptionReadValue options, string defaultValue = null, CancellationToken? token = null)
        where T : struct, IComparable
        {
            return console.ReadValueFromText
                (
                    GeneratorHelpMessage(title, AppendNumberIntervalInfoToMassage(startRange, endRange)),
                    (text) => ParseNumber<T>(text, startRange ?? minValue, endRange ?? maxValue, fParse),
                    GetGeneratorResultMessage(getResultTitle, title),
                    options,
                    Keyboard.Numeric,
                    defaultValue,
                    token
                );
        }
        #endregion
        #region Операции чтения
        public static async Task<bool> ReadBool(this IConsole console, string title, OptionReadValue options = OptionReadValue.None, GenerateMessagesResultUseTitle<bool> getResultMassage = null, CancellationToken? token = null)
        {
            var generatorMassegaResult = GetGeneratorResultMessage(getResultMassage, title);
            ContentView presenter = new ContentView()
            {
                HorizontalOptions = LayoutOptions.Start,
            };
            Button btnOk = new Button();
            Button btnNo = new Button();
            btnOk.SetDynamicResource(VisualElement.StyleProperty, ButtonAnswerOkStyle);
            btnNo.SetDynamicResource(VisualElement.StyleProperty, ButtonAnswerNoStyle);
            View labelPlacholder = CreateLabel(title, options.HasFlag(OptionReadValue.UseLaTeX) ? TextStyle.UseLaTeX : TextStyle.None);
            presenter.Content = new StackLayout()
            {
                Children =
                {
                    labelPlacholder,
                    new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            btnOk,
                            btnNo
                        }
                    }
                }
            };

            TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();

#pragma warning disable IDE0039 // Использовать локальную функцию
            Action<bool> clickButton = (bool state) =>
            {
                var insertNewContent = CreateLabel(generatorMassegaResult.Invoke(state), options.HasFlag(OptionReadValue.UseLaTeX) ? TextStyle.UseLaTeX : TextStyle.None);
                insertNewContent.HorizontalOptions = options.HasFlag(OptionReadValue.IsLeftResultMessage) ? LayoutOptions.Start : LayoutOptions.End;
                presenter.Content = insertNewContent;
                presenter.HorizontalOptions = options.HasFlag(OptionReadValue.IsLeftResultMessage) ? LayoutOptions.Start : LayoutOptions.End;
                result.SetResult(state);
            };
#pragma warning restore IDE0039 // Использовать локальную функцию
            btnNo.Clicked += (d, dd) => clickButton.Invoke(false);
            btnOk.Clicked += (d, dd) => clickButton.Invoke(true);
            await console.AddUIElement(presenter);

            return await result.WaitAsyncResultUseCancellationToken(token);
        }
        public static Task<string> ReadLine(this IConsole console, string title, OptionReadValue options = OptionReadValue.None, GenerateMessagesResultUseTitle<string> getResultMassage = null, CheckValidate<string> validF = null, Keyboard keyboard = null, string defaultValue = null, CancellationToken? token = null)
        {
            return console.ReadValueFromText(title, (t) =>
            {
                if (validF == null) return ResultValue<string>.CreateResult(t);
                ResultValidate resultValidate = validF.Invoke(t);
                if (resultValidate.State)
                    return ResultValue<string>.CreateResult(t);
                return ResultValue<string>.CreateError(resultValidate.MessageError);
            }, GetGeneratorResultMessage(getResultMassage, title), options, keyboard, defaultValue, token);
        }
        public static async Task<Color> ReadColor(this IConsole console, string title, OptionReadValue options = OptionReadValue.None, GenerateMessagesResultUseTitle<Color> getResultMassage = null, CheckValidate<Color> validF = null, Color? defaultValue = null, CancellationToken? token = null)
        {
            return await await console.ReadValue
                (
                    title,
                    new Views.PickColor(defaultValue),
                    (view) =>
                    {
                        if (validF == null) return ResultValue<Color>.CreateResult(view.Color);
                        var result = validF.Invoke(view.Color);
                        if (result.State) return ResultValue<Color>.CreateResult(view.Color);
                        return ResultValue<Color>.CreateError(result.MessageError);
                    }, GetGeneratorResultMessage(getResultMassage, title), options, token
                );
        }

        public static Task<long> ReadLong(this IConsole console, string title, long? startRange = null, long? endRange = null, GenerateMessagesResultUseTitle<long> getResultTitle = null, OptionReadValue options = OptionReadValue.None, long? defaultValue = null, CancellationToken? token = null)
        {
            return console.ReadNumber(title, startRange, endRange, long.MinValue, long.MaxValue, long.TryParse, getResultTitle, options, defaultValue?.ToString(), token);
        }
        public static Task<ulong> ReadULong(this IConsole console, string title, ulong? startRange = null, ulong? endRange = null, GenerateMessagesResultUseTitle<ulong> getResultTitle = null, OptionReadValue options = OptionReadValue.None, ulong? defaultValue = null, CancellationToken? token = null)
        {
            return console.ReadNumber(title, startRange ?? ulong.MinValue, endRange, ulong.MinValue, ulong.MaxValue, ulong.TryParse, getResultTitle, options, defaultValue?.ToString(), token);
        }
        public static Task<int> ReadInt(this IConsole console, string title, int? startRange = null, int? endRange = null, GenerateMessagesResultUseTitle<int> getResultTitle = null, OptionReadValue options = OptionReadValue.None, int? defaultValue = null, CancellationToken? token = null)
        {
            return console.ReadNumber(title, startRange, endRange, int.MinValue, int.MaxValue, int.TryParse, getResultTitle, options, defaultValue?.ToString(), token);
        }
        public static Task<double> ReadDouble(this IConsole console, string title, double? startRange = null, double? endRange = null, GenerateMessagesResultUseTitle<double> getResultTitle = null, OptionReadValue options = OptionReadValue.None, double? defaultValue = null, CancellationToken? token = null)
        {
            return console.ReadNumber(title, startRange, endRange, double.MinValue, double.MaxValue, double.TryParse, getResultTitle, options, defaultValue?.ToString(), token);
        }
        public static Task<float> ReadFloat(this IConsole console, string title, float? startRange = null, float? endRange = null, GenerateMessagesResultUseTitle<float> getResultTitle = null, OptionReadValue options = OptionReadValue.None, float? defaultValue = null, CancellationToken? token = null)
        {
            return console.ReadNumber(title, startRange, endRange, float.MinValue, float.MaxValue, float.TryParse, getResultTitle, options, defaultValue?.ToString(), token);
        }
        #endregion
    }
}
