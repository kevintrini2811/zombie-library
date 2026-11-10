using Il2CppReloaded.Gameplay;
using UnityEngine;

namespace ZombieLibrary;

/// <summary>Native character colour inputs for imported sprites. Use
/// CreateMaterial for their shared material, capture once per visual update,
/// then apply with each sprite's unmodified source colour.</summary>
public readonly record struct ZombieSpriteColor(Color Override, Color Additive, bool AdditiveEnabled,
    Color Overlay, bool OverlayEnabled)
{
    public static ZombieSpriteColor From(Zombie zombie)
    {
        var effects = zombie.mController?.m_materialEffectController;
        if (effects == null || !effects) return new(Color.white, Color.clear, false, Color.clear, false);
        return new(effects.m_overrideColor, effects.m_additiveColor, effects.m_enableAdditiveColor,
            effects.m_overlayColor, effects.m_enableOverlayColor);
    }

    /// <summary>Owned material using the game's existing native colour shader.
    /// The caller shares it across its sprites and destroys it on unload.
    /// Exported PNGs use straight texture alpha; Spine vertex colours use PMA.</summary>
    public static Material CreateMaterial(Zombie zombie)
    {
        var effects = zombie.mController?.m_materialEffectController;
        var source = effects?.m_sharedColorMaterial ?? effects?.m_colorMaterial;
        if (source == null || !source || !source.HasProperty("_OverrideColor")
            || !source.HasProperty("_EnableExtraAdditiveColor"))
            throw new InvalidOperationException("Native zombie colour material is unavailable.");
        var material = new Material(source) { name = "ZombieLibrary-NativeSpriteColor", hideFlags = HideFlags.HideAndDontSave };
        material.EnableKeyword("_STRAIGHT_ALPHA_INPUT");
        material.SetFloat("_StraightAlphaInput", 1);
        material.DisableKeyword("_TINT_BLACK_ON");
        material.SetColor("_OverrideColor", Color.white);
        material.SetFloat("_EnableExtraAdditiveColor", 0);
        material.SetFloat("_EnableExtraOverlayColor", 0);
        material.SetFloat("_EnableClipRect", 0);
        material.SetFloat("_HighContrastStrength", 0);
        return material;
    }

    /// <summary>Forwards native shader inputs without approximating per-pixel
    /// operations. Preserves source alpha and unrelated property-block values.
    /// Shadows retain their source colour and bypass character effects.</summary>
    public void Apply(SpriteRenderer renderer, Color source, MaterialPropertyBlock properties, bool shadow = false)
    {
        // The native Spine vertex shader expects premultiplied vertex colour.
        renderer.color = new Color(source.r * source.a, source.g * source.a, source.b * source.a, source.a);
        renderer.GetPropertyBlock(properties);
        properties.SetColor("_OverrideColor", shadow ? Color.white : Override);
        properties.SetFloat("_EnableExtraAdditiveColor", !shadow && AdditiveEnabled ? 1 : 0);
        properties.SetColor("_ExtraAdditiveColor", Additive);
        properties.SetFloat("_EnableExtraOverlayColor", !shadow && OverlayEnabled ? 1 : 0);
        properties.SetColor("_ExtraOverlayColor", Overlay);
        properties.SetFloat("_IgnoreColorOverride", shadow ? 1 : 0);
        renderer.SetPropertyBlock(properties);
    }
}
