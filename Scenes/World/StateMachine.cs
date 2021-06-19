using Godot;
using System;
using System.Collections.Generic;
using bit_shuter_server.Scenes.World;

public class StateMachine : Node
{
    [Export]
    public NodePath _startingStatePath;
    private IState _currentState;
    private IState _startingState;

    private Dictionary<string, IState> _states;

    public void Transist(string nextState) {
        SetPhysicsProcess(false);
        if(!_states.ContainsKey(nextState)) throw new MissingFieldException($"Missing state {nextState}");
        _currentState = _states[nextState];
        _currentState.BeginTransition();
        SetPhysicsProcess(true);
    }
    public override void _Ready()
    {
        _states = new Dictionary<string, IState> {
            { nameof(WaitingForConnections), GetNode<IState>(nameof(WaitingForConnections)) },
            { nameof(Simulating), GetNode<IState>(nameof(Simulating)) },
            { nameof(Restarting), GetNode<IState>(nameof(Restarting)) },
            { nameof(LoadingGame), GetNode<IState>(nameof(LoadingGame) )}
        };
        Transist(nameof(WaitingForConnections));
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_currentState == null) return;
        _currentState.Tick(delta);
    }
}
