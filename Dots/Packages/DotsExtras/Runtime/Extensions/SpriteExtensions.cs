using UnityEngine;

public static class SpriteExtensions
{
    public static void SetSColorA(this SpriteRenderer sprite, float a)
    {
        var color = sprite.color;
        color.a = a;
        sprite.color = color;
    }
}
