using System.Collections;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CompanionSpawnController : MonoBehaviour
{
    public GameObject companionPrefab;
    public Transform[] companionSpawnPoints;
    public Slider associatedHealthBarSlider;
    public Slider associatedEnergyBarSlider;
    public TextMeshProUGUI associatedHealthText;
    public TextMeshProUGUI associatedEnergyText;
    public Slider associatedHealthBarSlider2;
    public Slider associatedEnergyBarSlider2;
    public TextMeshProUGUI associatedHealthText2;
    public TextMeshProUGUI associatedEnergyText2;
    public GameObject heroHealthUI1;
    public GameObject heroHealthUI2;
    public BattleManager battleManager;
    public List<Character> activeCompanions = new List<Character>();

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentParty != null)
        {
            int companionIndex = 0;
            foreach (Companion companion in GameManager.Instance.currentParty)
            {
                Character instantiatedCompanion = GameManager.Instance.InstantiateSelectedCompanion(companion.heroID);
                Companion instantiatedCompanionAsCompanion = instantiatedCompanion as Companion;

                if (instantiatedCompanionAsCompanion != null)
                {
                    instantiatedCompanionAsCompanion.SetCharacterData(companion.characterData);
                    instantiatedCompanionAsCompanion.InitializeSkillsBasedOnLevel();
                    SetupCompanions(instantiatedCompanionAsCompanion, companionIndex);
                    activeCompanions.Add(instantiatedCompanionAsCompanion);
                    instantiatedCompanionAsCompanion.isSelected = true;
                    instantiatedCompanionAsCompanion.originalPosition = companionSpawnPoints[companionIndex].position;

                    Debug.Log($"Companion {companion.heroID} instantiated at index {companionIndex} and {instantiatedCompanionAsCompanion.originalPosition} location");
                }
                companionIndex++;
            }
        }
        else
        {
            Debug.LogWarning("GameManager or currentParty is null.");
        }
    }

    public void SetupCompanion(Character companionCharacter, int index)
    {
        var existingCompanion = activeCompanions.FirstOrDefault(c => c.characterIDNumber == companionCharacter.characterIDNumber);
        if (existingCompanion != null)
        {
            RemoveCompanion(existingCompanion);
        }

        if (index >= companionSpawnPoints.Length)
        {
            Debug.Log("All spawn points are occupied.");
            return;
        }

        Transform spawnPoint = companionSpawnPoints[index];

        companionCharacter.transform.position = spawnPoint.position;
        companionCharacter.transform.rotation = spawnPoint.rotation;

        if (index == 0)
        {   
            Debug.Log("INdex was 0");
            SetupBars(companionCharacter, associatedHealthBarSlider, associatedEnergyBarSlider, associatedHealthText, associatedEnergyText, heroHealthUI1);
        }
        else if (index == 1)
        {
            Debug.Log("INdex was 1");
            SetupBars(companionCharacter, associatedHealthBarSlider2, associatedEnergyBarSlider2, associatedHealthText2, associatedEnergyText2, heroHealthUI2);
        }

        activeCompanions.Add(companionCharacter);
        companionCharacter.isSelected = true;
    }

    private void SetupBars(Character character, Slider healthBar, Slider energyBar, TextMeshProUGUI healthText, TextMeshProUGUI energyText, GameObject heroHealthUI)
    {
        heroHealthUI.SetActive(true);
        character.healthText = healthText;
        healthBar.maxValue = character.maxHealth;
        healthBar.value = character.health;
        character.healthBar = healthBar;
        healthText.text = character.health.ToString();

        character.energyText = energyText;
        energyBar.maxValue = character.maxEnergy;
        energyBar.value = character.energy;
        character.energyBar = energyBar;
        energyText.text = character.energy.ToString();

        character.enemyHealthUI = heroHealthUI;
    }

    public void RemoveCompanion(Character companionCharacter)
    {
        if (activeCompanions.Remove(companionCharacter))
        {
            companionCharacter.isSelected = false;
            DisableHeroUI(companionCharacter);

            if (battleManager.playerParty.Contains(companionCharacter))
            {
                battleManager.playerParty.Remove(companionCharacter);
            }

            Destroy(companionCharacter.gameObject);
        }
    }

    public void CreateHeroSelectionUI()
    {
        foreach (Transform child in battleManager.heroSelectionPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Companion companion in GameManager.Instance.companions)
        {
            GameObject buttonObj = Instantiate(battleManager.heroButtonPrefab, battleManager.heroSelectionPanel.transform);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = companion.heroID;

            Companion localCompanion = companion;
            buttonObj.GetComponent<Button>().onClick.AddListener(() => OnHeroSelected(localCompanion));
        }

        battleManager.battleStartButton.SetActive(true);
    }

    private int selectedCompanionCount = 0;

    public void OnHeroSelected(Companion selectedCompanion)
    {
        if (selectedCompanion.isSelected)
        {
            RemoveCompanionFromGame(selectedCompanion);
            selectedCompanion.isSelected = false;
            selectedCompanionCount--;
        }
        else
        {
            Character instantiatedCompanion = GameManager.Instance.InstantiateSelectedCompanion(selectedCompanion.heroID);
            Companion companion = instantiatedCompanion as Companion;
            if (instantiatedCompanion != null)
            {
                companion.SetCharacterData(selectedCompanion.characterData);
                companion.InitializeSkillsBasedOnLevel();
                SetupCompanion(instantiatedCompanion, selectedCompanionCount);
                AssignCompanion(instantiatedCompanion);
                EnableHeroUI(instantiatedCompanion);
                selectedCompanion.InitializeSkillsBasedOnLevel();
                selectedCompanion.isSelected = true;
            }
        }
    }

    private void AssignCompanion(Character companion)
    {
        if (selectedCompanionCount == 0)
        {
            battleManager.companion1 = companion;
        }
        else if (selectedCompanionCount == 1)
        {
            battleManager.companion2 = companion;
        }

        selectedCompanionCount++;
    }

    private void SetupCompanions(Character selectedCompanion, int index)
    {
        SetupCompanion(selectedCompanion, index);
        battleManager.playerParty.Add(selectedCompanion);
    }

    public void EnableHeroUI(Character selectedCompanion)
    {
        selectedCompanion.healthBar.gameObject.SetActive(true);
        selectedCompanion.energyBar.gameObject.SetActive(true);
    }

    public void DisableHeroUI(Character selectedCompanion)
    {
        selectedCompanion.healthBar.gameObject.SetActive(false);
        selectedCompanion.energyBar.gameObject.SetActive(false);
    }

    private void RemoveCompanionFromGame(Companion companion)
    {
        var characterToRemove = activeCompanions.FirstOrDefault(c => c.characterIDNumber == companion.characterIDNumber);
        if (characterToRemove != null)
        {
            RemoveCompanion(characterToRemove);
        }
    }
}
