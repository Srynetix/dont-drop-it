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
    [OnReady("Footer")]
    private RichTextLabel _Footer;

    public override async void _Ready()
    {
        NodeExt.BindNodes(this);

        _Play.Connect("pressed", this, nameof(StartGame));
        _Footer.Connect("meta_clicked", this, nameof(LinkClicked));

        var titleMusic = LoadCache.GetInstance().LoadResource<AudioStreamOGGVorbis>("MainSong");
        GlobalMusic.Instance.FadeIn();
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

    private void LinkClicked(object data)
    {
        if (data is string stringData)
        {
            OS.ShellOpen(stringData);
        }
    }
}
