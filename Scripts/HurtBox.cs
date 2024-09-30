using System;
using BallSpiking.Scenes;
using Godot;

namespace BallSpiking.Scripts;

public partial class HurtBox : Area2D
{
    public event EventHandler<HitBoxEnteredEventArgs> HitBoxEntered;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;

        base._Ready();
        return;

        void OnAreaEntered(Area2D area)
        {
            if (area is HitBox hb)
            {
                HitBoxEntered?.Invoke(this, new HitBoxEnteredEventArgs(hb));
            }
        }
    }

    public class HitBoxEnteredEventArgs
    {
        public HitBox HitBox { get; set; }

        public HitBoxEnteredEventArgs(HitBox hb)
        {
            HitBox = hb;
        }
    }
}