using System;
using BallSpiking.Scenes;
using Godot;

namespace BallSpiking.Scripts;

public partial class HurtBox : Area2D
{
    public event EventHandler<HitBoxEnteredEventArgs> HitBoxEntered;

    public override void _Ready()
    {
        CollisionLayer = 0;
        CollisionMask = 8;
        AreaEntered += OnAreaEntered;

        base._Ready();
        return;

        void OnAreaEntered(Area2D area)
        {
            if (area is HitBox hb)
            {
                HitBoxEntered?.Invoke(this, new HitBoxEnteredEventArgs(hb, hb.Player));
            }
        }
    }

    public class HitBoxEnteredEventArgs
    {
        public HitBox HitBox { get; set; }
        public Character Player { get; set; }

        public HitBoxEnteredEventArgs(HitBox hb, Character c)
        {
            HitBox = hb;
            Player = c;
        }
    }
}