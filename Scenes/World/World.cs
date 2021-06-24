using Godot;
using System;
using Godot.Collections;

public class World : Node2D
{
    private Clients _clients;
    private Players _players;
    private Projectiles _projectiles;

    public override void _Ready()
    {
        _clients = GetNode<Clients>("/root/Clients");
        _players = GetNode<Players>("Players");
        _projectiles = GetNode<Projectiles>("Projectiles");
    }

    public Godot.Collections.Dictionary<string,object> SendGame()
    {
        return new  Godot.Collections.Dictionary<string,object>(){
            {"P",_players.GetSerlizedPlayers()},
            {"pr",_projectiles.GetSerlizedProjectiles()},
            {"t",OS.GetTicksMsec()}
        };;
    }
}
