extends SxLoadCache

func load_resources() -> void:
    # Scenes
    store_scene("Bomb", "res://scenes/entities/Bomb.tscn")
    store_scene("ZoneTile", "res://scenes/entities/ZoneTile.tscn")

    # Screens
    store_scene("Title", "res://scenes/screens/Title.tscn")
    store_scene("Credits", "res://scenes/screens/Credits.tscn")

    # Sounds
    store_resource("AmbientLoop", "res://assets/sounds/AmbientBomb.ogg")
    store_resource("BounceFX", "res://assets/sounds/Bounce.wav")
    store_resource("DisintegrateFX", "res://assets/sounds/Disintegrate.wav")
    store_resource("DizzyFX", "res://assets/sounds/Dizzy.wav")
    store_resource("ExplosionFX", "res://assets/sounds/Explosion.wav")
    store_resource("NormalFX", "res://assets/sounds/Normal.wav")
    store_resource("SecurityFX", "res://assets/sounds/Security.wav")
    store_resource("SuccessFX", "res://assets/sounds/Success.wav")
    store_resource("LockedFX", "res://assets/sounds/Locked.wav")

    # Music
    store_resource("MainSong", "res://assets/sounds/MainSong.ogg")
    store_resource("Track1", "res://assets/sounds/Track1.ogg")
    store_resource("Track2", "res://assets/sounds/Track2.ogg")

    # Textures
    store_resource("WhitePixel", "res://assets/textures/WhitePixel.png")

    # Loading levels
    for i in range(1, 10 + 1):
        store_scene("Level%d" % i, "res://scenes/levels/Level%d.tscn" % i);