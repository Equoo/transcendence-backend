using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API;

/// <summary>
/// Outcome of a use case that produces a value: either the value, or the ready-made
/// <c>ProblemDetails</c> response describing why it could not be produced.
/// </summary>
/// <remarks>
/// The failure is typed <see cref="ProblemHttpResult"/> rather than <c>IResult</c> on purpose:
/// C# refuses user-defined conversions to or from an interface, and it is that implicit conversion
/// which lets a handler simply <c>return EventProblems.NotFound(id);</c>.
/// </remarks>
public readonly record struct Result<T>
{
    private readonly T? _value;

    private Result(T value)
    {
        _value = value;
        Problem = null;
    }

    private Result(ProblemHttpResult problem)
    {
        _value = default;
        Problem = problem;
    }

    public ProblemHttpResult? Problem { get; }

    public bool IsSuccess => Problem is null;

    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Result is a failure, it carries no value.");

    /// <summary>Explicit factories, needed when <typeparamref name="T"/> is an interface: C# never
    /// applies a user-defined conversion to or from an interface type.</summary>
    public static Result<T> Ok(T value) => new(value);
    public static Result<T> Fail(ProblemHttpResult problem) => new(problem);

    public static implicit operator Result<T>(T value) => new(value);
    public static implicit operator Result<T>(ProblemHttpResult problem) => new(problem);
}

/// <summary>Outcome of a use case that produces nothing, such as an update or a delete.</summary>
public readonly record struct Result
{
    private Result(ProblemHttpResult problem) => Problem = problem;

    public ProblemHttpResult? Problem { get; }

    public bool IsSuccess => Problem is null;

    /// <summary>The success case: a <see cref="Result"/> carrying no problem.</summary>
    public static Result Success => default;

    public static Result Fail(ProblemHttpResult problem) => new(problem);

    public static implicit operator Result(ProblemHttpResult problem) => new(problem);
}
