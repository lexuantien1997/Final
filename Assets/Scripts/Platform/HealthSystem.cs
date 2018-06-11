using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{

    public float initHealth;
    public float maxHealth;

    public bool invulnerable = false;

   // public Transform damageEffect;
   // public AudioClip damageAudio;
    public bool flickerOnHit = true;

   // public Transform deadEffect;
   // public AudioClip deadAudio;
    public Vector2 deadForce;

    public float invincibilityDuration;

    // unity event:
    // public UnityEvent deadEvent,despawnEvent;

    public float currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color initialColor;
    private Color flickerColor = new Color32(255, 20, 20, 255);

    private Transform myTransform;

     private void OnEnable()
    {
        myTransform = transform;

        currentHealth = initHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        DamageEnabled();
        if (spriteRenderer != null)
        {
            if (spriteRenderer.material.HasProperty("_Color"))
            {
                initialColor = spriteRenderer.material.color;
            }
        }
    }


    public UnityEvent OnTakeDamage;
    public UnityEvent OnHealthGUI;
    public UnityEvent OnDie;

    public void TakeDamage(float damage, float flickerDuration)
    {


        if (currentHealth <= 0 && initHealth > 0)
            return;

        // Có đang bất tử hay ko:
        if (invulnerable)
            return;
        else
        {
            // set damage
            currentHealth -= damage;
            // Thay đổi thanh máu:
            OnHealthGUI.Invoke();
        }       

        // Bất tử:
        DamageDisabled();
        StartCoroutine(DamageEnabled(invincibilityDuration));

        // Xử lý vài thứ khi va chạm như: chuyển animation
        OnTakeDamage.Invoke();

        
        // PlayHitSfx();
        // active damage effect:
        //ActiveDamageEffect();        
        // Nhấp nháy:
        ActiveFlickering(flickerDuration);

        if (currentHealth <= 0)
        {
            // we set its health to zero (useful for the healthbar)
            currentHealth = 0;

            // we prevent further damage
            DamageDisabled();

            // Thực hiện một vài thứ trước khi chết: như chuyển trạng thái animation, return vào pool
            OnDie.Invoke();

            // Kill();
        }

       
     
    }

    /// <summary>
    /// Kills the character, vibrates the device, instantiates death effects, handles points, etc
    /// </summary>
    public  void Kill()
    {
  
        // we prevent further damage
        DamageDisabled();

        // Thực hiện một vài thứ trước khi chết: như chuyển trạng thái animation, return vào pool
        OnDie.Invoke();

        //ActiveDeadEffect();

        // PlayDeadSfx();

        // add dead force:

        // destroy obj
       // despawnEvent.Invoke();
       
    }

    public void PlayHitSfx()
    {
       
        //if (damageAudio != null)
        //{

        //}
    }

    public void PlayDeadSfx()
    {
        //if (deadAudio != null)
        //{

        //}
    }

    private void ActiveFlickering(float flickerDuration)
    {
        if (flickerOnHit)
        {
            // We make the character's sprite flicker
            if (spriteRenderer != null)
            {
                StartCoroutine(Flicker(0.05f, flickerDuration));
            }
        }
    }

    public void ActiveDamageEffect()
    {
        //if (damageEffect != null)
        //{
        //    PoolManager.Instance.myObjectPools["Damage Effect"].Spawn(damageEffect.transform, myTransform.position, Quaternion.identity, null);
        //}
        //else // run animation damage effect:
        //{

        //}
    }

    public void ActiveDeadEffect()
    {
        //if (deadEffect != null)
        //{
        //    PoolManager.Instance.myObjectPools["Dead Effect"].Spawn(deadEffect.transform, myTransform.position, Quaternion.identity, null);
        //}
        //else // run animation dead effect:
        //{

        //}
    }

    /// <summary>
    /// Prevents the character from taking any damage
    /// </summary>
    public  void DamageDisabled()
    {
        invulnerable = true;
    }

    /// <summary>
    /// Allows the character to take damage
    /// </summary>
    public  void DamageEnabled()
    {
        invulnerable = false;
    }

    /// <summary>
    /// makes the character able to take damage again after the specified delay
    /// </summary>
    /// <returns>The layer collision.</returns>
    public  IEnumerator DamageEnabled(float delay)
    {
        yield return new WaitForSeconds(delay);
        invulnerable = false;
      //  GetComponent<DamageSystem>().EnableDamage();
    }

    /// <summary>
    /// Coroutine used to make the character's sprite flicker (when hurt for example).
    /// </summary>
    public IEnumerator Flicker(float flickerSpeed, float flickerDuration)
    {
        if (spriteRenderer == null)
        {
            yield break;
        }

        if (!spriteRenderer.material.HasProperty("_Color"))
        {
            yield break;
        }

        if (initialColor == flickerColor)
        {
            yield break;
        }

        float flickerStop = Time.time + flickerDuration;

        while (Time.time < flickerStop)
        {
            spriteRenderer.material.color = flickerColor;
            yield return new WaitForSeconds(flickerSpeed);
            spriteRenderer.material.color = initialColor;
            yield return new WaitForSeconds(flickerSpeed);
        }

        spriteRenderer.material.color = initialColor;
    }

}
