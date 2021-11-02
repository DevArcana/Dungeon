using UnityEngine;
using Utils;
using World.Common;

namespace EntityLogic
{
  public class GridEntity : MonoBehaviour
  {
    public GridPos GridPos => MapUtils.ToMapPos(transform.position);
  }
}