using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DroneSpawner : MonoBehaviour
{
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private Vector3 spawnPoint;
    private bool used = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!used && collision.CompareTag("Player"))
        {
            used = true;

            GameObject drone = Instantiate(dronePrefab, spawnPoint, Quaternion.identity);
            drone.GetComponent<Drone>().enabled = false;
            Color color = drone.GetComponent<SpriteRenderer>().color;
            color.a = 0f;
            LeanTween.alpha(drone, 1f, 1f).setEase(LeanTweenType.linear).setOnComplete(() =>
            {
                drone.GetComponent<Drone>().enabled = true;
            }).setIgnoreTimeScale(true);
            Destroy(gameObject);
        }
    }
}
