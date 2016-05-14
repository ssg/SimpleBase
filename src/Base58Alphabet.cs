using System;

namespace SimpleBase
{
    public sealed class Base58Alphabet
    {
        public const int Length = 58;

        public string Value { get; private set; }

        public Base58Alphabet(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (text.Length != Length)
            {
                throw new ArgumentException("Base58 alphabets need to be 58-characters long", "text");
            }
            this.Value = text;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}