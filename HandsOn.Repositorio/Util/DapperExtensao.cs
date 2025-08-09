using Dapper;
using System.Data;
using static Dapper.SqlMapper;

namespace HandsOn.Repositorio.Util
{
    public static class DapperExtensao
    {
        public static async Task<T?> ExecuteAsyncWithRetry<T>(
            this IDbConnection cnn,
            string sql,
            object? param = null,
            IDbTransaction? transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            CancellationToken? cancellationToken = null,
            int retentativas = 3,
            Action<int, string>? retryCallback = null)
        {
            cancellationToken ??= CancellationToken.None;

            return await Util.PoliticaRetentativa.Sql(retentativas, retryCallback)
                .ExecuteAsync(
                    async () =>
                    await cnn.ExecuteScalarAsync<T>(
                        new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken.Value)));
        }

        public static async Task<int> ExecuteAsyncWithRetry(
            this IDbConnection cnn,
            string sql,
            object? param = null,
            IDbTransaction? transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            CancellationToken? cancellationToken = null,
            int retentativas = 3,
            Action<int, string>? retryCallback = null)
        {
            cancellationToken ??= CancellationToken.None;

            return await Util.PoliticaRetentativa.Sql(retentativas, retryCallback)
                .ExecuteAsync(
                    async () =>
                    await cnn.ExecuteAsync(
                        new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken.Value)));
        }

        public static async Task<IEnumerable<T>> QueryAsyncWithRetry<T>(
            this IDbConnection cnn,
            string sql,
            object? param = null,
            IDbTransaction? transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            CancellationToken? cancellationToken = null,
            int retentativas = 3,
            Action<int, string>? retryCallback = null)
        {
            cancellationToken ??= CancellationToken.None;

            return await Util.PoliticaRetentativa.Sql(retentativas, retryCallback)
                .ExecuteAsync(
                    async () =>
                    await cnn.QueryAsync<T>(
                        new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken.Value)));
        }

        public static async Task<IEnumerable<T3>> QueryAsyncWithRetry<T1, T2, T3>(
            this IDbConnection cnn,
            string sql,
            Func<T1, T2, T3> map,
            object? param = null,
            IDbTransaction? transaction = null,
            bool buffered = true,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null,
            CancellationToken? cancellationToken = null,
            int retentativas = 3,
            Action<int, string>? retryCallback = null)
        {
            cancellationToken ??= CancellationToken.None;

            return await Util.PoliticaRetentativa.Sql(retentativas, retryCallback)
               .ExecuteAsync(
                   async () =>
                   await cnn.QueryAsync<T1, T2, T3>(
                       new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken.Value),
                       map,
                       splitOn));
        }

        public static async Task<T?> QueryFirstOrDefaultAsyncWithRetry<T>(
            this IDbConnection cnn,
            string sql,
            object? param = null,
            IDbTransaction? transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            CancellationToken? cancellationToken = null,
            int retentativas = 3,
            Action<int, string>? retryCallback = null)
        {
            cancellationToken ??= CancellationToken.None;

            return await Util.PoliticaRetentativa.Sql(retentativas, retryCallback)
                .ExecuteAsync(
                    async () =>
                    await cnn.QueryFirstOrDefaultAsync<T>(
                        new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken.Value)));
        }


        public static async Task<GridReader> QueryMultipleAsync<T>(
            this IDbConnection cnn,
            string sql,
            object? param = null,
            IDbTransaction? transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            CancellationToken? cancellationToken = null,
            int retentativas = 3,
            Action<int, string>? retryCallback = null)
        {
            cancellationToken ??= CancellationToken.None;

            return await Util.PoliticaRetentativa.Sql(retentativas, retryCallback)
                .ExecuteAsync(
                    async () =>
                    await cnn.QueryMultipleAsync(
                        new CommandDefinition(sql, param, transaction, commandTimeout, commandType, cancellationToken: cancellationToken.Value)));
        }

    }
}
