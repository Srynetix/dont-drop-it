extends TileMap
class_name ZoneTileMap

func _ready() -> void:
    for item in get_used_cells():
        var pos: Vector2 = item
        var tile_idx := get_cellv(pos)
        var tile_name := tile_set.tile_get_name(tile_idx)
        
        var zone_tile: ZoneTile = GameLoadCache.instantiate_scene("ZoneTile")
        zone_tile.position = map_to_world(pos) + cell_size / 2
        zone_tile.tile_name = tile_name
        add_child(zone_tile)

func get_start_position() -> Vector2:
    var start_id := tile_set.find_tile_by_name("Start")
    var start_cells := get_used_cells_by_id(start_id)
    var cells_count := len(start_cells)
    if cells_count == 0:
        assert(false, "Missing starting position in level.")
        return Vector2()
    elif cells_count > 1:
        assert(false, "Too many starting positions in level. Only one is required.")
        return Vector2()
    else:
        var loc: Vector2 = start_cells[0]
        var world := map_to_world(loc)

        world += cell_size / 2
        return world