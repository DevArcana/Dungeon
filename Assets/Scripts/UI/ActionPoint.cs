using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  public enum ActionPointStatus
  {
    Spent,
    Reserved,
    Available
  }
  
  public class ActionPoint : MonoBehaviour
  {
    public Color spent;
    public Color reserved;
    public Color available;
    
    public Image image;

    public void SetStatus(ActionPointStatus status)
    {
      image.color = status switch
      {
        ActionPointStatus.Spent => spent,
        ActionPointStatus.Reserved => reserved,
        ActionPointStatus.Available => available,
        _ => image.color
      };
    }
  }
}