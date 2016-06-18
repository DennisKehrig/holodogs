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
}
