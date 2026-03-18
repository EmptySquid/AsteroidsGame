using UnityEngine;

public class Collectable : MonoBehaviour
{
    public GameObject particle;
    public SoundManager _SM;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _SM = GameObject.FindFirstObjectByType<SoundManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Instantiate(particle, transform.position, transform.rotation);
            _SM.PlayRandomSound(_SM.collectionSounds);
            Destroy(gameObject);
        }
    }
}
