using System.Text;
using Godot;
using SxGD;
using System.Text.RegularExpressions;
using System;

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
    [OnReady]
    private Timer _Timer;

    private static readonly Regex _WordRegex = new Regex(@"(?<block>(\[.*?\].*?\[/.*?\][^ ]*)|([^ ]+))", RegexOptions.Compiled);
    private static readonly Regex _Tag = new Regex(@"(?<tag>(\[\\?.*?\]))", RegexOptions.Compiled);

    private string _Text;
    private string _TextWithoutTags;
    private bool _AnimateText;
    private int _Cursor;

    public override void _Ready()
    {
        NodeExt.BindNodes(this);

        // Reset text
        _Label.BbcodeText = "";
        _Timer.Connect("timeout", this, nameof(TimeOut));

        _TextWithoutTags = StripTags(_Text);
    }

    private string StripTags(string s)
    {
        return _Tag.Replace(s, "");
    }

    public override void _Process(float delta)
    {
        if (_AnimateText)
        {
            var newBbCode = "[right][ghost start={0} length=5]" + _Text + "[/ghost][/right]";

            _Label.BbcodeText = String.Format(newBbCode, _Cursor);
            _AnimateText = false;
        }

        UpdateEditorText();
    }

    public void TimeOut()
    {
        if (_Cursor < _TextWithoutTags.Length)
        {
            _Cursor++;
            _AnimateText = true;
            _Timer.Start();
        }
        else
        {
            _Tween.InterpolateProperty(_Label, "modulate", Colors.White, Colors.White.WithAlphaf(0), 5);
            _Tween.Start();
        }
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

    public void FadeIn()
    {
        _Cursor = 0;
        _AnimateText = true;

        _Timer.Start();
    }
}
