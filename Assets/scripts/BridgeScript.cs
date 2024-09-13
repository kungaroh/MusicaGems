using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeScript : MonoBehaviour
{
    Material material;
    Collider coll;
    Animator animator;
    [SerializeField] GameObject correctTune;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        coll = GetComponent<Collider>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(material.GetColor("_Color").a < 0.5f)
        {
            coll.enabled = false;
        }
        if(material.GetColor("_Color").a >= 0.5f)
        {
            coll.enabled = true;
        }

        if(correctTune.GetComponent<AudioSource>().isPlaying)
        {
            animator.SetBool("isPlaying", true);
        }
        else
        {
            animator.SetBool("isPlaying", false);
        }
    }   
}
