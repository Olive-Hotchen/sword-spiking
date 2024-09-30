using System;
using Godot;

namespace BallSpiking.Scripts;

public partial class Camera2d : Camera2D
{
    [Export] public float NoiseShakeSpeed { get; set; } = 30.0f;
    [Export] public float NoiseSwaySpeed { get; set; } = 1.0f;
    [Export] public float NoiseShakeStrength { get; set; } = 60.0f;
    [Export] public float NoiseSwayStrength { get; set; } = 10.0f;
    [Export] public float RandomShakeStrength { get; set; } = 30.0f;
    [Export] public float ShakeDecayRate { get; set; } = 3f;
    [Export] public FastNoiseLite Noise { get; set; }
    public enum ShakeType
    {
        Random,
        Noise,
        Sway
    }

    private float _shakeStrength = 0;
    private ShakeType _shakeType = ShakeType.Random;
    private RandomNumberGenerator rand = new();
    private float noiseI = 0;
    
    public override void _Ready()
    {
        rand.Randomize();
        Noise.Seed = (int)rand.Randi();
        Noise.Frequency = 2;
        base._Ready();
    }

    public void ApplyRandomShake()
    {
        _shakeStrength = RandomShakeStrength;
        _shakeType = ShakeType.Noise;
    }

    public void ApplyNoiseSway()
    {
        _shakeType = ShakeType.Sway;
    }

    public override void _Process(double delta)
    {
        _shakeStrength = Mathf.Lerp(_shakeStrength, 0, ShakeDecayRate * (float) delta);

        var shakeOffset = _shakeType switch
        {
            ShakeType.Random => GetRandomOffset(),
            ShakeType.Noise => GetNoiseOffset((float)delta, NoiseShakeSpeed, _shakeStrength),
            ShakeType.Sway => GetNoiseOffset((float)delta, NoiseSwaySpeed, NoiseSwayStrength),
            _ => new Vector2()
        };

        Offset = shakeOffset;
        base._Process(delta);
    }

    private Vector2 GetNoiseOffset(float delta, float speed, float strength)
    {
        noiseI += delta * speed;
        return new Vector2(
            Noise.GetNoise2D(1, noiseI) * strength,
            Noise.GetNoise2D(100, noiseI) * strength
        );
    }

    private Vector2 GetRandomOffset()
    {
        return new Vector2(
            rand.RandfRange(-_shakeStrength, _shakeStrength),
            rand.RandfRange(-_shakeStrength, _shakeStrength)
        );
    }
}