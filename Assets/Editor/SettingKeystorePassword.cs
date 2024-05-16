using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class StartUp
{

#if UNITY_EDITOR
 
    static StartUp() {
 
        PlayerSettings.keystorePass = "drt78okj2";
        PlayerSettings.keyaliasPass = "drt78okj2";
    }
 
#endif
}