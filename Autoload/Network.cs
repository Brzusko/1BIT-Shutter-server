using Godot;
using Godot.Collections;
using System;
using bit_shuter_server.Autoload.Structs;
using bit_shuter_server.globals.enums;

public class Network : Node
{
	[Signal]
	public delegate void ClientJoinedLobby(Dictionary<string, object> client);
	[Signal]
	public delegate void ClientDisconnectedFromLobby(Dictionary<string, object> client);
	[Signal]
	public delegate void ClientJoinedToServer(Dictionary<string, object> client);
	[Signal]
	public delegate void ClientDisconnectedFromServer(Dictionary<string, object> client);

	[Signal]
	public delegate void ClientChangeReadyState();
	private static int _port = 7171;
	private WebSocketServer _network_peer = new WebSocketServer();
	private readonly int _maxClientCount = 2;

	private void CreateServer() {
		var error = _network_peer.Listen(7171, null, true);
		if (error != Error.Ok) return;
		GetTree().NetworkPeer = _network_peer;
		_network_peer.Connect("peer_connected", this, nameof(OnClientConnect));
		_network_peer.Connect("peer_disconnected", this, nameof(OnClientDisconnect));
	}

	public void OnClientConnect(int id) {
		var clients = GetNode<Clients>("/root/Clients");
		if(clients.ClientsCount >= _maxClientCount) {
			DisconnectPlayer(id);
			return;
		}
		clients.RegisterClient(id);
		GD.Print($"Client connected with id ${id}");
	}

	public void OnClientDisconnect(int id) => DisconnectPlayer(id);

#region rpcs
	[Remote]
	public void ReciveClientCredentials(Dictionary<string, object> creds) {
		var clients = GetNode<Clients>("/root/Clients");
		var id = GetTree().GetRpcSenderId();
		var credentials = new Credentials {
			ClientName = (string)creds["ClientName"]
		};
		clients.SetClientCredentials(credentials, id);
		GD.Print("Recived Credentials");
		StartClientClockSyncing(id);
	}
	
	[Remote]
	public void ClientClockSyncFinished() {
		var clients = GetNode<Clients>("/root/Clients");
		var id = GetTree().GetRpcSenderId();
		clients.ClientFinishedClockSync(id);
		RpcId(id, "ChangeUIScene", PlayerUIScenes.Lobby.ToString());		
	}

	[Remote]
	public void ClientLoadedLobby() {
		var clients = GetNode<Clients>("/root/Clients");
		EmitSignal("ClientJoinedLobby", clients.GetClientByID(GetTree().GetRpcSenderId()).AsGDDict);
	}

	[Remote]
	public void ReciveReadyState(bool state) {
		var clients = GetNode<Clients>("/root/Clients");
		clients.ChangePlayerReadyState(GetTree().GetRpcSenderId(), state);
		EmitSignal(nameof(ClientChangeReadyState));
	}

	[Remote]
	public void ClientLoadedGameScene() {
		var id = GetTree().GetRpcSenderId();
		var clients = GetNode<Clients>("/root/Clients");
		clients.ClientFinishedLoading(id);
	}

#region utils
	public void StartClientClockSyncing(int id) => RpcId(id, "StartClockSync");
	public void RequestUIChange(PlayerUIScenes uiScene, int id) => RpcId(id, "ChangeUIScene", uiScene.ToString());
	public void RequestUIChangeForAllPeers(PlayerUIScenes uiScene) => Rpc("ChangeUIScene", uiScene.ToString());
	public void RequestGameSceneChange(PlayerGameScenes gameScene, int id) => RpcId(id, "ChangeGameScene", gameScene.ToString());
	public void RequestGameSceneChangeForAllPeers(PlayerGameScenes gameScene, Dictionary<string, object> sceneState) => Rpc("ChangeGameScene", gameScene.ToString(), sceneState);
	public void SendLobbyState(System.Collections.Generic.IList<Client> clientsToSend, Dictionary<string, object> lobbyState) {
		foreach(var client in clientsToSend) {
			RpcId(client.id, "ReciveLobbyState", lobbyState);
		}
	}
	public void DisconnectPlayer(int id) {
		var clients = GetNode<Clients>("/root/Clients");
		var disconnectedClient = clients.GetClientByID(id);

		if(disconnectedClient.id != -1) {
			clients.DestroyClientOnDisconnect(id);
			if(disconnectedClient.State == Client.ClientState.LOBBY_NOT_READY || disconnectedClient.State == Client.ClientState.LOBBY_READY)
				EmitSignal(nameof(ClientDisconnectedFromLobby), disconnectedClient.AsGDDict);
		}
		GD.Print($"Client disconnected with ${id}");
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
