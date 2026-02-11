using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuiltTower : MonoBehaviour
{
    [Header("Colors")] 
    Color activeColor = new Color(1.0f,1.0f, 1.0f, 1.0f);
    Color inactiveColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    Color hiddenColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);

    [Header("UI References")]
    public Button button;
    public GameObject buildPanel;

    [Header("Tower References")]
    private GameObject currentBuildSpot;

    [Header("Tower Prefabs")]
    public GameObject canonPrefab;
    public GameObject archerPrefab;
    public GameObject wizardPrefab;
    public GameObject troopsPrefab;
    public enum TowerType{Cannon,Archer,Wizard,Troops}
    void Start()
    {
        SetButtonState(false, inactiveColor);
        buildPanel.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Buildspot")
        {
            SetButtonState(true, activeColor);
            currentBuildSpot = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Buildspot")
        {
            SetButtonState(false, inactiveColor);
        }
    }
    public void ActivateBuildPanel()
    {
        buildPanel.SetActive(true);
        SetButtonState(false, hiddenColor);
    }
    private void SetButtonState(bool isEnabled, Color color)
    {
        button.interactable = isEnabled;
        button.image.color = color;
    }
    public void PlaceTower(int towerIndex)
    {
        TowerType selectedTower = (TowerType)towerIndex;
        Debug.Log("Gewaehlter Tower: " + selectedTower);
        switch (selectedTower)
        {
            case TowerType.Cannon:
                Debug.Log("Kanone wird gebaut!");
                SpawnTower(canonPrefab);
                break;
            case TowerType.Archer:
                Debug.Log("Archer wird gebaut!");
                SpawnTower(archerPrefab);
                break;
            case TowerType.Wizard:
                SpawnTower(wizardPrefab);
                Debug.Log("Wizard wird gebaut!");
                break;
            case TowerType.Troops:
                SpawnTower(troopsPrefab);
                Debug.Log("Troops wird gebaut!");
                break;
            default:
                break;
        }
        ExitMenu();
    }
    public void SpawnTower(GameObject towerPrefab)
    {
        Instantiate(towerPrefab,currentBuildSpot.transform.position, currentBuildSpot.transform.rotation);
    }
    public void ExitMenu()
    {
        buildPanel.SetActive(false);
        SetButtonState(true, activeColor);
    }
}
