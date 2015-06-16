using UnityEngine;
using System.Collections;

public class Draw : MonoBehaviour
{
    private Transform _myTrans;

    // Use this for initialization
    void Start ()
    {
    
        _myTrans = this.transform;
    }
    
    // Update is called once per frame
    void Update ()
    {
    
        if (_myTrans.localPosition.y < -30.0f) {
            Destroy (this.gameObject);
        }
    }
}
