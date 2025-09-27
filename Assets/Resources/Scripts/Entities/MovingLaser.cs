using UnityEngine;

public class MovingLaser : DamagePart
{
    [SerializeField] private float rotStart;
    [SerializeField] private float rotEnd;
    [SerializeField] private float rotTime;
    [SerializeField] private GameObject laserSprite;
    private float rotTimer;
    private bool forward = true;
    protected override void Start()
    {
        base.Start();
        rotTimer = 0f;
        transform.rotation = Quaternion.Euler(0f, 0f, rotStart);
    }

    protected override void Update()
    {
        base.Update();
        if (forward)
        {
            rotTimer += Time.deltaTime;
            if (rotTimer >= rotTime)
            {
                rotTimer = rotTime;
                forward = false;
                laserSprite.SetActive(false);
            }
        }
        else
        {
            rotTimer -= Time.deltaTime;
            if (rotTimer <= 0f)
            {
                rotTimer = 0f;
                forward = true;
                laserSprite.SetActive(true);
            }
        }
        float alpha = rotTimer / rotTime;
        alpha = LeanTween.easeInOutSine(0, 1, alpha);
        float angle = Mathf.Lerp(rotStart, rotEnd, alpha);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
