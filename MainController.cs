using Godot;
using System;
using CVSS_Overlay;

public partial class MainController : Control
{
    private TeamLowerThird _t1 = new("Filmáci", ["Dan Švarc", "Antonín Vřešťál", "Kuba Krejčí", "Matesak tesak"],new Color("#0c5e1d"),new Color("#063810"),false);
    // private TeamLowerThird _t2 = new("Roboťáci", ["Nějakej roboťák", "Taky roboťák", "Dan Švarc", "Robovedouci 1", "Robovedouci 2"],new Color("#5e0c0c"),new Color("#380606"),false);
    //private TimerController _timer = new(Colors.DarkGreen, Colors.Blue);
    //private WebsocketHandler _wsh = new WebsocketHandler();
    public override void _Ready() {
        //AddChild(_timer);
        //AddChild(_wsh);
        AddChild(_t1);
        // AddChild(_t2);

        GetTree().CreateTimer(6).Timeout += () => {
            // _timer.Remove();
            _t1.Remove();
            // _t2.Remove();
            // _wsh.Remove();
        };
    }
}
