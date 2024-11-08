using Holonet.Databank.Web.Services;
using Microsoft.AspNetCore.Components;

namespace Holonet.Databank.Web.Components.Shared;

public class SetHeader : ComponentBase, IDisposable
{
    private bool _disposed = false;

    [Inject]
    private ILayoutService Layout { get; set; } = default!;

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (Layout != null)
        {
            Layout.HeaderSetter = this;
        }
        base.OnInitialized();
    }

    protected override bool ShouldRender()
    {
        var shouldRender = base.ShouldRender();
        if (shouldRender)
        {
            Layout.UpdateHeader();
        }
        return shouldRender;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing && Layout != null)
            {
                Layout.HeaderSetter = default!;
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
