using UnityEngine;

public class TennisPlayer : MonoBehaviour
{
    public int playerNumber;
    public float speed;
    public float hitPower;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            swing("Player swung");
        }
    }

    public void swing(string _string)
    {
        Debug.Log(_string);
    }
}
