using Godot;

namespace bit_shuter_server.Scenes.World
{
    public interface IState
    {
        void BeginTransition();
        void EndTransition(string stateName);
        void Tick(float delta);    
    }
}