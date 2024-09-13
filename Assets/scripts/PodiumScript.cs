using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodiumScript : MonoBehaviour
{
    [SerializeField] public bool interactable = true;
    [SerializeField] public string correctGem; // Set correct gem in the inspector name convention should be "[Colour]Gem" to match the tag of the correct gem
}
