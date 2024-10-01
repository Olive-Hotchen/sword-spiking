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
    
    private bool _wasPushed { get; set; }

    public override void _Process(double delta)
    {
        GetNode<Label>("Health").Text = Health.ToString();
        if (!GetNode<AnimatedSprite2D>("AnimatedSprite2D").IsPlaying() && GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation.ToString() == "knockback")
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle");
        }
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        Velocity = new Vector2(
                        Velocity.X,
                        Velocity.Y + GetLocalGravity() * (float) delta);
        MoveAndSlide();
        if (_wasPushed)
        {
            Velocity = new Vector2(Velocity.X, Velocity.Y -200);
        }

        Velocity = Velocity.Lerp(new Vector2(0, Velocity.Y), 0.3f);
        MoveAndSlide();
        base._PhysicsProcess(delta);
    }

    private float GetLocalGravity()
    {
        return Gravity;
    }
    
    public override void _Ready()
    {
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle");
        HurtBox.HitBoxEntered += (_, args) =>
        {
            if (args.HitBox.HitBoxSender is not Ball b) return;
            if (b.Speed < 200)
            {
                b.Bounce();
                return;
            }
            
            b.OnEnemyHit();
            Health -= b.Speed;
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("knockback");
            if (Health <= 0)
            {
                {
                    QueueFree();
                }
            }

            var ballImpulse = new Vector2(700, -100);
            Velocity += ballImpulse; 

            base._Ready();
        };
    }
}