using UnityEngine;
using Utils;
using World.Common;

namespace EntityLogic
{
  public abstract class GridEntity : MonoBehaviour
  {
    public GridPos GridPos => MapUtils.ToMapPos(transform.position);

    private void Start()
    {
      World.World.instance.Register(this);
    }
  }
}