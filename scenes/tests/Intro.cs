using Godot;
using SxGD;

public class Intro : CanvasLayer
{
    [Signal]
    public delegate void finished();

    [OnReady("Center/VBox/HBox/Num")]
    private readonly Label _LevelNum;
    [OnReady("Center/VBox/Name")]
    private readonly Label _Name;
    [OnReady]
    private readonly AnimationPlayer _AnimationPlayer;

    public string LevelName
    {
        set
        {
            _Name.Text = value;
        }
    }

    public int LevelNumber
    {
        set
        {
            _LevelNum.Text = $"{value}";
        }
    }

    public override async void _Ready()
    {
        NodeExt.BindNodes(this);

        _AnimationPlayer.Play("intro");
        await ToSignal(_AnimationPlayer, "animation_finished");

        EmitSignal(nameof(finished));
        QueueFree();
    }
}
