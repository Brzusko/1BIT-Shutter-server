using Godot;
using System;
using Godot.Collections;
using bit_shuter_server.Scenes.World;

public class Player : KinematicBody2D, ISerialized
{
    public Vector2 EntityPosition {get => GlobalPosition; }
    public float EntityRotation {get => Rotation; }
    public Boolean EntityLook {get;set;}

    public Dictionary<string,object> ToGodotDict(){
        return new Dictionary<string,object>(){
            {"p", EntityPosition},
            {"r", EntityRotation},
            {"l", EntityLook},
            {"n", Name }
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


    public override void _Ready()
    {
        _hand = GetNode<Position2D>("Hand"); 
        Status = LifeStatus.Alive;
    }

    public void Setup(Vector2 _position, string name, bool look) {
        Status = LifeStatus.Alive;
        GlobalPosition = _position;
        this.Name = name;
        this.EntityLook = look;
    }
    private void TrowBall()
    {
        _projectile.throwed = true;
        _projectile.Throwed();
    }

    public void ProcessMovement(float delta) {
        MoveAndCollide(_Player.Velocity * delta * speed);
    }
    [Remote]
    public void GetPlayerInput(Vector2 velocity,Vector2 currentMousePosition, Boolean click_Left)
    {
        _Player.Velocity = new Vector2();
        _Player.Velocity = velocity;
        _Player.CurrentMousePosition = currentMousePosition;
        
        if(click_Left == true)
            TrowBall();
    }
}
