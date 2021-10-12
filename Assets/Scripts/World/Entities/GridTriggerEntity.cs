namespace World.Entities
{
  public class GridTriggerEntity : GridEntity
  {
    public override EntityCollisionType CollisionType => EntityCollisionType.Trigger;
  }
}