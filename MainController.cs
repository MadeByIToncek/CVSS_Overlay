#nullable enable
using System;
using System.Threading.Tasks;
using Godot;

namespace CVSS_Overlay;

public partial class MainController : Control {
    public ApiHandler Api;
    private WebsocketHandler _wsh;
    public TimerController? Timer;
    public TeamLowerThird? LeftThird;
    public TeamLowerThird? RightThird;
    public CurrentMatch? Match;

    public MainController() {
        Api = new ApiHandler(this);
        _wsh = new WebsocketHandler(this);
    }

    public override void _EnterTree() {
        AddChild(Api);
        AddChild(_wsh);
    }

    public void SetCurrentTime(int i) {
        Timer?.SetCurrentTime(i);
    }
}