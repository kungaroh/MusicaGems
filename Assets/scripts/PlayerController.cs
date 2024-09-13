using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    public Rigidbody mv_rigidBody;
    public float speed = 12f;
    public Camera cam;
    public float lookSpeed = 2.5f;
    public bool isPaused = false;
    public int gemsPlaced = 0;


    Vector3 velocity = Vector3.zero;
    float rotationX = 0;
    RaycastHit raycastHit;
    float nextFire = 1.5f;
    float fireRate = 1.0f;
    GameObject CollectedGem = null;
    bool holdingGem = false;
    public GameObject selectedWeapon;
    GameObject equippedWeapon;
    Vector3 respawnPos;
    private bool wWheelUnlocked = false;

    [SerializeField] private Health healthScript;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI wheelToolTip;
    [SerializeField] private TextMeshProUGUI playToolTip;
    
    [SerializeField] private AudioSource fireSound;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private GameObject weaponCanvas;
    [SerializeField] private GameObject weaponCentre;
    [SerializeField] private GameObject soundPuzzle;


    // Start is called before the first frame update
    void Start()
    {
        mv_rigidBody = GetComponent<Rigidbody>();
        //pauseCanvas.SetActive(false);
        weaponCanvas.SetActive(false);


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        

        healthScript.health = healthScript.maxHealth;
        interactText.gameObject.SetActive(false);
        

    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            //Movement                          //Movement is bad, but it works fine in game
            if (Input.GetKey("w"))
            {
                mv_rigidBody.AddForce(transform.forward * (speed * Time.deltaTime));
            }
            if (Input.GetKey("s"))
            {
                mv_rigidBody.AddForce(-transform.forward * (speed * Time.deltaTime));
            }
            if (Input.GetKey("d"))
            {
                mv_rigidBody.AddForce(transform.right * (speed * Time.deltaTime));
            }
            if (Input.GetKey("a"))
            {
                mv_rigidBody.AddForce(-transform.right * (speed * Time.deltaTime));
            }


            //shooting      
            if (Input.GetMouseButton(0) && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Shoot();
            }

            //Playing tune
            if(Input.GetKeyDown("f") && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                try
                {
                    equippedWeapon.GetComponent<AudioSource>().Play();
                }
                catch { }
            }


            //Camera shizzle


            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -89, 89);

            cam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            

            //Checking health for death
            if (healthScript.health <= 0)
            {
                healthScript.Die();
            }

            //Interaction
            RaycastHit interactable;
            Physics.Raycast(cam.transform.position, cam.transform.forward, out interactable, 2.0f);
            

            try
            {
                //Trying to pick up a gem
                if (!holdingGem && interactable.transform.gameObject.tag.Contains("Gem") && interactable.transform.gameObject.GetComponent<GemScript>().interactable) 
                {
                    interactText.gameObject.SetActive(true);
                    if (Input.GetKey("e"))
                    {
                        CollectedGem = interactable.transform.gameObject;
                        CollectedGem.SendMessage("PickUp");
                        holdingGem = true;
                        interactText.gameObject.SetActive(false);
                    }
                }
                // Trying to place gem on a podium
                else if (holdingGem && interactable.transform.CompareTag("Podium"))
                {
                    interactText.gameObject.SetActive(true);
                    if (Input.GetKey("e"))
                    {
                        // correct gem, nothing on podium
                        if (CollectedGem.CompareTag(interactable.transform.GetComponent<PodiumScript>().correctGem) && interactable.transform.GetComponent<PodiumScript>().interactable) 
                        {
                            CollectedGem.GetComponent<GemScript>().Placed();

                            if (CollectedGem.CompareTag("GreenGem"))
                            {
                                soundPuzzle.GetComponent<Animator>().SetBool("greenGemPlaced", true);
                            }
                            if(CollectedGem.CompareTag("BlueGem"))
                            {
                                StartCoroutine("WeaponToolTip");
                                wWheelUnlocked = true;
                            }
                            gemsPlaced++;
                            holdingGem = false;
                            CollectedGem = null;
                        }
                        else // not correct gem
                        {
                            CollectedGem.transform.position = interactable.transform.position + new Vector3(0, 0.4f, 0);
                            CollectedGem.SetActive(true);
                            holdingGem = false;
                            CollectedGem = null;
                        }
                        interactText.gameObject.SetActive(false);
                    }
                }
                else
                {
                    interactText.gameObject.SetActive(false);
                }
            }
            catch
            {
                interactText.gameObject.SetActive(false);
            }

            /*//pause menu
            if (Input.GetKeyDown("escape"))
            {
                playerCanvas.SetActive(false);
                pauseCanvas.SetActive(true);

                isPaused = true;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;

                Time.timeScale = 0.0f;
            }*/

            //Weapon Wheel
            if(wWheelUnlocked && Input.GetKey(KeyCode.LeftAlt))
            {
                playerCanvas.SetActive(false);
                weaponCanvas.SetActive(true);

                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                Time.timeScale = 0.3f;
                isPaused = true;
            }
            if (gemsPlaced == 4)
            {
                gemsPlaced++;
                winStarter();

            }

        }
        else
        {

            /*//pause menu
            if (Input.GetKeyDown("escape"))
            {
                pauseCanvas.SetActive(false);
                playerCanvas.SetActive(true);

                isPaused = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                Time.timeScale = 1.0f;
            }*/

            if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                equippedWeapon = selectedWeapon;
                weaponCanvas.SetActive(false);
                playerCanvas.SetActive(true);

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1.0f;
                isPaused = false;
            }
        }

    }

    bool IsGrounded()       //would be for jumping
    {
        return Physics.Raycast(transform.position, -Vector3.up,0.4f);
    }

    void Shoot()
    {
        Physics.Raycast(cam.transform.position, cam.transform.forward, out raycastHit, 2.0f);
        

        try 
        { 
            GameObject hit = raycastHit.transform.gameObject;
            if (hit.tag.Contains("Gem") && hit.transform.gameObject.GetComponent<GemScript>().bonkable)
            {
                hit.GetComponent<GemScript>().Shot();
            }
        }
        catch 
        {
            //suck eggs
        };

    }

    void Shot(float damage)
    {
        healthScript.Hit(damage);
    }

    private void OnTriggerEnter(Collider trig)
    {
        if(trig.CompareTag("BridgeBorder"))
        {
            respawnPos = trig.transform.position;
        }
        if(trig.CompareTag("BridgeCatcher"))
        {
            transform.position = respawnPos + new Vector3(0, 0.5f, 0);
        }
    }

    private IEnumerator WeaponToolTip()
    {
        wheelToolTip.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        wheelToolTip.gameObject.SetActive(false);

        yield return new WaitForSeconds(2);

        playToolTip.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        playToolTip.gameObject.SetActive(false);
    }

    private IEnumerator Win()
    {
        Debug.Log("Win started, waiting for 21 seconds");
        mv_rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        isPaused = true;
        yield return new WaitForSeconds(21);

        Debug.Log("starting auto exposure");

        PostProcessProfile PPVol = cam.GetComponent<PostProcessVolume>().profile;
        PPVol.GetSetting<AutoExposure>().active = true;
        PPVol.GetSetting<AutoExposure>().minLuminance.value = -9;
        while(PPVol.GetSetting<AutoExposure>().maxLuminance.value > -9)
        {
            PPVol.GetSetting<AutoExposure>().maxLuminance.value --;
            Debug.Log("maxLuminance decreased by 1");
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(2);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        SceneManager.LoadScene(2);
    }

    void winStarter()
    {
        gameObject.GetComponent<AudioSource>().Play();
        Debug.Log("Playing Song");
        StartCoroutine("Win");
    }
}