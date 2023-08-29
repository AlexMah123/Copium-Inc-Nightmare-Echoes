using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NightmareEchoes.Unit
{
    public class FinishGettingHitEvent : MonoBehaviour
    {
        Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void FinishGettingHit()
        {
            animator.SetBool("GettingHit", false);
        }
    }
}
