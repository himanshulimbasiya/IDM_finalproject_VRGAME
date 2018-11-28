using UnityEngine;
using UnityEngine.Networking;

public class BehavioursToDisable : NetworkBehaviour {

    public Behaviour[] toDisable;

	// Use this for initialization
	void Start () {
        if (!isLocalPlayer)
        {
            foreach (Behaviour component in toDisable)
            {
                component.enabled = false;
            }
        }
	}
	

}
