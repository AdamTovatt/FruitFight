using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserCredentials
{ 
    [JsonProperty("userId")]
    public long UserId { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; }

    [JsonProperty("email")]
    public string Email;

    [JsonProperty("username")]
    public string Username;

    public static UserCredentials FromJson(string json)
    {
        return JsonConvert.DeserializeObject<UserCredentials>(json);
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}
