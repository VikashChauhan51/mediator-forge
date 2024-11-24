
namespace MediatorForge.Results;

public readonly struct Result<T> : IEquatable<Result<T>>, IComparable<Result<T>>
{
    private readonly ResultState _state;
    private readonly T? _value;
    private readonly Exception? _exception;

    public bool IsSuccess => _exception is null;
    public T? Value => _value;
    public Exception? Exception => _exception;

    public Result(T value)
    {
        _state = ResultState.Success;
        _exception = null;
        _value = value;
    }

    public Result(Exception error)
    {
        _exception = error;
        _state = ResultState.Failed;
        _value = default(T);
    }

    public static Result<T> Succ(T value) => new Result<T>(value);
    public static Result<T> Fail(Exception error) => new Result<T>(error);

    public int CompareTo(Result<T> other)
    {
        if (IsSuccess && other.IsSuccess)
        {
            return Comparer<T>.Default.Compare(_value, other._value);
        }
        if (IsSuccess)
        {
            return 1;
        }
        if (other.IsSuccess)
        {
            return -1;
        }
        return 0;
    }

    public bool Equals(Result<T> other)
    {
        if (IsSuccess && other.IsSuccess)
        {
            return EqualityComparer<T>.Default.Equals(_value, other._value);
        }
        if (!IsSuccess && !other.IsSuccess)
        {
            return EqualityComparer<Exception>.Default.Equals(_exception, other._exception);
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        return obj is Result<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_state, _value, _exception);
    }

    public static implicit operator Result<T>(T value) => new Result<T>(value);
    public static implicit operator Result<T>(Exception error) => new Result<T>(error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Exception, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(_value!) : onFailure(_exception!);
    }

    public void OnSuccess(Action<T> onSuccess)
    {
        if (IsSuccess)
        {
            onSuccess(_value!);
        }
    }

    public void OnFailure(Action<Exception> onFailure)
    {
        if (!IsSuccess)
        {
            onFailure(_exception!);
        }
    }

    public static bool operator ==(Result<T> left, Result<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Result<T> left, Result<T> right)
    {
        return !(left == right);
    }

    public static bool operator <(Result<T> left, Result<T> right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Result<T> left, Result<T> right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Result<T> left, Result<T> right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Result<T> left, Result<T> right)
    {
        return left.CompareTo(right) >= 0;
    }
}

