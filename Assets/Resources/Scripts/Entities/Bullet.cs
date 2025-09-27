using UnityEngine;

public class Bullet : Entity
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;

    private Collider2D col;

    protected override void Awake()
    {
        base.Awake();
        col = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Entity other = collision.gameObject.GetComponent<Entity>();
        if (other == null)
        {
            return;
        }
        // do other stuff
        Destroy(gameObject);
    }
}