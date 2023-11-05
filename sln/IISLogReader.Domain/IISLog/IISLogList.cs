using IISLogReader.Domain.IISLog;

namespace IISLogReader.Domain.Models;
public class IISLogList
{
    public required List<IISLog.IISLog> Logs { get; set; }
}
