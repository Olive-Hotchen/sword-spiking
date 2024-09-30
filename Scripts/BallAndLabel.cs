using Godot;

namespace BallSpiking.Scripts;

public partial class BallAndLabel : Node2D
{
    [Export]
    public Label Label { get; set; }
    
    [Export]
    public Ball Ball { get; set; }

    public override void _Process(double delta)
    {
        Label.GlobalPosition = Label.GlobalPosition.Lerp(new Vector2(
            Ball.GlobalPosition.X + 50,
            Ball.GlobalPosition.Y - 50), 0.99f);
        Label.Text = Ball.Speed.ToString();
        base._Process(delta);
    }
}