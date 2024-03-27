using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFileSystem
{
    public void SaveData(Dictionary<string, float> Params, string filePath); // ��������� ��������� � �� �������� � ����
    public Dictionary<string, float> GetParamsFromFile(string filePath); // ���������� �������� ���������, ����������� � �����
}
