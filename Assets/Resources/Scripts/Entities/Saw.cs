using UnityEngine;

public class Saw : OscillatingTrap
{
    public float rotationSize;
    public GameObject sawBlade;
    private Vector3 fromEuler, toEuler;

    protected override void Awake()
    {
        base.Awake();
        fromEuler = new Vector3(0, 0, 0);
        toEuler = new Vector3(0, 0, -rotationSize);
        sawBlade.transform.rotation = Quaternion.Euler(fromEuler);
    }
    
    protected override void FixedUpdate()
    {
        // advance time / movement first
        base.FixedUpdate();

        float u = Mathf.PingPong(elapsed / oscillationTime, 1f);
        float eased = LeanTween.easeInOutSine(0f, 1f, u);
        sawBlade.transform.rotation = Quaternion.Euler(Vector3.LerpUnclamped(fromEuler, toEuler, eased));
    }
}