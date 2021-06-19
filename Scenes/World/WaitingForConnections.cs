using Godot;
using System;
using bit_shuter_server.Scenes.World;
public class WaitingForConnections : Node, IState
{
    private string _nextState = nameof(Simulating);
    private Clients _clients;
    
    private int _timerExecutionTimes = 15; // that means 16 sec to start a game when everyone is ready
    private bool _isTimerExecuted = false;

    private Timer _timer;

    private void CreateTimer() {
        _timer = new Timer();
        _timer.WaitTime = 1.0f;
        _timer.Connect("timeout", this, nameof(Timeout));
        AddChild(_timer);
        _timer.Start();
        _isTimerExecuted = true;
    }

    private void DestroyTimer() {
        _timerExecutionTimes = 15;
        _isTimerExecuted = false;
    }
    public void Timeout() {
        _timerExecutionTimes--;
        // here send time left to start
    }
    public void BeginTransition() {
        GD.Print("Begin waiting for connections");
        _clients = GetNode<Clients>("/root/Clients");
    }

    public void EndTransition(string name) {
        _clients = null;
        GetParent<StateMachine>().Transist(name);
    }

    public void Tick(float delta) {
        if(_clients.CanStartGame && !_isTimerExecuted) {
            CreateTimer();
            return;
        }
        
        if (_timerExecutionTimes <= 0) {
            EndTransition(nameof(LoadingGame));
            return;
        }
    }
}
