using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Collections;

public class Projectiles : Node2D
{
    private List<Projectile> _projectilesInGame;
    public override void _Ready()
    {
        _projectilesInGame = new List<Projectile>();  
    }
    public void CreateProjectile(Projectile projectile)
    {
        _projectilesInGame.Add(projectile);   
    }
    public void DestroyPlayer(Projectile projectile)
    {
        _projectilesInGame.Remove(projectile);
    }
    public Godot.Collections.Array<Godot.Collections.Dictionary<string,object>> GetSerlizedProjectiles(){
        Godot.Collections.Array<Godot.Collections.Dictionary<string,object>> serializedProjectiles = new Godot.Collections.Array<Godot.Collections.Dictionary<string,object>>();
        foreach (Projectile projectile in _projectilesInGame)
        {
            serializedProjectiles.Add(projectile.ToGodotDict());
        }
        return serializedProjectiles;
    }
}
