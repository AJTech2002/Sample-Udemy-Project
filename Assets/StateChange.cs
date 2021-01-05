using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCStates
{
    Patrol,
    Chase,
    Search,
    Death
}

public class StateChange : StateMachineBehaviour
{
    public NPCStates state;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Tell the NPC Controller
        Debug.Log("Entered : " + state.ToString());

        if (state == NPCStates.Chase)
        {
            animator.SetFloat("curiosity", 1);
        }

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (state == NPCStates.Patrol)
        {
            if (animator.GetBool("heardSound"))
            {
                animator.SetFloat("curiosity", Mathf.Clamp(animator.GetFloat("curiosity") +0.4f, 0, 0.7f) );
            }
        }

        if (state == NPCStates.Search)
        {
            animator.SetFloat("curiosity", animator.GetFloat("curiosity") - Time.deltaTime*0.1f);
        }
    }
}
