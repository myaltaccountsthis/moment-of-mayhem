using UnityEngine;

public class TimeBullet : ProjectileEntity
{
    private Sprite defaultSprite;
    private Sprite pulseSprite;
    private float timer = 0f;
    const float AnimCooldown = .5f;
    const float RotationSpeed = 270f;
    protected override void Awake()
    {
        base.Awake();
        defaultSprite = spriteRenderer.sprite;
        pulseSprite = Resources.Load<Sprite>("Textures/TimeBullet2");
        Debug.Assert(pulseSprite != null, "TimeBullet pulse sprite not found in Resources/Textures/TimeBullet2");
    }

    protected override void Update()
    {
        base.Update();
        timer += Time.deltaTime;
        rigidbody.MoveRotation(rigidbody.rotation + RotationSpeed * Time.deltaTime);
        if (timer >= AnimCooldown)
        {
            timer = 0f;
            if (spriteRenderer.sprite == defaultSprite)
            {
                spriteRenderer.sprite = pulseSprite;
            }
            else
            {
                spriteRenderer.sprite = defaultSprite;
            }
        }
    }
}
