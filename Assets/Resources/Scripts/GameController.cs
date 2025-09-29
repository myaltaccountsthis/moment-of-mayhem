using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEditor;
using NUnit.Framework;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    [SerializeField] private SceneAsset nextWorld;
    [SerializeField] private Player player;
    private static readonly WaitForSecondsRealtime _waitForSeconds1 = new(1f);
    private readonly HashSet<ReversibleEntity> reversibleEntities = new();
    public InputActionAsset inputActions;
    public bool IsPlayerAlive { get; private set; }
    public Image coverImage;

    public AudioSource everythingReversed;
    public AudioSource entityReversed;

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


        FadeFromBlack(1f, () =>
        {
            Time.timeScale = 1f;
        });
    }

    public LTDescr FadeToBlack(float duration = 1f, System.Action callback = null)
    {
        coverImage.color = Color.clear;
        coverImage.enabled = true;
        return LeanTween.alpha(coverImage.rectTransform, 1f, duration).setEase(LeanTweenType.linear).setIgnoreTimeScale(true).setOnComplete(() =>
        {
            callback?.Invoke();
        });
    }

    public LTDescr FadeFromBlack(float duration = 1f, System.Action callback = null)
    {
        coverImage.color = Color.black;
        coverImage.enabled = true;
        return LeanTween.alpha(coverImage.rectTransform, 0f, duration).setEase(LeanTweenType.linear).setIgnoreTimeScale(true).setOnComplete(() =>
        {
            coverImage.enabled = false;
            callback?.Invoke();
        });
    }

    public void AddEntity(ReversibleEntity entity)
    {
        reversibleEntities.Add(entity);
    }

    public void RemoveEntity(ReversibleEntity entity)
    {
        reversibleEntities.Remove(entity);
    }

    public void ReverseAll(int frames, int durationInFrames = -1)
    {
        durationInFrames = durationInFrames == -1 ? frames * 3 / 4 : durationInFrames;
        everythingReversed.Play();
        foreach (var entity in reversibleEntities)
        {
            entity.Reverse(frames, durationInFrames);
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
        Assert.IsNotNull(nextWorld, "Next world not assigned in GameController");
        coverImage.color = Color.black;
        coverImage.enabled = true;
        player.isInvincible = true;
        LeanTween.delayedCall(0f, () =>
            LeanTween.alpha(coverImage.rectTransform, 0f, 1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true).setOnComplete(() =>
            {
                coverImage.enabled = false;
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextWorld.name);
                player.isInvincible = false;
            })
        ).setIgnoreTimeScale(true);
        Time.timeScale = 0f;
    }

    private readonly string[] tilemapNames = new string[] { "Void", "FarFuture", "Future", "Present" };
    private readonly Color[] backgroundColors = new Color[] {
        new(0f, 0f, 0f), // Void
        new(.4f, .3f, .9f), // Far Future
        new(.4f, .4f, .8f), // Future
        new(.2f, .8f, .4f)  // Present
    };
    public LTDescr NextBossLevel(BossPhase phase, System.Action callback = null)
    {
        // Goes backwards using special boss scenes
        return FadeToBlack(2, () =>
        {
            // Change tilemap
            int newPhase = (int)phase;
            int prevPhase = newPhase - 1;
            Transform grid = GameObject.Find("Grid").transform;
            TilemapRenderer oldTilemap = grid.Find(tilemapNames[prevPhase]).GetComponent<TilemapRenderer>();
            TilemapRenderer newTilemap = grid.Find(tilemapNames[newPhase]).GetComponent<TilemapRenderer>();
            oldTilemap.enabled = false;
            newTilemap.enabled = true;
            Camera.main.backgroundColor = backgroundColors[newPhase];

            player.Heal(100);

            LeanTween.delayedCall(2f, () => FadeFromBlack(2, callback));
        });
    }

    public void OnBossDefeated()
    {
        // End game sequence
        player.healthDrainScale = 0f;
        NextBossLevel(BossPhase.Defeated, () =>
        {
            // Show end screen

        });
    }
}
