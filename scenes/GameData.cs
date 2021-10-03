using Godot;
using Godot.Collections;

public class GameData : Node
{
    private readonly Dictionary _Data = new Dictionary();
    private static GameData _GlobalInstance;

    public static GameData Instance
    {
        get => _GlobalInstance;
    }

    public GameData()
    {
        if (_GlobalInstance == null)
        {
            _GlobalInstance = this;
        }
    }

    public void Store(string name, object value)
    {
        _Data[name] = value;
    }

    public T Load<T>(string name, object orDefault = null)
    {
        if (_Data.Contains(name))
        {
            return (T)_Data[name];
        }

        return (T)orDefault;
    }
}
