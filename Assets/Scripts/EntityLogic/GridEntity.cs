using UnityEngine;
using Utils;
using World.Common;

namespace EntityLogic
{
  public class GridEntity : MonoBehaviour
  {
    public string entityName = "Unnamed";
    public GridPos GridPos => MapUtils.ToMapPos(transform.position);
    
    public virtual string GetTooltip()
    {
      return "this is an entity";
    }
  }
}