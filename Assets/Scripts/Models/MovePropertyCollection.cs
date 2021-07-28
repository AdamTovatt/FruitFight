using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class MovePropertyCollection : BehaviourPropertyCollection
    {
        public bool HasValues;
        public int ActivatorBlockId;
        public Vector3 FinalPosition;

        public override void ApplyValues(MonoBehaviour behaviour)
        {
            if (behaviour.GetType() != typeof(MoveOnTrigger))
                throw new Exception("Only DetailColorController can be controlled by this behaviour property keeper");

            if (HasValues)
            {
                MoveOnTrigger move = (MoveOnTrigger)behaviour;
                move.FinalPosition = FinalPosition;
                move.ActivatorBlockId = ActivatorBlockId;
            }
            else
            {
                throw new Exception("This property collection has no values: " + this.ToString());
            }
        }
    }
}
