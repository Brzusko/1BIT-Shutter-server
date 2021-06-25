using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Collections;

public class Players : Node2D
{
    private List<Player> _playersInGame;
    public override void _Ready()
    {
        _playersInGame = new List<Player>();  
    }

    public void CreatePlayer(Player player)
    {
        _playersInGame.Add(player);   
    }

    public void DestroyPlayer(Player player)
    {
        _playersInGame.Remove(player);
    }

    public Godot.Collections.Array<Godot.Collections.Dictionary<string,object>> GetSerlizedPlayers(){
        Godot.Collections.Array<Godot.Collections.Dictionary<string,object>> serializedPlayers = new Godot.Collections.Array<Godot.Collections.Dictionary<string,object>>();
        foreach (Player player in _playersInGame)
        {
            serializedPlayers.Add(player.ToGodotDict());
        }
        return serializedPlayers;
    }

}
