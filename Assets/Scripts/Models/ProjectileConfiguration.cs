using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileConfiguration
{
    public List<ProjectileConfigurationEntry> ProjectileList;

    public static Dictionary<int, ProjectileConfigurationEntry> Projectiles { get { if (_projectiles == null) Load(); return _projectiles; } private set { _projectiles = value; } }
    private static Dictionary<int, ProjectileConfigurationEntry> _projectiles;

    public static void Load()
    {
        try
        {
            _projectiles = new Dictionary<int, ProjectileConfigurationEntry>();
            ProjectileConfiguration configuration = JsonUtility.FromJson<ProjectileConfiguration>(WorldUtilities.LoadTextFile("Configuration/ProjectileConfiguration"));

            foreach (ProjectileConfigurationEntry projectile in configuration.ProjectileList)
            {
                _projectiles.Add(projectile.Id, projectile);
            }
        }
        catch(Exception exception)
        {
            Debug.LogError("Error when loading projectile configuration! \n" + exception.Message);
        }
    }
}