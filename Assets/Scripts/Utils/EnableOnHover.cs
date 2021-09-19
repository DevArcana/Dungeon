using System;
using UnityEngine;

namespace Utils
{
  public class EnableOnHover : MonoBehaviour
  {
    public GameObject target;

    private void Start()
    {
      target.SetActive(false);
    }

    private void OnMouseEnter()
    {
      target.SetActive(true);
    }

    private void OnMouseExit()
    {
      target.SetActive(false);
    }
  }
}