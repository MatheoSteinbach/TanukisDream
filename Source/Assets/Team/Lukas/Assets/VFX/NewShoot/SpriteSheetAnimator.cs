using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetAnimator : MonoBehaviour
{
    public Sprite[] sprites;
    public float speed = 5f;

    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;
    private float lastUpdateTime = 0f;
    private Vector3 lastPosition;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastPosition = transform.position;
    }

    void Update()
    {
        // Get the direction and magnitude of movement
        Vector3 direction = transform.position - lastPosition;
        float magnitude = direction.magnitude;

        // If the object is moving, rotate the sprite sheet
        if (magnitude > 0)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // Update the sprite
        if (Time.time - lastUpdateTime > speed)
        {
            currentIndex++;
            if (currentIndex >= sprites.Length)
            {
                currentIndex = 0;
            }
            spriteRenderer.sprite = sprites[currentIndex];
            lastUpdateTime = Time.time;
        }

        lastPosition = transform.position;
    }
}