using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Entity : MonoBehaviour
{
    public bool IsFrozen { get; private set; }
    protected GameController gameController;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {

    }
}
