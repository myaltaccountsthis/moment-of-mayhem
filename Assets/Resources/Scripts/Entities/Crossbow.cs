using UnityEngine;

public class Crossbow : FiringDevice
{
    public GameObject wind;
    public float RotationAngle = 30f;

    private bool isWindLeft = true;

    private float halfPhase01 = 0f;
    private int swingDir = +1;
    
    protected override void Awake()
    {
        base.Awake();
        wind.transform.rotation = Quaternion.Euler(0, 0, -RotationAngle);
        fixedDirection = Quaternion.Euler(0, 0, -RotationAngle) * Vector2.up;
    }

    protected override void Update()
    {
        base.Update();

        float halfPeriod = Mathf.Max(0.0001f, CurrentDelay * 0.5f);

        halfPhase01 += Time.deltaTime / halfPeriod;
        if (halfPhase01 >= 1f)
        {
            halfPhase01 -= 1f;
            swingDir *= -1; 
        }

        float start = (swingDir > 0) ? -RotationAngle : +RotationAngle;
        float end   = (swingDir > 0) ? +RotationAngle : -RotationAngle;
        float easedAngle = LeanTween.easeInOutSine(start, end, halfPhase01);

        wind.transform.rotation = Quaternion.Euler(0f, 0f, easedAngle);
        fixedDirection = (Quaternion.Euler(0f, 0f, easedAngle) * Vector2.up).normalized;
    }

    protected override void OnWait(float seconds) { }

    protected override Vector3 GetProjectileStartPosition()
    {
        return wind.transform.position;
    }
}