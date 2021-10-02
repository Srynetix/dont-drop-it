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
    [OnReady("WallCollider/WallTileMap")]
    protected readonly TileMap _WallTileMap;
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
    [OnReady("FX/Vignette")]
    protected readonly Vignette _Vignette;
    [OnReady]
    protected readonly HUD _HUD;

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
        _HUD.Connect(nameof(HUD.times_up), this, nameof(TimesUp));

        // Prepare camera
        var size = _WallTileMap.GetUsedRect().Size;
        var vpSize = GetViewportRect().Size;
        _Camera.LimitLeft = 0;
        _Camera.LimitTop = 0;
        _Camera.SmoothingEnabled = true;
        _Camera.LimitRight = (int)Mathf.Max(size.x * _WallTileMap.CellSize.x, vpSize.x);
        _Camera.LimitBottom = (int)Mathf.Max(size.y * _WallTileMap.CellSize.y, vpSize.y);
    }

    public void StartGame()
    {
        _Help.FadeIn();
        _HUD.Start();

        _Bomb = LoadCache.GetInstance().InstantiateScene<Bomb>();
        _Bomb.Position = _StartPosition;
        _Bomb.Connect(nameof(Bomb.exploded), this, nameof(OnGameOver));
        _Bomb.Connect(nameof(Bomb.win), this, nameof(OnSuccess));
        AddChild(_Bomb);
    }

    private void TimesUp()
    {
        _Bomb.Explode();
    }

    public override void _Process(float delta)
    {
        if (_Bomb != null)
        {
            _Camera.ShakeRatio = _Bomb.SpeedRatio;
            _Vignette.Ratio = 0.01f + _Bomb.SpeedRatio / 8.0f;
            _Camera.GlobalPosition = _Bomb.Position;
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

        _HUD.Stop();
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

        _HUD.Stop();
        _Vignette.Fade(1.0f);

        _Bomb = null;
        _IsGameOver = true;
        _Camera.ShakeRatio = 0;
        _Success.Start(_HUD.InitialSeconds - _HUD.RemainingTime);
    }

    public void Continue()
    {
        EmitSignal(nameof(success));
    }
}
