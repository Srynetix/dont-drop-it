extends CanvasLayer
class_name GameOver

signal restart()

onready var _animation_player: AnimationPlayer = $AnimationPlayer
onready var _try_again: Button = $Center/VBox/TryAgain

func _ready() -> void:
    _try_again.disabled = true
    _try_again.mouse_filter = Control.MOUSE_FILTER_IGNORE
    _try_again.connect("pressed", self, "_restart_game")

func start() -> void:
    _try_again.disabled = false
    _try_again.mouse_filter = Control.MOUSE_FILTER_STOP
    _try_again.grab_focus()
    _animation_player.play("gameover")

func _restart_game() -> void:
    emit_signal("restart")