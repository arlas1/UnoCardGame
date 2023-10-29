using System;

namespace ConsoleApp
{
    public class InvalidValueSubmissionException : Exception
    {
        public UnoCard.Value Expected { get; }
        public UnoCard.Value Actual { get; }

        public InvalidValueSubmissionException(string message, UnoCard.Value actual, UnoCard.Value expected)
            : base(message)
        {
            Expected = expected;
            Actual = actual;
        }
    }  
}