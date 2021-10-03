using Godot;
using SxGD;

public class Bomb : RigidBody2D
{
    [Signal]
    public delegate void exploded();

    [Signal]
    public delegate void win();

    [Export]
    public float SpeedLimit = 1000.0f;
    [Export]
    public bool EnableInput = true;
    [Export]
    public bool SoundAtInit = true;

    public float SpeedRatio
    {
        get => LinearVelocity.LengthSquared() / _SpeedLimitSquared;
    }

    public bool Locked
    {
        get => _Locked;
    }

    public Sprite Sprite
    {
        get => _Sprite;
    }

    [OnReady("Timer")]
    private readonly Timer _Timer;
    [OnReady("AnimationPlayer")]
    private readonly AnimationPlayer _AnimationPlayer;
    [OnReady("MouseJoint")]
    private readonly MouseJoint _MouseJoint;
    [OnReady("Sprite")]
    private readonly Sprite _Sprite;
    [OnReady("Area")]
    private readonly Area2D _Area;
    [OnReady]
    private readonly AudioMultiStreamPlayer _AudioPlayer;

    private float _SpeedLimitSquared;
    private bool _End;
    private bool _Dizzy;
    private bool _Locked;
    private bool _OnSecurity;
    private AudioStreamPlayer _LoopPlayer;
    private float _InitialGravityScale;

    public override void _Ready()
    {
        NodeExt.BindNodes(this);

        _Timer.Connect("timeout", this, nameof(TimeOut));
        _Timer.Start();

        Connect("body_shape_entered", this, nameof(BodyShapeEntered));
        _Area.Connect("area_entered", this, nameof(AreaEntered));
        _Area.Connect("area_exited", this, nameof(AreaExited));
        _InitialGravityScale = GravityScale;

        _SpeedLimitSquared = SpeedLimit * SpeedLimit;
        _AnimationPlayer.Play("show");

        if (SoundAtInit)
        {
            var stream = LoadCache.GetInstance().LoadResource<AudioStreamSample>("DisintegrateFX");
            _AudioPlayer.Play(stream, voice: 1);
        }

        var loop = LoadCache.GetInstance().LoadResource<AudioStreamOGGVorbis>("AmbientLoop");
        _LoopPlayer = _AudioPlayer.GetVoice(3);
        _LoopPlayer.Stream = loop;
        _LoopPlayer.VolumeDb = -100;
        _LoopPlayer.Play();

        var bouncePlayer = _AudioPlayer.GetVoice(0);
        bouncePlayer.VolumeDb = -20;

        if (!EnableInput)
        {
            DisableMouseJoint();
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_End)
        {
            return;
        }

        var ratio = LinearVelocity.LengthSquared() / _SpeedLimitSquared;
        _LoopPlayer.VolumeDb = MathExt.Map(ratio, 0, 1, -80, -10);

        if (LinearVelocity.LengthSquared() > _SpeedLimitSquared)
        {
            Explode();
        }

        if (_Dizzy)
        {
            _Sprite.Modulate = Colors.Yellow;
        }
        else if (_OnSecurity)
        {
            _Sprite.Modulate = Colors.Blue;
            if (_MouseJoint.Active)
            {
                Explode();
            }
        }
        else
        {
            // Adapt tint depending on speed ratio
            var color = Colors.White.LinearInterpolate(Colors.Red, ratio);
            _Sprite.Modulate = color;
        }
    }

    public void Shake()
    {
        var amount = 200.0f;
        var randX = MathExt.RandRange(-1.0f, 1.0f);
        var randY = MathExt.RandRange(-1.0f, 1.0f);
        var dir = new Vector2(randX, randY).Normalized();
        var impulse = dir * amount;
        ApplyCentralImpulse(impulse);
        ApplyTorqueImpulse(amount);
    }

