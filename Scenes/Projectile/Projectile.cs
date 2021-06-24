using Godot;
using System;
using Godot.Collections;
using bit_shuter_server.Scenes.World;

public class Projectile : KinematicBody2D,ISerialized
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

    public Vector2 velocity;
    public int speed = 150;

        public override void _PhysicsProcess(float delta)
    {
        MoveAndCollide(velocity*delta*speed);
    }

}
