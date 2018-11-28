using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {

    [SerializeField]
    Collider collider;

    List<Collider> ignoredColliders = new List<Collider>();

    public void Start()
    {
        IgnoreAllFlatlanders();
    }

    void IgnoreAllFlatlanders()
    {
        GameObject[] gos = GameManagerMono.instance.GetFlatalnders().ToArray();

        foreach(GameObject flatlander in gos)
        {
            Collider[] colliders = flatlander.GetComponents<Collider>();
            foreach(Collider col in colliders)
            {
                Physics.IgnoreCollision(collider, col);
                AddToIgnored(col);
            }
        }
    }

    public void AddToIgnored(Collider col)
    {
        if(!ignoredColliders.Contains(col))
            ignoredColliders.Add(col);
    }

    public void RemoveIgnored(Collider col)
    {
        if (ignoredColliders.Contains(col))
            ignoredColliders.Remove(col);

        
    }

    public List<Collider> GetIgnoredColliders()
    {
        return ignoredColliders;
    }
}
