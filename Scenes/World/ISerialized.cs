using Godot;
using System;
using Godot.Collections;

namespace bit_shuter_server.Scenes.World
{
    public interface ISerialized
    {
        Vector2 position {get;set;}
        int rotation {get;set;}
        Boolean look {get;set;}
        string name {get;set;}

        Dictionary<string,object> ToGodotDict();
    }
}