using Godot;
using System;
using Godot.Collections;
using bit_shuter_server.Scenes.World;

public class Player : KinematicBody2D,ISerialized
{
    const int speed = 100;   

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

     public struct PlayerInput
     {
         public Vector2 Velocity;
         public Vector2 CurrentMousePosition;
     }
    private PlayerInput Player1;
    [Remote]
    public void GetPlayerInput(Vector2 velocity,Vector2 currentMousePosition)
    {
        Player1.Velocity = new Vector2();
        Player1.Velocity = velocity;
        Player1.CurrentMousePosition = currentMousePosition;
    }

    private Projectile projectile;

    public override void _Ready()
    {
        projectile = GetNode<Projectile>("../Projectile");
    }
    public void TrowBall()
    {
        projectile.LookAt(GetViewport().GetMousePosition());
        projectile.velocity = Vector2.Up;
    }
    public void GetInput()
    {
        Player1.Velocity = new Vector2();
        
        if (Input.IsActionPressed("ui_right"))
            Player1.Velocity.x += 1;

        if (Input.IsActionPressed("ui_left"))
            Player1.Velocity.x -= 1;

        if (Input.IsActionPressed("ui_down"))
            Player1.Velocity.y += 1;

        if (Input.IsActionPressed("ui_up"))
            Player1.Velocity.y -= 1;

        if (Input.IsActionPressed("Left_click"))
            TrowBall();
    }
    public override void _PhysicsProcess(float delta)
    {
        Player1.CurrentMousePosition = GetViewport().GetMousePosition();
        LookAt(Player1.CurrentMousePosition);
        GD.Print(Player1.CurrentMousePosition);
        GetInput();
        MoveAndCollide(Player1.Velocity*delta*speed);
    }
}
