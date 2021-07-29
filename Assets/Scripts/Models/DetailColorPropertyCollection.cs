using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class DetailColorPropertyCollection : BehaviourPropertyCollection
    {
        public DetailColor Color;

        public override void ApplyValues(MonoBehaviour behaviour)
        {
            if (behaviour.GetType() != typeof(DetailColorController))
                throw new Exception("Only DetailColorController can be controlled by this behaviour property keeper");

            DetailColorController detailColor = (DetailColorController)behaviour;
            detailColor.Color = Color;
            detailColor.SetTextureFromColor();
        }
    }
}
