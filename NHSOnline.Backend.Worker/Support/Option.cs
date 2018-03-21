using System;

namespace NHSOnline.Backend.Worker.Support
{
    public static class Option
    {
        public static Option<T> Some<T>(T value) => new Option<T>(value, true);
        public static Option<T> None<T>() => new Option<T>(default(T), false);
    }

    public struct Option<T>
    {
        public bool HasValue { get; }
        internal T Value { get; }

        internal Option(T value, bool hasValue)
        {
            Value = value;
            HasValue = hasValue;
        }

        public T ValueOr(T alternative)
        {
            return HasValue ? Value : alternative;
        }

        public T ValueOr(Func<T> alternativeFactory)
        {
            if (alternativeFactory == null)
            {
                throw new ArgumentNullException(nameof(alternativeFactory));
            }
            return HasValue ? Value : alternativeFactory();
        }

        public T ValueOrDefault()
        {
            return HasValue ? Value : default(T);
        }

        public T ValueOrFailure()
        {
            if (HasValue)
            {
                return Value;
            }

            throw new OptionalValueMissingException();
        }
    }
}