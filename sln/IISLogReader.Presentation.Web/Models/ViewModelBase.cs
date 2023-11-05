using IISLogReader.Presentation.Web.Common;

namespace IISLogReader.Presentation.Web.Models;

public class ViewModelBase
{
    public string? AlertMessage { get; set; }
    public AlertTypeEnum? AlertType { get; set; }
}
