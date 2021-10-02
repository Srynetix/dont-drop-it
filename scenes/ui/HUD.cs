using Godot;
using SxGD;

public class HUD : CanvasLayer
{
    [Signal]
    public delegate void times_up();

    [Export]
    public int InitialSeconds = 60;

    public float RemainingTime {
        get => _RemainingTime;
    }

    [OnReady]
    private Timer _Timer;
    [OnReady("Margin/RemainingTime")]
    private Label _RemainingTimeLabel;
    [OnReady]
    private AnimationPlayer _AnimationPlayer;

    private float _RemainingTime;

    public override void _Ready()
    {
        NodeExt.BindNodes(this);

        _Timer.Connect("timeout", this, nameof(Tick));
        _RemainingTime = InitialSeconds;
    }

    public void Start() {
        UpdateTimeLabel();
        _AnimationPlayer.Play("start");
        _Timer.Start();
    }

    public void Stop() {
        _Timer.Stop();
    }

    private void Tick() {
        if (_RemainingTime > 0) {
            _RemainingTime -= 0.01f;
            UpdateTimeLabel();
        } else {
            _Timer.Stop();
            EmitSignal(nameof(times_up));
        }
    }

    private void UpdateTimeLabel() {
        string time = _RemainingTime.ToString("0.00");
        _RemainingTimeLabel.Text = $"{time}'";
    }
}
