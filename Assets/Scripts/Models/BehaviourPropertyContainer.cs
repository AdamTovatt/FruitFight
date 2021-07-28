using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class BehaviourPropertyContainer
    {
        public List<BehaviourPropertyCollection> Properties = new List<BehaviourPropertyCollection>();

        private Dictionary<Type, BehaviourPropertyCollection> properties;

        public BehaviourPropertyCollection GetProperties(MonoBehaviour behaviour)
        {
            return Properties.Where(x => x.AssociatedType == typeof(Behaviour)).FirstOrDefault();
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
