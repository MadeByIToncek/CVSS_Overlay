#nullable enable
using System;
using System.Threading.Tasks;
using Godot;

namespace CVSS_Overlay;

public partial class MainController : Control {
    public ApiHandler Api;
    public WebsocketHandler Wsh;
    public TimerController? Timer;
    public TeamLowerThird? LeftThird;
    public TeamLowerThird? RightThird;
    public CurrentMatch? Match;

    public MainController() {
        Api = new ApiHandler(this);
        Wsh = new WebsocketHandler(this);
    }

    public override void _Ready() {
        AddChild(Api);
        AddChild(Wsh);

        // GetTree().CreateTimer(6).Timeout += () => {
        //     Wsh.Remove();
        //     Api.Remove();
        //     GetTree().Quit();
        // };
    }


    public void SetCurrentTime(int i) {
        if (Timer == null) {
           return; 
        }

        Timer.SetCurrentTime(i);
    }
}