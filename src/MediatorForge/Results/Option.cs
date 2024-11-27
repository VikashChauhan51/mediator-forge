namespace MediatorForge.Results;

/// <summary>
/// Represents an option type that can contain a value or be empty (None).
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public readonly struct Option<T> : IEquatable<Option<T>>, IComparable<Option<T>>
{
    private readonly bool _isSome;
    private readonly T? _value;

    private Option(T value)
    {
        _isSome = true;
        _value = value;
    }

    /// <summary>
    /// Creates an instance of <see cref="Option{T}"/> that contains a value.
    /// </summary>
    /// <param name="value">The value to be contained.</param>
    /// <returns>An <see cref="Option{T}"/> containing the specified value.</returns>
    public static Option<T> Some(T value) => new Option<T>(value);

    /// <summary>
    /// Gets an instance of <see cref="Option{T}"/> that represents no value.
    /// </summary>
    public static Option<T> None => new Option<T>();

    /// <summary>
    /// Gets a value indicating whether this instance contains a value.
    /// </summary>
    public bool IsSome => _isSome;

    /// <summary>
    /// Gets a value indicating whether this instance does not contain a value.
    /// </summary>
    public bool IsNone => !_isSome;

    /// <summary>
    /// Executes one of the specified functions based on whether this instance contains a value.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by the functions.</typeparam>
    /// <param name="onSome">The function to execute if this instance contains a value.</param>
    /// <param name="onNone">The function to execute if this instance does not contain a value.</param>
    /// <returns>The result of the executed function.</returns>
    public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone)
    {
        return IsSome ? onSome(_value!) : onNone();
    }


    /// <summary>
    /// Executes one of the specified functions based on whether this instance contains a value.
    /// </summary>
    /// <param name="onSome">The function to execute if this instance contains a value.</param>
    /// <param name="onNone">The function to execute if this instance does not contain a value.</param>
    public void Match(Action<T> onSome, Action onNone)
    {
        if (IsSome)
        {
            onSome(_value!);
        }
        else
        {
            onNone();
        }
    }

    /// <summary>
    /// Executes the specified action if this instance contains a value.
    /// </summary>
    /// <param name="onSome">The action to execute if this instance contains a value.</param>
    public void OnSome(Action<T> onSome)
    {
        if (IsSome)
        {
            onSome(_value!);
        }
    }

    /// <summary>
    /// Executes the specified action if this instance does not contain a value.
    /// </summary>
    /// <param name="onNone">The action to execute if this instance does not contain a value.</param>
    public void OnNone(Action onNone)
    {
        if (IsNone)
        {
            onNone();
        }
    }

    /// <summary>
    /// Compares this instance with another <see cref="Option{T}"/> and returns an integer that indicates whether this instance precedes, follows, or occurs in the same position in the sort order as the other <see cref="Option{T}"/>.
    /// </summary>
    /// <param name="other">An <see cref="Option{T}"/> to compare with this instance.</param>
    /// <returns>A value that indicates the relative order of the objects being compared.</returns>
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

    /// <summary>
    /// Determines whether this instance and another specified <see cref="Option{T}"/> have the same value.
    /// </summary>
    /// <param name="other">The <see cref="Option{T}"/> to compare to this instance.</param>
    /// <returns><c>true</c> if the value of the specified <see cref="Option{T}"/> is equal to the value of this instance; otherwise, <c>false</c>.</returns>
    public bool Equals(Option<T> other)
    {
        if (IsSome && other.IsSome)
        {
            return EqualityComparer<T>.Default.Equals(_value, other._value);
        }
        return IsNone && other.IsNone;
    }

    /// <summary>
    /// Determines whether the specified object is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns><c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return base.Equals(obj);
        }

        return obj is Option<T> other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(_isSome, _value);
    }

    /// <summary>
    /// Implicitly converts a value to an <see cref="Option{T}"/> containing that value.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Option<T>(T value) => Some(value);

    /// <summary>
    /// Determines whether two <see cref="Option{T}"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="Option{T}"/> to compare.</param>
    /// <param name="right">The second <see cref="Option{T}"/> to compare.</param>
    /// <returns><c>true</c> if the two <see cref="Option{T}"/> instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Option<T> left, Option<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="Option{T}"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="Option{T}"/> to compare.</param>
    /// <param name="right">The second <see cref="Option{T}"/> to compare.</param>
    /// <returns><c>true</c> if the two <see cref="Option{T}"/> instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Option<T> left, Option<T> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Determines whether one <see cref="Option{T}"/> instance precedes another in the sort order.
    /// </summary>
    /// <param name="left">The first <see cref="Option{T}"/> to compare.</param>
    /// <param name="right">The second <see cref="Option{T}"/> to compare.</param>
    /// <returns><c>true</c> if the first <see cref="Option{T}"/> precedes the second in the sort order; otherwise, <c>false</c>.</returns>
    public static bool operator <(Option<T> left, Option<T> right)
    {
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Determines whether one <see cref="Option{T}"/> instance precedes or is equal to another in the sort order.
    /// </summary>
    /// <param name="left">The first <see cref="Option{T}"/> to compare.</param>
    /// <param name="right">The second <see cref="Option{T}"/> to compare.</param>
    /// <returns><c>true</c> if the first <see cref="Option{T}"/> precedes or is equal to the second in the sort order; otherwise, <c>false</c>.</returns>
    public static bool operator <=(Option<T> left, Option<T> right)
    {
        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Determines whether one <see cref="Option{T}"/> instance follows another in the sort order.
    /// </summary>
    /// <param name="left">The first <see cref="Option{T}"/> to compare.</param>
    /// <param name="right">The second <see cref="Option{T}"/> to compare.</param>
    /// <returns><c>true</c> if the first <see cref="Option{T}"/> follows the second in the sort order; otherwise, <c>false</c>.</returns>
    public static bool operator >(Option<T> left, Option<T> right)
    {
        return left.CompareTo(right) > 0;
    }

    /// <summary>
    /// Determines whether one <see cref="Option{T}"/> instance follows or is equal to another in the sort order.
    /// </summary>
    /// <param name="left">The first <see cref="Option{T}"/> to compare.</param>
    /// <param name="right">The second <see cref="Option{T}"/> to compare.</param>
    /// <returns><c>true</c> if the first <see cref="Option{T}"/> follows or is equal to the second in the sort order; otherwise, <c>false</c>.</returns>
    public static bool operator >=(Option<T> left, Option<T> right)
    {
        return left.CompareTo(right) >= 0;
    }

}

/// <summary>
/// Provides helper methods for creating instances of <see cref="Option{T}"/>.
/// </summary>
public static class Option
{
    /// <summary>
    /// Creates an instance of <see cref="Option{T}"/> that contains a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to be contained.</param>
    /// <returns>An <see cref="Option{T}"/> containing the specified value.</returns>
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);

    /// <summary>
    /// Gets an instance of <see cref="Option{T}"/> that represents no value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>An <see cref="Option{T}"/> that represents no value.</returns>
    public static Option<T> None<T>() => Option<T>.None;
}

