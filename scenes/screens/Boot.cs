using Godot;
using SxGD;

public class Boot : Control
{
    public override void _Ready()
    {
        SceneTransitioner.GetInstance().FadeToScene("res://scenes/screens/Title.tscn");
    }
}
