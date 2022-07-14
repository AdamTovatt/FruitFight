using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourPropertiesScreen : MonoBehaviour
{
    public TextMeshProUGUI TitleText;
    public Transform InputContainer;
    public GameObject StringInputPrefab;
    public Button BackButton;
    public BlockInspector BlockInspector;
    public Transform BlockInspectorParent;

    List<GameObject> previousInputs = new List<GameObject>();

    BehaviourProperties currentProperties;
    Block currentBlock;

    private void Awake()
    {
        BindEvents();
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }

    private void BindEvents()
    {
        BackButton.onClick.AddListener(() => Back());
    }

    private void UnBindEvents()
    {
        BackButton.onClick.RemoveAllListeners();
    }

    public void Clean()
    {
        foreach(GameObject gameObject in previousInputs)
        {
            Destroy(gameObject);
        }

        previousInputs.Clear();
    }

    public void Show(Block block, BehaviourProperties behaviourProperties)
    {
        Clean();
        BlockInspectorParent.gameObject.SetActive(false);
        currentBlock = block;
        currentProperties = behaviourProperties;
        TitleText.text = string.Format("Editing properties for: {0}", behaviourProperties.GetType().ToString());

        Dictionary<PropertyInfo, BehaviourPropertyInputAttribute> inputs = GetPropertyInputs(behaviourProperties);

        foreach(PropertyInfo key in inputs.Keys)
        {
            switch(inputs[key])
            {
                case StringInputAttribute stringInput:
                    BehaviourStringInput behaviourStringInput = Instantiate(StringInputPrefab, InputContainer).GetComponent<BehaviourStringInput>();
                    behaviourStringInput.Initialize(stringInput, key, currentProperties);
                    previousInputs.Add(behaviourStringInput.gameObject);
                    break;
                default:
                    Debug.Log("We have a type that is not yet supported: " + inputs[key].GetType());
                    break;
            }
        }
    }

    private void Back()
    {
        BlockInspectorParent.gameObject.SetActive(true);
        BlockInspector.Show(currentBlock);
        gameObject.SetActive(false);
    }

    private Dictionary<PropertyInfo, BehaviourPropertyInputAttribute> GetPropertyInputs(BehaviourProperties behaviourProperties)
    {
        Dictionary<PropertyInfo, BehaviourPropertyInputAttribute> result = new Dictionary<PropertyInfo, BehaviourPropertyInputAttribute>();

        PropertyInfo[] properties = behaviourProperties.GetType().GetProperties();

        foreach (PropertyInfo property in properties)
        {
            object[] attributes = property.GetCustomAttributes(true);
            foreach (object attribute in attributes)
            {
                BehaviourPropertyInputAttribute inputAttribute = attribute as BehaviourPropertyInputAttribute;
                if (inputAttribute != null && !result.ContainsKey(property))
                    result.Add(property, inputAttribute);
            }
        }

        return result;
    }
}
