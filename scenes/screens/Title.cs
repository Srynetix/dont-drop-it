using Godot;
using SxGD;

public class Title : Control
{
    [OnReady("Center/Play")]
    private Button _Play;

    public override void _Ready()
    {
        NodeExt.BindNodes(this);

        _Play.Connect("pressed", this, nameof(Play));
    }

    private void Play()
    {
        SceneTransitioner.GetInstance().FadeToScene("res://scenes/screens/Game.tscn");
    }
}
