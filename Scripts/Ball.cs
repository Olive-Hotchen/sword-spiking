using System;
using System.Threading.Tasks;
using BallSpiking.Scenes;
using Godot;

namespace BallSpiking.Scripts;

public partial class Ball : RigidBody2D
{
    [Export]
    public HurtBox HurtBox { get; set; }

    public override void _Ready()
    {
        HurtBox.HitBoxEntered += HurtBoxOnHitBoxEntered;
        base._Ready();
    }

    private void HurtBoxOnHitBoxEntered(object sender, HurtBox.HitBoxEnteredEventArgs e)
    {
        PlayerTriggerOnAreaEntered(e.HitBox, e.Player);
    }

    private void PlayerTriggerOnAreaEntered(Area2D area, Character c)
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
            
        if(area.IsInGroup("spike_right"))
        {
            impulseValue = Mathf.IsEqualApprox(vel.X, 0) ? _baseRightImpulse : _rightImpulse;
            if (c.IsOnFloor())
            {
                impulseValue += _baseUpImpulse;
            }
        }
        
        if (area.IsInGroup("spike_left"))
        {
            impulseValue = Mathf.IsEqualApprox(vel.X, 0) ? _baseLeftImpulse : _leftImpulse;
            if (c.IsOnFloor())
            {
                impulseValue += _baseUpImpulse;
            }
        }
            
        if (area.IsInGroup("spike_up"))
        {
            impulseValue = !c.IsOnFloor() ? _airUpImpulse : _baseUpImpulse;
        }
        
        if (area.IsInGroup("spike_down"))
        {
            impulseValue = !c.IsOnFloor() ? _airDownImpulse : _baseDownImpulse;
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

       // GetNode<Singletons.HitStopManager>("/root/HitStopManager").HitStop(c.Sprinting);
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
}
