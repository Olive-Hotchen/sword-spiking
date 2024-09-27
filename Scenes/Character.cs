using Godot;

namespace BallSpiking.Scenes;

public partial class Character : CharacterBody2D
{
    [ExportGroup("Movement")]
    [Export]
    public float JumpVelocity { get; set; } = -1000.0f;
    [Export] public float InputBufferPatience { get; set; } = 0.1f;
    [Export] public float CoyoteTime { get; set; } = 0.08f;
    [Export] public float FastFallGravity { get; set; } = 10000.0f;
    [Export] public float Gravity { get; set; } = 2000.0f;
    [Export] public float FallGravity { get; set; } = 3000.0f;
    [Export] public float Speed { get; set; } = 500.0f;
    [Export] public float Acceleration { get; set; } = 2000.0f;
    [Export] public float Friction { get; set; } = 1800.0f;
    
    private Timer _inputBuffer;
    private Timer _coyoteTimer;
    private bool _coyoteJump = true;

    public override void _Ready()
    {
        _inputBuffer = new Timer();
        _inputBuffer.WaitTime = InputBufferPatience;
        _inputBuffer.OneShot = true;
        AddChild(_inputBuffer);

        _coyoteTimer = new Timer();
        _coyoteTimer.WaitTime = CoyoteTime;
        _coyoteTimer.OneShot = true;
        AddChild(_coyoteTimer);
        _coyoteTimer.Timeout += CoyoteTimerOnTimeout;
        base._Ready();
    }

    public override void _Process(double delta)
    {
        var horizontalInput = Input.GetAxis("move_left", "move_right");
        var jumpAttempted = Input.IsActionJustPressed("jump");

        if (jumpAttempted || _inputBuffer.TimeLeft > 0)
        {
            if (_coyoteJump)
            {
                Velocity = new Vector2(
                    Velocity.X,
                    JumpVelocity
                );
                
                _coyoteJump = false;
            } else if (jumpAttempted)
            {
                _inputBuffer.Start();
            }
        }

        if (Input.IsActionJustReleased("jump") && Velocity.Y < 0)
        {
            Velocity = new Vector2(
                Velocity.X,
                JumpVelocity / 4
            );
        }

        if (IsOnFloor())
        {
            _coyoteJump = true;
            _coyoteTimer.Stop();
        }
        else
        {
            if (_coyoteJump)
            {
                if (_coyoteTimer.IsStopped())
                {
                    _coyoteTimer.Start();
                }
            }

            Velocity = new Vector2(
                Velocity.X,
                Velocity.Y + GetLocalGravity() * (float) delta
            );
        }
        
        var floorDamping = IsOnFloor() ? 1.0f : 0.5f;
        var dashMultiplier = Input.IsActionPressed("dash") ? 2.0f : 1.0f;

        if (horizontalInput != 0)
        {
           Velocity = 
                new Vector2(
                (float)Mathf.MoveToward(Velocity.X, horizontalInput * Speed * dashMultiplier, Acceleration * delta),
                Velocity.Y);
        }
        else
        {
            Velocity =
                new Vector2(
                (float)Mathf.MoveToward(Velocity.X, 0,(Friction* delta)*floorDamping),
                Velocity.Y);
        }

        MoveAndSlide();
            
        base._PhysicsProcess(delta);
    }

    private float GetLocalGravity()
    {
        if (Input.IsActionPressed("fast_fall"))
        {
            return FastFallGravity;
        }

        return Velocity.Y < 0 ? Gravity : FallGravity;
    }

    private void CoyoteTimerOnTimeout()
    {
        _coyoteJump = false;
    }
}