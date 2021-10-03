using Godot;
using SxGD;

public class Credits : Control
{
    [OnReady("ZoneTileMap")]
    protected readonly ZoneTileMap _ZoneTileMap;
    [OnReady("WallCollider/WallTileMap")]
    protected readonly TileMap _WallTileMap;
    [OnReady]
    protected readonly FXCamera _Camera;
    [OnReady("FX/Vignette")]
    protected readonly Vignette _Vignette;
    [OnReady]
    protected readonly AnimationPlayer _AnimationPlayer;
    [OnReady]
    protected Bomb _Bomb;
    [OnReady("Layer/Center/VBox/Button")]
    protected Button _ReturnTitle;
    [OnReady("Layer/Center/VBox/Message")]
    protected RichTextLabel _Message;

    private string _MessageToDisplay;
    private bool _Locked;

    public override void _Ready()
    {
        NodeExt.BindNodes(this);

        // Prepare camera
        var rect = _WallTileMap.GetUsedRect();
        var size = rect.Position + rect.Size;
        var vpSize = GetViewportRect().Size;
        _Camera.LimitLeft = 0;
        _Camera.LimitTop = 0;
        _Camera.SmoothingEnabled = true;
        _Camera.LimitRight = (int)Mathf.Max(size.x * _WallTileMap.CellSize.x, vpSize.x);

        var titleMusic = LoadCache.GetInstance().LoadResource<AudioStreamOGGVorbis>("MainSong");
        GlobalMusic.Instance.Play(titleMusic);
        GlobalMusic.Instance.FadeIn();

        _ReturnTitle.Connect("pressed", this, nameof(LoadTitleScreen));

        var deaths = GameData.Instance.Load<int>("deaths", 1);
        var time = GameData.Instance.Load<float>("time", 3600.99f);

        if (deaths == 0)
        {
            _MessageToDisplay = $@"
[rainbow]Congratulations![/rainbow]

You did not drop it!
And finished in [color=green]{time:0.00}[/color] seconds!
You can close the game now, [rainbow]thanks[/rainbow]!";
        }
        else if (deaths == 1)
        {
            _MessageToDisplay = $@"
You almost did it!

You dropped the bomb [color=red]one[/color] time.
And finished in [color=green]{time:0.00}[/color] seconds.
Next time is the right one!";
        }
        else
        {
            _MessageToDisplay = $@"
You won!

But you did drop the bomb [color=red]{deaths}[/color] times.
And finished in [color=green]{time:0.00}[/color] seconds.
You can do better!";
        }

        _Message.BbcodeText = $"[center]{_MessageToDisplay}[/center]";
    }

    public override void _Process(float delta)
    {
        if (_Locked)
        {
            var maxScale = 100;
            var scale = _Bomb.Sprite.Scale + Vector2.One * delta;
            scale.x = Mathf.Min(scale.x, maxScale);
            scale.y = Mathf.Min(scale.y, maxScale);

            _Camera.GlobalPosition = _Bomb.Position;
            _Bomb.Sprite.Scale = scale;

            return;
        }

        _Camera.ShakeRatio = _Bomb.SpeedRatio;
        _Vignette.Ratio = 0.01f + _Bomb.SpeedRatio / 8.0f;
        _Camera.GlobalPosition = _Bomb.Position;

        if (_Bomb.Locked)
        {
            _Locked = true;
            _AnimationPlayer.Play("show");
            _Camera.ShakeRatio = 0;
        }
    }

    private void LoadTitleScreen()
    {
        GlobalMusic.Instance.FadeOut();

        var scene = LoadCache.GetInstance().LoadScene("Title");
        SceneTransitioner.GetInstance().FadeToScene(scene);
    }
}
