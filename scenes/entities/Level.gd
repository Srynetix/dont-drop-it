extends Control
class_name Level

signal success(time)
signal restart()

export var level_number := 0
export var level_name := "Unknown"

onready var _zone_tilemap: ZoneTileMap = $ZoneTileMap
onready var _wall_tilemap: TileMap = $WallCollider/WallTileMap
onready var _game_over: GameOver = $GameOver
onready var _success: Success = $Success
onready var _intro: Intro = $Intro
onready var _help: SxFadingRichTextLabel = $MarginContainer/Help
onready var _camera: SxFXCamera = $FXCamera
onready var _vignette: SxVignette = $FX/Vignette
onready var _hud: HUD = $HUD

var _is_gameover := false
var _bomb: Bomb
var _start_position := Vector2.ZERO

func _ready() -> void:
    _start_position = _zone_tilemap.get_start_position()
    _intro.set_level_name(level_name)
    _intro.set_level_number(level_number)
    _intro.connect("finished", self, "_start_game")
    _success.connect("success", self, "_continue")
    _game_over.connect("restart", self, "_restart_game")
    _hud.connect("times_up", self, "_times_up")

    # Prepare camera
    var rect := _wall_tilemap.get_used_rect()
    var size := rect.position + rect.size
    var vp_size := get_viewport_rect().size
    _camera.limit_left = 0
    _camera.limit_top = 0
    _camera.smoothing_enabled = true
    _camera.limit_right = int(max(size.x * _wall_tilemap.cell_size.x, vp_size.x))
    _camera.limit_bottom = int(max(size.y  * _wall_tilemap.cell_size.y, vp_size.y))

    var track_name := "Track1"
    if level_number > 5:
        track_name = "Track2"

    var title_music: AudioStreamOGGVorbis = GameLoadCache.load_resource(track_name)
    GameGlobalMusicPlayer.play_stream(title_music)
    GameGlobalMusicPlayer.fade_in()

func _start_game() -> void:
    _help.fade_in()
    _hud.start()

    _bomb = GameLoadCache.instantiate_scene("Bomb")
    _bomb.position = _start_position
    _bomb.connect("exploded", self, "_on_game_over")
    _bomb.connect("win", self, "_on_success")
    add_child(_bomb)

func _times_up() -> void:
    _bomb.explode()

func _process(_delta: float) -> void:
    if _bomb != null:
        _camera.shake_ratio = _bomb.speed_ratio()
        _vignette.vignette_ratio = 0.01 + _bomb.speed_ratio() / 8.0
        _camera.global_position = _bomb.position

func _on_game_over() -> void:
    if _is_gameover:
        return

    _hud.stop()
    _vignette.fade(0.25)

    _bomb = null
    _is_gameover = true
    _camera.shake_ratio = 0
    _game_over.start()

func _on_success() -> void:
    if _is_gameover:
        return

    _hud.stop()
    _vignette.fade(1.0)

    _bomb = null
    _is_gameover = true
    _camera.shake_ratio = 0
    _success.start(_hud.elapsed_time())

func _restart_game() -> void:
    if get_parent() == get_tree().root:
        # We can restart
        get_tree().reload_current_scene()
    else:
        emit_signal("restart")

func _continue() -> void:
    if get_parent() == get_tree().root:
        # We can quit
        get_tree().quit()
    else:
        emit_signal("success", _hud.elapsed_time())