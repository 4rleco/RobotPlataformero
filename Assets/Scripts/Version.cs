using System;
using UnityEngine;

public class Version :MonoBehaviour
{ 
    private String versionNumber;

    private void Awake()
    {
        versionNumber = Application.version;
    }
 
    public static String GetVersionNumber() 
    { 
        return versionNumber; 
    } 
} 
