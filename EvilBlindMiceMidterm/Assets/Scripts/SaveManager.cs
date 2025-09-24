using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    static SaveManager instance;
    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
    }

    void SaveIntList(List<int> _list, string _path)
    {
        int[] intArray = new int[_list.Count + 1];
        intArray[0] = _list.Count;
        for (int i = 1; i < intArray.Length; i++) 
        {
            intArray[i] = _list[i - 1];
        }
        Debug.Log(intArray);
        //File.WriteAllBytes()
    }
}
