using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBox : MonoBehaviour
{

    public SpriteRenderer render;
    public BoxCollider2D collider;
    public bool facingBaseOnOffset;
    private bool flipDir;
    float top;
    float btm;
    float left;
    float right;


    public float Top { get { return top; } }
    public float Btm { get { return btm; } }
    public float Left { get { return left; } }
    public float Right { get { return right; } }

    private void Awake()
    {
        //  collider = GetComponent<BoxCollider2D>();
        flipDir = render.flipX;


        top = collider.offset.y + (collider.size.y / 2f);
        btm = collider.offset.y - (collider.size.y / 2f);
        left = collider.offset.x - (collider.size.x / 2f);
        right = collider.offset.x + (collider.size.x / 2f);

    }


    // Update is called once per frame
    void Update()
    {
        //        Physics2D.BoxCastAll(collider.transform.position, collider.size,collider.)
        // Debug.Log("Render: " + render.flipX);
        // Debug.Log("Flip: " + flipDir);
        if (render.flipX != flipDir && facingBaseOnOffset)
        {
            Vector2 offset = collider.offset;
            offset.x *= -1;
            collider.offset = offset;
            flipDir = render.flipX;

            top = collider.offset.y + (collider.size.y / 2f);
            btm = collider.offset.y - (collider.size.y / 2f);
            left = collider.offset.x - (collider.size.x / 2f);
            right = collider.offset.x + (collider.size.x / 2f);
        }


        // Vector2 topLeft = transform.TransformPoint(new Vector2(left, top));
        // Vector2 topRight = transform.TransformPoint(new Vector2(right, top));
        // Vector2 btmLeft = transform.TransformPoint(new Vector2(left, btm));
        // Vector2 btmRight = transform.TransformPoint(new Vector2(right, btm));


    }
}
