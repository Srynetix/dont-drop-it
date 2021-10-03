using Godot;
using SxGD;

public class MovingBackground : CanvasLayer
{
    public Color BackgroundColor = Colors.Black;

    [OnReady]
    private readonly ColorRect _Background;
    [OnReady("Sprites")]
    private readonly Node2D _Sprites;

    private class BackgroundSprite : Sprite
    {
        public float Speed = 10;
        public float InitialT = 0;

        private float t;

        public override void _Ready()
        {
            Texture = LoadCache.GetInstance().LoadResource<Texture>("WhitePixel");
            t = InitialT;

            var pos = MathExt.RandRange(Vector2.Zero, GetViewportRect().Size);
            GlobalPosition = pos;
        }

        public override void _Process(float delta)
        {
            var vpSize = GetViewportRect().Size;
            var pos = GlobalPosition;
            pos.x -= Mathf.Sin(t) * Speed * delta;

            if (pos.x < -Scale.x / 2)
            {
                pos.x = vpSize.x + Scale.x;
            }

            GlobalPosition = pos;
            t += delta;

            while (t > Mathf.Pi)
            {
                t -= Mathf.Pi;
            }
        }
    }

    public override void _Ready()
    {
        NodeExt.BindNodes(this);

        _Background.Color = BackgroundColor;

        var spriteCount = 20;
        for (int i = 0; i < spriteCount; ++i)
        {
            var sprite = new BackgroundSprite()
            {
                Speed = MathExt.RandRange(10, 100),
                Scale = new Vector2(MathExt.RandRange(50, 100), 2),
                Modulate = ColorExt.Rand(),
            };

            _Sprites.AddChild(sprite);
        }
    }
}
