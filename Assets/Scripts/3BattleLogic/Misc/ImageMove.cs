using UnityEngine;

public class ImageMove : MonoBehaviour {
    public float Speed;
    public bool Lerp = false;
    public Vector3 TargetPosition;
    public float DistanceReaction = 0.01f;
    Transform parnt;

    private void Awake() {
        parnt = transform.parent;
    }

    private void Start() {
        TargetPosition = transform.position;
    }

    private void Update() {
        targ();
    }

    private void targ() {
        if (parnt != null) {
            if (Vector3.Distance(parnt.position, TargetPosition) > DistanceReaction) {
                if (Lerp) {
                    TargetPosition = Vector3.Lerp(TargetPosition, parnt.position, Speed * Time.deltaTime);
                } else {
                    TargetPosition = Vector3.MoveTowards(TargetPosition, parnt.position, Speed * Time.deltaTime);
                }
                transform.position = TargetPosition;
            }
        } else {
            Destroy(gameObject, 1f);
        }
    }

    public void Setup() {
        transform.position = TargetPosition;
    }
}
