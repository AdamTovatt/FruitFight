using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ApiHelper
{
    private static readonly HttpClient httpClient = new HttpClient();
    private static readonly string apiBasePath = "https://fruit-fight-api.herokuapp.com/api";

    public static async Task<HttpResponseMessage> PerformRequest(HttpMethod method, string path, object content = null, Dictionary<string, object> queryParameters = null)
    {
        StringBuilder requestPath = new StringBuilder(apiBasePath);
        requestPath.Append(path);

        if(queryParameters != null && queryParameters.Keys.Count > 0)
        {
            requestPath.Append("?");

            foreach(string key in queryParameters.Keys)
            {
                requestPath.Append(string.Format("{0}={1}&", key, queryParameters[key].ToString()));
            }

            requestPath.Remove(requestPath.Length - 1, 1);
        }

        if(method == HttpMethod.Get)
        {
            return await httpClient.GetAsync(requestPath.ToString());
        }
        else
        {
            throw new System.Exception(string.Format("{0} not supported", method));
        }
    }
}
