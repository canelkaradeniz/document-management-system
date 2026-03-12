namespace DocMan.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }
    public List<string> Warnings { get; init; } = new();

    public static Result<T> Success(T data, List<string>? warnings = null) =>
        new() { IsSuccess = true, Data = data, Warnings = warnings ?? new() };

    public static Result<T> Failure(string error) =>
        new() { IsSuccess = false, Error = error };
}

public class PagedResult<T>
{
    public List<T> Items { get; init; } = new();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
