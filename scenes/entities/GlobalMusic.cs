using Godot;
using SxGD;

public class GlobalMusic : AudioStreamPlayer
{
    public static GlobalMusic Instance
    {
        get => _GlobalInstance;
    }

    [OnReady]
    private Tween _Tween;
    private static GlobalMusic _GlobalInstance;

    public float GlobalVolumeDb
    {
        get => _GlobalVolumeDb;
        set
        {
            _GlobalVolumeDb = value;
            VolumeDb = value;
        }
    }

    private float _GlobalVolumeDb;

    public override void _Ready()
    {
        if (_GlobalInstance == null)
        {
            _GlobalInstance = this;
        }

        _GlobalVolumeDb = VolumeDb;
        NodeExt.BindNodes(this);
    }

    public void Play(AudioStream stream)
    {
        Stream = stream;
        Play();
    }

    public void FadeIn()
    {
        _Tween.InterpolateProperty(this, "volume_db", -100, GlobalVolumeDb, 2);
        _Tween.Start();
    }

    public void FadeOut()
    {
        _Tween.InterpolateProperty(this, "volume_db", GlobalVolumeDb, -100, 2);
        _Tween.Start();
    }
}
