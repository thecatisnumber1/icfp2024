﻿using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Lib;

public static class RestConnector
{
    private static string? apiKey;

    public static void SetKey(string key)
    {
        apiKey = key;
    }

    public static string? GetString(string url)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        try
        {
            return client.GetStringAsync(url).Result;
        }
        catch
        {
            return null;
        }
    }

    public static T? GetJson<T>(string url)
    {
        var strung = GetString(url);
        if (strung != null)
        {
            return JsonSerializer.Deserialize<T>(strung);
        }

        return default;
    }

    public static (string, HttpStatusCode) PostFile(string url, string content)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var result = client.PostAsync(url, new StringContent(content)).Result;

        return (result.Content.ReadAsStringAsync().Result, result.StatusCode);
    }
}
