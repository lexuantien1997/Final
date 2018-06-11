using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;
using UnityEditor;

public class TaurospearBT : MonoBehaviour {

    [System.Serializable]
    public class Vision
    {
        public float direction;
        public float fov;
        public float distance;
    }

    [System.Serializable]
    public class Movement
    {
        public float speed;
        public float jump;
        public float gravity;
    }

    [System.Serializable]
    public class ShortAttack
    {
        public DamageSystem meleeDamage;
        public float distance;
        public bool attackDash;
        public Vector2 attackForce;
        public float coolDownShortAttack;
    }

    [System.Serializable]
    public class LongAttack : Vision
    {
        public float coolDownLongAttack;
    }

    public bool spriteFacingLeft;
    public Movement movement;
    public Vision vision;
    public ShortAttack shortAttack;
    public LongAttack longAttack;
    public Vector2 HoleDetectionOffset = new Vector2(0, 0);
    public float RayOffset = 0.05f;
    public float obstacleHeightTolerance = 0.05f;
    public float HoleDetectionRaycastLength = 0.1f;
    public int numberHorizontalRaycast;
    public LayerMask mask;

    SpriteRenderer spriteRenderer;
    Vector3 moveVector;
    Vector2 spriteForward;
    Transform targetVisible;
    Animator animator;
    Rect bound;
    BoxCollider2D boxCollider2D;
    PlayerController2D playerController2D;
    public float _coolDownShortAttack;
    public float _coolDownLongAttack;
    Root ai = BT.Root();
    bool flag1, flag2,canpatrol;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteForward = spriteFacingLeft ? Vector2.left : Vector2.right;
        if (spriteRenderer.flipX) spriteForward = -spriteForward;
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        playerController2D = GetComponent<PlayerController2D>();
    }

    private void OnEnable()
    {
        _coolDownShortAttack = -1;
        _coolDownLongAttack = -1;
        canpatrol = true;
        BehaviourTree();
    }

    void BehaviourTree()
    {
        ai.OpenBranch(

            BT.If(() => { return targetVisible == null; }).OpenBranch(
                BT.Call(ScanTarget)
            ),


            BT.If(() => { return targetVisible != null /*&& canpatrol*/;  }).OpenBranch(              
                // Va chạm -> lật lại
                BT.If(() => { return CheckForObstacle() == true; }).OpenBranch(
                    BT.Call(Flip)
                ),
                BT.Call(SetHorizontalSpeed)
            ),

       
            BT.If(() => { return (ScanTargetInLongAttackArea() == true && flag2 == true); }).OpenBranch(

                BT.Call(RandomSkill),
                BT.If(() => { return flag2 == true; }).OpenBranch(

                    BT.SetBool(animator, "Patrolling", false),
                    // Dừng lại:
                    BT.Call(Stop),
                    //    BT.SetBool(animator, "Patrolling", false),
                    // Chuyển sang trạng thái chờ chiêu thức đánh:
                    BT.Trigger(animator, "Attack"),
                    BT.WaitForAnimatorState(animator, "Attack"),
                    // Thực hiện chiêu đánh 1:
                    BT.Trigger(animator, "Attack_2"),
                    // Khi thực hiện xong bật cờ kiểm tra lượt đánh tiếp theo khi nào sẽ diễn ra:            
                    BT.WaitForAnimatorState(animator, "Attack_2"),
                    BT.Call(ResetCoolDownLongAttack),
                    BT.SetBool(animator, "Patrolling", true)//,
                 )                                             //  BT.Call(ForgetTarget)
            ),

            // Nếu nằm trong vùng tấn công gần:
            BT.If(() => { return (CheckMeleeAttack() == true && flag1 == true); }).OpenBranch(

                BT.Call(RandomSkill),
                BT.If ( ()=> { return flag1 == true; } ).OpenBranch(
                                  
                    BT.SetBool(animator, "Patrolling",false),
                    // Dừng lại:
                    BT.Call(Stop),
                //    BT.SetBool(animator, "Patrolling", false),
                    // Chuyển sang trạng thái chờ chiêu thức đánh:
                    BT.Trigger(animator, "Attack"), 
                    BT.WaitForAnimatorState(animator, "Attack"),
                    // Thực hiện chiêu đánh 1:
                    BT.Trigger(animator, "Attack_1"),
                        // Khi thực hiện xong bật cờ kiểm tra lượt đánh tiếp theo khi nào sẽ diễn ra:            
                    BT.WaitForAnimatorState(animator, "Attack_1"),
                    BT.Call(ResetCoolDownShortAttack),
                        BT.SetBool(animator, "Patrolling", true)//,
                    //  BT.Call(ForgetTarget)
                )
           )
        );
    }

    public void RandomSkill()
    {
        if(flag1 && flag2)
        {
           int r = Random.Range(0, 2);
            Debug.Log(r);
           if(r == 0)
            {
                flag2 = false;
                ResetCoolDownLongAttack();
            } else
            {
                flag1 = false;
                ResetCoolDownShortAttack();
            }
        }
    }

    public bool ScanTargetInLongAttackArea()
    {
        if (targetVisible == null)
            return false;

        Vector3 dir = targetVisible.position - transform.position;

        if (dir.sqrMagnitude > longAttack.distance * longAttack.distance)
            return false;

        Vector3 testForward = Quaternion.Euler(0, 0, spriteFacingLeft ? Mathf.Sign(spriteForward.x) * -longAttack.direction : Mathf.Sign(spriteForward.x) * longAttack.direction) * spriteForward;
        // Debug.DrawRay(transform.position,testForward);
        float angle = Vector3.Angle(testForward, dir);
        if (angle > longAttack.fov * 0.5f)
            return false;

        return true;
    }



    public void ForgetTarget()
    {
        targetVisible = null;
    }

    public void Attack1()
    {
        // Trả về true để quái có thể tấn công => bật cờ
        if(_coolDownShortAttack <=0)
        {
            flag1 = true;
        } else
        {
            _coolDownShortAttack -= Time.deltaTime;
        }
    }

    public void Attack2()
    {
        // Trả về true để quái có thể tấn công => bật cờ
        if (_coolDownLongAttack <= 0)
        {
            flag2 = true;
        }
        else
        {
            _coolDownLongAttack -= Time.deltaTime;
        }
    }

    void ResetCoolDownShortAttack()
    {
        _coolDownShortAttack = shortAttack.coolDownShortAttack;
        flag1 = false;
    }

    void ResetCoolDownLongAttack()
    {
        _coolDownLongAttack = longAttack.coolDownLongAttack;
        flag2 = false;
    }

    public bool CheckMeleeAttack()
    {
        if (targetVisible == null)
            return false;

        if ((targetVisible.transform.position - transform.position).sqrMagnitude < shortAttack.distance * shortAttack.distance)
            return true;

        return false;
    }

    public void SetHorizontalSpeed()
    {
        moveVector.x = movement.speed * spriteForward.x;
    }

    public void Stop()
    {
        moveVector.x = 0;
    }

    public void Flip()
    {
        if (spriteForward.x < 0)
        {
            spriteRenderer.flipX = spriteFacingLeft;
            spriteForward.x *= -1;
        }
        else
        {
            spriteRenderer.flipX = !spriteFacingLeft;
            spriteForward.x *= -1;
        }
    }

    public bool CheckForObstacle()
    {

        // SetHorizontalSpeed(movement.speed);

        bound = new Rect(boxCollider2D.bounds.min.x,
                         boxCollider2D.bounds.min.y,
                         boxCollider2D.bounds.size.x,
                         boxCollider2D.bounds.size.y);


        float horizontalRayLength = Mathf.Abs(movement.speed * Time.deltaTime) + bound.width / 2 + RayOffset * 2;

        Vector2 horizontalRayCastFromBottom = new Vector2(bound.center.x,
                                                            bound.yMin + obstacleHeightTolerance);

        Vector2 horizontalRayCastToTop = new Vector2(bound.center.x,
                                                   bound.yMax - obstacleHeightTolerance);

        for (int i = 0; i < numberHorizontalRaycast; i++)
        {
            Vector2 rayOriginPoint = Vector2.Lerp(horizontalRayCastFromBottom, horizontalRayCastToTop, (float)i / (float)(numberHorizontalRaycast - 1));

            RaycastHit2D raycastHit = Physics2D.Raycast(rayOriginPoint, spriteForward, horizontalRayLength, mask);

            // Debug.Log(raycastHit.distance);

            Debug.DrawRay(rayOriginPoint, spriteForward * horizontalRayLength, Color.red);

            if (raycastHit.distance > 0) // collide
            {
                return true;
            }

        }


        // Vector2 raycastOrigin = new Vector2(transform.position.x + spriteForward.x * (HoleDetectionOffset.x + Mathf.Abs(boxCollider2D.bounds.size.x) / 2), transform.position.y + HoleDetectionOffset.y - (transform.localScale.y / 2));
        Vector2 raycastOrigin = (Vector2)transform.position + (Vector2)spriteForward * boxCollider2D.bounds.extents.x + (Vector2)spriteForward * HoleDetectionOffset;

        //  RaycastHit2D hit2d  = Physics2D.Raycast(raycastOrigin, Vector2.down, HoleDetectionRaycastLength, mask);
        RaycastHit2D hit2d = Physics2D.CircleCast(raycastOrigin, 0.1f, Vector2.down, HoleDetectionRaycastLength, mask);
        //  RaycastHit2D hit2d = Physics2D.Linecast(raycastOrigin, new Vector2(raycastOrigin.x, raycastOrigin.y + horizontalRayLength), mask);
        Debug.DrawRay(raycastOrigin, Vector3.down * HoleDetectionRaycastLength, Color.yellow);

        //  RaycastHit2D hit2d = Physics2D.Raycast(CastDown.position, spriteForward, HoleDetectionRaycastLength, mask);
        if (hit2d.collider == null)
        {
            //            return true;
            // Debug.Log("Not collide");
            return true;
        }// else
         //  {
         // Debug.Log("Collide");
         //  }


        //   Debug.Log(raycastOrigin);


        return false;
    }

    public void ScanTarget()
    {
        Vector3 dir = MainCharacterScript.Instance.transform.transform.position - transform.position;
        if (dir.sqrMagnitude > vision.distance * vision.distance)
            return;
        Vector3 testForward = Quaternion.Euler(0, 0, spriteFacingLeft ? Mathf.Sign(spriteForward.x) * -vision.direction : Mathf.Sign(spriteForward.x) * vision.direction) * spriteForward;
        
        float angle = Vector3.Angle(testForward, dir);
        if (angle > vision.fov * 0.5f)
            return;
        targetVisible = MainCharacterScript.Instance.transform.transform;
        animator.SetBool("Patrolling", true);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        moveVector.y = Mathf.Max(moveVector.y - movement.gravity * Time.deltaTime, -movement.gravity);
        playerController2D.Move(moveVector * Time.deltaTime);
        playerController2D.CheckCollisionDown();
    }

    private void Update()
    {
        ai.Tick();
        Attack1();
        Attack2();
    }

    private void OnDrawGizmosSelected()
    {
        //draw the cone of view
        Vector3 forward = spriteFacingLeft ? Vector2.left : Vector2.right;
        forward = Quaternion.Euler(0, 0, spriteFacingLeft ? -vision.direction : vision.direction) * forward;

        if (GetComponent<SpriteRenderer>().flipX)
            forward.x = -forward.x;

        Vector3 endpoint = transform.position + (Quaternion.Euler(0, 0, vision.fov * 0.5f) * forward);
        Handles.color = new Color(0, 1.0f, 0, 0.2f);
        Handles.DrawSolidArc(transform.position, -Vector3.forward, (endpoint - transform.position).normalized, vision.fov, vision.distance);

        // draw long attack
        forward = spriteFacingLeft ? Vector2.left : Vector2.right;
        forward = Quaternion.Euler(0, 0, spriteFacingLeft ? -longAttack.direction : longAttack.direction) * forward;

        if (GetComponent<SpriteRenderer>().flipX)
            forward.x = -forward.x;

        endpoint = transform.position + (Quaternion.Euler(0, 0, longAttack.fov * 0.5f) * forward);
        Handles.color = new Color(0.5f, 0, 1, 0.1f);
        Handles.DrawSolidArc(transform.position, -Vector3.forward, (endpoint - transform.position).normalized, longAttack.fov, longAttack.distance);

        ////Draw short
        Handles.color = new Color(1.0f, 0, 0, 0.1f);
        Handles.DrawSolidDisc(transform.position, Vector3.back, shortAttack.distance);

    }

}
