using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Collections;

public class Players : Node2D
{
	[Export]
	private PackedScene _playerScene;
	private List<Player> _playersInGame = new List<Player>(); 

	public void CreatePlayer(Vector2 startPosition, string playerName, bool look)
	{
		var player = _playerScene.Instance() as Player;
		player.Setup(startPosition, playerName, look);
		AddChild(player);
		_playersInGame.Add(player);
	}

	public void DestroyPlayer(string playerName)
	{
		var player = GetPlayerByName(playerName);
		if (player != null) player.Free();
		_playersInGame.Remove(player);
	}

	public Player GetPlayerByName(string playerName) => _playersInGame.Find(player => player.Name == playerName);

	public Godot.Collections.Array<Godot.Collections.Dictionary<string,object>> GetSerlizedPlayers(){
		Godot.Collections.Array<Godot.Collections.Dictionary<string,object>> serializedPlayers = new Godot.Collections.Array<Godot.Collections.Dictionary<string,object>>();
		foreach (Player player in _playersInGame)
		{
			serializedPlayers.Add(player.ToGodotDict());
		}
		return serializedPlayers;
	}

}
