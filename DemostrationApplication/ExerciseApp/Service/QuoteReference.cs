using System.Security.Cryptography;

namespace ExerciseApp.Service
{
    public static class QuoteReference
    {
        public const int Length = 8;

        // Excludes 0/O and 1/I so references are easy to read back and type.
        public const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        public static string Generate() => RandomNumberGenerator.GetString(Alphabet, Length);

        public static string Normalise(string reference) => reference?.Trim().ToUpperInvariant();
    }
}
