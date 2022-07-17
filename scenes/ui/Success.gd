extends CanvasLayer
class_name Success

signal success()

onready var _animation_player: AnimationPlayer = $AnimationPlayer
onready var _next: Button = $Center/VBox/Next
onready var _time_label: Label = $Center/VBox/TimeMessage/Time

func _ready() -> void:
    _next.disabled = true
    _next.mouse_filter = Control.MOUSE_FILTER_IGNORE
    _next.connect("pressed", self, "_continue")

func start(time: float) -> void:
    _time_label.text = "%0.2f'" % time
    _next.disabled = false
    _next.mouse_filter = Control.MOUSE_FILTER_STOP
    _next.grab_focus()
    _animation_player.play("success")

func _continue() -> void:
    emit_signal("success")