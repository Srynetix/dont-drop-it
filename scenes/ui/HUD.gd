extends CanvasLayer
class_name HUD

signal times_up()

export var initial_seconds := 60.0

var remaining_time := 0.0

onready var _timer: Timer = $Timer
onready var _remaining_time_label: Label = $Margin/RemainingTime
onready var _animation_player: AnimationPlayer = $AnimationPlayer

func _ready() -> void:
    _timer.connect("timeout", self, "_tick")
    remaining_time = initial_seconds

func start() -> void:
    _update_time_label()
    _animation_player.play("start")
    _timer.start()

func stop() -> void:
    _timer.stop()

func _tick() -> void:
    if remaining_time > 0:
        remaining_time -= 0.01
        _update_time_label()
    else:
        _timer.stop()
        remaining_time = 0.0
        emit_signal("times_up")

func _update_time_label() -> void:
    _remaining_time_label.text = "%0.2f'" % remaining_time

func elapsed_time() -> float:
    return initial_seconds - remaining_time