using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property)]
public class BehaviourPropertyInputAttribute : Attribute
{
    public string Name { get; set; }
    public string Description { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class IntInputAttribute : BehaviourPropertyInputAttribute
{
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public bool Limitless { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class FloatInputAttribute : BehaviourPropertyInputAttribute
{
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class StringInputAttribute : BehaviourPropertyInputAttribute
{
    public bool CheckIfPrefab { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class BoolInputAttribute : BehaviourPropertyInputAttribute
{

}

[AttributeUsage(AttributeTargets.Property)]
public class EnumInputAttribute : BehaviourPropertyInputAttribute
{
    public Type EnumType { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class ActivatorInputAttribute : BehaviourPropertyInputAttribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public class PositionInputAttribute : BehaviourPropertyInputAttribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public class SubZoneInputAttribute : BehaviourPropertyInputAttribute
{
}