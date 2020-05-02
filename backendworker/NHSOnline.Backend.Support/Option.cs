using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.Support
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

        public T ValueOr(Func<T> orElse)
        {
            if (HasValue)
            {
                return Value;
            }

            return orElse();
        }

        public Option<T> IfNone(Func<Option<T>> next)
        {
            return HasValue ? this : next();
        }

        public void IfSome(Action<T> next)
        {
            if (HasValue)
            {
                next(Value);
            }
        }

        public OptionElse<TResult> IfSome<TResult>(Func<T, TResult> next)
        {
            if (HasValue)
            {
                return new OptionElseSome<TResult>(next(Value));
            }

            return new OptionElseNone<TResult>();
        }

        public override string ToString()
        {
            return HasValue ? $"{Value}" : "None";
        }
    
        public Option<TResult> Select<TResult>(Func<T, TResult> next)
        {
            return HasValue ? Option.Some(next(Value)) : Option.None<TResult>();
        }

        public abstract class OptionElse<TResult>
        {
            public abstract TResult IfNone(TResult defaultValue);
            public abstract TResult IfNone(Func<TResult> defaultValueFactory);
        }

        private sealed class OptionElseNone<TResult>: OptionElse<TResult>
        {
            public override TResult IfNone(TResult defaultValue) => defaultValue;
            public override TResult IfNone(Func<TResult> defaultValueFactory) => defaultValueFactory();
        }

        private sealed class OptionElseSome<TResult>: OptionElse<TResult>
        {
            private readonly TResult _result;

            public OptionElseSome(TResult result) => _result = result;

            public override TResult IfNone(TResult defaultValue) => _result;
            public override TResult IfNone(Func<TResult> defaultValueFactory) => _result;
        }
    }
}
