using Godot;
using SxGD;

public class Success : CanvasLayer
{
    [Signal]
    public delegate void success();

    [OnReady]
    private readonly AnimationPlayer _AnimationPlayer;
    [OnReady("Center/VBox/Next")]
    private readonly Button _Next;
    [OnReady("Center/VBox/TimeMessage/Time")]
    private readonly Label _TimeLabel;

    public override void _Ready()
    {
        NodeExt.BindNodes(this);

        _Next.Disabled = true;
        _Next.MouseFilter = Control.MouseFilterEnum.Ignore;
        _Next.Connect("pressed", this, nameof(Continue));
    }

    public void Start(float time)
    {
        _TimeLabel.Text = $"{time:0.00}'";

        _Next.Disabled = false;
        _Next.MouseFilter = Control.MouseFilterEnum.Stop;
        _Next.GrabFocus();
        _AnimationPlayer.Play("success");
    }

    public void Continue()
    {
        EmitSignal(nameof(success));
    }
}