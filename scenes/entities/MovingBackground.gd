extends CanvasLayer
class_name MovingBackground

export var sprites_count := 10
var background_color := Color.black

class BackgroundSprite:
    extends Sprite

    var speed := 10.0
    var initial_t := 0.0

    var _t := 0.0

    func _ready() -> void:
        texture = GameLoadCache.load_resource("WhitePixel")
        _t = initial_t

        var pos := SxRand.range_vec2(Vector2.ZERO, get_viewport_rect().size)
        global_position = pos

    func _process(delta: float) -> void:
        var vp_size := get_viewport_rect().size
        var pos := global_position
        pos.x -= sin(_t) * speed * delta

        if pos.x < -scale.x / 2:
            pos.x = vp_size.x + scale.x

        global_position = pos
        _t = wrapf(_t + delta, 0, PI * 2)

onready var _background: ColorRect = $ColorRect
onready var _sprites: Node2D = $Sprites

func _ready() -> void:
    _background.color = background_color

    for _i in range(sprites_count):
        var sprite := BackgroundSprite.new()
        sprite.speed = rand_range(10.0, 100.0)
        sprite.scale = Vector2(rand_range(50.0, 100.0), 2)
        sprite.modulate = SxColor.rand()

        _sprites.add_child(sprite)
