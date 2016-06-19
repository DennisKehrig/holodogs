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

    GameObject targetObject = null;
    Vector3 initialPosition = Vector3.zero;
    Vector3 combinedDelta = Vector3.zero;

    void OnManipulationStarted(Vector3 cumulativeDelta)
    {
        targetObject = GestureManager.Instance.heldObject;
        Debug.Log("Started " + targetObject.name);
        while (targetObject != null && targetObject.GetComponent<Rigidbody>() == null)
        {
            targetObject = targetObject.transform.parent.gameObject;
            if (targetObject != null)
                Debug.Log("Trying " + targetObject.name + " instead");
        }
        if (targetObject == null)
            return;

        Rigidbody rigidbody = targetObject.GetComponent<Rigidbody>();
        rigidbody.detectCollisions = false;
        rigidbody.isKinematic = true;

        initialPosition = targetObject.transform.position;
        combinedDelta = cumulativeDelta;
        applyManipulation();
    }

    void OnManipulationUpdated(Vector3 cumulativeDelta)
    {
        if (targetObject == null)
            return;
        //Debug.Log("Updated " + targetObject.name);
        combinedDelta = cumulativeDelta;
        applyManipulation();
    }

    void OnManipulationCompleted(Vector3 cumulativeDelta)
    {
        if (targetObject == null)
            return;
        Debug.Log("Completed " + targetObject.name);
        combinedDelta = cumulativeDelta;
        applyManipulation();

        Rigidbody rigidbody = targetObject.GetComponent<Rigidbody>();
        rigidbody.detectCollisions = true;
        rigidbody.isKinematic = false;
    }

    void OnManipulationCanceled(Vector3 cumulativeDelta)
    {
        if (targetObject == null)
            return;
        Debug.Log("Canceled " + targetObject.name);
        targetObject.transform.position = initialPosition;

        Rigidbody rigidbody = targetObject.GetComponent<Rigidbody>();
        rigidbody.detectCollisions = true;
        rigidbody.isKinematic = false;
    }

    void applyManipulation()
    {
        Debug.Log("Combined delta: " + combinedDelta);
        targetObject.transform.position = initialPosition + combinedDelta;
    }
}
