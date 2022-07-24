using System;

public class DetailColorBehaviour : BehaviourBase
{
    public class DetailColorProperties : BehaviourProperties
    {
        [EnumInput(EnumType = typeof(DetailColor), Name = "Color", Description = "The detail color of this object")]
        public DetailColor DetailColor { get; set; }

        public override Type BehaviourType => typeof(DetailColorBehaviour);
    }

    public DetailColorProperties Properties;

    public override void Initialize(BehaviourProperties behaviourProperties)
    {
        Properties = (DetailColorProperties)behaviourProperties;

        DetailColorController controller = gameObject.GetComponent<DetailColorController>();
        if (controller != null)
            controller.SetColor(Properties.DetailColor);
    }
}
