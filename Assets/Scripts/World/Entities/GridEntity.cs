using UnityEngine;
using Utils;
using World.Common;

namespace World.Entities
{
  public abstract class GridEntity : MonoBehaviour
  {
    public GridPos GridPos => MapUtils.ToMapPos(transform.position);
    public virtual EntityCollisionType CollisionType => EntityCollisionType.Solid;

    private void Start()
    {
      World.instance.Register(this);
    }
  }
}