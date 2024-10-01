using System;
using BallSpiking.Scripts;
using BallSpiking.Singletons;
using Godot;

namespace BallSpiking.Scenes;

public partial class Character : CharacterBody2D
{
    [ExportGroup("Movement")]
    [Export]
    public float JumpVelocity { get; set; } = -900.0f;
    [Export] public float InputBufferPatience { get; set; } = 0.1f;
    [Export] public float CoyoteTime { get; set; } = 0.08f;
    [Export] public float FastFallGravity { get; set; } = 10000.0f;
    [Export] public float Gravity { get; set; } = 2000.0f;
    [Export] public float FallGravity { get; set; } = 4000.0f;
    [Export] public float Speed { get; set; } = 500.0f;
    [Export] public float Acceleration { get; set; } = 2000.0f;
    [Export] public float Friction { get; set; } = 1800.0f;
    [Export] public AnimationTree AnimationTree { get; set; }
    [Export] public AnimationPlayer AnimationPlayer { get; set; }

    private AnimationNodeStateMachinePlayback StateMachine { get; set; }
    
    private Timer _inputBuffer;
    private Timer _coyoteTimer;
    private bool _coyoteJump = true;
    private bool _jumpAttempted;
    private bool _extraJump;
    private bool _touchingBall;
    private Ball _ball;

    public override void _Ready()
    {
        _inputBuffer = new Timer();
        _inputBuffer.WaitTime = InputBufferPatience;
        _inputBuffer.OneShot = true;
        AddChild(_inputBuffer);

        StateMachine = (AnimationNodeStateMachinePlayback)AnimationTree.Get("parameters/playback");
        
        _coyoteTimer = new Timer();
        _coyoteTimer.WaitTime = CoyoteTime;
        _coyoteTimer.OneShot = true;
        AddChild(_coyoteTimer);
        _coyoteTimer.Timeout += CoyoteTimerOnTimeout;
        GetNode<Area2D>("BallHandler").BodyEntered += OnAreaEntered;
        GetNode<Area2D>("BallHandler").BodyExited += OnAreaExited;

        base._Ready();
        return;

        void OnAreaEntered(Node2D node)
        {
            if (node is not Ball b) return;
            _ball = b;
            _touchingBall = true;
        }

        void OnAreaExited(Node2D node)
        {
            if (node is not Ball) return;
            _ball = null;
            _touchingBall = false;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var horizontalInput = Input.GetAxis("move_left", "move_right");
        _jumpAttempted = Input.IsActionJustPressed("jump");

        if (_touchingBall && !IsOnFloor() && _jumpAttempted)
        {
            Velocity = new Vector2(
                Velocity.X,
                JumpVelocity
            );
            
            _ball.PlayerDoubleJump();
        }
        
        if (_jumpAttempted || _inputBuffer.TimeLeft > 0)
        {
            if (_coyoteJump)
            {
                Velocity = new Vector2(
                    Velocity.X,
                    JumpVelocity
                );
                
                _coyoteJump = false;
            } else if (_jumpAttempted)
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
        Sprinting = Input.IsActionPressed("dash");

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

        CachedVelocity = Velocity;

        GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = Velocity.X switch
        {
            > 0 => false,
            < 0 => true,
            _ => GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH
        };
        
        MoveAndSlide();
        
        base._PhysicsProcess(delta);
    }

    public override void _Process(double delta)
    {
        HandleInput();
        
        
        if (GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH)
        {
            BallBackPack.Position = GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH switch
                    {
                        false => new Vector2(-25, -93),
                        true => new Vector2(25, -93),
                    };
        } 
        
        base._Process(delta);
    }

    private void HandleInput()
    {
        
        if (Input.IsActionJustPressed("spike_right"))
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = false;
            StartRightKick();
        } else if (Input.IsActionJustPressed("spike_left"))
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = true;
            StartLeftKick();
        } else if (Input.IsActionJustPressed("spike_up"))
        {
           StartHighKick();
        }else if(_jumpAttempted || _extraJump)
        {
            StartJump();
        } else if (Velocity.Y > 0)
        { 
            StartFall();
        }
        else if (Sprinting && Velocity.X != 0 && IsOnFloor())
        {
            StartDash();
        } else if (Velocity.X != 0 && IsOnFloor())
        {
            StartRun();
        }
        else if (_jumpAttempted && IsOnFloor())
        {
          StartLand();
        } 
        else if(Velocity.IsEqualApprox(Vector2.Zero))
        { 
            StartIdle();
        }
    }

    private void StartLand()
    {
        StateMachine.Travel("land");
    }

    private void StartFall()
    {
        StateMachine.Travel("fall");
    }

    private void StartJump()
    {
        StateMachine.Travel("jump");
    }

    private void StartDash()
    {
        StateMachine.Travel("dash");
    }

    private void StartIdle()
    {
        StateMachine.Travel("idle");
    }

    private void StartLeftKick()
    {
        StateMachine.Travel("h_kick_left");
    }
    private void StartRightKick()
    {
        StateMachine.Travel("h_kick_right");
    }

    private void StartRun()
    {
        StateMachine.Travel("run");
    }

    private void StartHighKick()
    {
        StateMachine.Travel("v_kick");
    }
    
    public Vector2 CachedVelocity { get; set; }
    
    public bool Sprinting { get; set; }
    
    [Export]
    public Marker2D BallBackPack { get; set; }

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