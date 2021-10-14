using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
  [RequireComponent(typeof(Animator))]
  public class SceneLoader : MonoBehaviour
  {
    public string scene;
    public bool hard = false;

    private void Start()
    {
      DontDestroyOnLoad(gameObject);
    }

    public void LoadScene()
    {
      if (hard && World.World.instance != null)
      {
        Destroy(World.World.instance.gameObject);
      }
      
      SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
  }
}