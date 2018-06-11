using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layka : MonoBehaviour {

    EnemyAI enemyAI;
    HealthSystem healthSystem;

    public Transform fireRockGO;

    public float fireRockNumber = 5;
    public float fireRockLength = 10;
    public float timeUseFireRock = 5;

    private float _timeUseFireRock = 0;
    private Animator animator;
    
    
    private int attackFireRockAnimation = Animator.StringToHash("attack_fire_rock");

    // Use this for initialization
    void Start () {
        enemyAI = GetComponent<EnemyAI>();
        healthSystem = GetComponent<HealthSystem>();
        animator = GetComponent<Animator>();
	}


    void ShootFireRock()
    {
        for (int i = 0; i < fireRockNumber; i++)
        {          
            // random fire rock:
            Vector3 position = transform.position + (Vector3)enemyAI.SpriteForward * Random.Range(0, fireRockLength);
            PoolManager.Instance.myObjectPools["Bullets Pool"].Spawn(fireRockGO, position, Quaternion.identity, null);
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        float speed = enemyAI.movement.speed;
        if (enemyAI.CheckForObstacle(speed) == true)
            enemyAI.Flip();
        enemyAI.SetHorizontalSpeed(speed);


        //if(enemyAI.TargetVisible == null)
        //{

        //    float speed = enemyAI.movement.speed;
        //    if (enemyAI.CheckForObstacle(speed) == true)
        //        enemyAI.Flip();
        //    enemyAI.SetHorizontalSpeed(speed);

        //    enemyAI.ScanTargetInLongAttackArea();

        //} else
        //{
        //    enemyAI.CheckTargetStillVisible_InLongArea();

        //    enemyAI.LookAtTarget();

        //    enemyAI.CheckShootTime();

        //    enemyAI.RememberTargetPos();

        //}

        //// Enemy continue patrolling until player in vision:
        //// collide -> update facing:
        //float speed = enemyAI.movement.speed;
        //if(enemyAI.CheckForObstacle(speed) == true)
        //{
        //    enemyAI.Flip();
        //}

        //enemyAI.SetHorizontalSpeed(speed);

        //enemyAI.ScanTarget(false);

        //// found target:
        //if(enemyAI.TargetVisible == true) // random 
        //{
        //    _timeUseFireRock += Time.deltaTime;
        //    // use fire rock skill:
        //    if (_timeUseFireRock >= timeUseFireRock)
        //    {
        //        animator.SetTrigger(attackFireRockAnimation);
        //        _timeUseFireRock = 0;

        //        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Layka_Attack"))
        //        {
        //            animator.SetTrigger("running");
        //            enemyAI.TargetVisible = null;
        //        }
        //    }
        //}
    }

    public void Despawned()
    {
        var existInPool = PoolManager.Instance.myObjectPools["Enemies"].Despawn(transform);
        if (!existInPool)
            Destroy(gameObject);
    }
}
