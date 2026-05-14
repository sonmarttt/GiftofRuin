using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    public float grabDistance = 3f;
    public float holdDistance = 2f;
    public LayerMask grabLayer;

    private GrabbableCorpse heldCorpse = null;
    private Transform camTransform;

    void Start()
    {
        camTransform = Camera.main.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldCorpse != null)
                Release();
            else
                TryGrab();
        }

        if (heldCorpse != null)
        {
            Vector3 targetPos = camTransform.position + camTransform.forward * holdDistance;
            heldCorpse.MoveToPosition(targetPos);
        }
    }

    private void TryGrab()
    {
        GrabbableCorpse closest = null;
        float closestDist = grabDistance;

        // find all corpses in the scene
        foreach (var corpse in FindObjectsOfType<GrabbableCorpse>())
        {
            if (!corpse.IsDead) continue;
            float dist = Vector3.Distance(transform.position, corpse.transform.position);
            Debug.Log("Found dead corpse at distance: " + dist);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = corpse;
            }
        }

        if (closest != null)
        {
            heldCorpse = closest;
            heldCorpse.Grab();
            Debug.Log("Grabbed corpse!");
        }
        else
        {
            Debug.Log("No corpse in range (" + grabDistance + " units)");
        }
    }

    private void Release()
    {
        heldCorpse.Release();
        heldCorpse = null;
    }
}