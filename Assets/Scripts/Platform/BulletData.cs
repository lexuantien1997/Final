using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletData : MonoBehaviour {

    public bool destroyWhenOutOfView;
	public float timeWillBeDestruction = 5;
    public bool facingLeft;

    public Transform explosion;

    private float time;


    public SpriteRenderer spriteRenderer;

    void OnEnable()
    {
		time = 0.0f;
	}
	
    public void ActiveExplosion()
    {
        PoolManager.Instance.myObjectPools["Bullet Explosion"].Spawn(explosion, transform.position, Quaternion.identity, null);
    }

    public void DespawnBulletExplosion2Pool()
    {
        PoolManager.Instance.myObjectPools["Bullet Explosion"].Despawn(explosion);
    }

    public void DespawnBullet2Pool()
    {
        PoolManager.Instance.myObjectPools["Bullets"].Despawn(transform);
    }

    private void OnBecameInvisible()
    {
        if(destroyWhenOutOfView == true)
        {
            DespawnBullet2Pool();
        }
    }


    // Update is called once per frame
    void FixedUpdate ()
    {
   
		if (timeWillBeDestruction > 0 && !destroyWhenOutOfView) {
			time += Time.deltaTime;
			if (time > timeWillBeDestruction) {
                DespawnBullet2Pool();
            }
		}

	}
}
