using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMPLoader : MonoBehaviour {

    FlatlanderControllerMono ps;
    [SerializeField]
    ParticleSystem smoke, boom, explosion;
    [SerializeField]
    MonsterManagerMono mm;
    [SerializeField]
    float pmpDamage;
    [SerializeField]
    AudioClip charge, fire, load;
    [SerializeField]
    Color unloaded, loaded, fired;
    [SerializeField]
    Renderer[] renders;

    public PMPTrigger button;

    bool isLoaded = false, hasFired = false;
    GameObject ammo;

	// Use this for initialization
	void Start () {
        if(mm == null)
            mm = FindObjectOfType<MonsterManagerMono>();
    }

    private void Reset()
    {
        mm = FindObjectOfType<MonsterManagerMono>();
    }

    private void OnEnable()
    {
        EventManager.StartListening("Game Start", ResetLoader);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Game Start", ResetLoader);
    }



    void OnTriggerEnter(Collider col)
    {
        if (hasFired)
            return;

        ps = col.gameObject.GetComponent<FlatlanderControllerMono>();

        if (ps != null)
        {
            if(!isLoaded && ps.voluminiumShard)
            {

                isLoaded = true;
                ammo = ps.voluminiumShard;
                ammo.transform.position = transform.position;
                ChangeColour(loaded);
                ps.voluminiumShard.transform.parent = transform;
                ps.RemoveVoluminium();
                SoundManager.instance.PMPPlayClip(load);
            }
        }
    }

    IEnumerator WaitForCharge(float duration)
    {
        while(duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }

        Fire();
    }

    public void ResetLoader()
    {
        isLoaded = false;
        hasFired = false;
        smoke.Stop();

        ChangeColour(unloaded);

        if(ammo != null)
        {
            Destroy(ammo);
            ammo = null;
        }
    }

    void ChangeColour(Color colour)
    {
        foreach (Renderer renderer in renders)
        {
            Material mat = renderer.material;
            mat.color = colour;
            mat.SetColor("_EmissionColor", colour);
            
        }
    }

    public void Charge()
    {
        if (isLoaded)
        {
            SoundManager.instance.PMPPlayClip(charge);
            isLoaded = false;
            StartCoroutine(WaitForCharge(charge.length));
        }
    }

    public void Fire()
    {

            Destroy(ammo);


            boom.Play();
            explosion.Play();
            smoke.Play();

        ChangeColour(fired);

            SoundManager.instance.PMPPlayClip(fire);

            mm.TakeDamage();
            mm.ChangeVoluminium(-pmpDamage);
            hasFired = true;

        
    }
}
