using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
  public class MainMenu : MonoBehaviour
  {
    // called by Unity UI
    // ReSharper disable once UnusedMember.Global
    public void Play()
    {
      var obj = (GameObject) Instantiate(Resources.Load("Level Loader"));
      var loader = obj.GetComponent<SceneLoader>();
      loader.scene = "TestScene";
      loader.hard = true;
    }
    
    // called by Unity UI
    // ReSharper disable once UnusedMember.Global
    public void Quit()
    {
      Application.Quit();
    }
  }
}