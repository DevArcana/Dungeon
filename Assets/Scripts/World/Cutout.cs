using TurnSystem;
using UnityEngine;

namespace World
{
  public class Cutout : MonoBehaviour
  {
    public Transform target;
    private Renderer _renderer;
    private static readonly int CutoutPos = Shader.PropertyToID("_CutoutPos");
    private static readonly int CutoutSize = Shader.PropertyToID("_CutoutSize");

    private void Start()
    {
      _renderer = GetComponent<Renderer>();
      TurnManager.instance.TurnChanged += OnTurnChanged;
    }

    private void OnTurnChanged(object sender, TurnManager.TurnEventArgs e)
    {
      target = e.Entity.transform;
    }

    private void OnDestroy()
    {
      if (TurnManager.instance)
      {
        TurnManager.instance.TurnChanged -= OnTurnChanged;
      }
    }

    private void Update()
    {
      var camera = UnityEngine.Camera.main;
      var material = _renderer.material;
      if (target != null)
      {
        var position = target.position;
        var pos = camera.WorldToViewportPoint(position);
        var mousePos = camera.ScreenToViewportPoint(Input.mousePosition);
        pos.x = mousePos.x;
        pos.y = mousePos.y;
        material.SetVector(CutoutPos, pos);
        material.SetFloat(CutoutSize, 0.15f);
      }
      else
      {
        material.SetFloat(CutoutSize, 0.0f);
      }
    }
  }
}