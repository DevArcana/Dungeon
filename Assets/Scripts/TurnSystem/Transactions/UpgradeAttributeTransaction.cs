using System;
using EntityLogic;
using EntityLogic.AI;
using EntityLogic.Attributes;
using UnityEngine;
using Attribute = EntityLogic.Attributes.Attribute;

namespace TurnSystem.Transactions
{
    public class UpgradeAttributeTransaction : TransactionBase
    {
        private readonly GridLivingEntity _entity;
        private readonly Attribute _attribute;
        private readonly double _amount;
        
        public UpgradeAttributeTransaction(GridLivingEntity entity, Attribute attribute, double amount ,bool isAbility) : base(isAbility)
        {
            _entity = entity;
            _attribute = attribute;
            _amount = amount;
        }

        protected override void Process()
        {
            _entity.attributes.PermanentModifiers.Add(new AttributeModifier()
            {
                attribute = _attribute,
                type = ModifierType.Additive,
                value = _amount
            });
            LogConsole.Log($"Upgraded {_attribute} by {_amount}!" + Environment.NewLine);
            Finish();
        }
    }
}