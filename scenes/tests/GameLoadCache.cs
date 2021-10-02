using SxGD;
using Godot;

public class GameLoadCache : LoadCache
{
    private const int LEVEL_COUNT = 4;

    public override void Initialize()
    {
        // Loading scenes
        StoreScene<Bomb>("res://scenes/tests/Bomb.tscn");
        StoreScene<ZoneTile>("res://scenes/tests/ZoneTile.tscn");

        // Loading levels
        for (int i = 1; i <= LEVEL_COUNT; ++i)
        {
            StoreScene($"Level{i}", $"res://scenes/tests/levels/Level{i}.tscn");
        }
    }
}
