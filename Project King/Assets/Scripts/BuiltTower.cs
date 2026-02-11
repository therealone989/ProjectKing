using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class BuiltTower : MonoBehaviour
{
    [Header("SensorSettings")]
    [SerializeField] float buildRange = 7f;

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
    private void Update()
    {
        if(Time.frameCount % 10 == 0)
        {
            CheckForBuildSpot();
        }
    }
    void CheckForBuildSpot()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, buildRange);
        bool spotFound = false;

        foreach(var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Buildspot"))
            {
                currentBuildSpot = hitCollider.gameObject;
                SetButtonState(true, activeColor);
                spotFound = true;
                break;
            }
        }

        if(!spotFound && currentBuildSpot != null)
        {
            currentBuildSpot = null;
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
        switch (selectedTower)
        {
            case TowerType.Cannon:
                SpawnTower(canonPrefab);
                break;
            case TowerType.Archer:
                SpawnTower(archerPrefab);
                break;
            case TowerType.Wizard:
                SpawnTower(wizardPrefab);
                break;
            case TowerType.Troops:
                SpawnTower(troopsPrefab);
                break;
            default:
                break;
        }
        ExitMenu();
    }
    public void SpawnTower(GameObject towerPrefab)
    {
        Instantiate(towerPrefab,currentBuildSpot.transform.position, currentBuildSpot.transform.rotation);

        currentBuildSpot.SetActive(false);
    }
    public void ExitMenu()
    {
        buildPanel.SetActive(false);
        SetButtonState(true, activeColor);
    }
}
