using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ApiHelper
{
    private const string apiBasePath = "https://fruit-fight-api.herokuapp.com/api";

    private static readonly HttpClient httpClient = new HttpClient();

    public static UserCredentials UserCredentials { get; private set; }
    private static bool hasBeenInitialized;

    private static void Initialize()
    {
        UserCredentials userCredentials = FileHelper.LoadUserCredentials();

        if (userCredentials != null)
        {
            UserCredentials = userCredentials;
        }

        hasBeenInitialized = true;
    }

    public static void SetCredentialValues(UserCredentials userCredentials)
    {
        FileHelper.SaveUserCredentials(userCredentials);

        Initialize();
    }

    public static async Task<HttpResponseMessage> PerformRequest(HttpMethod method, string path, object content = null, Dictionary<string, object> queryParameters = null)
    {
        if (!hasBeenInitialized)
            Initialize();

        StringBuilder requestPath = new StringBuilder(apiBasePath);
        requestPath.Append(path);

        if (UserCredentials != null) //set token in header if we have token
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserCredentials.Token);

        if (queryParameters != null && queryParameters.Keys.Count > 0)
        {
            requestPath.Append("?");

            foreach (string key in queryParameters.Keys)
            {
                requestPath.Append(string.Format("{0}={1}&", key, queryParameters[key].ToString()));
            }

            requestPath.Remove(requestPath.Length - 1, 1);
        }

        if (method == HttpMethod.Get)
        {
            return await httpClient.GetAsync(requestPath.ToString());
        }
        else if (method == HttpMethod.Post)
        {
            string json = "";

            if (content != null)
                json = content.GetType() == typeof(string) ? (string)content : JsonConvert.SerializeObject(content);

            StringContent body = new StringContent(json, Encoding.UTF8, "application/json");
            return await httpClient.PostAsync(requestPath.ToString(), body);
        }
        else if(method == HttpMethod.Delete)
        {
            return await httpClient.DeleteAsync(requestPath.ToString());
        }
        else if (method == HttpMethod.Put)
        {
            string json = "";

            if (content != null)
                json = content.GetType() == typeof(string) ? (string)content : JsonConvert.SerializeObject(content);

            StringContent body = new StringContent(json, Encoding.UTF8, "application/json");
            return await httpClient.PutAsync(requestPath.ToString(), body);
        }
        else
        {
            throw new System.Exception(string.Format("{0} not supported in ApiHelper yet", method));
        }
    }
}
