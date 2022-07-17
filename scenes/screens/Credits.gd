extends Control
class_name Credits

const NO_DEATHS_MESSAGE := """
[rainbow]Congratulations![/rainbow]

You did not drop it!
And finished in [color=green]%0.2f[/color] seconds!
You can close the game now, [rainbow]thanks[/rainbow]!
"""

const ONE_DEATH_MESSAGE := """
You almost did it!

You dropped the bomb [color=red]one[/color] time.
And finished in [color=green]%0.2f[/color] seconds.
Next time is the right one!
"""

const DEFAULT_MESSAGE := """
You won!

But you did drop the bomb [color=red]%d[/color] times.
And finished in [color=green]%0.2f[/color] seconds.
You can do better!
"""

onready var _zone_tilemap: ZoneTileMap = $ZoneTileMap
onready var _wall_tilemap: TileMap = $WallCollider/WallTileMap
onready var _camera: SxFXCamera = $FXCamera
onready var _vignette: SxVignette = $FX/Vignette
onready var _animation_player: AnimationPlayer = $AnimationPlayer
onready var _bomb: Bomb = $Bomb
onready var _return_title: Button = $Layer/Center/VBox/Button
onready var _message: RichTextLabel = $Layer/Center/VBox/Message

var _message_to_display := ""
var _locked := false

func _ready() -> void:
    var rect := _wall_tilemap.get_used_rect()
    var size := rect.position + rect.size
    var vp_size := get_viewport_rect().size
    _camera.limit_left = 0
    _camera.limit_top = 0
    _camera.smoothing_enabled = true
    _camera.limit_right = int(max(size.x * _wall_tilemap.cell_size.x, vp_size.x))

    var title_music: AudioStreamOGGVorbis = GameLoadCache.load_resource("MainSong")
    GameGlobalMusicPlayer.play_stream(title_music)
    GameGlobalMusicPlayer.fade_in()

    _return_title.connect("pressed", self, "_load_title_screen")
    var deaths := GameData.deaths
    var time := GameData.time

    match deaths:
        0:
            _message_to_display = NO_DEATHS_MESSAGE % [time]
        1:
            _message_to_display = ONE_DEATH_MESSAGE % [time]
        _:
            _message_to_display = DEFAULT_MESSAGE % [deaths, time]
    
    _message.bbcode_text = "[center]%s[/center]" % _message_to_display

func _process(delta: float) -> void:
    if _locked:
        var max_scale := 100
        var scale := _bomb.sprite.scale + Vector2.ONE * delta
        scale.x = min(scale.x, max_scale)
        scale.y = min(scale.y, max_scale)
        _camera.global_position = _bomb.position
        _bomb.sprite.scale = scale
        return

    _camera.shake_ratio = _bomb.speed_ratio()
    _vignette.vignette_ratio = 0.01 + _bomb.speed_ratio() / 8.0
    _camera.global_position = _bomb.position

    if _bomb.locked:
        _locked = true
        _animation_player.play("show")
        _camera.shake_ratio = 0

func _load_title_screen() -> void:
    GameGlobalMusicPlayer.fade_out()
    GameSceneTransitioner.fade_to_cached_scene(GameLoadCache, "Title")
