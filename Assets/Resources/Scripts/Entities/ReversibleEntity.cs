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
            ReverseTime--;
            if (stateHistory.Count > 0)
            {
                // TODO change this to not be linear
                // Apply the last state and remove it from history
                ReversibleEntityData lastState = stateHistory.Last.Value;
                lastState.Apply(this);
                stateHistory.RemoveLast();
            }
            else if (DestroyableOnReverse)
            {
                Destroy(gameObject);
            }
            // Don't record state if reversing
            return;
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

    public void Reverse(int frames)
    {
        ReverseTime = Mathf.Max(ReverseTime, frames);
    }

    void OnDestroy()
    {
        gameController.RemoveEntity(this);
    }
}
