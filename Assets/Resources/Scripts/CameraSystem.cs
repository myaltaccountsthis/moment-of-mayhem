using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSystem : MonoBehaviour
{
    private new Camera camera;
    private Player player;
    private Vector2 moveVelocity;

    void Awake()
    {
        QualitySettings.vSyncCount = 1;

        camera = GetComponent<Camera>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void Start()
    {
        moveVelocity = Vector2.zero;
        camera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, camera.transform.position.z);
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector2 playerPos = player.transform.position;

            Vector2 cameraPos = Vector2.SmoothDamp(camera.transform.position, playerPos, ref moveVelocity, 0.06f, Mathf.Infinity, Time.deltaTime);
            camera.transform.position = new Vector3(cameraPos.x, cameraPos.y, camera.transform.position.z);
        }
    }
}
