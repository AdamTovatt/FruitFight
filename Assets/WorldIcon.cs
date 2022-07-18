using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldIcon : MonoBehaviour
{
    [Serializable]
    public struct NamedIconTexture
    {
        public string Name;
        public Texture2D Texture;
    }

    public TextMeshPro FrontText;
    public TextMeshPro BackText;
    public MeshRenderer FrontIcon;
    public MeshRenderer BackIcon;

    [SerializeField]
    public NamedIconTexture[] NamedIcons;

    private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

    private void Awake()
    {
        foreach (NamedIconTexture namedIconTexture in NamedIcons)
            textures.Add(namedIconTexture.Name, namedIconTexture.Texture);
    }

    public void SetupIcon(string iconName, string iconText)
    {
        FrontText.text = iconText;
        BackText.text = iconText;
        FrontIcon.material.mainTexture = textures[iconName];
        BackIcon.material.mainTexture = textures[iconName];
    }
}
