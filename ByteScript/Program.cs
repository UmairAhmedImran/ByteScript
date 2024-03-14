using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ByteScript
{

    class Program
    {

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    return;
                var lexer = new Lexer(line);
                while (true)
                {
                    var token = lexer.Lex();
                    if (token.Kind == SyntaxKind.EndOfFileToken)
                        break;
                    Console.Write($"{token.Kind}: '{token.Text}'");
                    if (token.Value != null)
                        Console.Write($" {token.Value}");

                    Console.WriteLine();
                }

            }
        }

    }


    enum SyntaxKind
    {
        NumberToken,
        WhitespaceToken,
        PlusToken,
        MinusToken,
        MultiplyToken,
        DivideToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EndOfFileToken,
        LetterToken,
        AmperandAmpersandToken,
        PipePipeToken,
        EqualsEqualsToken,
        BangEqualToken,
        BangToken,
        EqualsToken,
        BreakKeyword,
        ContinueKeyword,
        ElseKeyword,
        FalseKeyword,
        ForKeyword,
        IfKeyword,
        LetKeyword,
        TrueKeyword,
        VarKeyword,
        WhileKeyword,
        IdentifierToken,
        StringToken

    }


    class SyntaxToken
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;

        }
        public SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }

    }

    internal sealed class Lexer
    {
        private readonly string _text;
        private int _position;

        public Lexer(string text)
        {
            _text = text;
        }


        private char Current => Peek(0);

        private char Lookahead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;

            if (index >= _text.Length)
                return '\0';

            return _text[index];
        }

        private void Next()
        {
            _position++;
        }

        public SyntaxToken Lex()
        {

            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);

            var start = _position;

            if (char.IsDigit(Current))
            {

                while (char.IsDigit(Current))
                    Next();

                if (char.IsWhiteSpace(Current) || _position >= _text.Length)
                {
                    var length = _position - start;
                    var text = _text.Substring(start, length);
                    if (!int.TryParse(text, out var value))
                        Console.WriteLine($"The number {_text} isn't a valid Int32.");
                    return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
                }
            }

            if (char.IsWhiteSpace(Current))
            {


                while (char.IsWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, null);
            }


            switch (Current)
            {

                case '+':
                    return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
                case '*':
                    return new SyntaxToken(SyntaxKind.MultiplyToken, _position++, "*", null);
                case '/':
                    return new SyntaxToken(SyntaxKind.DivideToken, _position++, "/", null);
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);
                case '&':
                    if (Lookahead == '&')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.AmperandAmpersandToken, start, "&&", null);
                    }
                    break;

                case '|':
                    if (Lookahead == '|')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.PipePipeToken, start, "||", null);
                    }
                    break;
                case '=':
                    if (Lookahead == '=')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.EqualsEqualsToken, start, "==", null);
                    }
                    else
                    {
                        _position += 1;
                        return new SyntaxToken(SyntaxKind.BangToken, start, "!", null);
                    }
                case '!':
                    if (Lookahead == '=')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.EqualsToken, start, "=", null);
                    }
                    else
                    {
                        _position += 1;
                        return new SyntaxToken(SyntaxKind.BangToken, start, "!", null);
                    }
            }
            if (Current == '"')
            {
                _position++;
                while (Current != '"')
                {
                    _position++;
                    var length = _position - start;
                    var text = _text.Substring(start + 1, length - 1);
                    if (Current == '"')
                    {

                        return new SyntaxToken(SyntaxKind.StringToken, start, text, null);
                    }
                    else if (Current == '\n')
                    {
                        return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
                    }
                }


            }
            if (char.IsLetter(Current) || Current == '_')
            {
                _position++;
                while (char.IsLetterOrDigit(Current) || Current == '_')
                    _position++;
                var length = _position - start;
                var text = _text.Substring(start, length);
                switch (text)
                {
                    case "break":
                        return new SyntaxToken(SyntaxKind.BreakKeyword, start, text, null);
                    case "continue":
                        return new SyntaxToken(SyntaxKind.ContinueKeyword, start, text, null);
                    case "else":
                        return new SyntaxToken(SyntaxKind.ElseKeyword, start, text, null);
                    case "false":
                        return new SyntaxToken(SyntaxKind.FalseKeyword, start, text, null);
                    case "for":
                        return new SyntaxToken(SyntaxKind.ForKeyword, start, text, null);
                    case "if":
                        return new SyntaxToken(SyntaxKind.IfKeyword, start, text, null);
                    case "let":
                        return new SyntaxToken(SyntaxKind.LetKeyword, start, text, null);
                    case "true":
                        return new SyntaxToken(SyntaxKind.TrueKeyword, start, text, null);
                    case "var":
                        return new SyntaxToken(SyntaxKind.VarKeyword, start, text, null);
                    case "while":
                        return new SyntaxToken(SyntaxKind.WhileKeyword, start, text, null);

                    default:
                        return new SyntaxToken(SyntaxKind.IdentifierToken, start, text, null);
                }

            }
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }

}