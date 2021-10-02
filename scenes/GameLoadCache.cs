using SxGD;

public class GameLoadCache: LoadCache {
    public override void Initialize()
    {
        // Loading scenes
        StoreScene<Bomb>("res://scenes/entities/Bomb.tscn");
        StoreScene<ZoneTile>("res://scenes/entities/ZoneTile.tscn");

        // Loading levels
        for (int i = 1; i <= 3; ++i) {
            StoreScene($"Level{i}", $"res://scenes/levels/Level{i}.tscn");
        }
    }
}
