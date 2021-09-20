using UnityEngine;

namespace Utils
{
  public class Lifetime : MonoBehaviour
  {
    public float time = 1.0f;

    private void Update()
    {
      time -= Time.deltaTime;

      if (time <= 0.0f)
      {
        Destroy(gameObject);
      }
    }
  }
}