using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.ComTypes;

namespace NHSOnline.Backend.Worker.Support
{
    [SuppressMessage("Microsoft.Naming", "CA1716", 
        Justification = "We knowingly choose to use the 'Option' keyword. This class library will not be consumed externally.")]
    public static class Option
    {
        public static Option<T> Some<T>(T value) => new Option<T>(value, true);
        public static Option<T> None<T>() => new Option<T>(default(T), false);
    }

    [SuppressMessage("Microsoft.Naming", "CA1716", 
        Justification = "We knowingly choose to use the 'Option' keyword. This class library will not be consumed externally.")]
    [SuppressMessage("Microsoft.Performance", "CA1815",
        Justification = "Instances of the Option type will not be compared to each other.")]
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

        public Option<T> IfSome(Func<T, Option<T>> next)
        {
            return HasValue ? next.Invoke(Value) : this;
        }
    }
}
