namespace CivitaiSharp.Core.Response;

/// <summary>
/// Represents a void result type for API operations that return no data.
/// Use this as the type parameter for <see cref="Result{T}"/> when the operation
/// succeeds but returns no meaningful response body.
/// </summary>
/// <remarks>
/// This type is useful for DELETE and PUT operations where success is indicated
/// by the HTTP status code alone, without a response body.
/// </remarks>
public readonly struct Unit : IEquatable<Unit>
{
    /// <summary>
    /// Gets the singleton instance of <see cref="Unit"/>.
    /// </summary>
    public static Unit Value { get; } = default;

    /// <summary>
    /// Determines whether the specified <see cref="Unit"/> is equal to this instance.
    /// All <see cref="Unit"/> instances are considered equal.
    /// </summary>
    /// <param name="other">The other <see cref="Unit"/> to compare.</param>
    /// <returns>Always returns true.</returns>
    public bool Equals(Unit other) => true;

    /// <summary>
    /// Determines whether the specified object is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if the object is a <see cref="Unit"/>; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is Unit;

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A constant hash code value.</returns>
    public override int GetHashCode() => 0;

    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    /// <returns>The string "()".</returns>
    public override string ToString() => "()";

    /// <summary>
    /// Determines whether two <see cref="Unit"/> instances are equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>Always returns true since all <see cref="Unit"/> instances are equal.</returns>
    public static bool operator ==(Unit left, Unit right) => true;

    /// <summary>
    /// Determines whether two <see cref="Unit"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>Always returns false since all <see cref="Unit"/> instances are equal.</returns>
    public static bool operator !=(Unit left, Unit right) => false;
}
