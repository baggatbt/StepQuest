using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public Animator ani;


    // Start is called before the first frame update
    public void buttons(string nams)
    {
        ani.SetTrigger(nams);
    }
}
