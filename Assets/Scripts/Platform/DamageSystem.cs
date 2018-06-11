using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class DamageSystem : MonoBehaviour
{

    public float damage;
    public float damageTakenNonDamageable = 0;
    public float flickerDuration;
    public Vector2 damageCausedKnockbackForce = new Vector2(10, 2);
    public DamageBox damageBox;
    public LayerMask mask;
    ContactFilter2D filter;
    Collider2D[] results = new Collider2D[10];
    Collider2D lastHit;

    public bool CanDamage { get; set; }
    public bool DisableAfterHit { get; set; }
   // private HealthSystem healthSystem;
    protected Vector2 lastPosition, velocity;
 
    public UnityEvent OnDamageHit, OnNonDamageHit;

    private void Awake()
    {
        CanDamage = true;
        DisableAfterHit = false;
    }

    private void OnEnable()
    {
        CanDamage = true;
        DisableAfterHit = false;
    }

    // Use this for initialization
    void Start()
    {
        filter.layerMask = mask;
        filter.useLayerMask = true;
        filter.useTriggers = false;

        //     Physics2D.queriesHitTriggers = false;
        //     healthSystem = GetComponent<HealthSystem>();
    }


    private void ComputeVelocity()
    {
        velocity = (lastPosition - (Vector2)transform.position) / Time.deltaTime;
        lastPosition = transform.position;

    }

    //public void SelfDamage(float dam)
    //{
    //    if(healthSystem != null)
    //    {
    //        healthSystem.TakeDamage(dam, flickerDuration);
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        // Xử lý đánh xa hoặc đánh gần:
        if (!CanDamage)
            return;
        
        ComputeVelocity();
        Vector2 topLeft = transform.TransformPoint(new Vector2(damageBox.Left, damageBox.Top));
        Vector2 btmRight = transform.TransformPoint(new Vector2(damageBox.Right, damageBox.Btm));

        int hit = Physics2D.OverlapArea(topLeft, btmRight, filter, results);

        for (int i = 0; i < hit; i++)
        {
            lastHit = results[i];
            HealthSystem hsys = lastHit.GetComponent<HealthSystem>();

            // collide with damage:
            if (hsys != null)
            {
                 // Xử lý vài cái như bullet va chạm với nhân vật:
                OnDamageHit.Invoke();
                if (DisableAfterHit == true)
                    CanDamage = false;
                // Nhận damage
                hsys.TakeDamage(damage, flickerDuration);
              //  CanDamage = false;
            }
            else // collide with non damage:
            {
                // Xử lý vài cái như bullet va chạm với platform:
                OnNonDamageHit.Invoke();
              //  CanDamage = false;
                // SelfDamage(damageTakenNonDamageable);
            }       
        }

        //    Debug.Log(hit);
        

    }

    public void EnableDamage()
    {
        CanDamage = true;
    }

    public void DisableDamage()
    {
        CanDamage = false;
    }

}
