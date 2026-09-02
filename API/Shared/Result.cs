using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API;

public sealed record Result<T>
{
    private readonly T? _value;

    public ProblemHttpResult? Problem { get; init; }

    public bool IsProblem => Problem is not null;

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(ProblemHttpResult problem)
    {
        return new Result<T>(problem);
    }

    public T Value => _value is not null ? _value : throw new NullReferenceException("value is null");

    public Result(T value)
    {
        _value = value;
    }
    public Result(ProblemHttpResult problem)
    {
        Problem = problem;
    }
}

public sealed record Result
{
    public ProblemHttpResult? Problem { get; init; }

    public bool IsProblem => Problem is not null;

    public static implicit operator Result(ProblemHttpResult problem)
    {
        return new Result(problem);
    }

    public Result()
    {

    }

    public Result(ProblemHttpResult problem)
    {
        Problem = problem;
    }

    static public Result OK => new();
}

