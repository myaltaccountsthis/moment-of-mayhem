using UnityEngine;

public class Log : OscillatingTrap
{
    private Sprite log1;
    [SerializeField] private Sprite log2;
    const float FlipDuration = 0.2f;

    protected override void Awake()
    {
        base.Awake();
        log1 = spriteRenderer.sprite;
    }
    protected override void Update()
    {
        // advance time / movement first
        base.Update();

        if (elapsed % FlipDuration < FlipDuration / 2f)
        {
            spriteRenderer.sprite = log1;
        }
        else
        {
            spriteRenderer.sprite = log2;
        }
    }
}