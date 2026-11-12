using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Hero definition")]
public sealed class PlayableHeroDefinition : ScriptableObject
{
    public string heroID;
    public CharacterData characterData;
    public GameObject prefab;
    [Tooltip("Temporary availability in the Editor and Development Builds only. Never saved as a permanent unlock.")]
    public bool developmentUnlock = true;
    public bool DevelopmentAvailable => HeroAvailability.IsAvailable(false, developmentUnlock, Application.isEditor, Debug.isDebugBuild);
}
