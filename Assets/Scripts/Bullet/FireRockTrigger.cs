using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRockTrigger : MonoBehaviour {

    public float triggerTime = 2;

    private float _triggerTime;
    Animator animator;
	// Use this for initialization
	void Start () {
        _triggerTime = 0;
        animator = GetComponent<Animator>();
	}

    private void OnEnable()
    {
        _triggerTime = 0;
        
    }

    // Update is called once per frame
    void Update () {

        if(triggerTime > 0.0f)
        {
            _triggerTime += Time.deltaTime;
            if(_triggerTime >=triggerTime)
            {
                animator.SetTrigger("FireRockExplosion");
                _triggerTime = -1;
            }
        }	

	}
}
