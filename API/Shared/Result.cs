using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API;

public sealed record Result<T>
{
    private readonly T? _value;

    private readonly ProblemHttpResult? _problem;

    public bool IsProblem => _problem is not null;

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(ProblemHttpResult problem)
    {
        return new Result<T>(problem);
    }

    public T Value => _value is not null ? _value : throw new NullReferenceException("value is null");
    public ProblemHttpResult Problem => _problem is not null ? _problem : throw new NullReferenceException("problem is null");

    public Result(T value)
    {
        _value = value;
    }
    public Result(ProblemHttpResult problem)
    {
        _problem = problem;
    }
}

public sealed record Result
{
    private readonly ProblemHttpResult? _problem;

    public bool IsProblem => _problem is not null;
    public ProblemHttpResult Problem => _problem is not null ? _problem : throw new NullReferenceException("problem is null");


    public static implicit operator Result(ProblemHttpResult problem)
    {
        return new Result(problem);
    }

    public Result()
    {

    }

    public Result(ProblemHttpResult problem)
    {
        _problem = problem;
    }

    static public Result OK => new();
}

