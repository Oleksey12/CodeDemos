using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFileSystem
{
    public void SaveData(Dictionary<string, float> Params, string filePath); // Сохраняет параметры и их значения в файл
    public Dictionary<string, float> GetParamsFromFile(string filePath); // Возвращает значение параметра, хранящегося в файле
}
