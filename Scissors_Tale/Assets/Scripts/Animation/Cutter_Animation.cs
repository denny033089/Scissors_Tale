using UnityEngine;

public class Cutter_Animation : MonoBehaviour
{
    float speed;
    Vector3 direction;
    public void DestroySelf() {
        Destroy(gameObject);
    }

    public void Setup(Vector3 direction,float speed) {
        this.direction = direction;
        this.speed = speed;
    }
    
    void Update() {
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
