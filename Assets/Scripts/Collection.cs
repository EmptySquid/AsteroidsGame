using UnityEngine;

public class Collectable : MonoBehaviour
{
    public GameObject particle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Instantiate(particle, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
