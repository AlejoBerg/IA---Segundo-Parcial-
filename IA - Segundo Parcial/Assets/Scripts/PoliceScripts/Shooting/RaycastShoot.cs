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
        if (Time.time > nextFire) //Si apreta click izquierdo y si paso suficiente tiempo desde el ultimo disparo 
        {
            nextFire = Time.time + fireRate; //Cada vez que disparo guardo el tiempo actual + fire rate y cuando time.time haya alcanzado ese valor puedo disparar denuevo.

            StartCoroutine(ShotEffect());
            RaycastHit hit;

            laserLine.SetPosition(0, gunEnd.position);
            if (Physics.Raycast(shootPosition.transform.position, shootPosition.transform.forward, out hit, weaponRange, _layerToAvoid))
            {
                print($"Dispare raycast le pegue al {hit.collider.gameObject.name}");
                laserLine.SetPosition(1, hit.point); //Si colisiona muestro la linea ahi

                IShoot health = hit.collider.GetComponent<IShoot>(); //En vez de pasarle shootable box, le paso una interface idamageable que tenga la funcion Damage que la implementen los vandidos y la policia 
                if (health != null)
                {
                    health.GetDamage(gunDamage);
                }

                //if (hit.rigidbody != null)
                //{
                //    hit.rigidbody.AddForce(-hit.normal * hitForce); //Direccion opuesta a la que choco la bala 
                //}
            }
            else
            {
                laserLine.SetPosition(1, shootPosition.transform.position + shootPosition.transform.forward * weaponRange); //Si no choco contra nada dibujar el rayo 
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
