using UnityEngine;

public class Crossbow : FiringDevice
{
    public GameObject wind;
    public float RotationAngle = 30f;
    
    private bool isWindLeft = true;
    
    protected override void Awake()
    {
        base.Awake();
        wind.transform.rotation = Quaternion.Euler(0, 0, -RotationAngle);
        fixedDirection = Quaternion.Euler(0, 0, -RotationAngle) * Vector2.up;
    }
    
    // oscillate rotation of wind and fixedDirection property from -30 to 30 using LeanTween
    protected override void OnWait(float seconds)
    {
        base.OnWait(seconds);
        float targetAngle = isWindLeft ? RotationAngle : -RotationAngle;
        LeanTween.rotate(wind, new Vector3(0, 0, targetAngle), seconds)
            .setEaseInOutSine();
        fixedDirection = Quaternion.Euler(0, 0, targetAngle) * Vector2.up;
        fixedDirection = fixedDirection.normalized;
        Debug.Log("Fixed direction: " + fixedDirection);
        isWindLeft = !isWindLeft;
    }

    protected override Vector3 GetProjectileStartPosition()
    {
        return wind.transform.position;
    }
}