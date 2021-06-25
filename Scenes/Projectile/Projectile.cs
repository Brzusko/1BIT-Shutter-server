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
    public Boolean throwed;

    public Dictionary<string,object> ToGodotDict(){
        return new Dictionary<string,object>(){
            {"p",position},
            {"r",rotation},
            {"l",look},
            {"n",name}
        };
    }

    public Vector2 velocity;
    private int _speed = 500;
    private int _actualSpeed = 0;
    const int weight = 25;
    private Player player;

    public override void _Ready()
    {
        player = GetNode<Player>("../../Players/Player");
    }
    public override void _PhysicsProcess(float delta)
    {

        var collision = MoveAndCollide(velocity*delta*_actualSpeed);

        if (collision != null)
        {
            velocity = velocity.Bounce(collision.Normal);

            if(_actualSpeed > 0)
                _actualSpeed-=weight;
            else    
                _actualSpeed=0;
        }
        GD.Print(_actualSpeed);
    }

    public void Throwed()
    {
        _actualSpeed = _speed;
        velocity = (GetViewport().GetMousePosition() - GlobalPosition).Normalized();
    }

    private void _on_Area2D_body_entered(Node body)
    {
        if(body == player)
        {
            _actualSpeed = 0;
            throwed = false;
        }
    }

}
