using System;
using Godot;

namespace CVSS_Overlay;

public partial class WebsocketHandler : Node2D {
    private WebSocketPeer socket = new WebSocketPeer();
    
    public override void _Ready() {
        Error e = socket.ConnectToUrl("ws://localhost:4444/socket");
        if (e != Error.Ok) {
            GD.PrintErr("Unable to connect!");
            Remove();
        }
        else {
            GetTree().CreateTimer(2).Timeout += () => {
                socket.SendText("Test packet!");
            };
        }
    }

    public override void _Process(double delta) {
        socket.Poll();
        switch (socket.GetReadyState()) {
            case WebSocketPeer.State.Open:
                while (socket.GetAvailablePacketCount() > 0) {
                    GD.Print($"Got data from server {socket.GetPacket().GetStringFromUtf8()}");
                }

                break;
            case WebSocketPeer.State.Closed:
                GD.PrintErr($"WS closed! {socket.GetCloseCode()}, because {socket.GetCloseReason()}");
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
        socket.Close();
        socket.Dispose();
        GetParent().RemoveChild(this);
        QueueFree();
    }
}