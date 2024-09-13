using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GemTones")]

public class GemTones : ScriptableObject
{
    public string prefabName;
    public Queue<char> noteArray = new Queue<char>();
    [SerializeField] protected char[] cNoteArray = { 'G', 'R', 'G', 'Y' };
    


    private void ArrayReset()
    {
        noteArray.Clear();
    }

    public bool ArrayCheck()
    {
        if (Enumerable.SequenceEqual(noteArray, cNoteArray))
        {
            return true;
        }
        
        return false;
    }
}
