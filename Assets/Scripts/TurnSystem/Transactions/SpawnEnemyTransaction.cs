using EntityLogic;
using UnityEngine;
using Utils;
using World;
using World.Common;

namespace TurnSystem.Transactions
{
  public class SpawnEnemyTransaction : TransactionBase
  {
    private readonly GridLivingEntity _enemyPrefab;
    private readonly GridPos _pos;

    public SpawnEnemyTransaction(GridLivingEntity enemyPrefab, GridPos pos, bool isAbility) : base(isAbility)
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
      var statIncrement = 0.1f * (CrossSceneContainer.instance.currentFloor.CurrentValue - 1) + 1;
      enemy.baseAttributes.maximumHealth = Mathf.Floor(statIncrement * enemy.baseAttributes.maximumHealth);
      enemy.health.SetHealth(enemy.baseAttributes.maximumHealth);
      enemy.baseAttributes.agility = statIncrement * enemy.baseAttributes.agility;
      enemy.baseAttributes.focus = statIncrement * enemy.baseAttributes.focus;
      enemy.baseAttributes.strength = statIncrement * enemy.baseAttributes.strength;
      enemy.RecalculateAttributes();
      TurnManager.instance.RegisterTurnBasedEntity(enemy);
    }
  }
}