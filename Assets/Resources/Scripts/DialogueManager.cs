using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour, IPointerClickHandler
{
    enum TypingState { Typing, Finishing, Stopping }

    class TypingInfo { public TypingState state; }
    
    public TextMeshProUGUI header;
    public TextMeshProUGUI body;
    public Transform inactivePos;   // must share the same parent as `slate`
    public GameObject slate;

    private Vector3 activeLocalPos;
    private TypingInfo lastInfo;

    public void Awake()
    {
        activeLocalPos = slate.transform.localPosition;

        slate.SetActive(false);
        slate.transform.localPosition = inactivePos.localPosition;

        header.text = "";
        body.text = "";
    }

    public void ShowDialogue(string headerText, string bodyText)
    {
        LeanTween.cancel(slate);

        if (lastInfo != null)
            lastInfo.state = TypingState.Stopping;

        lastInfo = new TypingInfo { state = TypingState.Typing };

        slate.transform.localPosition = inactivePos.localPosition;
        slate.SetActive(true);

        header.text = headerText;
        StartCoroutine(TypeText(bodyText, body, 0.05f, lastInfo));

        LeanTween.moveLocal(slate, activeLocalPos, 1f).setEaseInOutSine();
    }

    IEnumerator TypeText(string fullText, TextMeshProUGUI textComponent, float delay, TypingInfo info)
    {
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
            info.state = TypingState.Finishing;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (lastInfo == null) return;

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

    public void HideDialogue()
    {
        LeanTween.cancel(slate);
        LeanTween.moveLocal(slate, inactivePos.localPosition, 1f)
            .setEaseInOutSine()
            .setOnComplete(() =>
            {
                slate.SetActive(false);
                header.text = "";
                body.text = "";
            });
    }
}
