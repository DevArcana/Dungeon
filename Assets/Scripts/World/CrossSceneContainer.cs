using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace World
{
  public class CrossSceneContainer : MonoBehaviour
  {
    public static CrossSceneContainer instance;
    
    private void Awake()
    {
      if (instance == null)
      {
        instance = this;
        DontDestroyOnLoad(gameObject);
      }
      else
      {
        Destroy(gameObject);
        return;
      }
      
      SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
      currentFloor.CurrentValue++;
    }

    /// <summary>
    /// Reactive variable showing current floor of the dungeon.
    /// </summary>
    public ReactiveVariable<int> currentFloor = new ReactiveVariable<int>();
  }
}