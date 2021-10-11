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
      SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
  }
}