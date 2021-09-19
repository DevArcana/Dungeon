using UnityEngine;

namespace Utils
{
  public class Billboard : MonoBehaviour
  {
    private UnityEngine.Camera _camera;

    private void Start()
    {
      _camera = UnityEngine.Camera.main;
    }

    private void Update()
    {
      var rotation = _camera.transform.rotation;
      transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }
  }
}