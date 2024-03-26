/*
 * Определяет взаимодействие со скриптами хранящие данные об объектах
 */
public interface IGameDataScript
{
    // Извлечь все данные объекта из файла
    public void GetAllParams();

    // Сохранить все данные об объекте в файл
    public void SaveStartParams();

}