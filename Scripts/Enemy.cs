using Godot;

namespace BallSpiking.Scripts;

public partial class Enemy : CharacterBody2D 
{
    
    [Export] public float Gravity { get; set; } = 2000.0f;
    [Export]
    public HurtBox HurtBox
    {
        get;
        set;
    }

    [Export]
    public int Health
    {
        get; set;
    }

    public override void _Process(double delta)
    {
        GetNode<Label>("Health").Text = Health.ToString();
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        Velocity = new Vector2(
                        Velocity.X,
                        Velocity.Y + GetLocalGravity() * (float) delta);
        MoveAndSlide();
        base._PhysicsProcess(delta);
    }

    private float GetLocalGravity()
    {
        return Gravity;
    }
    
    public override void _Ready()
    {
        HurtBox.HitBoxEntered += (_, args) =>
        {
            if (args.HitBox.HitBoxSender is not Ball b) return;
            b.OnEnemyHit();
            Health -= b.Speed;
            if (Health <= 0)
            {
                {
                    QueueFree();
                }
            }

            base._Ready();
        };
    }
}