using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace StepQuest.Adventure
{
    public partial class AdventureScreen
    {
        [Header("Town hub")]
        [SerializeField] TownHubDefinition townHub;
        [SerializeField] Button randomBattleButton, heroesBuilding, equipmentBuilding, innBuilding, travelBuilding;
        [SerializeField] GameObject[] townOverlays;
        TMP_Text randomBattleLabel;
        void InitializeTownHub()
        {
            if (townHub == null) return;
            Bind(randomBattleButton, ShowRandomBattle); Bind(heroesBuilding, ShowRoster);
            Bind(equipmentBuilding, () => ShowShop()); Bind(innBuilding, () => ShowInn());
            Bind(travelBuilding, () => OpenTravelMap());
            if (randomBattleButton != null) randomBattleLabel = randomBattleButton.GetComponentInChildren<TMP_Text>(true);
            BuildAdventureTown();
            RefreshTownBalance();
        }
        static void Bind(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null) return;
            button.onClick = new Button.ButtonClickedEvent(); button.onClick.AddListener(action);
        }
        void CloseTownOverlays()
        {
            if (townOverlays == null) return;
            foreach (var overlay in townOverlays) if (overlay != null) overlay.SetActive(false);
        }
        void LateUpdate() { RefreshTownBalance(); RefreshExploration(); }
        void RefreshTownBalance()
        {
            if (randomBattleLabel == null || townHub == null) return;
            int steps = PlayerData.Instance != null ? PlayerData.Instance.inGameSteps : 0;
            string balance = "TOWN GATES\n" + townHub.randomBattleStepCost + " steps / " + steps + " owned";
            if (randomBattleLabel.text != balance) randomBattleLabel.text = balance;
        }
        void ShowRandomBattle()
        {
            OpenPanel(154);
            BuildExploration(content, actionFooter);
        }
        void ShowShop(string notice = null)
        {
            OpenPanel(); Label("Bramble Equipment", 32); Label("Copper: " + PlayerData.Instance.totalCopper, 26);
            if (notice != null) Label(notice, 25);
            foreach (var offer in townHub.shop)
            {
                if (offer == null || offer.equipment == null) continue;
                var entry = offer;
                Art(entry.equipment.itemIcon, 65); Label(entry.equipment.itemName, 28);
                string stats = string.Join("  ", entry.equipment.guaranteedStatRanges.Select(r => r.statType + " +" + r.minValue + "-" + r.maxValue));
                Label(string.IsNullOrEmpty(stats) ? entry.equipment.GetStatDescription() : stats, 24);
                var buy = MakeButton(content, "Buy - " + entry.copperPrice + " copper", () => Buy(entry));
                buy.interactable = entry.copperPrice > 0 && PlayerData.Instance.totalCopper >= entry.copperPrice
                    && GameManager.Instance.itemList.Count < GameManager.Instance.maxInventorySlots;
            }
            Label("Purchased gear goes to your bag. Equip it at the guild hall.", 24);
            MakeButton(content, "Manage heroes & equipment", ShowRoster); MakeButton(content, "Back to town", Close);
        }
        void Buy(TownShopOffer offer)
        {
            var gm = GameManager.Instance; var player = PlayerData.Instance;
            if (offer == null || offer.equipment == null || offer.copperPrice <= 0 || player.totalCopper < offer.copperPrice)
            { ShowShop("You need more copper. Random encounters pay copper on victory."); return; }
            if (gm.itemList.Count >= gm.maxInventorySlots) { ShowShop("Your bag is full. Equip some gear first."); return; }
            var item = Instantiate(offer.equipment);
            if (item.guaranteedStatRanges.Count == 0 && item.secretStatRanges.Count == 0)
                item.LoadRolledData(System.Guid.NewGuid().ToString(), item.attackBonus, item.defenseBonus, item.maxHealthBonus, item.maxEnergyBonus, item.speedBonus);
            else item.RollNewStats();
            gm.AddItem(item, 1);
            if (!gm.itemList.Contains(item)) { Destroy(item); ShowShop("Your bag is full."); return; }
            player.totalCopper -= offer.copperPrice; player.SavePlayerData();
            ShowShop("Purchased " + item.itemName + ": " + item.GetStatDescription());
        }
        void ShowRoster()
        {
            OpenPanel(); Label("Guild hall", 32); Label("Choose up to two heroes. Prepare their gear and skills before venturing out.", 25);
            var gm = GameManager.Instance;
            foreach (var hero in gm.allCharacterData.Where(h => h != null).GroupBy(h => h.heroID).Select(g => g.First()))
            {
                var selected = hero; Art(hero.heroIcon, 85);
                var entry = MakeButton(content, hero.heroID + " - Lv " + hero.heroLevel +
                    (gm.currentPartyHeroIDs.Contains(hero.heroID) ? " - In party" : "") + "\n" + gm.GetHeroUnlockStatus(hero), () => ShowHero(selected));
                entry.interactable = gm.IsHeroAvailable(hero) || gm.currentPartyHeroIDs.Contains(hero.heroID);
            }
            MakeButton(content, "Back to town", Close);
        }
        void ShowHero(CharacterData hero, string notice = null)
        {
            OpenPanel(); var gm = GameManager.Instance; gm.currentCompanionData = hero;
            Art(hero.heroIcon, 95); Label(hero.heroID + " - Level " + hero.heroLevel, 32);
            Label(gm.GetHeroUnlockStatus(hero), 22);
            if (hero.heroID == Buffmage.HeroID) Label("Strength in place of sorcery. High ATK and HP; slower actions and lighter defense.", 22);
            Label("HP " + hero.health + "/" + hero.maxHealth + " - XP " + hero.heroExp + "/" + hero.ExpToNextLevel(hero.heroLevel)
                + "\nATK " + hero.attackPower + "  DEF " + hero.defensePower + "  SPD " + hero.speed, 25);
            if (notice != null) Label(notice, 24);
            bool inParty = gm.currentPartyHeroIDs.Contains(hero.heroID);
            var partyButton = MakeButton(content, inParty ? "Remove from party" : "Add to party", () =>
            {
                if (inParty && gm.currentPartyHeroIDs.Count > 1) gm.RemoveCharacterDataFromParty(hero);
                else if (!inParty && gm.currentPartyHeroIDs.Count < 2) gm.AddCharacterDataToParty(hero);
                ShowHero(hero);
            });
            partyButton.interactable = inParty ? gm.currentPartyHeroIDs.Count > 1 : gm.IsHeroAvailable(hero) && gm.currentPartyHeroIDs.Count < 2;
            Label("Equipment", 28);
            foreach (var pair in hero.GetEquippedDictionary()) Label(pair.Key + ": " + pair.Value.itemName + " " + pair.Value.GetStatDescription(), 23);
            foreach (var equipment in gm.itemList.OfType<Equipment>().ToArray())
            {
                var item = equipment;
                MakeButton(content, "Equip " + item.itemName + " - " + item.GetStatDescription(), () =>
                {
                    var manager = gm.equipmentManager != null ? gm.equipmentManager : FindFirstObjectByType<EquipmentManager>();
                    if (manager == null || !gm.itemList.Contains(item)) { ShowHero(hero, "Equipment is unavailable."); return; }
                    gm.currentCompanionData = hero; manager.Equip(item); SyncHero(hero); ShowHero(hero, "Equipped " + item.itemName);
                });
            }
            Label("Skills - choose " + hero.maxEquippedSkills, 28);
            foreach (var skillType in hero.AvailableSkills.ToArray())
            {
                var type = skillType; var skill = hero.GetSkillInstance(type); if (skill == null) continue;
                if (hero.heroID == Buffmage.HeroID) Label(skill.skillName + "\n" + skill.description + "\nMP " + skill.energyCost + " / Cooldown " + skill.cooldownTurns + " rounds", 22);
                bool equipped = hero.equippedSkills.Contains(type);
                var button = MakeButton(content, (equipped ? "[Equipped] " : "+ ") + skill.skillName, () =>
                {
                    if (hero.equippedSkills.Contains(type)) { if (hero.equippedSkills.Count > 1) hero.equippedSkills.Remove(type); }
                    else if (hero.equippedSkills.Count < hero.maxEquippedSkills) hero.equippedSkills.Add(type);
                    hero.SaveData(); SyncHero(hero); ShowHero(hero);
                });
                button.interactable = equipped ? hero.equippedSkills.Count > 1 : hero.equippedSkills.Count < hero.maxEquippedSkills;
            }
            MakeButton(content, "Back to guild hall", ShowRoster);
        }
        void ShowInn(string notice = null)
        {
            OpenPanel(); Label("The Questionable Pillow", 32); Label("Rest your heroes. Full health and energy; no charge for the first guild in town. Also the only guild.", 26);
            if (notice != null) Label(notice, 25);
            MakeButton(content, "Rest the party - free", () =>
            {
                foreach (var hero in GameManager.Instance.allCharacterData.Where(h => h != null && GameManager.Instance.currentPartyHeroIDs.Contains(h.heroID)))
                { hero.health = hero.maxHealth; hero.energy = hero.maxEnergy; hero.stamina = hero.maxStamina; hero.SaveData(); SyncHero(hero); }
                ShowInn("Your party is rested and ready.");
            });
            MakeButton(content, "Back to town", Close);
        }
        static void SyncHero(CharacterData hero)
        {
            foreach (var companion in GameManager.Instance.companions)
                if (companion != null && companion.heroID == hero.heroID) companion.SetCharacterData(hero);
            foreach (var companion in GameManager.Instance.currentParty)
                if (companion != null && companion.heroID == hero.heroID) companion.SetCharacterData(hero);
        }
    }
}
