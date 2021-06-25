using Godot;
using System;
using Godot.Collections;
using bit_shuter_server.Scenes.World;

public class Player : KinematicBody2D,ISerialized
{
    public Vector2 position {get;set;}
    public int rotation {get;set;}
    public Boolean look {get;set;}
    public string name {get;set;}

    public Dictionary<string,object> ToGodotDict(){
        return new Dictionary<string,object>(){
            {"p",position},
            {"r",rotation},
            {"l",look},
            {"n",name}
        };
    }

    const int speed = 100;   
    private Projectile _projectile;
    private Position2D _hand;


    public struct PlayerInput
     {
         public Vector2 Velocity;
         public Vector2 CurrentMousePosition;
     }
    public enum LifeStatus
    {
        Alive,
        Dead
    }
    private PlayerInput _Player;
    public LifeStatus Status;

    [Remote]
    public void GetPlayerInput(Vector2 velocity,Vector2 currentMousePosition)
    {
        _Player.Velocity = new Vector2();
        _Player.Velocity = velocity;
        _Player.CurrentMousePosition = currentMousePosition;
    }

    public override void _Ready()
    {
        _projectile = GetNode<Projectile>("../../Projectiles/Projectile");
        _hand = GetNode<Position2D>("Hand"); 
        _projectile.throwed = false;
        Status = LifeStatus.Alive;
    }
    private void TrowBall()
    {
        _projectile.throwed = true;
        _projectile.Throwed();
    }
    private void KillPlayer()
    {
        _projectile.QueueFree();
        this.QueueFree();
    }
    private void GetInput()
    {
        _Player.Velocity = new Vector2();
        
        if (Input.IsActionPressed("ui_right"))
            _Player.Velocity.x += 1;

        if (Input.IsActionPressed("ui_left"))
            _Player.Velocity.x -= 1;

        if (Input.IsActionPressed("ui_down"))
            _Player.Velocity.y += 1;

        if (Input.IsActionPressed("ui_up"))
            _Player.Velocity.y -= 1;

        if (Input.IsActionPressed("Left_click") && _projectile.throwed == false)
            TrowBall();
    }
    public override void _PhysicsProcess(float delta)
    {
        if(Status == LifeStatus.Alive)
        {
            _Player.CurrentMousePosition = GetViewport().GetMousePosition();
            LookAt(_Player.CurrentMousePosition);

            if(_projectile.throwed == false)
                _projectile.GlobalPosition = _hand.GlobalPosition;

            GetInput();
            MoveAndCollide(_Player.Velocity*delta*speed);
        }
        else if(Status == LifeStatus.Dead)
        {
            KillPlayer();
        }
    }
}