using Godot;
using SxGD;

public class Debug : CanvasLayer
{
    [OnReady("Margin/Label")]
    private RichTextLabel _Label;

    public override void _Ready()
    {
        NodeExt.BindNodes(this);

        // Hidden by default
        _Label.Visible = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key) {
            if (key.Pressed && key.Scancode == (int)KeyList.F12) {
                _Label.Visible = !_Label.Visible;
            }
        }
    }

    public override void _Process(float delta)
    {
        var version = Engine.GetVersionInfo();
        var memUsedBytes = Performance.GetMonitor(Performance.Monitor.RenderVideoMemUsed);
        var memUsedMegs = memUsedBytes / 1_000_000.0;

        _Label.Text = ""
            + $" Godot Engine {version["string"]}\n"
            + $" FPS: {Engine.GetFramesPerSecond()}\n"
            + $" Process time: {Performance.GetMonitor(Performance.Monitor.TimeProcess) * 1_000} ms\n"
            + $" Physics process time: {Performance.GetMonitor(Performance.Monitor.TimePhysicsProcess) * 1_000} ms\n"
            + $" Audio latency: {Performance.GetMonitor(Performance.Monitor.AudioOutputLatency) * 1_000} ms\n"
            + $" Video memory used: {memUsedMegs} MB\n"
            + "\n"
            + $" Draw calls: {Performance.GetMonitor(Performance.Monitor.Render2dDrawCallsInFrame)}\n"
            + $" 2D items: {Performance.GetMonitor(Performance.Monitor.Render2dItemsInFrame)}\n"
            + "\n"
            + $" Object count: {Performance.GetMonitor(Performance.Monitor.ObjectCount)}\n"
            + $" Node count: {Performance.GetMonitor(Performance.Monitor.ObjectNodeCount)}\n"
            + $" Orphaned node count: {Performance.GetMonitor(Performance.Monitor.ObjectOrphanNodeCount)}\n"
            + $" Resource count: {Performance.GetMonitor(Performance.Monitor.ObjectResourceCount)}\n"
            + "";
    }
}
