using UnityEditor;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] private SceneAsset firstLevel;
    public void StartTheGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(firstLevel.name);
    }
}
