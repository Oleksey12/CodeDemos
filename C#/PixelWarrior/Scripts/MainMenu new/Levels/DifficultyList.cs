using UnityEngine;

// Двусвязный циклический список
public class DifficultyList
{
    public float _difficultyVal;
    public string _difficultyName;
    public Sprite _difficultyImage;

    public DifficultyList _next;
    public DifficultyList _previous;

    public DifficultyList(float val, string name, Sprite img)
    {
        _difficultyVal = val;
        _difficultyName = name;
        _difficultyImage = img;
        this._next = this;
        this._previous = this;

    }
    public DifficultyList AddNewElement(DifficultyList newElement)
    {
        // Устанавливаем для нового элемента значения предыдущего и следующего элемента
        newElement._previous = this;
        newElement._next = this._next;
        // Говорим элементу, что был на этом месте, что его предыдущий элемент поменялся
        newElement._next._previous = newElement;

        // Добавляем новый элемент
        this._next = newElement;



        // Возвращаем элемент, на случай, если захотим добавить ещё элементов в это же место
        return newElement;
    }

    public void PrintList()
    {
        Debug.Log(this._difficultyName);
        for (DifficultyList it = this._previous; it != this; it = it._previous)
            Debug.Log(it._difficultyName);
    }

}
