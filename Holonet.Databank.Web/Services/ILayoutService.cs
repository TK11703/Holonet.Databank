using Holonet.Databank.Web.Components.Shared;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace Holonet.Databank.Web.Services;

public interface ILayoutService
{
    RenderFragment Header { get; }
    RenderFragment Footer { get; }
    SetHeader? HeaderSetter { get; set; }
    SetFooter? FooterSetter { get; set; }
    event PropertyChangedEventHandler PropertyChanged;
    void UpdateHeader();
    void UpdateFooter();
}
