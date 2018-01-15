using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    //public GameObject player;

    private Vector2 velocity;

    //public float smoothTimeX;
    //public float smoothTimeY;

	[SerializeField]
	private float xMax;
	[SerializeField]
	private float yMax;
	[SerializeField]
	private float xMin;
	[SerializeField]
	private float yMin;

	private Transform target;

	// Use this for initialization
	void Start () {

		target = GameObject.Find("Ironman").transform;

	}
	
    void FixedUpdated()
    {
        //float posX = Mathf.SmoothDamp(transform.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);
        //float posY = Mathf.SmoothDamp(transform.position.y, player.transform.position.y, ref velocity.y, smoothTimeY);



    }

	// Update is called once per frame
	void Update () {
        if(target != null)
        {
            transform.position = new Vector3(Mathf.Clamp(target.position.x, xMin, xMax), Mathf.Clamp(target.position.y, yMin, yMax), transform.position.z);
        }
			
	}
}
