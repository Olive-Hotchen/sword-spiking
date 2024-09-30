using Godot;

namespace BallSpiking.Scripts;

public partial class ParticleQueue : Node2D
{
    [Export]
    public PackedScene Particles { get; set; }

    [Export] public int QueueCount { get; set; } = 8;
    private int _index;

    public override void _Ready()
    {
        for (var i = 0; i < QueueCount; i++)
        {
            AddChild(Particles.Instantiate<GpuParticles2D>());    
        }
        
        base._Ready();
    }

    public GpuParticles2D GetNextParticle()
    {
        return GetChild<GpuParticles2D>(_index);
    }

    public void Trigger()
    {
        GetNextParticle().Restart();
        _index = (_index + 1) % QueueCount;
    }
}