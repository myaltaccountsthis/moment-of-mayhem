using UnityEngine;

public class Turret : FiringDevice
{
    public GameObject head;
    public Player target;
    public float aimingRange = 5f;
    public float degreesPerSec = 20f;

    protected override void Update()
    {
        base.Update();

        if (head == null || target == null) return;

        if ((target.transform.position - transform.position).sqrMagnitude > aimingRange * aimingRange)
            return;

        Vector2 toTarget = (Vector2)(target.transform.position - head.transform.position);
        if (toTarget.sqrMagnitude < 1e-8f) return;
        Vector2 desiredDir = toTarget.normalized;

        float speedScale   = Mathf.Clamp01(mainDelay / CurrentDelay);
        float angularSpeed = degreesPerSec * speedScale;

        Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, desiredDir);
        head.transform.rotation = Quaternion.RotateTowards(
            head.transform.rotation, targetRot, angularSpeed * Time.deltaTime);

        direction = head.transform.up;
    }

    private Vector2 getTargetDirection()
    {
        return (target.transform.position - transform.position).normalized;
    }
}