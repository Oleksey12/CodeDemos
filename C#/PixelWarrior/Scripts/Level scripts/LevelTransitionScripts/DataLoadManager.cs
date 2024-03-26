using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
public class DataLoadManager : MonoBehaviour, IGetAllData
{
    // Загружает данные из файла
    void Awake()
    {
        GetPlayerData();
        GetLevelData();
    }

    public void GetLevelData()
    {
        try
        {
            FindObjectOfType<BaseValues>().GetAllParams();
        }
        catch (Exception e)
        {

        }
    }

    public void GetPlayerData()
    {
        try
        {
            FindObjectOfType<PlayerData>().GetAllParams();
        }
        catch( Exception e)
        {

        }
    }

}
