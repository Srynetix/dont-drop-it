extends RigidBody2D
class_name Bomb

signal exploded()
signal win()

export var speed_limit := 1000.0
export var enable_input := true
export var sound_at_init := true

var locked := false

onready var sprite: Sprite = $Sprite
onready var _timer: Timer = $Timer
onready var _animation_player: AnimationPlayer = $AnimationPlayer
onready var _mouse_joint: MouseJoint = $MouseJoint
onready var _area: Area2D = $Area
onready var _audio_player: SxAudioMultiStreamPlayer = $AudioMultiStreamPlayer
onready var _speed_limit_squared := speed_limit * speed_limit

var _end := false
var _dizzy := false
var _on_security := false
var _loop_player: AudioStreamPlayer
var _initial_gravity_scale := 0.0

func _ready() -> void:
    _timer.connect("timeout", self, "_timeout")
    _timer.start()

    connect("body_shape_entered", self, "_on_body_shape_entered")
    _area.connect("area_entered", self, "_on_area_entered")
    _area.connect("area_exited", self, "_on_area_exited")
    _initial_gravity_scale = gravity_scale
    _animation_player.play("show")

    if sound_at_init:
        _play_effect("DisintegrateFX", 1)

    var loop: AudioStreamOGGVorbis = GameLoadCache.load_resource("AmbientLoop")
    _loop_player = _audio_player.get_voice(3)
    _loop_player.stream = loop
    _loop_player.volume_db = -100
    _loop_player.play()

    var bounce_player := _audio_player.get_voice(0)
    bounce_player.volume_db = -20

    if !enable_input:
        _disable_mouse_joint()

func _physics_process(_delta: float) -> void:
    if _end:
        return

    var ratio := speed_ratio()
    _loop_player.volume_db = SxMath.map(ratio, 0, 1, -80, -10)

    if linear_velocity.length_squared() > _speed_limit_squared:
        explode()

    if _dizzy:
        sprite.modulate = Color.yellow
    elif _on_security:
        sprite.modulate = Color.blue
        if _mouse_joint.active:
            explode()
    else:
        var color := Color.white.linear_interpolate(Color.red, ratio)
        sprite.modulate = color

func shake() -> void:
    var amount := 200.0
    var rand_x := rand_range(-1.0, 1.0)
    var rand_y := rand_range(-1.0, 1.0)
    var dir := Vector2(rand_x, rand_y).normalized()
    var impulse := dir * amount
    apply_central_impulse(impulse)
    apply_torque_impulse(amount)

func explode() -> void:
    if _end:
        return

    _loop_player.stop()
    _play_effect("ExplosionFX", 1)
    _disable_mouse_joint()
    _end = true
    sleeping = true

    emit_signal("exploded")
    _animation_player.play("explode")
    yield(_animation_player, "animation_finished")
    queue_free()

func win() -> void:
    if _end:
        return

    _loop_player.stop()
    _play_effect("SuccessFX", 1)
    _disable_mouse_joint()
    _end = true
    sleeping = true

    emit_signal("win")
    _animation_player.play("win")
    yield(_animation_player, "animation_finished")
    queue_free()

func _disable_mouse_joint() -> void:
    _mouse_joint.visible = false
    _mouse_joint.set_physics_process(false)

func speed_ratio() -> float:
    return linear_velocity.length_squared() / _speed_limit_squared

func _timeout() -> void:
    if _dizzy:
        shake()

func _on_body_shape_entered(_body_id: RID, body: Node, body_shape: int, _local_shape: int) -> void:
    if _end:
        return

    if body is StaticBody2D:
        var static_body: StaticBody2D = body
        var wall_node := static_body.get_node("WallTileMap")
        if wall_node is TileMap:
            var tilemap: TileMap = wall_node
            var stream: AudioStreamSample = GameLoadCache.load_resource("BounceFX")
            _audio_player.play_on_voice(stream, 0)

            var coord: Vector2 = Physics2DServer.body_get_shape_metadata(static_body.get_rid(), body_shape)
            var cell_idx := tilemap.get_cellv(coord)
            var cell_name := tilemap.tile_set.tile_get_name(cell_idx)
            _collide_with_tile(cell_name)

func _collide_with_tile(tile_name: String) -> void:
    match tile_name:
        "WallBorder":
            explode()
        "Security":
            _on_security = true
        "End":
            win()
        "Locked":
            if !locked:
                _play_effect("LockedFX", 2)
                locked = true
                gravity_scale = 0
                _disable_mouse_joint()
        "Dizzy":
            if !_dizzy:
                _play_effect("DizzyFX", 2)
                _dizzy = true
                _timer.start()
        "Normal":
            if _dizzy:
                _play_effect("NormalFX", 2)
                _dizzy = false
            if locked:
                _play_effect("NormalFX", 2)
                _enable_mouse_joint()
                gravity_scale = _initial_gravity_scale
                locked = false

func _play_effect(fx_name: String, voice_idx: int) -> void:
    var stream: AudioStreamSample = GameLoadCache.load_resource(fx_name)
    _audio_player.play_on_voice(stream, voice_idx)

func _enable_mouse_joint() -> void:
    _mouse_joint.visible = true
    _mouse_joint.set_physics_process(true)

func _on_area_entered(area: Area2D) -> void:
    if _end:
        return
    
    if area is ZoneTile:
        var zone_tile: ZoneTile = area
        _collide_with_tile(zone_tile.tile_name)

func _on_area_exited(area: Area2D) -> void:
    if _end:
        return

    if area is ZoneTile:
        var zone_tile: ZoneTile = area
        _exit_tile_collision(zone_tile.tile_name)

func _exit_tile_collision(tile_name: String) -> void:
    match tile_name:
        "Security":
            _on_security = false