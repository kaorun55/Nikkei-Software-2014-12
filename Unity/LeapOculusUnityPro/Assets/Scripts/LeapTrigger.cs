using UnityEngine;
using System.Collections;

public class LeapTrigger : MonoBehaviour {

    public AnimationClip clip;
    Animator anim;

    public float delayWeight;
    float current = 0;
    bool trigger = false;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( trigger ) {
            current = 1;
        }
        else {
            current = Mathf.Lerp( current, 0, delayWeight );
        }

        anim.SetLayerWeight( 1, current );
    }

    void OnTriggerEnter( Collider collider )
    {
        trigger = true;
        anim.CrossFade( clip.name, 0 );
    }

    void OnTriggerExit( Collider other )
    {
        trigger = false;
    }
}
