using Godot;
using SxGD;

public class MouseJoint : Position2D
{
    [Export]
    public float Speed = 100;
    [Export]
    public bool Debug;
    [Export]
    public float Radius = 0;

    public bool Active
    {
        get => _Active;
    }

    private RigidBody2D _Parent;
    private bool _Active;
    private int _LastTouchIdx = -1;

    public override void _Ready()
    {
        _Parent = GetParent<RigidBody2D>();
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_Active)
        {
            var vpSize = GetViewportRect().Size;
            var vpSizeLen = vpSize.Length();
            var position = GetGlobalMousePosition();
            var rel = position - _Parent.Position;
            var dist = rel.Length();
            var ratio = dist / vpSizeLen;
            var vel = rel.Normalized() * Speed * ratio;
            _Parent.LinearVelocity += vel;
        }

        Update();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventScreenTouch touch)
        {
            if (_LastTouchIdx == -1 && touch.Pressed)
            {
                _LastTouchIdx = touch.Index;
            }

            if (_LastTouchIdx == touch.Index)
            {
                if (!touch.Pressed)
                {
                    _LastTouchIdx = -1;
                    _Active = false;
                }

                else
                {
                    if (Radius > 0)
                    {
                        var worldPosition = GetCanvasTransform().XformInv(touch.Position);
                        var dist = worldPosition.DistanceSquaredTo(_Parent.Position);
                        if (dist < Radius * Radius)
                        {
                            _Active = true;
                        }
                    }
                }
            }
        }
    }

    public override void _Draw()
    {
        if (Debug)
        {
            if (_Active)
            {
                DrawLine(Vector2.Zero, (GetGlobalMousePosition() - _Parent.GlobalPosition).Rotated(-_Parent.GlobalRotation), Colors.Gray.WithAlphaf(0.25f), 2);
            }
            else if (Radius > 0)
            {
                DrawCircle(Vector2.Zero, Radius, Colors.LightBlue.WithAlphaf(0.25f));
            }
        }
    }
}
