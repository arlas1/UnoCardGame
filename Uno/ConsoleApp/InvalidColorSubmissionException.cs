using System;

namespace ConsoleApp
{
    public class InvalidColorSubmissionException : Exception
    {
        public UnoCard.Color Expected { get; }
        public UnoCard.Color Actual { get; }

        public InvalidColorSubmissionException(string message, UnoCard.Color actual, UnoCard.Color expected)
            : base(message)
        {
            Expected = expected;
            Actual = actual;
        }
    }
}