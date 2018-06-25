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

        public bool IsEmpty => !HasValue;
        private T Value { get; }

        internal Option(T value, bool hasValue)
        {
            Value = value;
            HasValue = hasValue;
        }

        public T ValueOrFailure()
        {
            if (HasValue)
            {
                return Value;
            }

            throw new OptionalValueMissingException();
        }

        public Option<T> IfNone(Func<Option<T>> next)
        {
            return HasValue ? this : next.Invoke();
        }
    }
}