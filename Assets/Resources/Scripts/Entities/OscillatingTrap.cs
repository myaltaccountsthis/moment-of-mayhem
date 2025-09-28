using UnityEngine;

public class OscillatingTrap : DamagePart
{
    // Oscillates between two positions
    public Transform leftPos;
    public Transform rightPos;
    public bool startRight = false;
    public float oscillationTime;
    
    protected override void Awake()
    {
        base.Awake();
        transform.position = startRight ? rightPos.position : leftPos.position;
        LeanTween
            .move(gameObject, startRight ? leftPos : rightPos, oscillationTime)
            .setEaseInOutSine()
            .setLoopPingPong();
    }
}