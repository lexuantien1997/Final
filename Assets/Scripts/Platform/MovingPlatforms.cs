using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is just a base platform. We will have another platform like: conditional platform, conditional moving platform
/// </summary>
public class MovingPlatforms : MonoBehaviour {
    
    /// <summary>
    /// All platform type
    /// </summary>
    public enum MovingPlatformType
    {
        BACK_FORTH, // inverse
        LOOP,  // Loop 4ever
        ONCE  // run once time
    }


    /// <summary>
    /// The game object attach to this platform
    /// </summary>
    [SerializeField]
    private GameObject platform;

    /// <summary>
    /// transform points at moving platform
    /// </summary>
    [SerializeField]
    private Transform[] points;

    /// <summary>
    /// wating time at specific point
    /// </summary>
    [SerializeField]
    private float[] waitTimes;

    /// <summary>
    /// attached moving platform to game object
    /// </summary>
    [SerializeField]
    private MovingPlatformType platformType; 

    /// <summary>
    /// the platform move speed
    /// </summary>
    [SerializeField]
    [Range(0.0f, 20.0f)] // Moving platform speed Slide Bar.
    private float moveSpeed = 3.0f;

    /// <summary>
    /// start moving platform ?
    /// </summary>
    [SerializeField]
    private bool started; 

    private int current; // current index
    private int next; // next index
    private int dir; // direction when move platform. 1: left to right - 2: right to left

    private float waitTime; // waiting time at each index

    private Transform myTransform;
  //private Rigidbody2D myRb;

    private void Awake() {

        // use object catching to optimize performance
        if (platform == null)
            myTransform = transform;
        else
            myTransform = platform.transform;

      //  myRb = GetComponent<Rigidbody2D>();
      //  myTransform.position = points[0].position;

        current = 0;
        next = points.Length > 1 ? 1 : 0;
        dir = 1; // left to right
        waitTime = waitTimes[0];
        // not active if delete this line
        if (points.Length != waitTimes.Length)
            waitTimes = new float[points.Length];
    }

	// Update is called once per frame
	void FixedUpdate () {
        
        // only start when it true
        if (!started)
            return;

        // We won't move if it is a sigle point
        if (next == current)
            return;

        // stop it here if wating time > 0
        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            return;
        }

        // s = v*t
        float distanceToGo = moveSpeed * Time.deltaTime;

        // move to new position
        myTransform.position = Vector3.MoveTowards(myTransform.position, points[next].position, distanceToGo);

        // when it comes to next position. we will change the index
        if (myTransform.position == points[next].position)
        {
            current = next; // 
            waitTime = waitTimes[current]; // get new wating time

            // Left to right
            if (dir > 0)
            {
                next += 1; // plus 1
                
                // check next position is not greater than points length
                if (next >= points.Length)
                {
                    switch (platformType)
                    {
                        case MovingPlatformType.BACK_FORTH:
                            next = points.Length - 2;
                            dir = -1; // change direction
                            break;
                        case MovingPlatformType.LOOP:
                            next = 0;
                            break;
                        case MovingPlatformType.ONCE: // have bug when change form ONCE to LOOP and BACK_FORTH
                            next -= 1;
                            started = false;
                            break;
                    }
                }               
            }
            else // Right to left
            {
                next -= 1; // sub -1 because it inverse
                if (next < 0)
                {
                    switch (platformType)
                    {
                        case MovingPlatformType.BACK_FORTH:
                            next = 1;
                            dir = 1;
                            break;
                        case MovingPlatformType.LOOP:
                            next = points.Length - 1;
                            break;
                        case MovingPlatformType.ONCE: // have bug when change form ONCE to LOOP and BACK_FORTH
                            next += 1;
                            started = false;
                            break;
                    }
                }
                
            }
            
           
        }

        // check distance always native
        //while (distanceToGo > 0)
        //{
        //    // Calculate current direction
        //    Vector2 direction = points[next].position - myTransform.position;

        //    float dist = distanceToGo;
        //}


    }


    // We draw the transform points to easy visualize the platform moving path.
    public void OnDrawGizmos()
    {
        if (points == null || points.Length < 2)
            return;

        for (var i = 1; i < points.Length; i++)
            Gizmos.DrawLine(points[i - 1].position, points[i].position);
        Gizmos.DrawLine(points[0].position, points[points.Length-1].position);
    }
}
