using UnityEngine;
using UnityEngine.UI;

public class BuiltTower : MonoBehaviour
{
    public Button button;
    Color newColor;
    public GameObject buildPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        newColor = new Color(1.0f,1.0f, 1.0f, 0.5f); 
        button.enabled = false;
        button.image.color = newColor;
        buildPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Buildspot")
        {
            button.enabled = true;
            newColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            button.image.color = newColor;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Buildspot")
        {
            button.enabled = false;
            newColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            button.image.color = newColor;
        }
    }
    public void PlaceCanon()
    {
        Debug.Log("Place Canon!");
    }
    public void PlaceArcher()
    {
        Debug.Log("Place Archer!");
    }
    public void PlaceWizard()
    {
        Debug.Log("Place Wizard!");
    }
    public void PlaceTroops()
    {
        Debug.Log("Place Troops!");
    }
    public void ActivateBuildPanel()
    {
        buildPanel.SetActive(true);
    }
}
