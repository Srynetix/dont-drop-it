extends Control
class_name Game

export var current_level_idx := 1

var _current_level: Level = null

func _ready() -> void:
    GameData.deaths = 0
    GameData.time = 0.0

    _load_level(current_level_idx)

func _load_level(level_id: int) -> void:
    var level_path := "Level%d" % level_id
    if GameLoadCache.has_scene(level_path):
        if _current_level != null:
            GameSceneTransitioner.fade_out()
            yield(GameSceneTransitioner, "animation_finished")
            _current_level.queue_free()

        _current_level = GameLoadCache.instantiate_scene(level_path)
        _current_level.connect("success", self, "_load_next_level")
        _current_level.connect("restart", self, "_reload_current_level")
        add_child(_current_level)

        GameSceneTransitioner.fade_in()
        yield(GameSceneTransitioner, "animation_finished")

        current_level_idx = level_id
    else:
        # Credits
        GameSceneTransitioner.fade_to_cached_scene(GameLoadCache, "Credits")

func _load_next_level(time: float) -> void:
    GameData.time = GameData.time + time
    _load_level(current_level_idx + 1)

func _reload_current_level() -> void:
    GameData.deaths = GameData.deaths + 1
    _load_level(current_level_idx)