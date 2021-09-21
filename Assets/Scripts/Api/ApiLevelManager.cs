using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class ApiLevelManager
{
    public async static Task<WorldMetadataResponse> GetLevelsList(int resultsPerPage, int page)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("resultsPerPage", resultsPerPage);
        parameters.Add("page", page);

        HttpResponseMessage response = await ApiHelper.PerformRequest(HttpMethod.Get, "/level/list", queryParameters: parameters);

        if(response.IsSuccessStatusCode)
        {
            return WorldMetadataResponse.FromJson(await response.Content.ReadAsStringAsync());
        }
        else
        {
            Debug.LogError("Something went wrong when fetching levels: " + await response.Content.ReadAsStringAsync());
            return null;
        }
    }
}
