using UnityEngine;

public class TimeBomb : CollidableEntity, IInteractable
{
    const float CountdownTime = 15f, CooldownTime = 1f;
    private float countdown = CountdownTime;
    private float cooldown = 0f;
    private bool reversed = false;
    private Sprite safeSprite;
    private Sprite safeSprite1;
    private Sprite explodingSprite;
    private Sprite explodedSprite;
    [SerializeField] private DamagePart[] explosionBlocks;
    private float timer = 0f;
    const float AnimCooldown = .5f;

    protected override void Awake()
    {
        base.Awake();
        explodedSprite = spriteRenderer.sprite;
        safeSprite = Resources.Load<Sprite>("Textures/Entities/Time-Bomb-Priming");
        safeSprite1 = Resources.Load<Sprite>("Textures/Entities/Time-Bomb-Priming-2");
        explodingSprite = Resources.Load<Sprite>("Textures/Entities/Time-Bomb-Exploding");
        Debug.Assert(safeSprite != null, "TimeBomb safe sprite not found in Resources/Textures/Entities/Time-Bomb-Priming");
        Debug.Assert(safeSprite1 != null, "TimeBomb safe sprite 1 not found in Resources/Textures/Entities/Time-Bomb-Priming-2");
        Debug.Assert(explodingSprite != null, "TimeBomb exploding sprite not found in Resources/Textures/Entities/Time-Bomb-Exploding");
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
            timer += Time.deltaTime;
            if (timer >= AnimCooldown)
            {
                timer = 0f;
                if (spriteRenderer.sprite == safeSprite)
                {
                    spriteRenderer.sprite = safeSprite1;
                }
                else
                {
                    spriteRenderer.sprite = safeSprite;
                }
            }
            countdown -= Time.deltaTime;
            if (countdown <= 0f)
            {
                // Explode
                foreach (DamagePart block in explosionBlocks)
                {
                    block.enabled = true;
                }
                spriteRenderer.sprite = explodingSprite;
                reversed = false;

            }
        }
        cooldown -= Time.deltaTime;
        cooldown = Mathf.Max(0f, cooldown);
        if (cooldown <= 0f && spriteRenderer.sprite == explodingSprite)
        {
            spriteRenderer.sprite = explodedSprite;
        }
    }
}
