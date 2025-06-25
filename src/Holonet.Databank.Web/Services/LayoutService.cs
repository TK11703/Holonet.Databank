using Holonet.Databank.Web.Components.Shared;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Holonet.Databank.Web.Services;

public class LayoutService : ILayoutService, INotifyPropertyChanged
{
    public RenderFragment Header => HeaderSetter?.ChildContent ?? (_ => { });

    public RenderFragment Footer => FooterSetter?.ChildContent ?? (_ => { });

    public SetHeader? HeaderSetter
    {
        get => headerSetter;
        set
        {
            if (headerSetter == value) return;
            headerSetter = value;
            UpdateHeader();
        }
    }

    public SetFooter? FooterSetter
    {
        get => footerSetter;
        set
        {
            if (footerSetter == value) return;
            footerSetter = value;
            UpdateFooter();
        }
    }

    public void UpdateHeader() => NotifyPropertyChanged(nameof(Header));
    public void UpdateFooter() => NotifyPropertyChanged(nameof(Footer));

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private SetHeader? headerSetter;
    private SetFooter? footerSetter;
}
