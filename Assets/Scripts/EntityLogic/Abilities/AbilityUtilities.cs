using TurnSystem;

namespace EntityLogic.Abilities
{
  public static class AbilityUtilities
  {
    public static bool AreEnemies(GridLivingEntity first, GridLivingEntity second)
    {
      return first is EnemyEntity && second is PlayerEntity || first is PlayerEntity && second is EnemyEntity;
    }
  }
}