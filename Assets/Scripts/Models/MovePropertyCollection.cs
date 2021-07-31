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
        public float MoveSpeed = 0.5f;
        public float EndpointDelay;
        public bool LinearMovement;
        public bool PingPong;

        public override void ApplyValues(MonoBehaviour behaviour)
        {
            if (behaviour.GetType() != typeof(MoveOnTrigger))
                throw new Exception("Only MoveOnTrigger can be controlled by this behaviour property keeper");

            if (HasValues)
            {
                MoveOnTrigger move = (MoveOnTrigger)behaviour;
                move.FinalPosition = FinalPosition;
                move.ActivatorBlockId = ActivatorBlockId;
                move.MoveSpeed = MoveSpeed;
                move.EndpointDelay = EndpointDelay;
                move.PingPong = PingPong;
                move.LinearMovement = LinearMovement;
            }
            else
            {
                throw new Exception("This property collection has no values: " + this.ToString());
            }
        }
    }
}
