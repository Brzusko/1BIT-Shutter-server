using Godot;
using System;
using bit_shuter_server.Scenes.World;
public class Restarting : Node, IState
{
    private Clients _clients;
    public void BeginTransition() {
        _clients = GetNode<Clients>("/root/Clients");
    }

    public void EndTransition(string name) {
        GetParent<StateMachine>().Transist(name);
    }

    public void Tick(float delta) {
		if(_clients.ClientsCount == 0) {
             EndTransition(nameof(WaitingForConnections));
             return;
        }
    }
}
