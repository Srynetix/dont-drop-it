using Godot;
using SxGD;

public class Game : Control
{
    [Export]
    public int CurrentLevelIdx = 1;

    private Level CurrentLevel = null;

    public override void _Ready()
    {
        LoadLevel(CurrentLevelIdx);
    }

    public void LoadLevel(int levelId)
    {
        var levelPath = $"Level{levelId}";
        if (LoadCache.GetInstance().HasScene(levelPath))
        {
            if (CurrentLevel != null)
            {
                CurrentLevel.QueueFree();
            }
            CurrentLevel = LoadCache.GetInstance().InstantiateScene<Level>(levelPath);
            CurrentLevel.Connect(nameof(Level.success), this, nameof(LoadNextLevel));
            CurrentLevel.Connect(nameof(Level.restart), this, nameof(ReloadCurrentLevel));
            AddChild(CurrentLevel);

            CurrentLevelIdx = levelId;
        }
        else
        {
            GD.PrintErr($"Unknown level number {levelId}");
        }
    }

    public void LoadNextLevel()
    {
        LoadLevel(CurrentLevelIdx + 1);
    }

    public void ReloadCurrentLevel()
    {
        LoadLevel(CurrentLevelIdx);
    }
}
