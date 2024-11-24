
namespace MediatorForge.Results;

public readonly struct Option<T> : IEquatable<Option<T>>, IComparable<Option<T>>
{
    private readonly bool _isSome;
    private readonly T? _value;

    private Option(T value)
    {
        _isSome = true;
        _value = value;
    }

    public static Option<T> Some(T value) => new Option<T>(value);
    public static Option<T> None => new Option<T>();

    public bool IsSome => _isSome;
    public bool IsNone => !_isSome;

    public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone)
    {
        return IsSome ? onSome(_value!) : onNone();
    }

    public void OnSome(Action<T> onSome)
    {
        if (IsSome)
        {
            onSome(_value!);
        }
    }

    public void OnNone(Action onNone)
    {
        if (IsNone)
        {
            onNone();
        }
    }

    public int CompareTo(Option<T> other)
    {
        if (IsSome && other.IsSome)
        {
            return Comparer<T>.Default.Compare(_value, other._value);
        }
        if (IsSome)
        {
            return 1;
        }
        if (other.IsSome)
        {
            return -1;
        }
        return 0;
    }

    public bool Equals(Option<T> other)
    {
        if (IsSome && other.IsSome)
        {
            return EqualityComparer<T>.Default.Equals(_value, other._value);
        }
        return IsNone && other.IsNone;
    }

    public override bool Equals(object obj)
    {
        return obj is Option<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_isSome, _value);
    }

    public static implicit operator Option<T>(T value) => Some(value);

    public static bool operator ==(Option<T> left, Option<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Option<T> left, Option<T> right)
    {
        return !(left == right);
    }

    public static bool operator <(Option<T> left, Option<T> right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Option<T> left, Option<T> right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Option<T> left, Option<T> right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Option<T> left, Option<T> right)
    {
        return left.CompareTo(right) >= 0;
    }
}

public static class Option
{
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);
    public static Option<T> None<T>() => Option<T>.None;
}

