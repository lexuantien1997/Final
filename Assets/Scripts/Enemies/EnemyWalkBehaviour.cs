using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalkBehaviour : StateMachineBehaviour {

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    //    animator.gameObject.GetComponent<EnemyAI>().LookAtTarget();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyAI enemyAI = animator.gameObject.GetComponent<EnemyAI>();

        if (enemyAI.CheckForObstacle(enemyAI.movement.speed))
            enemyAI.Flip();
        

        enemyAI.SetHorizontalSpeed(enemyAI.movement.speed);

        enemyAI.ScanTarget();
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        animator.gameObject.GetComponent<EnemyAI>().SetHorizontalSpeed(0);

    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
