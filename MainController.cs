#nullable enable
using System;
using System.Threading.Tasks;
using CVSS_GodotCommons;
using Godot;

namespace CVSS_Overlay;

public partial class MainController : Control {
    private ApiHandler _api;
    private WebsocketHandlerImpl _wsh;
    private TimerController? _timer;
    private TeamLowerThird? _leftThird;
    private TeamLowerThird? _rightThird;

    public MainController() {
        _api = new ApiHandler();
        _wsh = new WebsocketHandlerImpl(_api, this);
    }

    public override void _EnterTree() {
        AddChild(_api);
        AddChild(_wsh);
    }

    
    private void ShowLeft() {
        new Func<Task>(async () =>
        {
            HideRight();
            HideLeft();

            Team m = (await _api.GetCurrentMatch()).LeftTeam;
            _leftThird = new TeamLowerThird(m.Name, m.Members,m.ColorBright, m.ColorDark,true);
            AddChild(_leftThird);
        }).Invoke();
    }

    private void ShowRight() {
        new Func<Task>(async () => {
            HideRight();
            HideLeft();
            
            Team m = (await _api.GetCurrentMatch()).RightTeam;
            _rightThird = new TeamLowerThird(m.Name, m.Members, m.ColorBright, m.ColorDark, false);
            AddChild(_rightThird);
        }).Invoke();
    }

    private void HideRight() {
        if(_rightThird == null) return;
        
        _rightThird.Remove();
        _rightThird = null;
    }
    private void HideLeft() {
        if(_leftThird == null) return;
        
        _leftThird.Remove();
        _leftThird = null;
    }

    private void HideTime() {
        if (_timer == null) return;
        
        _timer.Remove();
        _timer = null;
    }

    private void ShowTime() {
        new Func<Task>(async () => {
            HideTime();

            CurrentMatch currentMatch = await _api.GetCurrentMatch();
            _timer = new TimerController(
                currentMatch.LeftTeam.ColorDark,
                currentMatch.RightTeam.ColorDark);

            int matchDuration = await _api.GetMatchDuration();
            AddChild(_timer);
            _timer.SetCurrentTime(matchDuration);
        }).Invoke();
    }


    private void SetCurrentTime(int i) {
        _timer?.SetCurrentTime(i);
    }

    private partial class WebsocketHandlerImpl(ApiHandler api, MainController controller) : WebsocketHandler(api) {
        protected override void ReceivedCommand(OverlayCommand cmd) {
            switch (cmd) {
                case OverlayCommand.SHOW_RIGHT:
                    controller.ShowRight();
                    break;
                case OverlayCommand.HIDE_RIGHT:
                    controller.HideRight();
                    break;
                case OverlayCommand.SHOW_LEFT:
                    controller.ShowLeft();
                    break;
                case OverlayCommand.HIDE_LEFT:
                    controller.HideLeft();
                    break;
                case OverlayCommand.SHOW_TIME:
                    controller.ShowTime();
                    break;
                case OverlayCommand.HIDE_TIME:
                    controller.HideTime();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cmd), cmd, null);
            }
        }
        protected override void ReceivedTime(int time) {
            if (time >= 0) {
                controller.SetCurrentTime(time);
            } 
        }
    }
}