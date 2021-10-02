using System.Text;
using Godot;
using SxGD;
using System.Text.RegularExpressions;

[Tool]
public class Help : CanvasLayer
{
    [Export(PropertyHint.MultilineText)]
    public string Text
    {
        get => _Text;
        set
        {
            _Text = value;
            UpdateEditorText();
        }
    }

    [OnReady("Margin/RichTextLabel")]
    private RichTextLabel _Label;
    [OnReady]
    private Tween _Tween;

    private static readonly Regex _WordRegex = new Regex(@"(?<block>(\[.*?\].*?\[/.*?\][^ ]*)|([^ ]+))", RegexOptions.Compiled);

    private string _Text = "";

    public override void _Ready()
    {
        NodeExt.BindNodes(this);

        // Reset text
        _Label.BbcodeText = "";
    }

    public override void _Process(float delta)
    {
        UpdateEditorText();
    }

    public void UpdateEditorText()
    {
        if (Engine.EditorHint)
        {
            if (_Label == null)
            {
                _Label = (RichTextLabel)GetNode("Margin/RichTextLabel");
            }

            _Label.BbcodeText = $"[right]{_Text}[/right]";
        }
    }

    public async void FadeIn()
    {
        _Label.BbcodeText = "[right]";

        foreach (string line in _Text.Split('\n'))
        {
            foreach (var match in _WordRegex.Matches(line))
            {
                // _Label.AppendBbcode($"[shake]{match}[/shake] ");
                _Label.BbcodeText += $"[shake rate=20 level=2]{match}[/shake] ";
                await ToSignal(GetTree().CreateTimer(0.25f), "timeout");
            }
            // _Label.AppendBbcode("\n");
            _Label.BbcodeText += "\n";
            await ToSignal(GetTree().CreateTimer(0.25f), "timeout");
        }

        _Label.BbcodeText += "[/right]";

        _Tween.InterpolateProperty(_Label, "modulate", Colors.White, Colors.White.WithAlphaf(0), 5);
        _Tween.Start();

        await ToSignal(_Tween, "tween_all_completed");
    }
}
