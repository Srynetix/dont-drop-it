using Godot;
using SxGD;

public class Level : Control
{
    [Signal]
    public delegate void success();
    [Signal]
    public delegate void restart();

    [Export]
    public int LevelNumber;
    [Export]
    public string LevelName = "Unknown";

    [OnReady("ZoneTileMap")]
    protected readonly ZoneTileMap _ZoneTileMap;
    [OnReady]
    protected readonly GameOver _GameOver;
    [OnReady]
    protected readonly Success _Success;
    [OnReady]
    protected readonly Intro _Intro;
    [OnReady]
    protected readonly Help _Help;
    [OnReady]
    protected readonly FXCamera _Camera;
    [OnReady]
    protected readonly Vignette _Vignette;

    protected bool _IsGameOver;
    protected Bomb _Bomb;
    protected Vector2 _StartPosition;

    public override void _Ready()
    {
        NodeExt.BindNodes(this);

        _StartPosition = _ZoneTileMap.GetStartPosition();
        _Intro.LevelName = LevelName;
        _Intro.LevelNumber = LevelNumber;
        _Intro.Connect(nameof(Intro.finished), this, nameof(StartGame));
        _Success.Connect(nameof(Success.success), this, nameof(Continue));
        _GameOver.Connect(nameof(GameOver.restart), this, nameof(RestartGame));
    }

    public void StartGame()
    {
        _Help.FadeIn();

        _Bomb = LoadCache.GetInstance().InstantiateScene<Bomb>();
        _Bomb.Position = _StartPosition;
        _Bomb.Connect(nameof(Bomb.exploded), this, nameof(OnGameOver));
        _Bomb.Connect(nameof(Bomb.win), this, nameof(OnSuccess));
        AddChild(_Bomb);
    }

    public override void _Process(float delta)
    {
        if (_Bomb != null)
        {
            _Camera.ShakeRatio = _Bomb.SpeedRatio;
            _Vignette.Ratio = _Bomb.SpeedRatio / 4.0f;
        }
    }

    public void RestartGame()
    {
        EmitSignal(nameof(restart));
    }

    public void OnGameOver()
    {
        if (_IsGameOver)
        {
            return;
        }

        _Vignette.Fade(0.25f);

        _Bomb = null;
        _IsGameOver = true;
        _Camera.ShakeRatio = 0;
        _GameOver.Start();
    }

    public void OnSuccess()
    {
        if (_IsGameOver)
        {
            return;
        }

        _Vignette.Fade(1.0f);

        _Bomb = null;
        _IsGameOver = true;
        _Camera.ShakeRatio = 0;
        _Success.Start();
    }

    public void Continue()
    {
        EmitSignal(nameof(success));
    }
}
