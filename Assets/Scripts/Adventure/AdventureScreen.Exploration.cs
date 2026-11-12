using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StepQuest.Adventure
{
    public partial class AdventureScreen
    {
        Button explorationButton;
        TMP_Text explorationBalance, explorationStatus;
        string explorationError;

        void BuildExploration(Transform body, Transform footer)
        {
            explorationError = null;
            var header = ui.Panel(body, "Location header", true); ui.Height(header, 68);
            var location = ui.Text(header.transform, "BRAMBLE VILLAGE", 18, ui.Theme.gold);
            location.margin = new Vector4(12, 5, 12, 34) * UiScale;
            var title = ui.Text(header.transform, "Beyond the Town Gates", 28);
            title.margin = new Vector4(12, 28, 12, 4) * UiScale;
            Art(dialogue.forestBackdrop, 172, true);
            ui.Paragraph(body, "Follow the forest road. A new encounter awaits.");

            explorationBalance = ui.InfoRow(body, "ENCOUNTER", townHub.randomBattleStepCost + " steps");
            var copper = ui.Theme.copperIcon != null ? ui.Theme.copperIcon : Resources.Load<Sprite>("Sprites/GUI/ui/CoinIcon_16x18");
            ui.InfoRow(body, "REWARDS", "Copper + hero XP", copper);
            var partyHeading = ui.Text(body, "ACTIVE PARTY", 19, ui.Theme.gold);
            ui.Height(partyHeading, 26);
            var gm = GameManager.Instance;
            if (gm != null && gm.currentPartyHeroIDs.Count > 0)
            {
                var row = RpgUi.Rect("Active party", body);
                var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
                layout.spacing = 6 * UiScale;
                layout.childControlWidth = layout.childControlHeight = true;
                layout.childForceExpandWidth = true; layout.childForceExpandHeight = false;
                ui.Height(row, 64);
                foreach (var id in gm.currentPartyHeroIDs)
                {
                    var hero = gm.allCharacterData.FirstOrDefault(h => h != null && h.heroID == id);
                    if (hero == null) continue;
                    // Portrait, name, level, and health remain grouped in each compact slot.
                    var card = ui.Panel(row, "Party status", true);
                    card.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
                    var portrait = Image("Hero portrait", card.transform, hero.heroIcon);
                    portrait.preserveAspect = true; portrait.raycastTarget = false;
                    Place(portrait.rectTransform, new Vector2(.03f, .12f), new Vector2(.25f, .88f));
                    var name = ui.Text(card.transform, hero.heroID + "  Lv " + hero.heroLevel, 18);
                    name.textWrappingMode = TextWrappingModes.NoWrap;
                    name.overflowMode = TextOverflowModes.Ellipsis;
                    Place(name.rectTransform, new Vector2(.28f, .49f), new Vector2(.96f, .94f));
                    var health = ui.Text(card.transform, "HP " + hero.health + "/" + hero.maxHealth, 17, ui.Theme.muted);
                    Place(health.rectTransform, new Vector2(.28f, .09f), new Vector2(.96f, .49f));
                }
            }
            else ui.Paragraph(body, "No heroes selected. Visit the guild hall.");

            explorationStatus = ui.Text(footer, "", 18, ui.Theme.muted);
            ui.Height(explorationStatus, 36);
            explorationStatus.alignment = TextAlignmentOptions.Center;
            explorationButton = ui.Button(footer, "FIND A BATTLE  <size=75%>" + townHub.randomBattleStepCost + " steps</size>", () =>
            {
                if (!AdventureProgress.LaunchRandom(townHub, out var error))
                { explorationError = error; RefreshExploration(); }
            }, RpgButtonKind.Primary);

            var navigation = RpgUi.Rect("Secondary actions", footer);
            ui.Height(navigation, 46);
            var actions = navigation.gameObject.AddComponent<HorizontalLayoutGroup>();
            actions.spacing = 6 * UiScale; actions.childControlWidth = actions.childControlHeight = true;
            actions.childForceExpandWidth = true; actions.childForceExpandHeight = false;
            var heroes = ui.Button(navigation, "MANAGE HEROES", ShowRoster);
            ui.Height(heroes, 46);
            var back = ui.Button(navigation, "BACK TO TOWN", Close, RpgButtonKind.Navigation);
            ui.Height(back, 46);
            foreach (var label in navigation.GetComponentsInChildren<TMP_Text>()) label.fontSize = 19 * UiScale;
            RefreshExploration();
        }

        void RefreshExploration()
        {
            if (explorationButton == null || townHub == null) return;
            var player = PlayerData.Instance; var gm = GameManager.Instance;
            int steps = player != null ? player.inGameSteps : 0;
            bool partyReady = gm != null && gm.currentPartyHeroIDs.Count > 0;
            if (partyReady)
                partyReady = gm.currentPartyHeroIDs.All(id => gm.allCharacterData.Any(h => h != null && h.heroID == id && h.health > 0 && gm.IsHeroAvailable(h)));
            bool affordable = player != null && steps >= townHub.randomBattleStepCost;
            bool busy = gm != null && (gm.InDungeonRun || AdventureProgress.InStandaloneBattle);
            explorationButton.interactable = affordable && partyReady && !busy;
            string balance = townHub.randomBattleStepCost + " steps  /  " + steps + " owned";
            if (explorationBalance != null && explorationBalance.text != balance) explorationBalance.text = balance;
            if (explorationBalance != null) explorationBalance.color = affordable ? ui.Theme.light : ui.Theme.warning;
            string status = explorationError ?? (busy ? "Finish your current encounter first."
                : !partyReady ? "Choose heroes and rest at the inn before leaving."
                : !affordable ? "Walk " + Mathf.Max(0, townHub.randomBattleStepCost - steps) + " more steps to explore."
                : "Your party is ready to venture out.");
            if (explorationStatus.text != status) explorationStatus.text = status;
            explorationStatus.color = explorationButton.interactable ? ui.Theme.muted : ui.Theme.warning;
        }
    }
}
