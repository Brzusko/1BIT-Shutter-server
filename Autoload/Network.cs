using Godot;
using Godot.Collections;
using System;
using bit_shuter_server.Autoload.Structs;

public class Network : Node
{
	private static int _port = 7171;
	private WebSocketServer _network_peer = new WebSocketServer();

	private void CreateServer() {
		var error = _network_peer.Listen(7171, null, true);
		if (error != Error.Ok) return;
		GetTree().NetworkPeer = _network_peer;
		_network_peer.Connect("peer_connected", this, nameof(OnClientConnect));
		_network_peer.Connect("peer_disconnected", this, nameof(OnClientDisconnect));
	}

	public void OnClientConnect(int id) {
		var clients = GetNode<Clients>("/root/Clients");
		clients.RegisterClient(id);
		GD.Print($"Client connected with id ${id}");
	}

	public void OnClientDisconnect(int id) {
		var clients = GetNode<Clients>("/root/Clients");
		clients.DestroyClientOnDisconnect(id);
		GD.Print($"Client disconnected with ${id}");
	}

#region rpcs
	[Remote]
	public void ReciveClientCredentials(Dictionary<string, object> creds) {
		var clients = GetNode<Clients>("/root/Clients");
		var id = GetTree().GetRpcSenderId();
		var credentials = new Credentials {
			ClientName = (string)creds["ClientName"]
		};
		clients.SetClientCredentials(credentials, id);
		StartClientClockSyncing(id);
	}

	[Remote]
	public void ClientClockSyncFinished() {
		var clients = GetNode<Clients>("/root/Clients");
	}

#region utils
	public void StartClientClockSyncing(int id) {
		RpcId(id, "StartClockSync");
	}

#endregion

#endregion
	public override void _Ready() => CreateServer();
	public override void _Process(float delta)
	{
		if(!_network_peer.IsListening()) return;
		_network_peer.Poll();
	}
}
