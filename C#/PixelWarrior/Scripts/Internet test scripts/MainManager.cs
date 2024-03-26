using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(WeatherManager))]

public class MainManager : MonoBehaviour
{
    public static WeatherManager weather { get; private set; }
    public static VKManager vk { get; private set; }

    IList<IGameManager> _startSequence;

    private void Awake()
    {
        weather = GetComponent<WeatherManager>();
        vk = GetComponent<VKManager>();

  
        _startSequence = new List<IGameManager>();
        _startSequence.Add(weather);
        _startSequence.Add(vk);

        StartCoroutine(StartupManagers());
    }

    private IEnumerator StartupManagers()
    {
        NetworkService network = new NetworkService();
        foreach (IGameManager manager in _startSequence)
            manager.Startup(network);


        yield return null;

        int allModules = _startSequence.Count;
        int activeModules = 0;

        while (activeModules < allModules)
        {
            int prevReady = activeModules;
            activeModules = 0;
            foreach (IGameManager manager in _startSequence)
                if (manager.status == ManagerStatus.Started)
                    ++activeModules;

            if (prevReady != activeModules) // Если количество активных модулей изменилось
                Debug.Log("Modules ready: " + activeModules + " / " + allModules);

            yield return null;

        }


        Debug.Log("All managers are ready");
    }


}

