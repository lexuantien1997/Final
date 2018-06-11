using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefinePath : MonoBehaviour {

	[System.Serializable]
	public enum PathType {
		Loop,
		Back
	}

	public Transform[] points;

	public bool allowStart;
	public PathType pathType;
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
		dir = -1; // default right 2 left
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
				case PathType.Loop:
					next = 0;
					break;
				case PathType.Back:
					next = points.Length - 2;
					dir = -1;
					break;
				}
			}
		} else {
			next -= 1;
			if (next < 0) {
				switch (pathType) {
				case PathType.Loop:
					next = points.Length-1;
					break;
				case PathType.Back:
					next = 1;
					dir = 1;
					break;
				}
			} 
		}

		moveDir = (points [next].position - points [current].position).normalized;

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
