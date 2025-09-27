using UnityEngine;

public class Entity : MonoBehaviour
{
    public bool IsFrozen { get; private set; }
    protected GameController gameController;

    protected virtual void Awake()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
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
