using UnityEngine;

public class Log : OscillatingTrap
{
    const float FlipDuration = 0.2f;
    protected override void Update()
    {
        // advance time / movement first
        base.Update();

        if (elapsed % FlipDuration < FlipDuration / 2f)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }
}