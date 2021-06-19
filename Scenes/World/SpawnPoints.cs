using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class SpawnPoints : Node2D
{
    public Vector2 GetFarthestSpawnPoint(Vector2 enemyPlayerPosition) {
        var childCount = GetChildCount();
        Vector2 farthestPoint = Vector2.Zero;
        foreach(Position2D point in GetChildren()) {
            var magnitude = (point.GlobalPosition - enemyPlayerPosition).Length();
            if (farthestPoint.Length() < magnitude) farthestPoint = point.GlobalPosition;
        }
        return farthestPoint;
    }
}
