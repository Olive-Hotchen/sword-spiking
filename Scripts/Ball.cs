using System;
using BallSpiking.Scenes;
using Godot;

namespace BallSpiking.Scripts;

public partial class Ball : RigidBody2D
{
    [Export]
    public HurtBox HurtBox { get; set; }
    
    [Export]
    public HitBox HitBox { get; set; }
    
    [Export]
    public ParticleQueue ParticleQueue { get; set; }

    public int Speed => (int) Math.Round(LinearVelocity.Length() / 10.0) * 10;

    [Signal]
    public delegate void FreezeFrameRequestedEventHandler(); 
    
    public override void _Ready()
    {
        HurtBox.HitBoxEntered += HurtBoxOnHitBoxEntered;
        base._Ready();
    }

    private void HurtBoxOnHitBoxEntered(object sender, HurtBox.HitBoxEnteredEventArgs e)
    {
        if (e.HitBox.HitBoxSender is Character c)
        {
            PlayerTriggerOnAreaEntered(e.HitBox, c);
        }
    }

    private void PlayerTriggerOnAreaEntered(HitBox area, Character c)
    {
        LinearVelocity = Vector2.Zero;
        var vel = c.CachedVelocity; 
        var impulseValue= Vector2.Zero;
            
        if (c.IsOnFloor()) 
        { 
            vel = new Vector2(
                vel.X, 
                1
            );
        }
        
        if (!Mathf.IsEqualApprox(vel.X, 0))
        {
            if (vel.X < 0)
            {
                impulseValue *= -vel;
            }
            else
            {
                impulseValue *= vel;
            }
        }
        
        impulseValue += area.Impulse;

        ParticleQueue.Trigger();
        EmitSignal(SignalName.FreezeFrameRequested);
        ApplyCentralImpulse(impulseValue);
    }

    private Vector2 _rightImpulse = new(1, 0);
    private Vector2 _leftImpulse = new(-1, 0);
    private Vector2 _floorUpImpulse = new(0, -700);
    private Vector2 _floorDownImpulse = new(0, 700);
    private Vector2 _airUpImpulse = new(0, -1);
    private Vector2 _airDownImpulse = new(0, 1);

    private Vector2 _baseRightImpulse = new(400, 0);
    private Vector2 _baseLeftImpulse = new(-400, 0);
    private Vector2 _baseUpImpulse = new(0, -700);
    private Vector2 _baseDownImpulse = new(0, 700);

    public void OnEnemyHit()
    {
        var cachedBounce = PhysicsMaterialOverride.Bounce;
        PhysicsMaterialOverride.Bounce = 0.4f;
        ApplyCentralImpulse(new Vector2(0, -200));
        GetTree().CreateTimer(0.4f).Timeout += () => PhysicsMaterialOverride.Bounce = cachedBounce;
        ParticleQueue.Trigger();
    }

    public void Bounce()
    { 
        ApplyCentralImpulse(new Vector2(0, -300));
    }

    public void DisableBall()
    {
        SetPhysicsProcess(false);
        HitBox.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
        HurtBox.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
    }
    public void EnableBall()
    {
        SetPhysicsProcess(true);
        HitBox.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
        HurtBox.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
        LinearVelocity = Vector2.Zero;
    }
    
    public void PlayerDoubleJump()
    {
        EmitSignal(SignalName.FreezeFrameRequested); 
        LinearVelocity = Vector2.Zero;
        ApplyCentralImpulse(new Vector2(0, 100));
    }
}
