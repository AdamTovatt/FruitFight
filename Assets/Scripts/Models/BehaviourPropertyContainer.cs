using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class BehaviourPropertyContainer
    {
        public DetailColorPropertyCollection DetailColorPropertyCollection;
        public MovePropertyCollection MovePropertyCollection;
        public TriggerZonePropertyCollection TriggerZonePropertyCollection;
        public NotificationPropertyCollection NotificationPropertyCollection;
        public EventCameraPropertyCollection EventCameraPropertyCollection;

        private Dictionary<Type, BehaviourPropertyCollection> properties;

        public BehaviourPropertyCollection GetProperties(Type behaviour)
        {
            if (behaviour == typeof(DetailColorController))
            {
                return DetailColorPropertyCollection;
            }
            else if(behaviour == typeof(MoveOnTrigger))
            {
                return MovePropertyCollection;
            }
            else if(behaviour == typeof(TriggerZone))
            {
                return TriggerZonePropertyCollection;
            }
            else if(behaviour == typeof(NotificationBlock))
            {
                return NotificationPropertyCollection;
            }
            else if(behaviour == typeof(EventCamera))
            {
                return EventCameraPropertyCollection;
            }
            else
                throw new Exception("Unsupported behaviour type");
        }

        public static BehaviourPropertyCollection GetBehaviourProperty(MonoBehaviour behaviour)
        {
            if(behaviour.GetType() == typeof(DetailColorController))
            {
                return new DetailColorPropertyCollection();
            }
            else
            {
                throw new Exception("Behaviour of type " + behaviour.GetType() + " does not have a linked property collection");
            }
        }
    }
}
