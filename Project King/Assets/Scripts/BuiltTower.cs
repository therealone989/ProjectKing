using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class BuiltTower : MonoBehaviour
{
    [Header("SensorSettings")]
    [SerializeField] float buildRange = 7f;
    [SerializeField] LayerMask buildLayer;

    [Header("Colors")] 
    Color activeColor = new Color(1.0f,1.0f, 1.0f, 1.0f);
    Color inactiveColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    Color hiddenColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);

    [Header("UI References")]
    public Button button;
    public GameObject buildPanel;

    [Header("Tower Prefabs")]
    public GameObject canonPrefab;
    public GameObject archerPrefab;
    public GameObject wizardPrefab;
    public GameObject troopsPrefab;

    private GameObject currentBuildSpot;
    public enum TowerType{Cannon,Archer,Wizard,Troops}
    private Collider[] hitResults = new Collider[5]; // Speicher-Reservierung für NonAlloc

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
        // 1. Nur EINE Abfrage ohne neuen Speicher (NonAlloc)
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, buildRange, hitResults, buildLayer);

        GameObject nearestSpot = null;
        float shortestDistance = Mathf.Infinity;

        // 2. Den mathematisch nächsten Spot finden
        for (int i = 0; i < hitCount; i++)
        {
            float distance = Vector3.Distance(transform.position, hitResults[i].transform.position);
            if(distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestSpot = hitResults[i].gameObject;
            }
        }
        // 3. Logik-Check: Hat sich der gewählte Spot geändert?
        if (nearestSpot != currentBuildSpot)
        {
            currentBuildSpot = nearestSpot;
            if (currentBuildSpot != null){ SetButtonState(true, activeColor); }
            else { SetButtonState(false, inactiveColor); }
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
            case TowerType.Cannon:SpawnTower(canonPrefab);break;
            case TowerType.Archer:SpawnTower(archerPrefab);break;
            case TowerType.Wizard:SpawnTower(wizardPrefab);break;
            case TowerType.Troops:SpawnTower(troopsPrefab);break;
            default:break;
        }
        ExitMenu();
    }
    public void SpawnTower(GameObject towerPrefab)
    {
        Instantiate(towerPrefab,currentBuildSpot.transform.position, towerPrefab.transform.rotation);

        currentBuildSpot.SetActive(false);
    }
    public void ExitMenu()
    {
        buildPanel.SetActive(false);
        SetButtonState(true, activeColor);
    }
}
