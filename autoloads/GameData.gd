extends SxGameData

var deaths: int = 0 setget set_deaths
var time: float = 0.0 setget set_time

func _ready() -> void:
    var logger := SxLog.get_logger("SxGameData")
    logger.set_max_log_level(SxLog.LogLevel.DEBUG)

    load_from_disk()
    deaths = load_value("deaths", 0)

func set_deaths(value: int) -> void:
    deaths = value
    store_value("deaths", value)
    persist_to_disk()

func set_time(value: float) -> void:
    time = value
    store_value("time", value)
    persist_to_disk()