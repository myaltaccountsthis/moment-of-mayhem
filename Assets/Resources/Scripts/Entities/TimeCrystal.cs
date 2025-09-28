using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TimeCrystal : Entity
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            gameController.GetComponent<GameController>().ToNextWorld();
            Destroy(gameObject);
        }
    }
}
