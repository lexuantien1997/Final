using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainCharacterScript : MonoBehaviour
{

    private static MainCharacterScript instance;

    public static MainCharacterScript Instance
    {
        get
        {
            return instance;
        }
    }

    public float maxHSpeed = 5;
    public float hSpeed = 50f;
    public float vSpeed = 250;
    public int bullet = 0;
    public int bullet2 = 0;
    public int life = 1;

    private bool grounded;

    private bool isFacingRight;

    private bool sliding;
    private float slidetime;
    private float maxSlideTime = 0.3f;
    private float slideCoolDownTime;
    private float slideCoolDown = 1f;

    private bool attackMelee;
    private float attackMeleeCoolDownTime;
    private float attackMeleeCoolDown = 0.5f;

    private bool shield;
    private float shieldCoolDownTime;
    private float shieldCoolDown = 30f;

    public bool knockback;
    private float knockbackCoolDownTime;
    private float knockbackCoolDown = 0.5f;

    private bool crouch;

    private bool die;

    public bool shoot;
    public float shootCoolDownTime;
    public float shootCooldown = 0.5f;
    public bool shoot2;
    public float shoot2CoolDownTime;
    public float shoot2Cooldown = 0.5f;
    public bool gun1;
    public bool gun2;
    public bool hasgun1;
    public bool hasgun2;


    private Rigidbody2D rigidbody2D;
    private Animator animator;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask whatIsGround;

    public Transform projectile;
    public Transform projectile2;
    public Vector2 velocity;
    public Transform shotpos;

    private DamageSystem damageSystem;
    private HealthSystem healthSystem;

    public UnityEvent OnRevive;

    // Use this for initialization
    void Start()
    {
        instance = this;
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        isFacingRight = true;

        sliding = false;
        animator.SetBool("Sliding", sliding);
        slidetime = maxSlideTime;
        slideCoolDownTime = slideCoolDown;

        attackMelee = false;
        animator.SetBool("Attack_Melee", attackMelee);
        attackMeleeCoolDownTime = attackMeleeCoolDown;

        shield = false;
        animator.SetBool("Shield", shield);
        shieldCoolDownTime = shieldCoolDown;

        crouch = false;
        animator.SetBool("Crouch", crouch);

        shoot = false;
        gun1 = false;
        hasgun1 = false;
        animator.SetBool("Gun1", gun1);

        shoot2 = false;
        gun2 = false;
        hasgun2 = false;
        animator.SetBool("Gun2", gun2);

        die = false;
        animator.SetBool("Die", die);

        knockback = false;
        knockbackCoolDownTime = 0.5f;

        damageSystem = GetComponent<DamageSystem>();
        damageSystem.DisableDamage();

        healthSystem = GetComponent<HealthSystem>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Gun1")
        {
            hasgun1 = true;
            //gun1 = true;
            //animator.SetBool("Gun1", gun1);
            hasgun2 = true;
            gun2 = true;
            animator.SetBool("Gun2", gun2);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "BulletPack")
        {
            bullet = 10;
            bullet2 = 10;
            Destroy(collision.gameObject);
        }
    }

    // Update is called once per frame --> Moving Non-Physics objects; Simple timers; Receiving input
    void Update()
    {
        HandleInput();
    }

    // Update is called every physics step --> Rigidbody objects
    private void FixedUpdate()
    {
        float hInput = Input.GetAxisRaw("Horizontal");

        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        animator.SetBool("Grounded", grounded);

        HandleMoveMent(hInput);

        Flip(hInput);

    }

    private void Flip(float Horizontal)
    {

        if (((Horizontal > 0 && !isFacingRight) || (Horizontal < 0 && isFacingRight)) && (!sliding && !attackMelee && !shield && !die && !crouch && !knockback))
        {
            isFacingRight = !isFacingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    private void HandleMoveMent(float Horizontal)
    {
        if (!attackMelee && !shield && !sliding && !die && !crouch && !knockback)
        {
            rigidbody2D.velocity = new Vector2(Horizontal * hSpeed, rigidbody2D.velocity.y);
            animator.SetFloat("hSpeed", Mathf.Abs(Input.GetAxis("Horizontal")));
        }
        if (rigidbody2D.velocity.x > maxHSpeed && !sliding && !attackMelee && !shield && !die && !crouch && !knockback)
        {
            rigidbody2D.velocity = new Vector2(maxHSpeed, rigidbody2D.velocity.y);
        }
        if (rigidbody2D.velocity.x < -maxHSpeed && !sliding && !attackMelee && !shield && !die && !crouch && !knockback)
        {
            rigidbody2D.velocity = new Vector2(-maxHSpeed, rigidbody2D.velocity.y);
        }

        if (sliding)
        {
            if (!isFacingRight) rigidbody2D.velocity = new Vector2(-15, rigidbody2D.velocity.y);
            else rigidbody2D.velocity = new Vector2(15, rigidbody2D.velocity.y);
        }
        else
        {
            animator.SetBool("Sliding", sliding);
        }
        if (slidetime < maxSlideTime)
        {
            slidetime += Time.deltaTime;
        }
        else sliding = false;
        if (slideCoolDownTime < slideCoolDown)
        {
            slideCoolDownTime += Time.deltaTime;
        }

        if (attackMelee)
        {
            rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
        }
        else
        {
            animator.SetBool("Attack_Melee", attackMelee);
        }
        if (attackMeleeCoolDownTime < attackMeleeCoolDown)
        {
            attackMeleeCoolDownTime += Time.deltaTime;
        }
        else attackMelee = false;

        if (shield)
        {
            rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);

        }
        else
        {
            if (shieldCoolDownTime < shieldCoolDown) shieldCoolDownTime += Time.deltaTime;
        }

        if (crouch)
        {
            rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
            grounded = true;
        }

        if (shootCoolDownTime < shootCooldown)
        {
            shootCoolDownTime += Time.deltaTime;
        }
        else shoot = false;

        if (shoot2CoolDownTime < shoot2Cooldown)
        {
            shoot2CoolDownTime += Time.deltaTime;
        }
        else shoot2 = false;

        if (die)
        {
            animator.SetBool("Die", die);
            rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
        }


        if (knockback)
        {
            if (isFacingRight)
            {
                rigidbody2D.velocity = new Vector2(-5, rigidbody2D.velocity.y);
            }
            else
            {
                rigidbody2D.velocity = new Vector2(5, rigidbody2D.velocity.y);
            }
        }
        if (knockbackCoolDownTime < knockbackCoolDown)
        {
            knockbackCoolDownTime += Time.deltaTime;
        }
        else
        {
            knockback = false;
            animator.SetBool("KnockBack", knockback);
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded && !sliding && !attackMelee && !shield && !crouch && !die && !knockback)
        {
            rigidbody2D.AddForce(Vector2.up * vSpeed);
            grounded = false;
        }

        if (Input.GetKeyDown(KeyCode.Z) && !sliding && slidetime >= maxSlideTime && slideCoolDownTime >= slideCoolDown && grounded && !attackMelee && !shield && !crouch && !die && !knockback)
        {
            sliding = true;
            animator.SetBool("Sliding", sliding);
            slidetime = 0;
            slideCoolDownTime = 0;
        }

        if (Input.GetKeyDown(KeyCode.C) && !attackMelee && attackMeleeCoolDownTime >= attackMeleeCoolDown && grounded && !sliding && !shield && !crouch && !die && !knockback)
        {
            attackMelee = true;
            animator.SetBool("Attack_Melee", attackMelee);
            attackMeleeCoolDownTime = 0;
        }

        if (Input.GetKeyDown(KeyCode.X) && !shield && shieldCoolDownTime >= shieldCoolDown && grounded && !sliding && !attackMelee && !crouch && !die && !knockback)
        {
            shield = true;
            animator.SetBool("Shield", shield);
            healthSystem.invulnerable = true;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.X) && shield && grounded && !sliding && !attackMelee && !crouch && !die && !knockback)
            {
                shield = false;
                animator.SetBool("Shield", shield);
                shieldCoolDownTime = 0;
                healthSystem.invulnerable = false;
            }
        }

        if (Input.GetKey(KeyCode.DownArrow) && !shield && !sliding && !attackMelee && !die && !knockback)
        {
            crouch = true;
            animator.SetBool("Crouch", crouch);
        }
        else
        {
            crouch = false;
            animator.SetBool("Crouch", crouch);
        }

        if (Input.GetKeyDown(KeyCode.V) && !shoot && shootCoolDownTime >= shootCooldown && bullet >= 1 && hasgun1 && gun1 && !gun2 && !shield && !sliding && !crouch && !attackMelee && !die && !knockback)
        {
            shoot = true;
            shootCoolDownTime = 0;
            bullet -= 1;

            if (isFacingRight)
            {
                var a = PoolManager.Instance.myObjectPools["Bullets"].Spawn(projectile, (Vector3)shotpos.position, Quaternion.identity, null);

                a.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(8 * transform.localScale.x, 0);
            }
            else
            {
                var a = PoolManager.Instance.myObjectPools["Bullets"].Spawn(projectile, (Vector3)shotpos.position, Quaternion.identity, null);

                Vector3 bulletRotation = a.gameObject.transform.localScale;
                bulletRotation.x *= -1;
                a.gameObject.transform.localScale = bulletRotation;

                a.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(8 * transform.localScale.x, 0);
            }

        }

        if (Input.GetKeyDown(KeyCode.V) && !shoot2 && shoot2CoolDownTime >= shoot2Cooldown && bullet2 >= 1 && hasgun2 && gun2 && !gun1 && !shield && !sliding && !crouch && !attackMelee && !die && !knockback)
        {
            shoot2 = true;
            shoot2CoolDownTime = 0;
            bullet2 -= 1;

            if (isFacingRight)
            {
                var a = PoolManager.Instance.myObjectPools["Bullets"].Spawn(projectile2, (Vector3)shotpos.position, Quaternion.identity, null);

                a.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(8 * transform.localScale.x, 0);
            }
            else
            {
                var a = PoolManager.Instance.myObjectPools["Bullets"].Spawn(projectile2, (Vector3)shotpos.position, Quaternion.identity, null);

                Vector3 bulletRotation = a.gameObject.transform.localScale;
                bulletRotation.x *= -1;
                a.gameObject.transform.localScale = bulletRotation;

                a.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(8 * transform.localScale.x, 0);
            }

        }

        if (Input.GetKeyDown(KeyCode.B) && die && life > 0 && !shield && !sliding && !attackMelee && !crouch && !knockback)
        {
            die = false;
            animator.SetBool("Die", die);
            life -= 1;
            healthSystem.currentHealth = healthSystem.initHealth;
            OnRevive.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.A) && hasgun1 && !gun1 && gun2 && !sliding && !attackMelee && !shield && !crouch && !die && !knockback && !shoot && !shoot2)
        {
            gun1 = true;
            gun2 = false;
            animator.SetBool("Gun1", gun1);
            animator.SetBool("Gun2", gun2);
        }

        if (Input.GetKeyDown(KeyCode.S) && hasgun2 && !gun2 && gun1 && !sliding && !attackMelee && !shield && !crouch && !die && !knockback && !shoot && !shoot2)
        {
            gun2 = true;
            gun1 = false;
            animator.SetBool("Gun2", gun2);
            animator.SetBool("Gun1", gun1);
        }
    }

    public void Die()
    {
        die = true;
    }

    public void OnHit()
    {
        if (!knockback && knockbackCoolDownTime >= knockbackCoolDown)
        {
            knockback = true;
            knockbackCoolDownTime = 0;
            animator.SetBool("KnockBack", knockback);
        }
        rigidbody2D.AddForce(Vector2.up * vSpeed);
    }

    public void Revive()
    {

    }

    public void EnableMeleeAttack()
    {
        damageSystem.CanDamage = true;
        damageSystem.DisableAfterHit = true;
    }

    public void DisableMeleeAttack()
    {
        damageSystem.CanDamage = false;
    }
}