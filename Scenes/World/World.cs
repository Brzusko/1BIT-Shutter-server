using Godot;
using System;
using Godot.Collections;

public class World : Node2D
{
    private Clients _clients;
    private Players _players;
    private Projectiles _projectiles;

    private SpawnPoints _spawnPoints;

    public override void _Ready()
    {
        _clients = GetNode<Clients>("/root/Clients");
        _players = GetNode<Players>("Players");
        _projectiles = GetNode<Projectiles>("Projectiles");
        _spawnPoints = GetNode<SpawnPoints>("SpawnPoints");
    }

    public void CreatePlayer(Vector2 startingPos, string name, bool playerLook) {
        _players.CreatePlayer(startingPos, name, playerLook);
    }
    public Godot.Collections.Dictionary<string,object> SendGameWorldState()
    {
        return new  Godot.Collections.Dictionary<string,object>(){
            { "P", _players.GetSerlizedPlayers() },
            { "pr", _projectiles.GetSerlizedProjectiles() },
            { "t", OS.GetTicksMsec() }
        };;
    }
}
