using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

[JsonConverter(typeof(BaseConverter))]
public abstract class BehaviourProperties //base class for all second generation behaviour properties, obviously. For example, Container.cs should inherit from this in ContainerProperties
{
    public string Type;
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

        switch (jObject["Type"].Value<string>())
        {
            case "ContainerProperties":
                return JsonConvert.DeserializeObject<Container.ContainerProperties>(jObject.ToString(), SpecifiedSubclassConversion);
            default:
                throw new Exception();
        }

        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
