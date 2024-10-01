using BallSpiking.Scenes;
using BallSpiking.Scripts;
using Godot;

namespace BallSpiking;

public partial class BallAndCharacter : Node2D
{
    [Export] public Character Character;
    [Export] public Ball Ball;
    private bool _isHoldingBall;

    [Export]
    public bool IsHoldingBall
    {
        get { return _isHoldingBall; }
        set
        {
            _isHoldingBall = value;
            if (IsHoldingBall)
            {
                Ball.DisableBall();
            }
            else
            {
                Ball.EnableBall();
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsHoldingBall)
        {
            Ball.GlobalPosition = Ball.GlobalPosition.Lerp(Character.BallBackPack.GlobalPosition, 0.4f);
        }
        
        base._PhysicsProcess(delta);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("pick_up"))
        {
            IsHoldingBall = !IsHoldingBall;
        }
        
        base._Process(delta);
    }
}