using UnityEngine;
using HoloToolkit.Unity;

public class UnitSpawner : MonoBehaviour
{
    public GameObject Agent;
    
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnSelect()
    {
        GameObject agent = (GameObject)Instantiate(Agent);
        RaycastHit hitInfo = GazeManager.Instance.HitInfo;
        agent.transform.position = hitInfo.point;
        agent.SetActive(true);
    }

    Vector3 appliedDelta = Vector3.zero;

    void OnManipulationStarted(Vector3 cumulativeDelta)
    {
        Debug.Log("Started " + GestureManager.Instance.heldObject.name);
        applyManipulation(cumulativeDelta);
    }

    void OnManipulationUpdated(Vector3 cumulativeDelta)
    {
        //Debug.Log("Updated " + GestureManager.Instance.heldObject.name);
        applyManipulation(cumulativeDelta);
    }

    void OnManipulationCompleted(Vector3 cumulativeDelta)
    {
        Debug.Log("Completed " + GestureManager.Instance.heldObject.name);
        applyManipulation(cumulativeDelta);
    }

    void OnManipulationCanceled(Vector3 cumulativeDelta)
    {
        Debug.Log("Canceled " + GestureManager.Instance.heldObject.name);
        //GestureManager.Instance.heldObject.transform.Translate(-appliedDelta);
    }

    void applyManipulation(Vector3 cumulativeDelta)
    {
        Debug.Log("cumulative: " + cumulativeDelta);
        Debug.Log("change:     " + (cumulativeDelta - appliedDelta));
        //GestureManager.Instance.heldObject.transform.Translate(cumulativeDelta - appliedDelta);
    }
}
