using BallSpiking.Scenes;
using Godot;

namespace BallSpiking.Scripts;

public partial class HitBox : Area2D
{
    [Export]
    public Character Player { get; set; }
    
    public override void _Ready()
    {
        CollisionLayer = 8;
        CollisionMask = 0;
        base._Ready();
    }
}