using Godot;

namespace NarrowCaster;

public partial class TimerController : Control {
    [Export] public Control TimerBox;
    [Export] public ColorRect T1Box;
    [Export] public ColorRect T2Box;
    [Export] public Label TimerLabel;
    [Export] public Label T1Label;
    [Export] public Label T2Label;
    public override void _Ready() {
        Tween tw = GetTree().CreateTween().SetParallel().SetTrans(Tween.TransitionType.Cubic);

        tw.TweenProperty(TimerBox, "position", new Vector2(1720.0f, 59.0f), 1f);
        tw.TweenInterval(0.809016994374947d).Finished += () => {
            Tween tw2 = GetTree().CreateTween().SetParallel().SetTrans(Tween.TransitionType.Cubic);
            
            tw2.TweenProperty(T1Box, "position", new Vector2(-200f, 0f), 1f);
            tw2.TweenProperty(T2Box, "position", new Vector2(+400f, 0f), 1f);    
        };
    }
}