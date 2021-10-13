using UnityEngine;

namespace UI
{
  public class SceneLoaderLoadBehaviour : StateMachineBehaviour
  {
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      animator.GetComponent<SceneLoader>().LoadScene();
    }
  }
}