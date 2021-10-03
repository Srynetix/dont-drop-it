using SxGD;
using Godot;

public class GameLoadCache : LoadCache
{
    public override void Initialize()
    {
        // Loading scenes
        StoreScene<Bomb>("res://scenes/entities/Bomb.tscn");
        StoreScene<ZoneTile>("res://scenes/entities/ZoneTile.tscn");

        // Loading screens
        StoreScene<Title>("res://scenes/screens/Title.tscn");
        StoreScene<Credits>("res://scenes/screens/Credits.tscn");

        // Loading sounds
        StoreResource<AudioStreamOGGVorbis>("AmbientLoop", "res://assets/sounds/AmbientBomb.ogg");
        StoreResource<AudioStreamSample>("BounceFX", "res://assets/sounds/Bounce.wav");
        StoreResource<AudioStreamSample>("DisintegrateFX", "res://assets/sounds/Disintegrate.wav");
        StoreResource<AudioStreamSample>("DizzyFX", "res://assets/sounds/Dizzy.wav");
        StoreResource<AudioStreamSample>("ExplosionFX", "res://assets/sounds/Explosion.wav");
        StoreResource<AudioStreamSample>("NormalFX", "res://assets/sounds/Normal.wav");
        StoreResource<AudioStreamSample>("SecurityFX", "res://assets/sounds/Security.wav");
        StoreResource<AudioStreamSample>("SuccessFX", "res://assets/sounds/Success.wav");
        StoreResource<AudioStreamSample>("LockedFX", "res://assets/sounds/Locked.wav");

        // Loading music
        StoreResource<AudioStreamOGGVorbis>("MainSong", "res://assets/sounds/MainSong.ogg");
        StoreResource<AudioStreamOGGVorbis>("Track1", "res://assets/sounds/Track1.ogg");
        StoreResource<AudioStreamOGGVorbis>("Track2", "res://assets/sounds/Track2.ogg");

        // Loading textures
        StoreResource<Texture>("WhitePixel", "res://assets/textures/WhitePixel.png");

        // Loading levels
        for (int i = 1; i <= 10; ++i)
        {
            StoreScene($"Level{i}", $"res://scenes/levels/Level{i}.tscn");
        }
    }
}
