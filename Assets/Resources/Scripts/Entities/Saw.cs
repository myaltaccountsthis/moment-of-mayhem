using UnityEngine;

public class Saw : OscillatingTrap
{
    public float rotationSize;
    public GameObject sawBlade;
    
    protected override void Awake()
    {
        base.Awake();
        LeanTween.rotate(sawBlade, rotationSize * (startRight ? Vector3.forward : Vector3.back), oscillationTime)
            .setEaseInOutSine()
            .setLoopPingPong();
    }
}