    public async void Explode()
    {
        if (_End)
        {
            return;
        }

        _LoopPlayer.Stop();

        var stream = LoadCache.GetInstance().LoadResource<AudioStreamSample>("ExplosionFX");
        _AudioPlayer.Play(stream, voice: 1);

        DisableMouseJoint();
        _End = true;
        Sleeping = true;

        EmitSignal(nameof(exploded));
        _AnimationPlayer.Play("explode");
        await ToSignal(_AnimationPlayer, "animation_finished");
        QueueFree();
    }

    public async void Win()
    {
        if (_End)
        {
            return;
        }

        _LoopPlayer.Stop();

        var stream = LoadCache.GetInstance().LoadResource<AudioStreamSample>("SuccessFX");
        _AudioPlayer.Play(stream, voice: 1);

        DisableMouseJoint();
        _End = true;

        EmitSignal(nameof(win));
        _AnimationPlayer.Play("win");
        await ToSignal(_AnimationPlayer, "animation_finished");
        QueueFree();
    }

    private void TimeOut()
    {
        if (_Dizzy)
        {
            Shake();
        }
    }

    private void BodyShapeEntered(int bodyId, Node body, int bodyShape, int localShape)
    {
        if (_End)
        {
            return;
        }

        if (body is StaticBody2D staticBody)
        {
            if (staticBody.GetNode("WallTileMap") is TileMap map)
            {
                // Play sound
                var stream = LoadCache.GetInstance().LoadResource<AudioStreamSample>("BounceFX");
                _AudioPlayer.Play(stream, voice: 0);

                var coord = (Vector2)Physics2DServer.BodyGetShapeMetadata(staticBody.GetRid(), bodyShape);
                var cellIdx = map.GetCellv(coord);
                var cellName = map.TileSet.TileGetName(cellIdx);
                CollideWithTile(cellName);
            }
        }
    }

    public void DisableMouseJoint()
    {
        _MouseJoint.Visible = false;
        _MouseJoint.SetPhysicsProcess(false);
    }

    public void EnableMouseJoint()
    {
        _MouseJoint.Visible = true;
        _MouseJoint.SetPhysicsProcess(true);
    }

    private void AreaEntered(Area2D area)
    {
        if (_End)
        {
            return;
        }

        if (area is ZoneTile tile)
        {
            CollideWithTile(tile.TileName);
        }
    }

    private void AreaExited(Area2D area)
    {
        if (_End)
        {
            return;
        }

        if (area is ZoneTile tile)
        {
            ExitTileCollision(tile.TileName);
        }
    }

    private void CollideWithTile(string name)
    {
        if (name == "WallBorder")
        {
            Explode();
        }

        else if (name == "Security")
        {
            _OnSecurity = true;
        }

        else if (name == "End")
        {
            Win();
        }

        else if (name == "Locked")
        {
            if (!_Locked)
            {
                var stream = LoadCache.GetInstance().LoadResource<AudioStreamSample>("LockedFX");
                _AudioPlayer.Play(stream, voice: 2);

                _Locked = true;
                GravityScale = 0;
                DisableMouseJoint();
            }
        }

        else if (name == "Dizzy")
        {
            if (!_Dizzy)
            {
                var stream = LoadCache.GetInstance().LoadResource<AudioStreamSample>("DizzyFX");
                _AudioPlayer.Play(stream, voice: 2);

                _Dizzy = true;
                _Timer.Start();
            }
        }

        else if (name == "Normal")
        {
            if (_Dizzy)
            {
                var stream = LoadCache.GetInstance().LoadResource<AudioStreamSample>("NormalFX");
                _AudioPlayer.Play(stream, voice: 2);

                _Dizzy = false;
            }

            if (_Locked)
            {
                var stream = LoadCache.GetInstance().LoadResource<AudioStreamSample>("NormalFX");
                _AudioPlayer.Play(stream, voice: 2);

                EnableMouseJoint();
                GravityScale = _InitialGravityScale;
                _Locked = false;
            }
        }
    }

    private void ExitTileCollision(string name)
    {
        if (name == "Security")
        {
            _OnSecurity = false;
        }
    }
}
