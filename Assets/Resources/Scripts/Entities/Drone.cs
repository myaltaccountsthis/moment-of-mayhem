using UnityEngine;

public class Drone : ReversibleEntity
{
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private int bulletsLeft;
    [SerializeField] private float fireRate;

    [SerializeField] private Bullet bulletPrefab;
    private Rigidbody2D rb;
    

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }
}