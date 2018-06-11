using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{

    public LayerMask layerMask;
    public float distance;

    public Rigidbody2D rigidbody2D { get; set; }
    ContactFilter2D contactFilter2D;

    Vector2 prevPosition;
    Vector2 currPosition;
    Vector2 nextPosition;

    Vector2 velocity;

    RaycastHit2D[] hit2D = new RaycastHit2D[10];
    RaycastHit2D[] groundHit = new RaycastHit2D[3];
    Vector2[] groundPosition = new Vector2[3];
    Collider2D[] groundColliders = new Collider2D[3];

    public bool OnGround { get; set; }

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        contactFilter2D.useTriggers = false;
        contactFilter2D.useLayerMask = true;
        contactFilter2D.layerMask = layerMask;
        Physics2D.queriesStartInColliders = false;
        currPosition = rigidbody2D.position;
        prevPosition = rigidbody2D.position;
    }




    public void Move(Vector2 mov)
    {
        nextPosition += mov;
    }


    public void CheckCollisionDown()
    {
        Vector2 raycastPosition = rigidbody2D.position;

        groundPosition[0] = raycastPosition + Vector2.left * 0.4f;
        groundPosition[1] = raycastPosition;
        groundPosition[2] = raycastPosition + Vector2.right * 0.4f;

        for (int i = 0; i < groundPosition.Length; i++)
        {
            int count = Physics2D.Raycast(groundPosition[i], Vector2.down, contactFilter2D, hit2D, distance);
            // Nó sẽ ở trên mặt đất nếu 1 trong 3 cái ground collide với mặt đất:
            groundHit[i] = count > 0 ? hit2D[0] : new RaycastHit2D();
            groundColliders[i] = groundHit[i].collider;
        }

        Vector2 groundNormal = Vector2.zero;
        int hitCount = 0;

        for (int i = 0; i < groundHit.Length; i++)
        {
            if (groundHit[i].collider != null)
            {
                groundNormal += groundHit[i].normal;
                hitCount++;
            }
        }

        if (hitCount > 0)
        {
            groundNormal.Normalize();
        }

        Vector2 relativeVelocity = velocity;

        if (Mathf.Approximately(groundNormal.x, 0f) && Mathf.Approximately(groundNormal.y, 0f))
        {
            OnGround = false;
        }
        else
        {
            OnGround = relativeVelocity.y <= 0f;
        }


        for (int i = 0; i < hit2D.Length; i++)
        {
            hit2D[i] = new RaycastHit2D();
        }

    }
    public void CheckCollisionUp()
    {
        Vector2 raycastPosition = rigidbody2D.position;
        Vector2 dir = Vector2.up;
        groundPosition[0] = raycastPosition + Vector2.left * 0.4f + dir;
        groundPosition[1] = raycastPosition + dir;
        groundPosition[2] = raycastPosition + Vector2.right * 0.4f + dir;

        for (int i = 0; i < groundPosition.Length; i++)
        {
            int count = Physics2D.Raycast(groundPosition[i], dir, contactFilter2D, hit2D, distance);
            // Nó sẽ ở trên mặt đất nếu 1 trong 3 cái ground collide với mặt đất:
            groundHit[i] = count > 0 ? hit2D[0] : new RaycastHit2D();
            groundColliders[i] = groundHit[i].collider;
        }

        Vector2 groundNormal = Vector2.zero;
        int hitCount = 0;

        for (int i = 0; i < groundHit.Length; i++)
        {
            if (groundHit[i].collider != null)
            {
                groundNormal += groundHit[i].normal;
                hitCount++;
            }
        }

        if (hitCount > 0)
        {
            groundNormal.Normalize();
        }

        Vector2 relativeVelocity = velocity;

        if (Mathf.Approximately(groundNormal.x, 0f) && Mathf.Approximately(groundNormal.y, 0f))
        {
            OnGround = false;
        }
        else
        {
            OnGround = relativeVelocity.y <= 0f;
        }


        for (int i = 0; i < hit2D.Length; i++)
        {
            hit2D[i] = new RaycastHit2D();
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        prevPosition = rigidbody2D.position;
        currPosition = prevPosition + nextPosition;
        velocity = (currPosition - prevPosition) / Time.deltaTime;
        rigidbody2D.MovePosition(currPosition);
        nextPosition = Vector2.zero;


        CheckCollisionDown();

    }
}
