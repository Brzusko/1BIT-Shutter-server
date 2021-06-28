using Godot;
using System;
using Godot.Collections;
using bit_shuter_server.Scenes.World;
using bit_shuter_server.globals.enums;
public class WaitingForConnections : Node, IState
{
	private string _nextState = nameof(Simulating);
	private Clients _clients;
	private Network _network;

	[Export]
	private NodePath _worldPath;
	private World _world;
	[Export]
	private NodePath _spawnPointsPath;
	private SpawnPoints _spawnPoints;
	
	private static int _executionCount = 3;
	private int _timerExecutionTimes = _executionCount; // that means 16 sec to start a game when everyone is ready
	private bool _isTimerExecuted = false;

	private Timer _timer;

	private void CreateTimer() {
		_timer = new Timer();
		_timer.WaitTime = 1.0f;
		_timer.Connect("timeout", this, nameof(Timeout));
		AddChild(_timer);
	}
	private void StartTimer() {
		_timer.Start();
		_isTimerExecuted = true;
	}

	private void ResetTimer(bool sendToClient = true) {
		_timerExecutionTimes = _executionCount;
		_isTimerExecuted = false;
		_timer.Stop();
		if (sendToClient) _network.SendLobbyState(_clients.ClientsInLobby, GetLobbyState());
	}

	private void DestroyTimer() {
		ResetTimer();
		_timer.Disconnect("timeout", this, nameof(Timeout));
		_timer.Free();
	}

	private void ConnectEvents() {
		_network.Connect("ClientJoinedLobby", this, nameof(OnClientJoinedLobby));
		_network.Connect("ClientDisconnectedFromLobby", this, nameof(OnClientDisconnectedFromLobby));
		_network.Connect("ClientChangeReadyState", this, nameof(OnClientChangeReadyState));
	}

	private void DicsonnectEvents() {
		_network.Disconnect("ClientJoinedLobby", this, nameof(OnClientJoinedLobby));
		_network.Disconnect("ClientDisconnectedFromLobby", this, nameof(OnClientDisconnectedFromLobby));
		_network.Disconnect("ClientChangeReadyState", this, nameof(OnClientChangeReadyState));
	}

	private Godot.Collections.Dictionary<string, object> GetLobbyState() {
		var state = new Godot.Collections.Dictionary<string, object>();
		var clients = _clients.ClientsInLobby;
		var clientsGDArray = new Godot.Collections.Array<Godot.Collections.Dictionary<string, object>>();
		foreach(var client in clients) clientsGDArray.Add(client.AsGDDict);
		state.Add("t", _timerExecutionTimes);
		state.Add("ts", _isTimerExecuted);
		state.Add("c", clientsGDArray);
		return state;
	}

	private void SpawnPlayers() {
		var clients = _clients.ClientsInLobby;
		var iterator = 0;
		Vector2 startingPos = Vector2.Zero;
		bool playerLook = false;

		foreach (var client in clients) {
			var modulo = iterator % 2;
			if (modulo == 0) {
				playerLook = false;
				startingPos = _spawnPoints.FirstPlayerSpawnPoint;
			} else {
				playerLook = true;
				startingPos = _spawnPoints.SecondPlayerSpawnPoint;
			}
			_world.CreatePlayer(startingPos, client.ClientName, playerLook);
			iterator++;
			GD.Print(startingPos);
		}
	}
	public void Timeout() {
		_timerExecutionTimes--;
		if (_network != null) _network.SendLobbyState(_clients.ClientsInLobby, GetLobbyState());
		// here send time left to start
	}

	public void OnClientJoinedLobby(Dictionary<string, object> client) => _network.SendLobbyState(_clients.ClientsInLobby, GetLobbyState());
	public void OnClientChangeReadyState() => _network.SendLobbyState(_clients.ClientsInLobby, GetLobbyState());

	public void OnClientDisconnectedFromLobby(Dictionary<string, object> client) => _network.SendLobbyState(_clients.ClientsInLobby, GetLobbyState());
	public void BeginTransition() {
		GD.Print("Begin waiting for connections");
		CreateTimer();
		_clients = GetNode<Clients>("/root/Clients");
		_network = GetNode<Network>("/root/Network");
		_world = GetNode<World>(_worldPath);
		_spawnPoints = GetNode<SpawnPoints>(_spawnPointsPath);
		ConnectEvents();
	}

	public void EndTransition(string name) {
		SpawnPlayers();
		DicsonnectEvents();
		_network.RequestUIChangeForAllPeers(PlayerUIScenes.LoadingScreen);
		_clients = null;
		_network = null;
		GetParent<StateMachine>().Transist(name);
	}

	public void Tick(float delta) {
		if(_clients.CanStartGame && !_isTimerExecuted) {
			StartTimer();
			return;
		}
		if (!_clients.CanStartGame && _isTimerExecuted) {
			ResetTimer();
			return;
		}
		if (_timerExecutionTimes <= 0) {
			EndTransition(nameof(LoadingGame));
			return;
		}
	}
}
