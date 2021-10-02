using SxGD;
using Godot;

public class GameLoadCache : LoadCache
{
    public const int LEVEL_COUNT = 6;

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
