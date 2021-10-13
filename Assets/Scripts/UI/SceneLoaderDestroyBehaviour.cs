using UnityEngine;

namespace UI
{
  public class SceneLoaderDestroyBehaviour : StateMachineBehaviour
  {
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      Destroy(animator.gameObject);
    }
  }
}