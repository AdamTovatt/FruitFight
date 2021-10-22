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

        if (response.IsSuccessStatusCode)
        {
            return WorldMetadataResponse.FromJson(await response.Content.ReadAsStringAsync());
        }
        else
        {
            Debug.LogError("Something went wrong when fetching levels: " + await response.Content.ReadAsStringAsync());
            return null;
        }
    }

    public async static Task<UploadLevelResponse> UploadLevel(World world)
    {
        UploadLevelRequestBody body = new UploadLevelRequestBody()
        {
            Description = world.Metadata.Description,
            Name = world.Metadata.Name,
            PuzzleRatio = 50,
            Thumbnail = world.Metadata.ImageData,
            twoPlayers = true
        };

        world.Metadata = null;
        body.WorldData = world.ToJson();

        HttpResponseMessage response = await ApiHelper.PerformRequest(HttpMethod.Post, "/level/upload", body);
        string responseJson = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            ApiHelper.RemoveUserCredentials();

        return new UploadLevelResponse(responseJson, response.IsSuccessStatusCode, response.StatusCode);
    }

    public async static Task<GetLevelResponse> GetLevel(long levelId)
    {
        HttpResponseMessage response = await ApiHelper.PerformRequest(HttpMethod.Get, "/level/get", null, new Dictionary<string, object>() { { "id", levelId } });

        if (response.IsSuccessStatusCode)
        {
            return GetLevelResponse.FromJson(await response.Content.ReadAsStringAsync());
        }

        return null;
    }

    public async static Task<bool> DeleteLevel(long levelId)
    {
        HttpResponseMessage response = await ApiHelper.PerformRequest(HttpMethod.Delete, "/level/delete", null, new Dictionary<string, object>() { { "levelId", levelId } });

        if (response.IsSuccessStatusCode)
            return true;

        return false;
    }
}
