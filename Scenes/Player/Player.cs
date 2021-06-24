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

    public override void _PhysicsProcess(float delta)
    {
        LookAt(Player1.CurrentMousePosition);
        MoveAndCollide(Player1.Velocity*delta*speed);
    }
}
