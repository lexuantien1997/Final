using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerAI : MonoBehaviour {

	[System.Serializable]
	public enum EnemyControllerType {
		Terrain,
		Specific
	}

	private DefinePath path;
	public EnemyControllerType enemyControllerType;

	private float speed;


	public void Move(float Speed) {
		if (enemyControllerType == EnemyControllerType.Specific) {
			transform.position += path.moveDir * Speed * Time.deltaTime;
		}
	}



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnDrawGizmosSelected()
	{
		if (enemyControllerType == EnemyControllerType.Specific)
			path.DebugDrawLines ();
	}
}
