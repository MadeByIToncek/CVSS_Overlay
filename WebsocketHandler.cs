using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Godot;

namespace CVSS_Overlay;

public partial class WebsocketHandler(MainController main) : Node2D {
    private readonly WebSocketPeer _overlaySocket = new();
    private readonly WebSocketPeer _timeSocket = new();
    
    public override void _Ready() {
        Error e1 = _overlaySocket.ConnectToUrl(main.Api.GetOverlayStreamAddress());
        if (e1 != Error.Ok) {
            GD.PrintErr("Unable to connect!");
            Remove();
        }
        
        Error e2 = _timeSocket.ConnectToUrl(main.Api.GetTimeStreamAddress());
        if (e2 != Error.Ok) {
            GD.PrintErr("Unable to connect!");
            Remove();
        }
    }

    public override void _Process(double delta) {
        ProcessOverlaySocket();
        ProcessTimeSocket();
    }

    private void ProcessOverlaySocket() {
        _overlaySocket.Poll();
        switch (_overlaySocket.GetReadyState()) {
            case WebSocketPeer.State.Open:
                while (_overlaySocket.GetAvailablePacketCount() > 0) {
                    string s = _overlaySocket.GetPacket().GetStringFromUtf8();
                    if (Enum.TryParse(s, true, out OverlayCommand command)) {
                        GD.Print($"Received {s}");
                        switch (command) {
                            case OverlayCommand.SHOW_RIGHT:
                                ShowRight();
                                break;
                            case OverlayCommand.HIDE_RIGHT:
                                HideRight();
                                break;
                            case OverlayCommand.SHOW_LEFT:
                                ShowLeft();
                                break;
                            case OverlayCommand.HIDE_LEFT:
                                HideLeft();
                                break;
                            case OverlayCommand.SHOW_TIME:
                                ShowTime();
                                break;
                            case OverlayCommand.HIDE_TIME:
                                HideTime();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else {
                        GD.PrintErr($"Unknown command {s}");
                    }
                }

                break;
            case WebSocketPeer.State.Closed:
                GD.PrintErr($"WS closed! {_overlaySocket.GetCloseCode()}, because {_overlaySocket.GetCloseReason()}");
                Remove();
                break;
            case WebSocketPeer.State.Connecting:
            case WebSocketPeer.State.Closing:
            default:
                break;
        }
    }

    private void ShowLeft() {
        new Func<Task>(async () =>
        {
            HideRight();
            HideLeft();
            main.Match = await main.Api.GetCurrentMatch();

            Team m = main.Match.Value.LeftTeam;
            main.LeftThird = new TeamLowerThird(m.Name, m.Members,m.ColorBright, m.ColorDark,true,main);
            main.AddChild(main.LeftThird);
        }).Invoke();
    }

    private void ShowRight() {
        new Func<Task>(async () => {
            HideRight();
            HideLeft();
            main.Match = await main.Api.GetCurrentMatch();

            Team m = main.Match.Value.RightTeam;
            main.RightThird = new TeamLowerThird(m.Name, m.Members, m.ColorBright, m.ColorDark, false, main);
            main.AddChild(main.RightThird);
        }).Invoke();
    }

    private void HideRight() {
        if(main.RightThird == null) return;
        
        main.RightThird.Remove();
        main.RightThird = null;
    }
    private void HideLeft() {
        if(main.LeftThird == null) return;
        
        main.LeftThird.Remove();
        main.LeftThird = null;
    }

    private void HideTime() {
        if (main.Timer == null) return;
        
        main.Timer.Remove();
        main.Timer = null;
    }

    private void ShowTime() {
        new Func<Task>(async () => {
            HideTime();
            main.Match = await main.Api.GetCurrentMatch();

            main.Timer = new TimerController(
                main.Match.Value.LeftTeam.ColorDark,
                main.Match.Value.RightTeam.ColorDark,
                main);

            int matchDuration = await main.Api.GetMatchDuration();
            main.AddChild(main.Timer);
            main.Timer.SetCurrentTime(matchDuration);
        }).Invoke();
    }

    private void ProcessTimeSocket() {
        _timeSocket.Poll();
        switch (_timeSocket.GetReadyState()) {
            case WebSocketPeer.State.Open:
                while (_timeSocket.GetAvailablePacketCount() > 0) {
                    string s = _timeSocket.GetPacket().GetStringFromUtf8();
                    //GD.Print(s);
                    int i = int.Parse(s);
                    if (i >= 0) {
                        main.SetCurrentTime(i);
                    } 
                }
                break;
            case WebSocketPeer.State.Closed:
                GD.PrintErr($"WS closed! {_timeSocket.GetCloseCode()}, because {_timeSocket.GetCloseReason()}");
                Remove();
                break;
            case WebSocketPeer.State.Connecting:
            case WebSocketPeer.State.Closing:
            default:
                break;
        }
    }


    public void Remove() {
        SetProcess(false);
        _overlaySocket.Dispose();
        //GetParent().RemoveChild(this);
        QueueFree();
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum OverlayCommand {
    SHOW_RIGHT,
    HIDE_RIGHT,
    SHOW_LEFT,
    HIDE_LEFT,
    SHOW_TIME,
    HIDE_TIME
}
