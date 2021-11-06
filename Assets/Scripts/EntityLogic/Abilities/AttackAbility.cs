using System.Collections.Generic;
using TurnSystem;
using TurnSystem.Transactions;
using World.Common;

namespace EntityLogic.Abilities
{
  public class AttackAbility : IAbility
  {
    public int damage = 10;
    
    public IEnumerable<GridPos> GetValidTargetPositions()
    {
      var entity = TurnManager.instance.CurrentTurnTaker;
      var tiles = new List<GridPos>();
      for (var y = entity.GridPos.y - 1; y <= entity.GridPos.y + 1; y++)
      {
        for (var x = entity.GridPos.x - 1; x <= entity.GridPos.x + 1; x++)
        {
          var pos = GridPos.At(x, y);
          var occupant = World.World.instance.GetOccupant(pos);
          if (occupant is PlayerEntity ? entity is EnemyEntity : entity is PlayerEntity)
          {
            tiles.Add(occupant.GridPos);
          }
        }
      }

      return tiles;
    }

    public IEnumerable<GridPos> GetEffectiveRange(GridPos pos)
    {
      return new []{pos};
    }

    public int GetEffectiveCost(GridPos pos)
    {
      return 2;
    }

    public void Execute(GridPos pos)
    {
      var enemy = World.World.instance.GetOccupant(pos);
      TurnManager.instance.Transactions.EnqueueTransaction(new AttackTransaction(TurnManager.instance.CurrentTurnTaker, enemy, damage));
    }
  }
}