using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

	[System.Serializable]
	public class _Path {

		[System.Serializable]
		public enum _PathType {
			Loop,
			Back
		}

		public Transform[] points;

		public bool allowStart;
		public _PathType pathType;
		public float[] waitTimes;

		[HideInInspector]
		public int current,next;
		[HideInInspector]
		public float waittime,dir;
		[HideInInspector]
		public Vector3 moveDir;
		private Transform transform;
		public void DebugDrawLines(){
			for (int i = 1; i < points.Length; i++) {
				var p0 = points [i - 1].position;
				var p1 = points [i].position;
				Gizmos.DrawLine (p0, p1);
			}
		}

		public void Create(Transform trans){
			dir = -1;
			current = 0;
			next = 1;
			waittime = waitTimes[0];
			moveDir = (points[next].position - points[current].position).normalized;
			transform = trans;
		}

		public void SetNewData(){
			current = next;
			waittime = waitTimes [current];				
		}

		public bool CanMove() {
			if (!allowStart)
				return false;

			if (current == next)
				return false;

			if (waittime > 0) {
				waittime -= Time.deltaTime;
				return false;
			}

			return true;
		}

		public bool NeedNextPoint(){
			var current2A = points [current].position - transform.position;
			var A2B = points [current].position - points [next].position;
			if(current2A.sqrMagnitude >= A2B.sqrMagnitude)
				return true;
			return false;
		}

		public void GetNextPoint() {
			if (dir > 0) {
				next += 1;
				if (next >= points.Length) {
					switch (pathType) {
					case _PathType.Loop:
						next = 0;
						break;
					case _PathType.Back:
						next = points.Length - 2;
						dir = -1;
						break;
					}
				}
			} else {
				next -= 1;
				if (next < 0) {
					switch (pathType) {
					case _PathType.Loop:
						next = points.Length-1;
						break;
					case _PathType.Back:
						next = 1;
						dir = 1;
						break;
					}
				} 
			}

			moveDir = (points [next].position - points [current].position).normalized;

		}
	}

	[System.Serializable]
	public enum _EnemyMovementType {
		Specific,
		Terrain
	}

	public _EnemyMovementType enemyMovementType;
	public _Path path;
	//[HideInInspector]
	public float speed;
	public bool spriteFacingLeft;
	[HideInInspector]
	public bool stopMoving;
	private Transform myTranform;

	public void Move(){
		if (enemyMovementType == _EnemyMovementType.Specific) {
			transform.position += path.moveDir * speed * Time.deltaTime;
		}
	}

	void Awake(){
		myTranform = transform;
		stopMoving = false;
		if (enemyMovementType == _EnemyMovementType.Specific) {
			path.Create (transform);	
		//	myTranform.position = path.points[0].position;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (enemyMovementType == _EnemyMovementType.Specific && path.CanMove()) {
			if (stopMoving)
				return;
			if (path.NeedNextPoint () == true) {
				path.SetNewData ();
				path.GetNextPoint ();
			} else {				
				Move ();
				flip ();
			//	Debug.LogWarning (path.moveDir);
			}
		}
	}

	private void flip(){
		if (enemyMovementType == _EnemyMovementType.Specific) {
			if (path.moveDir.x > 0 &&  spriteFacingLeft || path.moveDir.x<0 && !spriteFacingLeft) {
				spriteFacingLeft = !spriteFacingLeft;
				var x =	myTranform.localScale;
				x.x *= -1;
				myTranform.localScale = x;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (enemyMovementType == _EnemyMovementType.Specific)
			path.DebugDrawLines ();
	}
}
