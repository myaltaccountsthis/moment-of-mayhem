using UnityEngine;

public class Saw : OscillatingTrap
{
    public float rotationSize;
    protected override void Awake()
    {
        base.Awake();
        LeanTween.rotate(gameObject, rotationSize * (startRight ? Vector3.back : Vector3.forward), oscillationTime)
            .setEaseInOutSine()
            .setLoopPingPong();
    }
}