using Godot;
using SxGD;

[Tool]
public class FXTest : RichTextEffect
{
    public string bbcode = "ghost";

    public override bool _ProcessCustomFx(CharFXTransform charFx)
    {
        uint start = (uint)(float)charFx.Env["start"];
        uint length = (uint)(float)charFx.Env["length"];

        if (charFx.AbsoluteIndex > start)
        {
            var ratio = 0.0f;
            if (charFx.AbsoluteIndex - start < length)
            {
                ratio = 1 - (charFx.AbsoluteIndex - start) / (float)length;
            }

            charFx.Color = charFx.Color.WithAlphaf(ratio);
        }

        return true;
    }
}
