using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(Animator))]
public class ChimeraAI : MonoBehaviour {

	EnemyAI enemyAI;
	Animator animator;

	void Start(){
		enemyAI = GetComponent<EnemyAI> ();
		animator = GetComponent<Animator> ();
	}
			
	// Update is called once per frame
	void Update () {
		if (enemyAI.TargetVisible == null) {
			enemyAI.ScanTargetInLongAttackArea ();
		} else {
			enemyAI.CheckTargetStillVisible_InLongArea ();
			enemyAI.Face2Target ();
		//	if(enemyAI.timeLostFireRate <= 0.0f)
		//		animator.SetTrigger ("shoot");
			enemyAI.RememberTargetPos ();
		}         
	}
}
