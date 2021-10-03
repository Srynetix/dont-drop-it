using Godot;
using SxGD;

public class Game : Control
{
    [Export]
    public int CurrentLevelIdx = 1;

    private Level _CurrentLevel = null;
    private int _TotalDeaths = 0;
    private float _TotalTime = 0;

    public override void _Ready()
    {
        // Reset data
        GameData.Instance.Store("deaths", 0);
        GameData.Instance.Store("time", 0.0f);

        LoadLevel(CurrentLevelIdx);
    }

    public async void LoadLevel(int levelId)
    {
        var levelPath = $"Level{levelId}";
        if (LoadCache.GetInstance().HasScene(levelPath))
        {
            var transitioner = SceneTransitioner.GetInstance();

            if (_CurrentLevel != null)
            {
                transitioner.FadeOut();
                await ToSignal(transitioner, nameof(SceneTransitioner.animation_finished));
                _CurrentLevel.QueueFree();
            }

            _CurrentLevel = LoadCache.GetInstance().InstantiateScene<Level>(levelPath);
            _CurrentLevel.Connect(nameof(Level.success), this, nameof(LoadNextLevel));
            _CurrentLevel.Connect(nameof(Level.restart), this, nameof(ReloadCurrentLevel));
            AddChild(_CurrentLevel);

            transitioner.FadeIn();
            await ToSignal(transitioner, nameof(SceneTransitioner.animation_finished));

            CurrentLevelIdx = levelId;
        }
        else
        {
            // Go to credits
            GameData.Instance.Store("deaths", _TotalDeaths);
            GameData.Instance.Store("time", _TotalTime);

            var scene = LoadCache.GetInstance().LoadScene("Credits");
            SceneTransitioner.GetInstance().FadeToScene(scene);
        }
    }

    public void LoadNextLevel(float time)
    {
        _TotalTime += time;
        GD.Print("Deaths: ", _TotalDeaths, " / Time: ", _TotalTime);
        LoadLevel(CurrentLevelIdx + 1);
    }

    public void ReloadCurrentLevel()
    {
        _TotalDeaths += 1;
        LoadLevel(CurrentLevelIdx);
    }
}
