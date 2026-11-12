using System;
using System.Linq;
using StepQuest.Adventure;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class BuffmageValidation
{
    static void Require(bool condition, string message)
    { if (!condition) throw new InvalidOperationException("Buffmage validation: " + message); }

    [MenuItem("StepQuest/Heroes/Validate Buffmage assets")]
    public static void ValidateAssets()
    {
        var definition = Resources.Load<PlayableHeroDefinition>("Heroes/Buffmage/Definition");
        var settings = BuffmageSettings.Load();
        Require(definition != null && settings != null, "Hero definition and settings must load from Resources.");
        Require(definition.heroID == Buffmage.HeroID && definition.characterData != null && definition.characterData.heroID == Buffmage.HeroID, "Stable ID and CharacterData must match.");
        Require(!definition.characterData.isUnlocked, "Authoring defaults must not permanently grant the testing unlock.");
        Require(definition.prefab != null, "Prefab is missing.");
        var hero = definition.prefab.GetComponent<Buffmage>();
        Require(hero != null && hero.characterData == definition.characterData && hero.settings == settings, "Prefab data/settings bindings do not match the definition.");
        Require(hero.GetComponent<Collider2D>() != null && hero.GetComponent<Rigidbody2D>() != null, "Battle physics components are missing.");
        Require(hero.GetComponent<Animator>()?.runtimeAnimatorController != null, "Shared hurt/block animation hooks need the sprite controller.");
        Require(hero.GetComponent<SpriteRenderer>()?.sprite != null && definition.characterData.heroIcon != null, "Character sprite or portrait is missing.");
        var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(definition.characterData.heroIcon)) as TextureImporter;
        Require(importer != null && importer.filterMode == FilterMode.Point && !importer.mipmapEnabled, "Character art must use point filtering without mipmaps.");
        Require(settings.bookSprite != null && settings.fireballIcon != null && settings.punchIcon != null && settings.meteorIcon != null && settings.impactSprite != null, "A skill prop/icon/effect reference is missing.");
        Require(settings.flameFrames != null && settings.flameFrames.Length > 0 && settings.flameFrames.All(s => s != null), "Burning-book frames are missing.");
        foreach (var type in hero.AllSkills)
        {
            Require(definition.characterData.AvailableSkills.Contains(type) && definition.characterData.equippedSkills.Contains(type), "Default loadout is missing " + type);
            Require(definition.characterData.GetSkillInstance(type)?.Type == type, "CharacterData skill factory cannot resolve " + type);
        }
        var theme = Resources.Load<RpgUiTheme>("Adventure/RpgUiTheme");
        Require(theme != null && theme.pixelFont != null && theme.pixelFontShader != null && theme.windowFrame != null, "Shared RPG UI assets are incomplete.");
        ValidateAnimations();
        Debug.Log("Buffmage asset validation passed. Run the progression check in Play mode, then exercise mouse/touch interactions in battle.");
    }

    [MenuItem("StepQuest/Heroes/Validate Buffmage animations")]
    public static void ValidateAnimations()
    {
        var definition = Resources.Load<PlayableHeroDefinition>("Heroes/Buffmage/Definition");
        var set = Resources.Load<BuffmageAnimationSet>("Heroes/Buffmage/Animations/AnimationSet");
        Require(definition != null && set != null && set.controller != null, "Animation set/controller is missing.");
        var driver = definition.prefab.GetComponent<BuffmageAnimationDriver>();
        Require(driver != null && driver.animations == set && BuffmageSettings.Load().animations == set, "Animation driver/settings are not wired.");
        Require(definition.prefab.GetComponent<Animator>().runtimeAnimatorController == set.controller, "Prefab still uses a fallback controller.");
        var sprites = Resources.LoadAll<Sprite>("Heroes/Buffmage/Animations/BuffmageSheet-v2");
        Require(sprites.Length == 48 && sprites.All(s => s != null), "Atlas must import as 48 separate sprites.");
        Require(set.controller.animationClips.Length == 14, "Controller must retain every action and recovery clip.");
        var binding = new EditorCurveBinding { path = "", type = typeof(SpriteRenderer), propertyName = "m_Sprite" };
        foreach (var clip in set.controller.animationClips)
        {
            var keys = AnimationUtility.GetObjectReferenceCurve(clip, binding);
            Require(keys != null && keys.Length >= 2 && keys.All(k => k.value is Sprite && sprites.Contains(k.value)), "A clip has a missing or foreign sprite: " + clip.name);
            Require(clip.events.Length == 0, "Damage must remain skill-owned rather than driven by animation events.");
        }
        Require(set.idle.isLooping && set.bookHold.isLooping && set.meteorAir.isLooping && !set.death.isLooping, "Loop/one-shot settings are incorrect.");
        Require(set.bookReleaseSeconds < set.bookThrow.length && set.leftPunchImpactSeconds < set.punchLeft.length
            && set.rightPunchImpactSeconds < set.punchRight.length && set.meteorImpactSeconds < set.meteorSlam.length, "An impact/release time falls outside its clip.");
        var material = definition.prefab.GetComponent<SpriteRenderer>().sharedMaterial;
        Require(material != null && material.shader.name == "StepQuest/Buffmage Pixel Sprite", "Clean pixel alpha material is missing.");
        Require(!ShaderUtil.ShaderHasError(material.shader), "Sprite shader has an import/compilation error.");
        var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprites[0])) as TextureImporter;
        Require(importer != null && importer.filterMode == FilterMode.Point && !importer.mipmapEnabled, "Sheet import must stay crisp.");
        Debug.Log("Buffmage animation validation passed: 48 sprites, 14 clips, timing markers, loop flags, material, and prefab/driver bindings.");
    }

    [MenuItem("StepQuest/Heroes/Validate Buffmage save and progression (Play mode)")]
    public static void ValidateProgression()
    {
        Require(EditorApplication.isPlaying && GameManager.Instance != null, "Enter Play mode in the home scene first.");
        ValidateAssets();
        var definition = Resources.Load<PlayableHeroDefinition>("Heroes/Buffmage/Definition");
        var settings = BuffmageSettings.Load();
        var data = Object.Instantiate(definition.characterData);
        var reloaded = Object.Instantiate(definition.characterData);
        var sword = Object.Instantiate(Resources.Load<Equipment>("Items/Equipment/WoodenSword"));
        var holder = new GameObject("Isolated Buffmage progression check"); holder.SetActive(false);
        string id = "__BuffmageValidation_" + Guid.NewGuid().ToString("N");
        string key = "CharacterData_" + id;
        try
        {
            Buffmage.InitializeNewData(data, settings); data.heroID = id; reloaded.heroID = id;
            sword.LoadRolledData(Guid.NewGuid().ToString("N"), 3, 0, 0, 0, 0);
            data.EquipItem(sword);
            int attackWithGear = data.attackPower;
            var obj = Object.Instantiate(definition.prefab, holder.transform);
            var hero = obj.GetComponent<Buffmage>(); hero.SetCharacterData(data);
            hero.heroExp = hero.ExpToNextLevel(hero.heroLevel) + 7;
            hero.LevelUp();
            Require(hero.heroLevel == 2 && hero.heroExp == 7, "XP was not converted into a level.");
            Require(hero.attackPower == attackWithGear + settings.attackPerLevel, "Leveling erased or duplicated equipment stats.");
            Require(hero.maxHealth == settings.startingHealth + settings.healthPerLevel, "Health growth did not apply.");
            Require(!data.isUnlocked, "Testing wrote a permanent unlock.");
            reloaded.LoadData();
            Require(reloaded.heroLevel == hero.heroLevel && reloaded.heroExp == hero.heroExp && reloaded.attackPower == hero.attackPower, "Saved progression did not reload.");
            Require(reloaded.GetEquipped(sword.equipmentType)?.uniqueInstanceId == sword.uniqueInstanceId, "Equipped item identity did not reload.");
            int before = reloaded.attackPower; reloaded.LoadData();
            Require(reloaded.attackPower == before, "Loading twice stacked equipment bonuses.");
            Require(!reloaded.isUnlocked, "Saved test progression unexpectedly unlocked the hero.");
            Debug.Log("Buffmage XP, level growth, equipment, save/load, repeated load, and nonpersistent testing-unlock checks passed. Only a unique temporary save key was used.");
        }
        finally
        {
            PlayerPrefs.DeleteKey(key); PlayerPrefs.Save();
            foreach (var equipment in reloaded.GetEquippedDictionary().Values) if (equipment != null) Object.Destroy(equipment);
            Object.Destroy(holder); Object.Destroy(data); Object.Destroy(reloaded); Object.Destroy(sword);
        }
    }
}
