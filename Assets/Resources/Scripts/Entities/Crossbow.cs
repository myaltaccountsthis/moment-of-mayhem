using UnityEngine;

public class Crossbow : FiringDevice
{
    public GameObject wind;
    public float RotationAngle = 30f;

    private float halfPhase01 = 0.75f; 
    private int swingDir = +1;

    // NEW: remember the original rotation as the center
    private Quaternion baseRotation;

    protected override void Awake()
    {
        base.Awake();

        baseRotation = wind.transform.rotation; // capture center

        var startRot = baseRotation * Quaternion.Euler(0, 0, -RotationAngle);
        wind.transform.rotation = startRot;

        direction = (startRot * Vector2.up).normalized;
    }

    protected override void Update()
    {
        base.Update();

        float halfPeriod = Mathf.Max(0.0001f, CurrentDelay);

        halfPhase01 += Time.deltaTime / halfPeriod;
        if (halfPhase01 >= 1f)
        {
            halfPhase01 -= 1f;
            swingDir *= -1;
        }

        float start = (swingDir > 0) ? -RotationAngle : +RotationAngle; 
        float end   = (swingDir > 0) ? +RotationAngle : -RotationAngle;
        float easedOffset = LeanTween.easeInOutSine(start, end, halfPhase01);

        Quaternion rot = baseRotation * Quaternion.Euler(0f, 0f, easedOffset);
        wind.transform.rotation = rot;

        direction = (rot * Vector2.up).normalized;
    }

    protected override void OnWait(float seconds) { }

    protected override Vector3 GetProjectileStartPosition()
    {
        return wind.transform.position;
    }
}