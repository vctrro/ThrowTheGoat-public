using UnityEngine;

public class Pendulum : MonoBehaviour
{
    [SerializeField] private float forceCoefficient;
    private Transform weight;
    private float anchorPositionX;
    private Rigidbody2D weightR2D;
    private AudioSource[] audioSource;

    private void Start() {
        audioSource = GetComponents<AudioSource>();
        weight = transform.Find("Weight");
        weightR2D = weight.GetComponent<Rigidbody2D>();
        anchorPositionX = transform.Find("Anchor").position.x;
    }

    private void FixedUpdate()
    {
        //when the pendulum is down
        if (Mathf.Abs(weight.position.x - anchorPositionX) < 0.15f) AddSoundAndForce();
    }
    /// <summary>
    /// Adds force and plays sound
    /// </summary>
    private void AddSoundAndForce()
    {        
        //Debug.Log($"{weightR2D.velocity}, {weight.position.x}");
        weightR2D.AddForce(weightR2D.velocity * forceCoefficient);
        audioSource[0].Play();
    }
}
