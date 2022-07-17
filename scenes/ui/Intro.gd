extends CanvasLayer
class_name Intro

signal finished()

onready var _level_num: Label = $Center/VBox/HBox/Num
onready var _name: Label = $Center/VBox/Name
onready var _animation_player: AnimationPlayer = $AnimationPlayer

func set_level_name(name: String) -> void:
    _name.text = name

func set_level_number(number: int) -> void:
    _level_num.text = str(number)

func _ready() -> void:
    _animation_player.play("intro")
    yield(_animation_player, "animation_finished")

    emit_signal("finished")
    queue_free()