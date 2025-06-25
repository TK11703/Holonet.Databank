using Holonet.Databank.Web.Services;
using Microsoft.AspNetCore.Components;

namespace Holonet.Databank.Web.Components.Shared;

public class SetFooter : ComponentBase, IDisposable
{
    [Inject]
    private ILayoutService Layout { get; set; } = default!;

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    private bool _disposed = false;

    protected override void OnInitialized()
    {
        if (Layout != null)
        {
            Layout.FooterSetter = this;
        }
        base.OnInitialized();
    }

    protected override bool ShouldRender()
    {
        var shouldRender = base.ShouldRender();
        if (shouldRender)
        {
            Layout.UpdateFooter();
        }
        return shouldRender;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing && Layout != null)
            {
                Layout.FooterSetter = default!;
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
