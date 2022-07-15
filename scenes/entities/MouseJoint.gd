extends Position2D
class_name MouseJoint

export var speed := 100.0
export var debug := false
export var radius := 0.0

var active := false

onready var _parent: RigidBody2D = get_parent()

var _last_touch_idx := -1

func _physics_process(_delta: float) -> void:
    if active:
        var vp_size := get_viewport_rect().size
        var vp_size_len := vp_size.length()
        var position := get_global_mouse_position()
        var rel := position - _parent.position
        var dist := rel.length()
        var ratio := dist / vp_size_len
        var vel := rel.normalized() * speed * ratio
        
        _parent.linear_velocity += vel

    update()

func _input(event: InputEvent) -> void:
    if event is InputEventScreenTouch:
        var touch_event: InputEventScreenTouch = event
        if _last_touch_idx == -1 && touch_event.pressed:
            _last_touch_idx = touch_event.index
        
        if _last_touch_idx == touch_event.index:
            if !touch_event.pressed:
                _last_touch_idx = -1
                active = false
            
            else:
                if radius > 0:
                    var world_position = get_canvas_transform().xform_inv(touch_event.position)
                    var dist = world_position.distance_squared_to(_parent.position)
                    if dist < radius * radius:
                        active = true

func _draw() -> void:
    if debug:
        if active:
            draw_line(Vector2.ZERO, (get_global_mouse_position() - _parent.global_position).rotated(-_parent.global_rotation), SxColor.with_alpha_f(Color.gray, 0.25), 2)
        elif radius > 0:
            draw_circle(Vector2.ZERO, radius, SxColor.with_alpha_f(Color.lightblue, 0.25))