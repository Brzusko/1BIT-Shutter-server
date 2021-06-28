using Godot;
using System;
using bit_shuter_server.Scenes.World;
using bit_shuter_server.globals.enums;
public class Simulating : Node, IState
{
    private Clients _clients;
    private Network _network;

    [Export]
    private NodePath _worldPath;
    private World _world;
    public void BeginTransition() {
        GD.Print("Starting game");
        _clients = GetNode<Clients>("/root/Clients");
        _network = GetNode<Network>("/root/Network");
        _world = GetNode<World>(_worldPath);

        foreach(var client in _clients.LoadedClients) 
            _clients.ClientReadyToSync(client.id);

        _network.RequestUIChangeForAllPeers(PlayerUIScenes.Game);
        
    }

    public void EndTransition(string name) {

    }

    public void Tick(float delta) {
        _world.ProcessGame(delta);
    }
}
