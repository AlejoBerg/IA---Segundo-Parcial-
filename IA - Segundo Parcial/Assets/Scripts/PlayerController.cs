using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IShoot
{
    public event Action OnShoot;
    public event Action OnDead;
    public event Action OnWin;
    private Animator anim;

    private float life = 100;

    private void Start()
    {
        GameManager.Instance.bandides.Add(this.gameObject);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponent<Animator>();

    }

    public void GetDamage(float _damage)
    {
        if(life - _damage < 0)
        {

            StartCoroutine(Delay());
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            life -= _damage;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetBool("IsInSight", true);
            OnShoot?.Invoke();
        }
    }


    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2);
    }



}