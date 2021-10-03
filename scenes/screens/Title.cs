using Godot;
using SxGD;

public class Title : Control
{
    [OnReady("Center/VBox/Play")]
    private Button _Play;
    [OnReady]
    private Bomb _Bomb;
    [OnReady("AnimationPlayer")]
    private AnimationPlayer _AnimationPlayer;

    public override async void _Ready()
    {
        NodeExt.BindNodes(this);

        _Play.Connect("pressed", this, nameof(StartGame));

        await ToSignal(GetTree().CreateTimer(1), "timeout");
        ShowAnim();
    }

    private async void ShowAnim() {
        _AnimationPlayer.Play("play");
        await ToSignal(_AnimationPlayer, "animation_finished");

        _AnimationPlayer.Play("colors");
    }

    private void StartGame()
    {
        SceneTransitioner.GetInstance().FadeToScene("res://scenes/screens/Game.tscn");
    }
}
