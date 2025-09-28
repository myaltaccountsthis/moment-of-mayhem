using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class FiringDevice : ReversibleEntity
{   
    public ProjectileEntity projectilePrefab;
    public Vector2 fixedDirection;
    public bool isEnabled = true;
    public float mainDelay;
    public float slowedDelay;
    
    public bool isAiming;
    public Player target;
    // public AudioSource fireSound;
    
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite shootingSprite;
    private Sprite normalSprite;
    private Coroutine coroutine;
    
    
    protected override void Awake()
    {
        base.Awake();
        fixedDirection = fixedDirection.normalized;
    }

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalSprite = spriteRenderer.sprite;
        coroutine = StartCoroutine(firing());
    }

    protected override void Update()
    {
        base.Update();
        if (enabled && isAiming && (target is not null))
        {
            transform.up = getTargetDirection();
        }
    }
    
    private Vector2 getTargetDirection()
    {
        return (target.transform.position - transform.position).normalized;
    }
    
    public float GetCurrentDelay()
    {
        return IsReversing ? slowedDelay : mainDelay;
    }

    IEnumerator firing()
    {
        while (true)
        {
            if (enabled)
                spriteRenderer.sprite = readySprite;
            
            float delay = GetCurrentDelay();
            yield return new WaitForSeconds(delay / 4);
            if (!enabled) continue;
            
            
            ProjectileEntity proj = Instantiate(projectilePrefab, GetProjectileStartPosition(), Quaternion.identity);
            Vector2 direction = isAiming && (target is not null) 
                ? getTargetDirection() 
                : fixedDirection;
            proj.transform.up = direction;
            
        
            // AudioSource audio = Instantiate(fireSound, transform.position, Quaternion.identity);
            // audio.Play();
            // Destroy(audio.gameObject, audio.clip.length);
            spriteRenderer.sprite = shootingSprite;
            yield return new WaitForSeconds(delay / 2);
            spriteRenderer.sprite = normalSprite;
            OnWait(delay / 2);
            yield return new WaitForSeconds(delay / 4);
        }
    }


    protected virtual void OnWait(float seconds)
    {
        
    }
    
    protected virtual Vector3 GetProjectileStartPosition()
    {
        return transform.position;
    }
    
    public void SetEnabled(bool enabled)
    {
        this.isEnabled = enabled;
        if (!enabled) {
            spriteRenderer.sprite = inactiveSprite;
            StopCoroutine(coroutine);
        }
    }
}