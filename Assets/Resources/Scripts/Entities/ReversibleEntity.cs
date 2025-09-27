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
        entity.transform.position = position;
    }
}

[RequireComponent(typeof(Rigidbody2D))]
public class ReversibleEntity : InteractableEntity
{
    private const int MaxStateHistory = 1200;
    private readonly LinkedList<ReversibleEntityData> stateHistory = new();

    protected new Rigidbody2D rigidbody;
    public int ReverseTime { get; private set; } = 0;
    public bool IsReversing => ReverseTime > 0;
    
    private int totalReverseTime = 0;
    private int totalFrames = 0;
    
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

    protected override void Update()
    {
        base.Update();

        if (IsReversing)
        {
            int reverseElapsedFrames = totalReverseTime - ReverseTime;
            float alpha = Mathf.Clamp01((float) reverseElapsedFrames / totalReverseTime);

            // Interpolate frame index with easing
            float val = EaseOutQuad(0f, totalReverseTime, alpha);

            float idx = Mathf.Clamp(totalFrames - val, 0f, stateHistory.Count - 1);
            int right = Mathf.CeilToInt(idx);
            float delta = 1.0f - (right - idx);
            while (stateHistory.Count > right + 1) stateHistory.RemoveLast();

            // Find left and right states of the current index
            ReversibleEntityData rightState = stateHistory.Last.Value;
            ReversibleEntityData leftState = Mathf.FloorToInt(idx) == right 
                ? rightState : stateHistory.Last.Previous!.Value;

            // Interpolate between the two states
            Vector3 newPos = Vector3.Lerp(leftState.position, rightState.position, delta);
            transform.position = newPos;
            
            ReverseTime--;
            if (ReverseTime <= 0)
            {
                totalReverseTime = 0;
                totalFrames = 0;
                ReverseTime = 0;
            }
            return; // don't record state while reversing
        }

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

    public override void Interact(Player player)
    {
        // Start reversing time for this entity
        Reverse(60);
    }

    public void Reverse(int timeInFrames)
    {
        if (IsReversing) return;
        IsReversing = true;
        // n + 1 states for n frames of rewind
        stateHistory.AddLast(CaptureState()); 
        ReverseTime = timeInFrames;
        
        totalFrames = stateHistory.Count;
        totalReverseTime = timeInFrames;
    }

    private static float EaseOutQuad(float start, float end, float t)
    {
        t = Mathf.Clamp01(t);
        float eased01 = 1f - (1f - t) * (1f - t);
        return Mathf.LerpUnclamped(start, end, eased01);
    }

    void OnDestroy()
    {
        gameController.RemoveEntity(this);
    }
}
