using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StepQuest.Adventure
{
    public enum StoryQuestState { Available, Accepted, Won, Complete }
    public static class AdventureProgress
    {
        const string Key = "StepQuest.Adventure.v1";
        [Serializable] class Record { public int version = 1; public StoryQuestState quest; }
        static Record record;
        static bool returnToTravelMap;
        public static bool ConsumeTravelReturn() { bool result = returnToTravelMap; returnToTravelMap = false; return result; }
        public static bool InRandomBattle { get; private set; }
        public static bool InStandaloneBattle => InStoryBattle || InRandomBattle;
        public static bool InStoryBattle { get; private set; }
        public static StoryQuestState State { get { Ensure(); return record.quest; } }
        static void Ensure()
        {
            if (record != null) return;
            try { record = JsonUtility.FromJson<Record>(PlayerPrefs.GetString(Key, "")); }
            catch (ArgumentException) { record = null; }
            if (record == null) record = new Record();
        }
        static void Set(StoryQuestState state)
        {
            Ensure(); record.quest = state;
            PlayerPrefs.SetString(Key, JsonUtility.ToJson(record)); PlayerPrefs.Save();
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetRuntime() { record = null; InStoryBattle = false; InRandomBattle = false; returnToTravelMap = false; }
        public static void Accept() { if (State == StoryQuestState.Available) Set(StoryQuestState.Accepted); }
        public static bool Claim(int copper)
        {
            if (State != StoryQuestState.Won || PlayerData.Instance == null) return false;
            // Only a won quest can grant its reward; completing it disables further claims.
            PlayerData.Instance.totalCopper += Math.Max(0, copper);
            PlayerData.Instance.SavePlayerData();
            Set(StoryQuestState.Complete); return true;
        }
        public static bool Launch(AdventureDialogue dialogue, out string error)
        {
            error = null;
            if (State != StoryQuestState.Accepted) { error = "Speak to the mayor first."; return false; }
            var gm = GameManager.Instance;
            if (gm == null) { error = "Your guild is still loading."; return false; }
            var stage = gm.GetStageData(dialogue.encounterStageId);
            if (stage == null || stage.stageBattleConfig == null || !Application.CanStreamedLevelBeLoaded(stage.battleSceneName))
            { error = "This encounter is not configured. Check the dialogue asset's encounter stage ID."; return false; }
            if (gm.currentPartyHeroIDs.Count == 0) { error = "Choose a hero for your party first."; return false; }
            foreach (var id in gm.currentPartyHeroIDs)
                if (!gm.characterDataDictionary.TryGetValue(id, out var hero) || !gm.IsHeroAvailable(hero))
                { error = "A selected hero is locked. Choose an available party at the guild hall."; return false; }
            if (gm.InDungeonRun) { error = "Finish your current dungeon run first."; return false; }
            AdventureScreen.RememberTravelMap();
            gm.SaveCurrentParty();
            gm.currentStage = stage; gm.CurrentBattleConfig = stage.stageBattleConfig;
            InStoryBattle = true; Time.timeScale = 1;
            SceneManager.LoadScene(stage.battleSceneName, LoadSceneMode.Single); return true;
        }
        public static bool LaunchRandom(TownHubDefinition hub, out string error)
        {
            error = null;
            var gm = GameManager.Instance;
            if (gm == null || PlayerData.Instance == null || hub == null)
            { error = "Your guild is still loading."; return false; }
            if (InStandaloneBattle || gm.InDungeonRun)
            { error = "Finish the current encounter first."; return false; }
            if (gm.currentPartyHeroIDs.Count == 0)
            { error = "Choose at least one hero at the guild hall."; return false; }
            foreach (var id in gm.currentPartyHeroIDs)
            {
                var hero = gm.allCharacterData.Find(h => h != null && h.heroID == id);
                if (!gm.IsHeroAvailable(hero))
                { error = "A selected hero is locked. Choose an available party at the guild hall."; return false; }
                if (hero == null || hero.health <= 0)
                { error = "Your party needs to rest at the inn first."; return false; }
            }
            var stages = gm.allStagesData.FindAll(s => s != null && hub.randomEncounterStageIds.Contains(s.stageID)
                && s.stageBattleConfig != null && s.stageBattleConfig.maxEnemiesToSpawn > 0
                && Application.CanStreamedLevelBeLoaded(s.battleSceneName));
            if (stages.Count == 0) { error = "No random encounters are configured."; return false; }
            if (hub.randomBattleStepCost <= 0 || PlayerData.Instance.inGameSteps < hub.randomBattleStepCost)
            { error = "Not enough steps. Walk to earn more, then come back."; return false; }
            var stage = stages[UnityEngine.Random.Range(0, stages.Count)];
            gm.SaveCurrentParty();
            if (!PlayerData.Instance.UseSteps(hub.randomBattleStepCost)) return false;
            gm.currentStage = stage; gm.CurrentBattleConfig = stage.stageBattleConfig;
            InRandomBattle = true; Time.timeScale = 1;
            SceneManager.LoadScene(stage.battleSceneName, LoadSceneMode.Single);
            return true;
        }
        public static void Victory() { if (InStoryBattle && State == StoryQuestState.Accepted) Set(StoryQuestState.Won); }
        public static void ReturnToVillage()
        {
            returnToTravelMap = InStoryBattle;
            InRandomBattle = false;
            InStoryBattle = false; Time.timeScale = 1;
            SceneManager.LoadScene("CharacterInfoPage", LoadSceneMode.Single);
        }
    }
}
