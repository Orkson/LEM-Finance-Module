using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

public class BusyTimeoutCommandInterceptor : DbCommandInterceptor
{
    private readonly int _timeout;

    public BusyTimeoutCommandInterceptor(int timeout)
    {
        _timeout = timeout;
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        command.CommandText = "PRAGMA busy_timeout = " + _timeout + "; " + command.CommandText;
        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        command.CommandText = "PRAGMA busy_timeout = " + _timeout + "; " + command.CommandText;
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result,
        CancellationToken cancellationToken = default)
    {
        command.CommandText = "PRAGMA busy_timeout = " + _timeout + "; " + command.CommandText;
        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }
}
