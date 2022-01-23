using System;
using System.Diagnostics.Tracing;
using System.Threading;

namespace AuditLogDemo.EventSources
{
    /// <summary>
    /// Api.EventCounter 事件源
    /// </summary>
    [EventSource(Name = "Api.EventCounter")]
    public sealed class ApiEventCounterSource : EventSource
    {
        public static readonly ApiEventCounterSource Log = new ApiEventCounterSource();

        private EventCounter _requestCounter;

        private PollingCounter _workingSetCounter;

        private PollingCounter _totalRequestsCounter;

        private IncrementingPollingCounter _incrementingPollingCounter;

        private long _totalRequests;

        private ApiEventCounterSource()
        {
        }


        protected override void OnEventCommand(EventCommandEventArgs command)
        {
            if (command.Command == EventCommand.Enable)
            {
                //请求响应耗时
                _requestCounter = new EventCounter("request-time", this)
                {
                    DisplayName = "Request Processing Time",
                    DisplayUnits = "ms"
                };

                //内存占用
                _workingSetCounter = new PollingCounter("working-set", this, () => (double)(Environment.WorkingSet / 1_000_000))
                {
                    DisplayName = "Working Set",
                    DisplayUnits = "MB"
                };

                //总请求量
                _totalRequestsCounter = new PollingCounter("total-requests", this, () => Volatile.Read(ref _totalRequests))
                {
                    DisplayName = "Total Requests",
                    DisplayUnits = "次"
                };

                //单位时间请求速率
                _incrementingPollingCounter = new IncrementingPollingCounter("Request Rate", this, () =>
                {
                    return Volatile.Read(ref _totalRequests);
                })
                {
                    DisplayName = "Request Rate",
                    DisplayUnits = "次/s",
                    //时间间隔1s
                    DisplayRateTimeScale = new TimeSpan(0, 0, 1)
                };

                var monitorContentionCounter = new IncrementingPollingCounter("monitor-lock-contention-count", this, () => Monitor.LockContentionCount)
                {
                    DisplayName = "Monitor Lock Contention Count",
                    DisplayRateTimeScale = TimeSpan.FromSeconds(1)
                };
            }
        }

        public void Request(string url, float elapsedMilliseconds)
        {
            //更新请求数量（保证线程安全）
            Interlocked.Increment(ref _totalRequests);

            //写入指标值(请求处理耗时)
            _requestCounter?.WriteMetric(elapsedMilliseconds);

        }

        protected override void Dispose(bool disposing)
        {
            _requestCounter?.Dispose();
            _requestCounter = null;

            _workingSetCounter?.Dispose();
            _workingSetCounter = null;

            _totalRequestsCounter?.Dispose();
            _totalRequestsCounter = null;

            _incrementingPollingCounter?.Dispose();
            _incrementingPollingCounter = null;

            base.Dispose(disposing);
        }
    }
}
