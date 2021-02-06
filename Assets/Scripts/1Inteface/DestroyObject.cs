using System.Collections;
using UnityEngine;

public class DestroyObject : MonoBehaviour {
    private IEnumerator num;
    public AnimationClip stop;
    private float stTime;
    public float lifeTime { get { return Time.time - stTime; } }

    void Start()
    {
        stTime = Time.time;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
