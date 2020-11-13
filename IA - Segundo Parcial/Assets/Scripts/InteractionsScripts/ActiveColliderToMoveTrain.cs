using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveColliderToMoveTrain : MonoBehaviour
{
  [SerializeField] private GameObject activeCollider;
  
  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.tag.Equals("Player"))
    {
      activeCollider.SetActive(true);
    }
  }
}