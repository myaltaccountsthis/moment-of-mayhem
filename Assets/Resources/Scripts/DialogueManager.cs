using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class DialogueManager : MonoBehaviour, IPointerClickHandler
{
    enum TypingState
    {
        Typing,
        Finishing,
        Stopping
    }

    class TypingInfo
    {
        public TypingState state;
    }
    
    public TextMeshProUGUI header;
    public TextMeshProUGUI body;
    public Transform inactivePos;
    public GameObject slate;

    private Vector3 activePos;
    private TypingInfo lastInfo;
    
    public void Awake()
    {
        activePos = slate.transform.position;
        gameObject.SetActive(false);
        
        slate.transform.position = inactivePos.position;
        header.text = "";
        body.text = "";
        // temporary
        ShowDialogue("Hey", "yo yo yo yo yo yo yo yo yo");
        Debug.Log("Showing dialogue");
    }
    
    public void ShowDialogue(string headerText, string bodyText)
    {
        LeanTween.cancel(slate);
        if (lastInfo != null)
        {
            lastInfo.state = TypingState.Stopping;
        }
        lastInfo = new TypingInfo { state = TypingState.Typing };
        slate.transform.position = inactivePos.position;
        gameObject.SetActive(true);
        header.text = headerText;
        StartCoroutine(TypeText(bodyText, body, 0.05f, lastInfo));
        LeanTween.move(slate, activePos, 1f).setEaseInOutSine();
    }

    IEnumerator TypeText(string fullText, TextMeshProUGUI textComponent, float delay, TypingInfo info) {
        textComponent.text = "";
        for (int i = 0; i < fullText.Length; i++)
        {
            if (info.state == TypingState.Finishing)
            {
                textComponent.text = fullText;
                break;
            }
            else if (info.state == TypingState.Stopping)
            {
                break;
            }
            textComponent.text += fullText[i];
            yield return new WaitForSeconds(delay);
        }
        
        if (info.state == TypingState.Typing)
        {
            info.state = TypingState.Finishing;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (lastInfo != null)
        {
            if (lastInfo.state == TypingState.Typing)
            {
                lastInfo.state = TypingState.Finishing;
            }
            else if (lastInfo.state == TypingState.Finishing)
            {
                lastInfo.state = TypingState.Stopping;
                HideDialogue();
            }
        }
    }

    public void HideDialogue()
    {
        LeanTween.cancel(slate);
        LeanTween.move(slate, inactivePos.position, 1f).setEaseInOutSine().setOnComplete(() =>
        {
            gameObject.SetActive(false);
            header.text = "";
            body.text = "";
        });
    }
}