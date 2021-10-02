using Godot;
using SxGD;

public class GameOver : CanvasLayer
{
    [Signal]
    public delegate void restart();

    [OnReady]
    private readonly AnimationPlayer _AnimationPlayer;
    [OnReady("Center/VBox/TryAgain")]
    private readonly Button _TryAgain;

    public override void _Ready()
    {
        NodeExt.BindNodes(this);

        _TryAgain.Disabled = true;
        _TryAgain.MouseFilter = Control.MouseFilterEnum.Ignore;
        _TryAgain.Connect("pressed", this, nameof(RestartGame));
    }

    public void Start()
    {
        _TryAgain.Disabled = false;
        _TryAgain.MouseFilter = Control.MouseFilterEnum.Stop;
        _TryAgain.GrabFocus();
        _AnimationPlayer.Play("gameover");
    }

    public void RestartGame()
    {
        EmitSignal(nameof(restart));
    }
}
