using System.Collections.Generic;
using System.Linq;
using Godot;

namespace CVSS_Overlay;

public partial class TeamLowerThird(string teamName, string[] teamMembers, Color teamColorBrighter, Color teamColorDarker, bool left) : Control {
    
    private readonly List<Control> _children = [];
    private LabelSettings _memberLabelSettings = new() {
        FontSize = 85,
        Font = GD.Load<FontFile>("res://fonts/regular.ttf")
    };
    private LabelSettings _teamLabelSettings = new() {
        FontSize = 150,
        Font = GD.Load<FontFile>("res://fonts/regular.ttf")
    };
    
    public override void _Ready() {
        
    }

    public override void _EnterTree() {
        Position = left ? new Vector2(80, 2060) : new Vector2(3760, 2060);

        List<string> sort = teamMembers.OrderBy(x => x.Length).ToList();
        
        for (int i = 0; i < sort.Count; i++) {
            string member = sort[i];

            (float fontwidth, float fontheight) = _memberLabelSettings.Font.GetStringSize(member,fontSize: _memberLabelSettings.FontSize);
            
            ColorRect rect = new();
            rect.Size = new Vector2(fontwidth + 30, 125);
            rect.Position = new Vector2(left ? 0 : -rect.Size.X, -125 * (i + 1) - 10 * i);
            rect.Color = teamColorDarker;
            
            _children.Add(rect);
            AddChild(rect);
            
            Label label = new();
            label.Text = member;
            label.SetLabelSettings(_memberLabelSettings);
            label.Position = new Vector2(15, 5);
            label.Size = new Vector2(fontwidth, fontheight);
        
            _children.Add(label);
            rect.AddChild(label);
        }
        
        (float mfw, float mfh) = _teamLabelSettings.Font.GetStringSize(teamName,fontSize: _teamLabelSettings.FontSize);
        ColorRect mainRect = new();
        mainRect.Size = new Vector2(mfw + 100, 250);
        mainRect.Position = new Vector2(left? 0 : -mainRect.Size.X, -125 * sort.Count - 10 * (sort.Count -1) - 265);
        mainRect.Color = teamColorBrighter;
            
        _children.Add(mainRect);
        AddChild(mainRect);
            
        Label mainLabel = new();
        mainLabel.Text = teamName;
        mainLabel.SetLabelSettings(_teamLabelSettings);
        mainLabel.Position = new Vector2(50, 25);
        mainLabel.Size = new Vector2(mfw, mfh);
        
        _children.Add(mainLabel);
        mainRect.AddChild(mainLabel);
    }

    public void Remove() {
        foreach (Control c in _children) {
            c.QueueFree();
        }
        QueueFree();
    }
}