using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEditor;
using NUnit.Framework;

public class GameController : MonoBehaviour
{
    [SerializeField] private SceneAsset[] worlds;
    [SerializeField] private Player player;
    private static readonly WaitForSecondsRealtime _waitForSeconds1 = new(1f);
    private readonly HashSet<ReversibleEntity> reversibleEntities = new();
    public InputActionAsset inputActions;
    public bool IsPlayerAlive { get; private set; }
    public Image coverImage;
    private int currentWorldIndex = 0;

    void Awake()
    {
        Debug.Assert(inputActions != null, "Input Actions not assigned in GameController");
        Debug.Assert(coverImage != null, "Cover Image not assigned in GameController");

        inputActions.Enable();
    }

    void Start()
    {
        IsPlayerAlive = true;
        Time.timeScale = 0f;

        coverImage.color = Color.black;
        coverImage.enabled = true;
        LeanTween.delayedCall(1f, () =>
            LeanTween.alpha(coverImage.rectTransform, 0f, 1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true).setOnComplete(() =>
            {
                coverImage.enabled = false;
                Time.timeScale = 1f;
            })
        ).setIgnoreTimeScale(true);
    }

    public void AddEntity(ReversibleEntity entity)
    {
        reversibleEntities.Add(entity);
    }

    public void RemoveEntity(ReversibleEntity entity)
    {
        reversibleEntities.Remove(entity);
    }

    public void ReverseAll(int frames)
    {
        foreach (var entity in reversibleEntities)
        {
            entity.Reverse(frames, frames / 2);
        }
    }

    public void OnPlayerDeath()
    {
        if (!IsPlayerAlive) return;
        IsPlayerAlive = false;

        Time.timeScale = 0f;

        // Pause game, cover screen, reload scene
        StartCoroutine(StartPlayerDeathAnimation());
    }

    private IEnumerator StartPlayerDeathAnimation()
    {
        yield return _waitForSeconds1;

        coverImage.color = Color.clear;
        coverImage.enabled = true;

        bool tweenFinished = false;
        LeanTween.alpha(coverImage.rectTransform, 1f, 1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true).setOnComplete(() =>
        {
            tweenFinished = true;
        });

        yield return new WaitUntil(() => tweenFinished);
        yield return _waitForSeconds1;

        // Restart the scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        yield return null;
    }

    public void ToNextWorld()
    {
        currentWorldIndex++;
        Assert.IsTrue(currentWorldIndex < worlds.Length, "No more worlds available!");
        coverImage.color = Color.black;
        coverImage.enabled = true;
        player.isInvincible = true;
        LeanTween.delayedCall(0f, () =>
            LeanTween.alpha(coverImage.rectTransform, 0f, 1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true).setOnComplete(() =>
            {
                coverImage.enabled = false;
                UnityEngine.SceneManagement.SceneManager.LoadScene(worlds[currentWorldIndex].name);
                player.isInvincible = false;
            })
        ).setIgnoreTimeScale(true);
        Time.timeScale = 0f;
    }

    public void NextBossLevel()
    {
        // Goes backwards using special boss scenes
    }
}
