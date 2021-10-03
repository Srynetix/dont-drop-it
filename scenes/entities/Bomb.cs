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

    public float SpeedRatio
    {
        get => LinearVelocity.LengthSquared() / _SpeedLimitSquared;
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

    private float _SpeedLimitSquared;
    private bool _End;
    private bool _Dizzy;
    private bool _OnSecurity;

    public override void _Ready()
    {
        NodeExt.BindNodes(this);

        _Timer.Connect("timeout", this, nameof(TimeOut));
        _Timer.Start();

        Connect("body_shape_entered", this, nameof(BodyShapeEntered));
        _Area.Connect("area_entered", this, nameof(AreaEntered));
        _Area.Connect("area_exited", this, nameof(AreaExited));

        _SpeedLimitSquared = SpeedLimit * SpeedLimit;
        _AnimationPlayer.Play("show");
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_End)
        {
            return;
        }

        if (LinearVelocity.LengthSquared() > _SpeedLimitSquared)
        {
            Explode();
        }
    }

    public override void _Process(float delta)
    {
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
            var ratio = LinearVelocity.LengthSquared() / _SpeedLimitSquared;
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

        _MouseJoint.Visible = false;
        _MouseJoint.SetPhysicsProcess(false);
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

        _MouseJoint.Visible = false;
        _MouseJoint.SetPhysicsProcess(false);
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
                var coord = (Vector2)Physics2DServer.BodyGetShapeMetadata(staticBody.GetRid(), bodyShape);
                var cellIdx = map.GetCellv(coord);
                var cellName = map.TileSet.TileGetName(cellIdx);
                CollideWithTile(cellName);
            }
        }
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

        else if (name == "Dizzy")
        {
            if (!_Dizzy)
            {
                _Dizzy = true;
                _Timer.Start();
            }
        }

        else if (name == "Normal")
        {
            _Dizzy = false;
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