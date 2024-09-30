using Godot;
using System;

public partial class Root : Node2D
{
    public override void _Ready()
    {
        GetNode<Camera2D>("Camera2D").MakeCurrent();
        base._Ready();
    }
}
