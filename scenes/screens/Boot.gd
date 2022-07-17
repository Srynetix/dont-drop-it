extends Control
class_name Boot

func _ready() -> void:
    GameSceneTransitioner.fade_to_scene_path("res://scenes/screens/Title.tscn")