using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class ApiUserManager
{
    public async Task<string> GetUserToken(string username, string password)
    {
        HttpResponseMessage response = await ApiHelper.PerformRequest(HttpMethod.Post, "/user/login", new { username = username, password = password });
        return null;
    }
}
