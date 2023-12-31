﻿using IISLogReader.Domain.IISLog;

namespace IISLogReader.App.Abstractions;

public interface IIISLogService
{
    IISLog? CurrentLog { get; }
    Task<IISLog> ParseLogAsync(Stream logStream);
    IAsyncEnumerable<LogEntrySummary> GetLogEntriesSummaryAsync(IISLog log);

}