using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DroneSpawner : MonoBehaviour
{
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 0f, 0f);
    [SerializeField] private float spawnDelay = 1f;
    private bool used = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!used && collision.CompareTag("Player"))
        {
            used = true;

            GameObject drone = Instantiate(dronePrefab, transform.position + spawnOffset, Quaternion.identity);
            drone.GetComponent<Drone>().enabled = false;
            drone.GetComponent<Collider2D>().enabled = false;
            Color color = drone.GetComponent<SpriteRenderer>().color;
            color.a = 0f;
            LeanTween.value(0f, 1f, spawnDelay).setEase(LeanTweenType.linear).setOnUpdate((float value) =>
            {
                color.a = value;
                drone.GetComponent<SpriteRenderer>().color = color;
            }).setOnComplete(() =>
            {
                drone.GetComponent<Drone>().enabled = true;
                drone.GetComponent<Collider2D>().enabled = true;
            }).setIgnoreTimeScale(true);
        }
    }
}
