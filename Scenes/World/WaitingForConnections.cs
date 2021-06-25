using Godot;
using System;
using Godot.Collections;
using bit_shuter_server.Scenes.World;
using bit_shuter_server.Autoload.Structs;
public class WaitingForConnections : Node, IState
{
    private string _nextState = nameof(Simulating);
    private Clients _clients;
    private Network _network;
    
    private int _timerExecutionTimes = 300; // that means 16 sec to start a game when everyone is ready
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

    private void ResetTimer() {
        _timerExecutionTimes = 15;
        _isTimerExecuted = false;
        _timer.Stop();
        _network.SendLobbyState(_clients.ClientsInLobby, GetLobbyState());
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
    public void Timeout() {
        _timerExecutionTimes--;
        _network.SendLobbyState(_clients.ClientsInLobby, GetLobbyState());
        // here send time left to start
    }

    public void OnClientJoinedLobby(Dictionary<string, object> client) => _network.SendLobbyState(_clients.ClientsInLobby, GetLobbyState());
    public void OnClientChangeReadyState() => _network.SendLobbyState(_clients.ClientsInLobby, GetLobbyState());

    public void OnClientDisconnectedFromLobby(Dictionary<string, object> client) => _network.SendLobbyState(_clients.ClientsInLobby, GetLobbyState());
    public void BeginTransition() {
        GD.Print("Begin waiting for connections");
        _clients = GetNode<Clients>("/root/Clients");
        _network = GetNode<Network>("/root/Network");
        ConnectEvents();
    }

    public void EndTransition(string name) {
        DicsonnectEvents();
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
            //EndTransition(nameof(LoadingGame));
            EndTransition(nameof(Restarting));
            return;
        }
    }

    public override void _Ready()
    {
        CreateTimer();
    }
}
