using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class WeatherManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    public float cloudValue { get; private set; }
    private NetworkService _network;



    public void Startup(NetworkService networkService)
    {

        _network = networkService;
        StartCoroutine(_network.GetWeatherXML(GetWeatherForecast));
        Debug.Log("Сервис погоды был запущен");
        StartCoroutine(_network.UpdateWeatherXML(UpdateTemprature));
    }

    private void GetWeatherForecast(string data)
    {

        Debug.Log(data);

        string value = ConvertJSONData(data);

        cloudValue = Convert.ToSingle(value.Replace('.', ',')) -273.15f;

        Messenger.Broadcast(GameEvent.WEATHER_UPDATED);


        status = ManagerStatus.Started;

    }


    private void UpdateTemprature(string data)
    {
        string value = ConvertJSONData(data);

        float newTemprature = Convert.ToSingle(value.Replace('.', ',')) - 273.15f;

        if (newTemprature != cloudValue)
        {
            cloudValue = newTemprature;
            Messenger.Broadcast(GameEvent.WEATHER_UPDATED);
        }

    }

    private static string ConvertXMLData(string data)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(data);
        XmlNode root = xmlDoc.DocumentElement;

        XmlNode node = root.SelectSingleNode("temperature");
        string value = node.Attributes["value"].Value;
        return value;
    }

    public string ConvertJSONData(string data)
    {      
        Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

        /*
        foreach (KeyValuePair<string, object> pair in dict)
            Debug.Log(pair.Key + " " + pair.Value.GetType());
        */

        JObject tempratures = (JObject)dict["main"];

        Dictionary<string, object> temprature = tempratures.ToObject<Dictionary<string, object>>();

        

        var cloudValue = Convert.ToDouble(temprature["temp"]);


        return (Convert.ToString(cloudValue));
    }
}

