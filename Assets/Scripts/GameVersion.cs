using System;
using UnityEngine;

public class GameVersion :MonoBehaviour
{ 
    private String versionNumber;

    private void Awake()
    {
        versionNumber = Application.version;
    }
 
    public static String GetVersionNumber() 
    { 
        return Application.version; 
    } 
} 
