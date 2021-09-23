using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class ApiUserManager
{
    public static async Task<bool> Login(string username, string password)
    {
        HttpResponseMessage response = await ApiHelper.PerformRequest(HttpMethod.Post, "/user/login", new { username = username, password = password });

        if(response.IsSuccessStatusCode)
        {
            ApiHelper.SetCredentialValues(UserCredentials.FromJson(await response.Content.ReadAsStringAsync()));
            return true;
        }

        return false;
    }
}
