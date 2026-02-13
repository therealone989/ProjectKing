using UnityEngine;

public class BuildSpot : MonoBehaviour
{

    [SerializeField] private ParticleSystem buildEffect;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Collider buildCollider;

    public void PlayBuildEffect()
    {
        if (buildEffect != null)
        {
            // Partikel abkoppeln, damit sie weiterfliegen, 
            // auch wenn der Bauplatz deaktiviert wird
            buildEffect.transform.SetParent(null);
            buildEffect.Play();

            // Optional: Das Partikel-Objekt nach dem Abspielen zerstören/löschen
            //Destroy(buildEffect.gameObject, buildEffect.main.duration + 1f);
        }
    }

    public void DisableSpot()
    {
        // Anstatt das ganze Objekt zu deaktivieren, schalten wir nur Sichtbarkeit 
        // und Physik aus. So können Partikel-Kinder zu Ende spielen.
        meshRenderer.enabled = false;
        buildCollider.enabled = false;
    }
}
