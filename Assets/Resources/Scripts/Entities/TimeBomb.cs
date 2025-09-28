using UnityEngine;

public class TimeBomb : CollidableEntity, IInteractable
{
    const float CountdownTime = 15f;
    private float countdown = CountdownTime;
    private bool reversed = false;
    [SerializeField] private Sprite safeSprite;
    [SerializeField] private Sprite explodedSprite;
    [SerializeField] private DamagePart[] explosionBlocks;
    private SpriteRenderer spriteRenderer;
    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Interact(Player player)
    {
        if (!reversed)
        {
            reversed = true;
            // possible animation
            foreach (DamagePart block in explosionBlocks)
            {
                block.enabled = false;
            }
            // Start countdown
            countdown = CountdownTime;
            spriteRenderer.sprite = safeSprite;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (reversed)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f)
            {
                // Explode
                foreach (DamagePart block in explosionBlocks)
                {
                    block.enabled = true;
                }
                spriteRenderer.sprite = explodedSprite;
                reversed = false;
            }
        }
    }
}
