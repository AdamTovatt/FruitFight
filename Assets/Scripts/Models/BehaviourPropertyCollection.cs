using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [Serializable]
    public abstract class BehaviourPropertyCollection
    {
        public abstract void ApplyValues(MonoBehaviour behaviour);
    }
}
