using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class VKManager: MonoBehaviour, IGameManager
{
    private const string serviceToken = "d4a7e2b0d4a7e2b0d4a7e2b02bd7b53c25dd4a7d4a7e2b0b759fbd6da47696634fff837";
    private const string communityToken = "vk1.a.YFmOEp5YMZqHLm-xwiJorw_v8mwnbFRzBg2rvuxzyyQdepsvXBv13QuCv-hH8m4_suzyCoU5shLKEE52F_gXKpVoGz9mx0UuCb8ulzLFJkIxdRTBXVVzndVLuhJUy-iKj-KkVHUjX6pGK7Hp8Eq4ayJ0avMxAf2bZYgDCIvOW-d-xRc0PREWXMGBpkaOubLqn6gI1w-RRdTWnKVqIb-E4Q";
    private const string ApiRequest = "https://api.vk.com/method/users.get?user_ids=kitten_lover_0w0&fields=photo_200&access_token=d4a7e2b0d4a7e2b0d4a7e2b02bd7b53c25dd4a7d4a7e2b0b759fbd6da47696634fff837&v=5.131";
    private NetworkService _service;
    public ManagerStatus status {get;private set;}

    public string ImgUrl { get; private set; }
    private Texture2D _webImage;


    public void Startup(NetworkService networkService)
    {
        _service = networkService;

        status = ManagerStatus.Started;

        StartCoroutine(_service.CallAdress(ApiRequest, SaveImageUrl));
    }

    public void SaveImageUrl(string data)
    {
        Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

        JArray responceArray = (JArray)dict["response"];

        foreach(JObject obj in responceArray)
        {
            if(obj.ContainsKey("photo_200"))
            {
                ImgUrl = (string)obj.Property("photo_200").Value;
                Debug.Log(ImgUrl);
            }
        }


    }

    public void GetImage(Action<Texture2D> callback)
    {

        if (_webImage == null)
            if(ImgUrl == null)
                StartCoroutine(_service.CallAdress(ApiRequest, SaveImageUrl));
            else
                StartCoroutine(_service.GetImage(ImgUrl, (Texture2D img) => { _webImage = img; callback(img); }));
        else
            callback(_webImage);
    }
}

