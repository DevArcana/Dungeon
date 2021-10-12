using UnityEngine;
using Utils;
using World.Common;

namespace World.Entities
{
  public abstract class GridEntity : MonoBehaviour
  {
    public GridPos GridPos => MapUtils.ToMapPos(transform.position);

    private void Start()
    {
      World.instance.Register(this);
    }
  }
}