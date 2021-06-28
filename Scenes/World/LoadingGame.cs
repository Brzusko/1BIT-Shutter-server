using Godot;
using System;
using bit_shuter_server.Scenes.World;
using bit_shuter_server.globals.enums;
public class LoadingGame : Node, IState
{
    private Clients _clients;
    private Network _network;
    [Export]
    private NodePath _worldPath;
    private World _world;
    public void BeginTransition() {
        _clients = GetNode<Clients>("/root/Clients");
        _network = GetNode<Network>("/root/Network");
        _world = GetNode<World>(_worldPath);
        _network.RequestGameSceneChangeForAllPeers(PlayerGameScenes.World, _world.WorldState());
    }

    public void EndTransition(string name) {
        GetParent<StateMachine>().Transist(name);
    }

    public void Tick(float delta) {

		if(_clients.ClientsCount == 0) {
             EndTransition(nameof(WaitingForConnections));
             return;
        }

        if (_clients.CanStartSyncing) {
            EndTransition(nameof(Simulating));
        }
    }

}
