using System.Linq;
using Godot;

namespace BallSpiking.Singletons;

public partial class HitStopManager : Node
{
    public bool Enabled { get; set; } = true;
    private int delay = 100;
    
    public override void _Ready()
    {
        foreach (var nodeToFreezeFrame in GetTree().GetNodesInGroup("freezer"))
        {
            nodeToFreezeFrame.Connect("FreezeFrameRequested", new Callable(this, "OnFreezeFrame"));
        }
        
        base._Ready();
    }

    private void OnFreezeFrame()
    {
        if (!Enabled)
        {
            return;
        }

        GetTree().CreateTimer(0.1f, true, false, true).Timeout += () => GetTree().Paused = false;
        GetTree().Paused = true;
        GetTree().GetNodesInGroup("camera").FirstOrDefault()?.Call("ApplyRandomShake");
    }
}