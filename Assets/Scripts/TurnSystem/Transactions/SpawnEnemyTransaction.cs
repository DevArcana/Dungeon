using EntityLogic;
using UnityEngine;
using Utils;
using World.Common;

namespace TurnSystem.Transactions
{
  public class SpawnEnemyTransaction : TransactionBase
  {
    private readonly GridLivingEntity _enemyPrefab;
    private readonly GridPos _pos;

    public SpawnEnemyTransaction(GridLivingEntity enemyPrefab, GridPos pos)
    {
      _enemyPrefab = enemyPrefab;
      _pos = pos;
    }

    protected override void Start()
    {
      if (World.World.instance.IsOccupied(_pos))
      {
        return;
      }

      var position = MapUtils.ToWorldPos(_pos);
      var enemy = Object.Instantiate(_enemyPrefab, position, Quaternion.identity);
      TurnManager.instance.RegisterTurnBasedEntity(enemy);
    }
  }
}