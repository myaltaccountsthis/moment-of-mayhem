using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialoguePrompt : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private string headerText;
    [SerializeField] private string dialogueText;
    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            dialogueManager.ShowDialogue(headerText, dialogueText);
            Destroy(gameObject);
        }
    }
}
