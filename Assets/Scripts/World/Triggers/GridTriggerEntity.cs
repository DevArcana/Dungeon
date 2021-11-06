using EntityLogic;

namespace World.Triggers
{
  public abstract class GridTriggerEntity : GridEntity
  {
    public abstract void OnTileEntered(GridLivingEntity entity);
  }
}