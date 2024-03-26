using UnityEngine;

/*
 * Управляет поведением облаков в меню
 */
public interface ICloudManager
{
    public void ReCreateCloud(int cloudNum); // Пересоздаёт облако под cloudNum номером
    public void CloudController(); // Управляет передвижением облак

}