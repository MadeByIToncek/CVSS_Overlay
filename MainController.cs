using Godot;

namespace CVSS_Overlay;

public partial class MainController : Control
{
    public ApiHandler _api;
    public WebsocketHandler _wsh;
    public MainController() {
        _api = new ApiHandler(this);
        _wsh = new WebsocketHandler(this);
    }
    
    public override void _Ready() { 
        AddChild(_api);
        AddChild(_wsh);

        GetTree().CreateTimer(6).Timeout += () => {
            _wsh.Remove();
            _api.Remove();
            GetTree().Quit();
        };
    }
}