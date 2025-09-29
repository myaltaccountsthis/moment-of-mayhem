using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] private string firstLevel;
    public void StartTheGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(firstLevel);
    }
}
