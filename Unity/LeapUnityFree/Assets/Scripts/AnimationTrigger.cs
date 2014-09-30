using UnityEngine;
using System.Collections;

public class AnimationTrigger : MonoBehaviour {
    public AnimationClip clip;
    Animator anim;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // あたったときに、指定したアニメーションを再生する
    void OnTriggerEnter( Collider collider )
    {
        anim.Play( clip.name );
    }
}
