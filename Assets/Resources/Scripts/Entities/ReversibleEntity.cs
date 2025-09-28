using UnityEngine;
using System.Collections.Generic;

public class ReversibleEntityData
{
    public readonly Vector3 position;

    public ReversibleEntityData(Vector3 pos)
    {
        position = pos;
    }

    // Subclasses must cast to the respective Entity type in the Apply method
    public virtual void Apply(ReversibleEntity entity)
    {
        entity.SetRigidbodyPosition(position);
    }
}

[RequireComponent(typeof(Rigidbody2D))]
public class ReversibleEntity : CollidableEntity, IInteractable
{
    private const int MaxStateHistory = 1200;
    private readonly LinkedList<ReversibleEntityData> stateHistory = new();

    protected new Rigidbody2D rigidbody;

    private int ReverseTime = 0;
    public bool IsReversing => ReverseTime > 0;

    private int totalReverseTime = 0;
    private int totalReverseFrames = 0;
    private int totalFrameCount = 0;

    public virtual bool DestroyableOnReverse => false;

    protected override void Awake()
    {
        base.Awake();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();
        gameController.AddEntity(this);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (IsReversing)
        {
            int elapsedTime = totalReverseTime - ReverseTime;
            float alpha = Mathf.Clamp01((float)elapsedTime / totalReverseTime);

            // Interpolate frame index with easing
            float val = LeanTween.easeOutQuad(0f, totalReverseFrames, alpha);

            float idx = Mathf.Clamp(totalFrameCount - 1 - val, 0f, stateHistory.Count - 1);
            int right = Mathf.CeilToInt(idx);
            float delta = 1.0f - (right - idx);
            while (stateHistory.Count > right + 1) stateHistory.RemoveLast();

            // Find left and right states of the current index
            ReversibleEntityData rightState = stateHistory.Last.Value;
            ReversibleEntityData leftState = Mathf.FloorToInt(idx) == right
                ? rightState : stateHistory.Last.Previous!.Value;

            // Interpolate between the two states
            Vector3 newPos = Vector3.Lerp(leftState.position, rightState.position, delta);
            rigidbody.MovePosition(newPos);


            if (DestroyableOnReverse && stateHistory.Count == 1)
            {
                Destroy(gameObject);
                return;
            }

            ReverseTime--;
            if (ReverseTime <= 0)
            {
                totalReverseTime = 0;
                totalReverseFrames = 0;
                totalFrameCount = 0;
                ReverseTime = 0;
            }
            return; // don't record state while reversing
        }

        spriteRenderer.color = Color.white;
        // Add current state to history
        stateHistory.AddLast(CaptureState());
        // Remove first state if we exceed max history size
        while (stateHistory.Count > MaxStateHistory)
        {
            stateHistory.RemoveFirst();
        }
    }

    protected virtual ReversibleEntityData CaptureState()
    {
        return new ReversibleEntityData(transform.position);
    }

    public void Interact(Player player)
    {
        // if (IsReversing) return;
        // Start reversing time for this entity
        Reverse(60, 30);
    }

    // reverse timeInFrames: number of frames to rewind
    // durationInFrames: number of frames over which to perform the rewind (for easing)
    public void Reverse(int timeInFrames, int durationInFrames)
    {
        if (IsReversing) return;
        timeInFrames = Mathf.Max(1, timeInFrames);
        // n + 1 states for n frames of rewind
        stateHistory.AddLast(CaptureState());
        ReverseTime = durationInFrames;
        totalFrameCount = stateHistory.Count;
        totalReverseTime = durationInFrames;
        totalReverseFrames = timeInFrames;
        spriteRenderer.color = Color.lightGoldenRodYellow;
    }

    public void SetRigidbodyPosition(Vector3 position)
    {
        rigidbody.MovePosition(position);
    }

    void OnDestroy()
    {
        gameController.RemoveEntity(this);
    }
}
