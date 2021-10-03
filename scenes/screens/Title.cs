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

        var titleMusic = LoadCache.GetInstance().LoadResource<AudioStreamOGGVorbis>("MainSong");
        GlobalMusic.Instance.Play(titleMusic);

        await ToSignal(GetTree().CreateTimer(3), "timeout");
        ShowAnim();
    }

    private async void ShowAnim()
    {
        _AnimationPlayer.Play("play");
        await ToSignal(_AnimationPlayer, "animation_finished");

        _AnimationPlayer.Play("colors");
    }

    private void StartGame()
    {
        GlobalMusic.Instance.FadeOut();
        SceneTransitioner.GetInstance().FadeToScene("res://scenes/screens/Game.tscn");
    }
}
