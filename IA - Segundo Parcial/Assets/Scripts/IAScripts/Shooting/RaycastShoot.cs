using System;
using System.Collections;
using UnityEngine;

public class RaycastShoot : MonoBehaviour
{
    [SerializeField] private int gunDamage = 1;
    [SerializeField] private float fireRate = 0.25f; 
    [SerializeField] private float weaponRange = 50f; // Distancia del raycast
    [SerializeField] private float hitForce = 100f; //Cuanto impulso le va a hacer al target
    [SerializeField] private Transform gunEnd; //desded donde sale el raycast, empty
    [SerializeField] private Transform shootPosition; //Puede ser gun end o puede ser la camara 
    [SerializeField] private LayerMask _layerToAvoid;

    private WaitForSeconds shotDuration = new WaitForSeconds(0.1f);
    private AudioSource gunAudio;
    private LineRenderer laserLine;
    private IShoot shooterRef;
    private float nextFire;

    private void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        shooterRef = GetComponentInParent<IShoot>();
        shooterRef.OnShoot += OnShootHandler;
    }

    public void OnShootHandler()
    {
        if (Time.time > nextFire) 
        {
            nextFire = Time.time + fireRate; 

            StartCoroutine(ShotEffect());
            RaycastHit hit;

            laserLine.SetPosition(0, gunEnd.position);
            if (Physics.Raycast(shootPosition.transform.position, shootPosition.transform.forward, out hit, weaponRange, _layerToAvoid))
            {
                laserLine.SetPosition(1, hit.point); 
                print("dispare");
                IShoot health = hit.collider.GetComponent<IShoot>();  
                if (health != null)
                {
                    print("health != null");
                    health.GetDamage(gunDamage);
                }
            }
            else
            {
                laserLine.SetPosition(1, shootPosition.transform.position + shootPosition.transform.forward * weaponRange); 
            }
        }
    }

    private IEnumerator ShotEffect()
    {
        gunAudio.Play();
        laserLine.enabled = true;
        yield return shotDuration;
        laserLine.enabled = false;
    }

}
