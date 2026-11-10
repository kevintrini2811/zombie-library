using Il2CppReloaded.Gameplay;
using UnityEngine;
using ZombieLibrary;

static void Check(bool value, string message) { if (!value) throw new Exception(message); }
static void Same(Color actual, Color expected, string message)
{
    Check(Math.Abs(actual.r - expected.r) + Math.Abs(actual.g - expected.g)
        + Math.Abs(actual.b - expected.b) + Math.Abs(actual.a - expected.a) < .00001f, message);
}
static Color Pma(Color color) => new(color.r * color.a, color.g * color.a, color.b * color.a, color.a);
var zombie = new Zombie();
var renderer = new SpriteRenderer();
var block = new MaterialPropertyBlock();
var source = new Color(.7f, .6f, .5f, .4f);
ZombieSpriteColor.From(zombie).Apply(renderer, source, block);
Same(renderer.color, Pma(source), "missing controller retains source colour with native PMA vertex encoding");
var effects = new Effects(); zombie.mController = new Controller { m_materialEffectController = effects };
var material = ZombieSpriteColor.CreateMaterial(zombie);
Check(!ReferenceEquals(material, effects.m_sharedColorMaterial), "owned material must not mutate shared native material");
Check(material.Keywords.Contains("_STRAIGHT_ALPHA_INPUT"), "PNG textures use native straight-alpha shader variant");
Check(material.NameOfShader == "NativeSkeletonAdditive", "reuse native per-pixel shader, never sprite lerp");
var nativeBlue = new Color(75f / 255, 75f / 255, 1, 1);
effects.m_overrideColor = nativeBlue;
effects.m_enableAdditiveColor = true; effects.m_additiveColor = nativeBlue;
renderer.Values["_PoolClip"] = 23f;
var colors = ZombieSpriteColor.From(zombie);
colors.Apply(renderer, source, block);
Same(renderer.color, Pma(source), "native override must not be applied twice through vertex colour");
Same((Color)renderer.Values["_OverrideColor"], nativeBlue, "native override forwarded exactly");
Same((Color)renderer.Values["_ExtraAdditiveColor"], nativeBlue, "native additive input forwarded exactly");
Check((float)renderer.Values["_EnableExtraAdditiveColor"] == 1, "native additive enable");
Check(!renderer.Values.ContainsKey("_BlendStrength"), "native additive must never become sprite lerp");
Check((float)renderer.Values["_PoolClip"] == 23, "pool clipping survives colour update");
colors.Apply(renderer, source, block, shadow: true);
Same(renderer.color, Pma(source), "shadow retains source alpha");
Check((float)renderer.Values["_EnableExtraAdditiveColor"] == 0, "shadow never uses additive");
Check((float)renderer.Values["_IgnoreColorOverride"] == 1, "shadow bypasses override");
effects.m_overrideColor = Color.white; effects.m_enableAdditiveColor = false;
ZombieSpriteColor.From(zombie).Apply(renderer, source, block);
Same((Color)renderer.Values["_OverrideColor"], Color.white, "thaw restores source colour");
Check((float)renderer.Values["_EnableExtraAdditiveColor"] == 0, "thaw clears stale additive enable");
Check((float)renderer.Values["_IgnoreColorOverride"] == 0, "body restores character override after shadow");
var armor = new SpriteRenderer();
effects.m_overrideColor = new Color(1, 1, 1, .5f);
foreach (float alpha in new[] { .4f, .2f, .9f, .9f })
{
    colors = ZombieSpriteColor.From(zombie);
    colors.Apply(renderer, new Color(1, 1, 1, alpha), block);
    colors.Apply(armor, new Color(1, 1, 1, alpha), block);
    float finalAlpha = armor.color.a * ((Color)armor.Values["_OverrideColor"]).a;
    Check(Math.Abs(finalAlpha - alpha * .5f) < .00001f, "armour fade applies once in current frame");
    Check(armor.color.a == renderer.color.a, "armour and head use current source alpha without lag");
}
effects.m_enableOverlayColor = true; effects.m_overlayColor = new Color(1, 0, 0, .3f);
ZombieSpriteColor.From(zombie).Apply(renderer, source, block);
Same((Color)renderer.Values["_ExtraOverlayColor"], effects.m_overlayColor, "overlay forwarded to native shader");
Check((float)renderer.Values["_EnableExtraOverlayColor"] == 1, "native overlay enable");
effects.Alive = false;
Same(ZombieSpriteColor.From(zombie).Override, Color.white, "destroyed native effects ignored");
Console.WriteLine("Zombie sprite colour: native shader reuse, PMA vertices, straight-alpha textures, exact native inputs, pool properties, shadows, thaw, armour fade and overlay passed.");

namespace UnityEngine
{
    public readonly record struct Color(float r, float g, float b, float a)
    {
        public static Color white => new(1, 1, 1, 1);
        public static Color clear => new(0, 0, 0, 0);
        public static Color operator *(Color x, Color y) => new(x.r * y.r, x.g * y.g, x.b * y.b, x.a * y.a);
        public static Color Lerp(Color x, Color y, float t) => new(x.r + (y.r - x.r) * t, x.g + (y.g - x.g) * t, x.b + (y.b - x.b) * t, x.a + (y.a - x.a) * t);
    }
    public enum HideFlags { HideAndDontSave }
    public sealed class Material
    {
        public string name = "", NameOfShader = "NativeSkeletonAdditive";
        public HideFlags hideFlags;
        public HashSet<string> Keywords = new();
        public Material() { }
        public Material(Material source) { NameOfShader = source.NameOfShader; }
        public bool HasProperty(string name) => true;
        public void EnableKeyword(string name) => Keywords.Add(name);
        public void DisableKeyword(string name) => Keywords.Remove(name);
        public void SetFloat(string name, float value) { }
        public void SetColor(string name, Color value) { }
        public static implicit operator bool(Material value) => true;
    }
    public sealed class MaterialPropertyBlock
    {
        internal Dictionary<string, object> Values = new();
        public void SetColor(string key, Color color) => Values[key] = color;
        public void SetFloat(string key, float value) => Values[key] = value;
    }
    public sealed class SpriteRenderer
    {
        public Color color;
        internal Dictionary<string, object> Values = new();
        public void GetPropertyBlock(MaterialPropertyBlock target) => target.Values = new(Values);
        public void SetPropertyBlock(MaterialPropertyBlock source) => Values = new(source.Values);
    }
}
namespace Il2CppReloaded.Gameplay
{
    public sealed class Zombie { public Controller? mController; }
    public sealed class Controller { public Effects? m_materialEffectController; }
    public sealed class Effects
    {
        public bool Alive = true;
        public Material m_sharedColorMaterial = new();
        public Material? m_colorMaterial;
        public Color m_overrideColor = Color.white, m_overlayColor, m_additiveColor;
        public bool m_enableOverlayColor, m_enableAdditiveColor;
        public static implicit operator bool(Effects value) => value.Alive;
    }
}
