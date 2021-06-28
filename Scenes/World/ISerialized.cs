using Godot;
using System;
using Godot.Collections;

namespace bit_shuter_server.Scenes.World
{
    public interface ISerialized
    {
        Vector2 EntityPosition {get;}
        float EntityRotation {get;}
        Boolean EntityLook {get;set;}

        Dictionary<string,object> ToGodotDict();
    }
}