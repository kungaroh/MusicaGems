using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GemScript : MonoBehaviour
{
    [SerializeField] public bool interactable = false;
    [SerializeField] public bool bonkable = false;
    [SerializeField] GemTones gemTones;
    [SerializeField] GameObject CPodium;
    [SerializeField] GameObject soundPuzzleChest;
    [SerializeField] GameObject[] bonkableGems = new GameObject[4];
    private AudioSource _podiumAudioSource;
    private PodiumScript _podiumScript;

    private void Start()
    {

        try
        {
            _podiumScript = CPodium.GetComponent<PodiumScript>();
            _podiumAudioSource = CPodium.GetComponent<AudioSource>();
        }
        catch
        {
        }
    }

    public void Shot()
    {
        GetComponent<AudioSource>().Play();
        
        gemTones.noteArray.Enqueue(gameObject.name.ToCharArray()[0]);
        
        if (gemTones.noteArray.Count >= 4)
        {
            if (gemTones.ArrayCheck())
                soundPuzzleChest.GetComponent<Animator>().SetBool("soundPuzzleComplete", true);
            else
            {
                StartCoroutine(BadGemCombo());
            }
            gemTones.noteArray.Clear();
        }
    }

    public void PickUp()
    {
        gameObject.SetActive(false);
        gameObject.GetComponent<AudioSource>().playOnAwake = false;
        CPodium.GetComponent<AudioSource>().Play();
    }

    public void Placed() // correctly placed
    {
        transform.position = CPodium.transform.position + new Vector3(0, 0.4f, 0);
        gameObject.SetActive(true);
        _podiumAudioSource.Stop();
        _podiumScript.interactable = false;
        interactable = false;
    }

    private IEnumerator BadGemCombo()
    {
        foreach (GameObject go in bonkableGems)
        {
            go.GetComponent<GemScript>().bonkable = false;
            go.GetComponent<AudioSource>().Play();
        }

        yield return new WaitForSeconds(1);

        foreach (GameObject go in bonkableGems)
        {
            go.GetComponent<GemScript>().bonkable = true;
        }
    }
}
