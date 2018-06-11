using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    [System.Serializable]
    public enum Style
    {
        Long,
        Short,
        Both
    }

    public DamageSystem touchDamage;

	[System.Serializable]
	public class Timer {
		public float timeLostTarget;
		public float flickeringDuration;
	}

    [System.Serializable]
    public class Movement
    {
        public float speed;
        public float jump;
        public float gravity;
    }

    public bool spriteFacingLeft;
    

    [System.Serializable]
    public class Vision
    {
        public float direction;
        public float fov;
        public float distance;
    }

    [System.Serializable]
    public class LongAttack : Vision
    {
        public GameObject bullet;
        public Transform shootPos;
        public float shootRate;
        public float shootForce;
		public float shootAngle;
    }
    [System.Serializable]
    public class ShortAttack 
    {
        public DamageSystem meleeDamage;
        public float distance;
        public bool attackDash;
        public Vector2 attackForce;        
    }


    private Vector3 targetShootPosition;
    public Style style;
	public Timer timer;
    public Vision vision;
    public ShortAttack shortAttack;
    public LongAttack longAttack;
    public Movement movement;
    
    private PlayerController2D playerController2D;
    private Transform targetVisible;
    private Vector3 moveVector;
	public Transform TargetVisible { get { return targetVisible; } set { targetVisible = value; } }

    private Vector2 spriteForward;
    private SpriteRenderer spriteRenderer;
	private Animator animator;
    public float timeLastLostTarget,timeLostShootRate;

    private int deadAnimation = Animator.StringToHash("dead");
    private int spotAnimation = Animator.StringToHash("spot");
    private int lostAnimation = Animator.StringToHash("lost");
    private int shootAnimation = Animator.StringToHash("shoot");
    private int hitAnimation = Animator.StringToHash("hit");
    private int meleeAttackAnimation = Animator.StringToHash("melee_attack");

    public Vector2 SpriteForward { get { return spriteForward; } }

    private bool visibleInlongArea;
    private bool dead;
    ContactFilter2D contactFilter2D;
    RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[16];

    protected Rect bound;
    protected BoxCollider2D boxCollider2D;

    // Use this for initialization
    void Awake () {
        boxCollider2D = GetComponent<BoxCollider2D>();
		animator = GetComponent<Animator> ();
        spriteRenderer = GetComponent<SpriteRenderer>();
        dead = true;
        playerController2D = GetComponent<PlayerController2D>();

        spriteForward = spriteFacingLeft ? Vector2.left : Vector2.right;
        if (spriteRenderer.flipX) spriteForward = -spriteForward;
    }

    private void Start()
    {
        contactFilter2D = new ContactFilter2D();
        contactFilter2D.layerMask = playerController2D.layerMask;
        contactFilter2D.useLayerMask = true;
        contactFilter2D.useTriggers = false;

      
        DisableMeleeDamage();
        EnableTouchDamage();
    }

    public void StartAttack()
    {
        if (shortAttack.attackDash == true)
            moveVector = new Vector2(SpriteForward.x * shortAttack.attackForce.x, shortAttack.attackForce.y);
        EnableMeleeDamage();
    }

    public void EndAttack()
    {
        DisableMeleeDamage();
    }

    private void OnEnable()
    {
        dead = false;
        targetVisible = null;
        timeLastLostTarget = 0;
        DisableMeleeDamage();
        EnableTouchDamage();
    }

    public void EnableTouchDamage()
    {
        if(touchDamage != null)
        {
            touchDamage.EnableDamage();
        }
    }

    public void DisableAllDamage()
    {
        if(touchDamage != null)
        {
            touchDamage.DisableDamage();
        }

        if(shortAttack.meleeDamage != null)
        {
            shortAttack.meleeDamage.DisableDamage();
        }
    }

    public void DisableMeleeDamage()
    {
        if(shortAttack.meleeDamage != null)
        {
            shortAttack.meleeDamage.DisableDamage();
        }
    }

    public void EnableMeleeDamage()
    {
        if (shortAttack.meleeDamage != null)
        {
            shortAttack.meleeDamage.EnableDamage();
        }
    }

    public void SetHorizontalSpeed(float speed){
        moveVector.x = speed * spriteForward.x;
    }

    public Transform CastDown;
    public LayerMask mask;
    /// The offset the hole detection should take into account
    public Vector2 HoleDetectionOffset = new Vector2(0, 0);
    public float RayOffset = 0.05f;
    public float obstacleHeightTolerance = 0.05f;
    public float HoleDetectionRaycastLength = 0.1f;
    public int numberHorizontalRaycast;
    public bool CheckForObstacle(float distance)
    {

       // SetHorizontalSpeed(movement.speed);

        bound = new Rect(boxCollider2D.bounds.min.x,
                         boxCollider2D.bounds.min.y,
                         boxCollider2D.bounds.size.x,
                         boxCollider2D.bounds.size.y);


        float horizontalRayLength = Mathf.Abs(distance * Time.deltaTime) + bound.width / 2 + RayOffset * 2;

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
        Vector2 raycastOrigin = (Vector2)transform.position +  (Vector2)spriteForward * boxCollider2D.bounds.extents.x + (Vector2)spriteForward * HoleDetectionOffset;

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


    int i = 1;
    private void OnSpawned(MyObjectPool.PrefabPool pool)
    {
        Debug.Log(i);

        i++;
    }

    // Update is ca   lled once per frame
    void FixedUpdate () {

        if (dead)
            return;

        moveVector.y = Mathf.Max(moveVector.y - movement.gravity * Time.deltaTime, -movement.gravity);

        playerController2D.Move(moveVector * Time.deltaTime);

        playerController2D.CheckCollisionDown();
            
        if (timeLastLostTarget > 0.0f)
            timeLastLostTarget -= Time.deltaTime;

        if (timeLostShootRate > 0.0f)
            timeLostShootRate -= Time.deltaTime;

    }

	public void Shoot(){

        if (targetVisible == null)
            return;

		Vector2 force = spriteForward.x > 0 ? Vector2.right.Rotate (longAttack.shootAngle) : Vector2.left.Rotate (longAttack.shootAngle);

		force *= longAttack.shootForce;

		Vector2 shootPos = longAttack.shootPos.localPosition;

		if ((spriteFacingLeft && spriteForward.x > 0) || (!spriteFacingLeft && spriteForward.x > 0))
			shootPos.x *= -1;
		// Quaternion rot = Quaternion.LookRotation (targetVisible.position);
		Transform obj = PoolManager.Instance.myObjectPools ["Bullets"].Spawn (longAttack.bullet.transform, longAttack.shootPos.TransformPoint(shootPos), Quaternion.identity, null );
        BulletData data = obj.gameObject.GetComponent<BulletData>();
        bool d = true;
        if (spriteForward.x > 0)
            d = false;         
        data.spriteRenderer.flipX = d ^ data.facingLeft;
        obj.gameObject.GetComponent<Rigidbody2D> ().velocity = GetProjectilVelocity (targetShootPosition,longAttack.shootPos.transform.position);
		
	}

    public void CheckShootTime()
    {

        if (targetVisible == null)
            return;

        if (timeLostShootRate > 0.0f)
            return;
       
        animator.SetTrigger(shootAnimation);

        timeLostShootRate = longAttack.shootRate;
    }

	public void RememberTargetPos() {
		if (targetVisible == null)
			return;
		targetShootPosition = targetVisible.transform.position;

	}

	private Vector3 GetProjectilVelocity(Vector3 _target, Vector3 origin)
	{
		const float projectileSpeed = 30.0f;

		Vector3 velocity = Vector3.zero;
		Vector3 toTarget = _target - origin;

		float gSquared = Physics.gravity.sqrMagnitude;
		float b = projectileSpeed * projectileSpeed + Vector3.Dot(toTarget, Physics.gravity);
		float discriminant = b * b - gSquared * toTarget.sqrMagnitude;

		// Check whether the target is reachable at max speed or less.
		if (discriminant < 0)
		{
			velocity = toTarget;
			velocity.y = 0;
			velocity.Normalize();
			velocity.y = 0.7f;

			velocity *= projectileSpeed;
			return velocity;
		}

		float discRoot = Mathf.Sqrt(discriminant);

		// Highest
		float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared);

		// Lowest speed arc
		float T_lowEnergy = Mathf.Sqrt(Mathf.Sqrt(toTarget.sqrMagnitude * 4f / gSquared));

		// Most direct with max speed
		float T_min = Mathf.Sqrt((b - discRoot) * 2f / gSquared);

		float T = 0;

		// 0 = highest, 1 = lowest, 2 = most direct
		int shotType = 1;

		switch (shotType)
		{
		case 0:
			T = T_max;
			break;
		case 1:
			T = T_lowEnergy;
			break;
		case 2:
			T = T_min;
			break;
		default:
			break;
		}

		velocity = toTarget / T - Physics.gravity * T / 2f;

		return velocity;
	}

    public void ScanTarget(bool run = true)
    {
		Vector3 dir = MainCharacterScript.Instance.transform.transform.position - transform.position;

        if (dir.sqrMagnitude > vision.distance * vision.distance)
            return;

        Vector3 testForward = Quaternion.Euler(0, 0, spriteFacingLeft ? Mathf.Sign(spriteForward.x) * -vision.direction : Mathf.Sign(spriteForward.x) * vision.direction) * spriteForward;
		// Debug.DrawRay(transform.position, testForward);
        float angle = Vector3.Angle(testForward, dir);    
        if (angle > vision.fov * 0.5f)
            return;

		targetVisible = MainCharacterScript.Instance.transform.transform;
		timeLastLostTarget = timer.timeLostTarget;
        if(run == true)
    		animator.SetTrigger (spotAnimation);
    }

    public void ScanTargetInLongAttackArea(bool run = true)
    {
        Vector3 dir = MainCharacterScript.Instance.transform.transform.position - transform.position;

        if (dir.sqrMagnitude > longAttack.distance * longAttack.distance)
            return;

        Vector3 testForward = Quaternion.Euler(0, 0, spriteFacingLeft ? Mathf.Sign(spriteForward.x) * -longAttack.direction : Mathf.Sign(spriteForward.x) * longAttack.direction) * spriteForward;
		// Debug.DrawRay(transform.position,testForward);
        float angle = Vector3.Angle(testForward, dir);
        if (angle > longAttack.fov * 0.5f)
            return;

		targetVisible = MainCharacterScript.Instance.transform.transform;
        timeLostShootRate = -0.1f;
		timeLastLostTarget = timer.timeLostTarget;
        if (run == true)
            animator.SetTrigger(spotAnimation);
    }

    public void CheckTargetStillVisible()
    {
        if (targetVisible == null)
            return;

        Vector3 dir = targetVisible.position - transform.position;
       
        if (dir.sqrMagnitude <= vision.distance * vision.distance)
        {
			Vector3 testForward = Quaternion.Euler(0, 0, spriteFacingLeft ? -vision.direction : vision.direction) * spriteForward;
			// Debug.DrawRay(transform.position, testForward);
	
			//if (spriteRenderer.flipX)
			//	testForward.x = -testForward.x;

			float angle = Vector3.Angle(testForward, dir);

			if (angle <= vision.fov * 0.5f)
				timeLastLostTarget = timer.timeLostTarget;
        }


        if (timeLastLostTarget <= 0.0f)
        {
            ForgetTarget();
        }
    }

    public void CheckTargetStillVisible_InLongArea()
    {
        if (targetVisible == null)
            return;
              
        Vector3 dir = targetVisible.position - transform.position;

        if (dir.sqrMagnitude <= longAttack.distance * longAttack.distance)
        {
            Vector3 testForward = Quaternion.Euler(0, 0, spriteFacingLeft ? -longAttack.direction : longAttack.direction) * spriteForward;
            //		Debug.DrawRay(transform.position, testForward);

           // if (spriteRenderer.flipX)
           //     testForward.x = -testForward.x;
            float angle = Vector3.Angle(testForward, dir);
            if (angle <= longAttack.fov * 0.5f)
            {
                timeLastLostTarget = timer.timeLostTarget;
            }
        }

        if (timeLastLostTarget <= 0.0f)
            ForgetTarget();
    }

    public void Face2Target()
    {
        if (targetVisible == null)
            return;

		Vector3 dir = targetVisible.position - transform.position;
        if (Vector2.Dot(dir, spriteForward) < 0)
        {     
            SetFacingData(Mathf.RoundToInt(-spriteForward.x));
        }

    }

    public void SetFacingData(int facing)
    {
        if (facing == -1)
        {
            spriteRenderer.flipX = !spriteFacingLeft;
            spriteForward = spriteFacingLeft ? Vector2.right : Vector2.left;
        }
        else if (facing == 1)
        {
            spriteRenderer.flipX = spriteFacingLeft;
            spriteForward = spriteFacingLeft ? Vector2.left : Vector2.right;
        }
    }

    public void ForgetTarget()
    {       
        animator.SetTrigger (lostAnimation);
        targetVisible = null;
        if (style == Style.Long)
        {
            timeLostShootRate = 0;
        }
        timeLastLostTarget = -1;        
    }

    public void Hit()
    {
        animator.SetTrigger(hitAnimation);
    }

    public void Die()
    {
        animator.SetTrigger(deadAnimation);

        dead = true;
    }

    public void LookAtTarget()
    {
        if (targetVisible == null)
            return;

        Vector3 dir = (targetVisible.position - transform.position).normalized;

        // left - right
        if (dir.x < 0 && spriteForward.x > 0)
        {
          //  SetFacingData(-1);
            spriteRenderer.flipX = !spriteFacingLeft;
            spriteForward.x *= -1;// = spriteFacingLeft ? Vector2.right : Vector2.left;
        } 
         if (dir.x > 0 && spriteForward.x < 0) // right - left
        {
            // SetFacingData(1);
            spriteRenderer.flipX = spriteFacingLeft;
            spriteForward.x *= -1;
        }

      //  Debug.Log("Dir: "+dir);
      //  Debug.Log("Sprite forward: "+spriteForward);
    }

    public void CheckMeleeAttack()
    {
        if (targetVisible == null)
            return;

        if((targetVisible.transform.position - transform.position).sqrMagnitude < shortAttack.distance * shortAttack.distance)
        {
            animator.SetTrigger(meleeAttackAnimation);
        }
    }

    float timeWaiting = 3,m_timeWaiting = 0;
    
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

    public void SamplePatroll()
    {
                  
        m_timeWaiting += Time.deltaTime;
        if(m_timeWaiting >= timeWaiting)
        {
            m_timeWaiting = 0; 
            if (spriteForward.x < 0)
            {
                spriteRenderer.flipX = spriteFacingLeft;
                spriteForward.x *= -1;
                //Vector3 t = CastDown.localPosition;
                //t.x *=-1;
                //CastDown.localPosition = t;
            }
            else
            {
                spriteRenderer.flipX = !spriteFacingLeft;
                spriteForward.x *= -1;
                //Vector3 t = CastDown.localPosition;
                //t.x *= -1;
                //CastDown.localPosition = t;
            }
        }
        ScanTarget();       
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

        //Draw short
        Handles.color = new Color(1.0f, 0, 0, 0.1f);
        Handles.DrawSolidDisc(transform.position, Vector3.back, shortAttack.distance);

    }
}
