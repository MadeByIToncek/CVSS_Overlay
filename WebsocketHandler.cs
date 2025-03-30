using System;
using Godot;

namespace CVSS_Overlay;

public partial class WebsocketHandler(MainController main) : Node2D {
    private readonly WebSocketPeer _eventSocket = new();
    private readonly WebSocketPeer _timeSocket = new();
    
    public override void _Ready() {
        Error e1 = _eventSocket.ConnectToUrl(main._api.GetEventStreamAddress());
        if (e1 != Error.Ok) {
            GD.PrintErr("Unable to connect!");
            Remove();
        }
        
        Error e2 = _timeSocket.ConnectToUrl(main._api.GetTimeStreamAddress());
        if (e2 != Error.Ok) {
            GD.PrintErr("Unable to connect!");
            Remove();
        }
    }

    public override void _Process(double delta) {
        ProcessEvents();
        ProcessTime();
    }

    private void ProcessTime() {
        _timeSocket.Poll();
        switch (_timeSocket.GetReadyState()) {
            case WebSocketPeer.State.Open:
                while (_timeSocket.GetAvailablePacketCount() > 0) {
                    GD.Print($"Got data from server {_timeSocket.GetPacket().GetStringFromUtf8()}");
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

    private void ProcessEvents() {
        _eventSocket.Poll();
        switch (_eventSocket.GetReadyState()) {
            case WebSocketPeer.State.Open:
                while (_eventSocket.GetAvailablePacketCount() > 0) {
                    GD.Print($"Got data from server {_eventSocket.GetPacket().GetStringFromUtf8()}");
                }

                break;
            case WebSocketPeer.State.Closed:
                GD.PrintErr($"WS closed! {_eventSocket.GetCloseCode()}, because {_eventSocket.GetCloseReason()}");
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
        _eventSocket.Close();
        _eventSocket.Dispose();
        GetParent().RemoveChild(this);
        QueueFree();
    }
}