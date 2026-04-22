using System.Collections.Generic;
using UnityEngine;

public class Altord : MonoBehaviour
{
    public Rigidbody2D body;
    public SpriteRenderer spriteRenderer;

    // WALK
    public List<Sprite> eSprites;
    public List<Sprite> nSprites;
    public List<Sprite> neSprites;
    public List<Sprite> nwSprites;
    public List<Sprite> sSprites;
    public List<Sprite> seSprites;
    public List<Sprite> swSprites;
    public List<Sprite> wSprites;

    // IDLE (INI KUNCI UTAMANYA 🔥)
    public Sprite idleE;
    public Sprite idleN;
    public Sprite idleNE;
    public Sprite idleNW;
    public Sprite idleS;
    public Sprite idleSE;
    public Sprite idleSW;
    public Sprite idleW;

    public float runSpeed = 5f;
    public float frameRate = 10f;

    private Vector2 direction;
    private Vector2 lastDirection;

    private float timer;
    private int frame;

    void Update()
    {
        direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        if (direction != Vector2.zero)
        {
            lastDirection = direction;
        }

        body.linearVelocity = direction * runSpeed;

        SetSprite();
    }

    void SetSprite()
    {
        List<Sprite> selectedWalk = null;
        Sprite selectedIdle = null;

        Vector2 dir = direction == Vector2.zero ? lastDirection : direction;

        // === PILIH ARAH ===
        if (dir.y > 0.1f)
        {
            if (dir.x > 0.1f)
            {
                selectedWalk = neSprites;
                selectedIdle = idleNE;
            }
            else if (dir.x < -0.1f)
            {
                selectedWalk = nwSprites;
                selectedIdle = idleNW;
            }
            else
            {
                selectedWalk = nSprites;
                selectedIdle = idleN;
            }
        }
        else if (dir.y < -0.1f)
        {
            if (dir.x > 0.1f)
            {
                selectedWalk = seSprites;
                selectedIdle = idleSE;
            }
            else if (dir.x < -0.1f)
            {
                selectedWalk = swSprites;
                selectedIdle = idleSW;
            }
            else
            {
                selectedWalk = sSprites;
                selectedIdle = idleS;
            }
        }
        else
        {
            if (dir.x > 0.1f)
            {
                selectedWalk = eSprites;
                selectedIdle = idleE;
            }
            else if (dir.x < -0.1f)
            {
                selectedWalk = wSprites;
                selectedIdle = idleW;
            }
        }

        // === IDLE ===
        if (direction == Vector2.zero)
        {
            timer = 0f;
            frame = 0;

            if (selectedIdle != null)
            {
                spriteRenderer.sprite = selectedIdle;
            }

            return;
        }

        // === WALK ANIMATION ===
        timer += Time.deltaTime;

        if (timer >= 1f / frameRate)
        {
            timer = 0f;
            frame++;

            if (selectedWalk != null && selectedWalk.Count > 0)
            {
                frame %= selectedWalk.Count;
                spriteRenderer.sprite = selectedWalk[frame];
            }
        }
    }
}