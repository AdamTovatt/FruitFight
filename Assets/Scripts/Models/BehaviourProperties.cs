using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

[JsonConverter(typeof(BaseConverter))]
public abstract class BehaviourProperties //base class for all second generation behaviour properties, obviously. For example, Container.cs should inherit from this in ContainerProperties
{
    [JsonIgnore]
    public abstract Type BehaviourType { get; }

    public string Type { get; set; }

    public static Type GetTypeFromName(string name)
    {
        switch (name)
        {
            case "ContainerProperties":
                return typeof(Container.ContainerProperties);
            case "MoveBehaviourProperties":
                return typeof(MoveBehaviour.MoveBehaviourProperties);
            case "ItemTriggerProperties":
                return typeof(ItemTrigger.ItemTriggerProperties);
            case "ActivatableProperties":
                return typeof(Activatable.ActivatableProperties);
            case "ChildObjectProperties":
                return typeof(ChildObject.ChildObjectProperties);
            case "DetailColorProperties":
                return typeof(DetailColorBehaviour.DetailColorProperties);
            case "NotificationBlockProperties":
                return typeof(NotificationBlock.NotificationBlockProperties);
            default:
                return null;
        }
    }

    public static BehaviourProperties FromJson(string json)
    {
        return JsonConvert.DeserializeObject<BehaviourProperties>(json);
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
{
    protected override JsonConverter ResolveContractConverter(Type objectType)
    {
        if (typeof(BehaviourProperties).IsAssignableFrom(objectType) && !objectType.IsAbstract)
            return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
        return base.ResolveContractConverter(objectType);
    }
}

public class BaseConverter : JsonConverter
{
    static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassConverter() };

    public override bool CanWrite { get { return false; } }

    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(BehaviourProperties));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);

        Type type = BehaviourProperties.GetTypeFromName(jObject["Type"].Value<string>());

        if (type != null)
            return JsonConvert.DeserializeObject(jObject.ToString(), type, SpecifiedSubclassConversion);

        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
