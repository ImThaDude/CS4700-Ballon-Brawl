using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSLogger
{
    
    public static void Log(object o) {
        Debug.Log(o.ToString());
    }
    
    public static void L(object o) {
        Log(o);
    }

}
