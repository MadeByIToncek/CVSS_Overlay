#nullable enable
using System;
using System.Threading.Tasks;
using CVSS_GodotCommons;
using Godot;

namespace CVSS_Overlay;

public partial class MainController : Control {
    private ApiHandler _api;
    private WebsocketHandler _wsh;
    private TimerController? _timer;
    private TeamLowerThird? _leftThird;
    private TeamLowerThird? _rightThird;

    public MainController() {
        _api = new ApiHandler();
        _wsh = new WebsocketHandler(_api);

        _wsh.CommandReceived += (sender, cmd) => {
            switch (cmd) {
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
                    throw new ArgumentOutOfRangeException(nameof(cmd), cmd, null);
            }
        };
        _wsh.TimeReceived += (sender, time) => {
            if (time >= 0) {
                SetCurrentTime(time);
            }
        };
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
}