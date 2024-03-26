using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Linq;

public class FileSystem : MonoBehaviour, IFileSystem
{

    public void SaveData(Dictionary<string, float> Params, string filePath) // Сохраняет параметры и их значения в файл
    {
        try
        {
            using StreamWriter AddToFile = new StreamWriter(Application.persistentDataPath + "/" + filePath, false);
            {
                if (AddToFile == null)
                    throw new ArgumentNullException("Coudn't open file");
                foreach (KeyValuePair<string, float> pair in Params)
                    LogParam(AddToFile, pair.Key, pair.Value);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }
    private void LogParam(StreamWriter File, string paramName, float param) => File.WriteLine(paramName + " " + param.ToString());

    public Dictionary<string, float> GetParamsFromFile(string filePath) // Возвращает хэш таблицу значений, хранящихся в файле
    {
        Dictionary<string, float> values = new Dictionary<string, float>();
        try
        {
            using StreamReader LogFile = new StreamReader(Application.persistentDataPath + "/" + filePath);
            {
                if (LogFile == null)
                    throw new ArgumentNullException("Coudn't open file");
                string curLine;
                StringBuilder sb = new StringBuilder();
                curLine = LogFile.ReadLine();
                while (curLine != null)
                {
                    int nameLength;
                    for (nameLength = 0; curLine[nameLength] != ' '; ++nameLength)
                    {
                        sb.Append(curLine[nameLength]);
                    }
                    values.Add(sb.ToString(), Convert.ToSingle(curLine.Substring(nameLength + 1)));
                    sb = new StringBuilder();
                    curLine = LogFile.ReadLine();
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return values;
    }
}


