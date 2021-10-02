using Godot;
using SxGD;
using System;

public class ZoneTileMap : TileMap
{
    public override void _Ready()
    {
        foreach (Vector2 pos in GetUsedCells())
        {
            var tileIdx = GetCellv(pos);
            var tileName = TileSet.TileGetName(tileIdx);

            var zoneTile = LoadCache.GetInstance().InstantiateScene<ZoneTile>();
            zoneTile.Position = MapToWorld(pos) + CellSize / 2;
            zoneTile.TileName = tileName;

            AddChild(zoneTile);
        }
    }

    public Vector2 GetStartPosition()
    {
        var startId = TileSet.FindTileByName("Start");
        var startCells = GetUsedCellsById(startId);
        if (startCells.Count == 0)
        {
            GD.PushError("Missing starting position in level.");
            throw new InvalidOperationException();
        }
        else if (startCells.Count > 1)
        {
            GD.PushError("Too many starting positions in level. Only one is required.");
            throw new InvalidOperationException();
        }
        else
        {
            var loc = (Vector2)startCells[0];
            var world = MapToWorld(loc);

            // Add half tile size
            world += CellSize / 2;

            return world;
        }
    }
}
