using System;

namespace ConsoleApp
{
    public class InvalidPlayerTurnException : Exception
    {
        public string PlayerId { get; }

        public InvalidPlayerTurnException(string message, string pid) : base(message)
        {
            PlayerId = pid;
        }
    }
}