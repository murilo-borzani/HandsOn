using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;
using System.ComponentModel;

namespace HandsOn.Repositorio.Util
{
    public static class PoliticaRetentativa
    {
        static readonly int TempoBase = 2;

        static Type[] _execoes = {
            typeof(HttpRequestException),
            typeof(TaskCanceledException) };

        public static AsyncRetryPolicy Sql
            (int retentativas = 3, Action<int, string>? retryCallback = null)
        {
            return Policy
            .Handle<SqlException>(SqlServerTransientExceptionDetector.ShouldRetryOn)
            .Or<TimeoutException>()
            .OrInner<Win32Exception>(SqlServerTransientExceptionDetector.ShouldRetryOn)
            .WaitAndRetryAsync(
                        retryCount: retentativas,
                        sleepDurationProvider: (retryCount, context) =>
                        {
                            return TimeSpan.FromSeconds(TempoBase * retryCount);
                        },
                        onRetry: (exception, timeSpam, retryCount, context) =>
                        {
                            if (retryCallback != null)
                                retryCallback(retryCount, exception?.Message ?? String.Empty);
                        });
        }
    }
}
