using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
  [RequireComponent(typeof(Animator))]
  public class SceneLoader : MonoBehaviour
  {
    public string scene;

    private void Start()
    {
      DontDestroyOnLoad(gameObject);
    }

    public void LoadScene()
    {
      SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
  }
}