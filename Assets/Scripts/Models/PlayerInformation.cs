using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation
{
    public PlayerConfiguration Configuration { get; set; }
    public PlayerMovement Movement { get; set; }
    public Health Health { get; set; }

    public PlayerInformation(PlayerConfiguration configuration, PlayerMovement movement, Health health)
    {
        Configuration = configuration;
        Movement = movement;
        Health = health;
    }
}
