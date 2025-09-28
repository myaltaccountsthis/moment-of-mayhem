using UnityEngine;
using UnityEngine.Serialization;

// extend reversibleentity and damagepart
public class OscillatingTrap : DamagePart, IInteractable
{
    // Oscillates between two positions
    public Transform leftPos;
    public Transform rightPos;
    public bool startRight = false; 
    public float oscillationTime;
    public float slowDuration;
    public float slowedTimeScale;
    
    private ParticleSystem particles;
    public bool isSlowed = false;
    protected float elapsed = 0f;
    
    protected float timeScale => isSlowed ? slowedTimeScale : 1f;
    
    protected override void Awake()
    {
        base.Awake();
        transform.position = startRight ? rightPos.position : leftPos.position;
        particles = Instantiate(Resources.Load<ParticleSystem>("Prefabs/Particles"), transform);
    }


    protected override void Update()
    {
        base.Update();
        // advance time (scaled)
        elapsed += Time.deltaTime * timeScale;
        float u = Mathf.PingPong(elapsed / oscillationTime, 1f);

        float eased = LeanTween.easeInOutSine(0f, 1f, u);
        // interpolate between endpoints 
        transform.position = Vector3.LerpUnclamped(leftPos.position, rightPos.position, eased);
    }

    public void Interact(Player player)
    {
        if (!isSlowed)
        {
            isSlowed = true;
            Invoke(nameof(ResetSpeed), slowDuration);
            var emission = particles.emission;
            emission.enabled = true;
            spriteRenderer.color = Color.lightGoldenRod;
        }
    }
    
    private void ResetSpeed()
    {
        isSlowed = false;
        var emission = particles.emission;
        emission.enabled = false;
        spriteRenderer.color = Color.white;
    }
}