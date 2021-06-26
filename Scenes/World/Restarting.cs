using Godot;
using System;
using bit_shuter_server.Scenes.World;
using bit_shuter_server.globals.enums;
using bit_shuter_server.Autoload.Structs;
using System.Collections.Generic;
public class Restarting : Node, IState
{
    [Export]
    private NodePath _players;
	[Export]
	private NodePath _projectiles;


    private string _nextState = nameof(WaitingForConnections);
    private Clients _clients;
    private Network _network;
    private Node2D _PlayersInGame;
    private Node2D _ProjectilesInGame;
    private IList<Client> _clientsInGame;
    public void BeginTransition() {
        GD.Print("Begin restarting");
        _clients = GetNode<Clients>("/root/Clients");
        _network = GetNode<Network>("/root/Network");
        _PlayersInGame = GetNode<Node2D>(_players);
        _ProjectilesInGame = GetNode<Node2D>(_projectiles);
        _clientsInGame = _clients.ClientsInGame;
    }

    public void EndTransition(string name) {
        _clients = null;
        _network = null;
        GetParent<StateMachine>().Transist(name);
    }

    public void Tick(float delta) {

        foreach (var client in _clientsInGame)
        {
            _network.RequestUIChange(PlayerUIScenes.Lobby,client.id);
            _clients.ClientNotReady(client.id);
        }
        
        EndTransition(_nextState);
    }
}
