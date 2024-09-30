using BallSpiking.Scenes;
using Godot;

namespace BallSpiking.Scripts;

public partial class HitBox : Area2D
{
    [Export]
    public Node2D HitBoxSender { get; set; }
   
    [Export]
    public Vector2 Impulse { get; set; }
}