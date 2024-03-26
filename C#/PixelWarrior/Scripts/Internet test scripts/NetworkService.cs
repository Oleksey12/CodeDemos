using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;
public class NetworkService
{
    private const string xmlApi = "https://api.openweathermap.org/data/2.5/weather?q=Novosibirsk&appid=3c40e90c1945542de5987c445dc7cc8b";
    private bool stopCourutine = false;
    private IEnumerator CallApi(string url,Action<string> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();


            if (request.result == UnityWebRequest.Result.ConnectionError)
                Debug.LogError("Problems with network connection " + request.error);
            else if (request.responseCode != (long)System.Net.HttpStatusCode.OK)
                Debug.Log("Responce error " + request.responseCode);
            else
                callback(request.downloadHandler.text);
        }
    }

    public IEnumerator GetWeatherXML(Action<string> callback)
    {
        return CallApi(xmlApi, callback);
    }

    public IEnumerator CallAdress(string url, Action<string> callback)
    {
        return CallApi(url, callback);
    }

    public IEnumerator UpdateWeatherXML(Action<string> callback)
    {
        while (!stopCourutine)
        {
            yield return new WaitForSeconds(60);
            yield return CallApi(xmlApi, callback);
        }
    }

    public IEnumerator GetImage(string url, Action<Texture2D> callback)
    {
        yield return DownloadImage(url, callback);
    }
    public IEnumerator DownloadImage(string url, Action<Texture2D> callback)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
                Debug.LogError("Problems with network connection " + request.error);
            else if (request.responseCode != (long)System.Net.HttpStatusCode.OK)
                Debug.Log("Responce error " + request.responseCode);
            else
                callback(DownloadHandlerTexture.GetContent(request));

        }
    }



}

