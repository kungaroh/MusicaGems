using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponWheelScript : MonoBehaviour
{
    [SerializeField] PlayerController playerScript;
    [SerializeField] GameObject Tune1;
    [SerializeField] GameObject Tune2;
    [SerializeField] GameObject Tune3;
    [SerializeField] GameObject Tune4;
    public void Top()
    {
        playerScript.selectedWeapon = Tune1;
    }
    public void Right()
    {
        playerScript.selectedWeapon = Tune2;
    }
    public void Bottom()
    {
        playerScript.selectedWeapon = Tune3;
    }
    public void Left()
    {
        playerScript.selectedWeapon = Tune4;
    }

}
