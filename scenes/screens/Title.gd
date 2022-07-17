extends Control
class_name Title

onready var _play: Button = $Center/VBox/Play
onready var _bomb: Bomb = $Bomb
onready var _animation_player: AnimationPlayer = $AnimationPlayer
onready var _footer: RichTextLabel = $Footer

func _ready() -> void:
    _play.connect("pressed", self, "_start_game")
    _footer.connect("meta_clicked", self, "_link_clicked")

    var title_music: AudioStreamOGGVorbis = GameLoadCache.load_resource("MainSong")
    GameGlobalMusicPlayer.fade_in()
    GameGlobalMusicPlayer.play_stream(title_music)
    yield(get_tree().create_timer(3), "timeout")

    _show_anim()

func _show_anim() -> void:
    _animation_player.play("play")
    yield(_animation_player, "animation_finished")

    _play.grab_focus()
    _animation_player.play("colors")

func _start_game() -> void:
    GameGlobalMusicPlayer.fade_out()
    GameSceneTransitioner.fade_to_scene_path("res://scenes/screens/Game.tscn")

func _link_clicked(data) -> void:
    if data is String:
        var string_data: String = data
        OS.shell_open(string_data)