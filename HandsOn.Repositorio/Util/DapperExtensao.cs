using Dapper;
using System.Data;

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
            int retentativas = 3,
            Action<int, string>? retryCallback = null)
        {
            return await Util.PoliticaRetentativa.Sql(retentativas, retryCallback)
                .ExecuteAsync(
                    async () =>
                    await cnn.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType));
        }

        public static async Task<int> ExecuteAsyncWithRetry(
            this IDbConnection cnn,
            string sql,
            object? param = null,
            IDbTransaction? transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            int retentativas = 3,
            Action<int, string>? retryCallback = null)
        {
            return await Util.PoliticaRetentativa.Sql(retentativas, retryCallback)
                .ExecuteAsync(
                    async () =>
                    await cnn.ExecuteAsync(sql, param, transaction, commandTimeout, commandType));
        }

        public static async Task<IEnumerable<T>> QueryAsyncWithRetry<T>(
            this IDbConnection cnn,
            string sql,
            object? param = null,
            IDbTransaction? transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            int retentativas = 3,
            Action<int, string>? retryCallback = null)
        {
            return await Util.PoliticaRetentativa.Sql(retentativas, retryCallback)
                .ExecuteAsync(
                    async () =>
                    await cnn.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType));
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
            int retentativas = 3,
            Action<int, string>? retryCallback = null)
        {
            return await Util.PoliticaRetentativa.Sql(retentativas, retryCallback)
                .ExecuteAsync(
                    async () =>
                    await cnn.QueryAsync<T1, T2, T3>(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType));
        }

        public static async Task<T?> QueryFirstOrDefaultAsyncWithRetry<T>(
            this IDbConnection cnn,
            string sql,
            object? param = null,
            IDbTransaction? transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null,
            int retentativas = 3,
            Action<int, string>? retryCallback = null)
        {
            return await Util.PoliticaRetentativa.Sql(retentativas, retryCallback)
                .ExecuteAsync(
                    async () =>
                    await cnn.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType));
        }
    }
}
