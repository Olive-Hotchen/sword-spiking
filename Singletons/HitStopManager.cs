using Godot;

namespace BallSpiking.Singletons;

public partial class HitStopManager : Node
{
    public void HitStop(bool isSprinting)
    {
        Engine.TimeScale = 0;
        ToSignal(GetTree().CreateTimer(isSprinting ? 0.2 : 0.1f, true, false, true), "timeout");
        Engine.TimeScale = 1;
    }
}