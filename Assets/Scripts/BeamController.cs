using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamController : MonoBehaviour {

    public float damage = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnCollisionEnter2D(Collision2D collision)
    {
		if (collision.gameObject.tag == "Enemy")
        {
            //Debug.Log("Damage done" + damage);
            //Debug.Log("Hulk's health before:" + collision.gameObject.GetComponent<EnemyController>().health);
            collision.gameObject.GetComponent<EnemyController>().health -= damage;
			collision.gameObject.GetComponent<EnemyController>().tookDamage = true;
            //Debug.Log("Hulk's health" + collision.gameObject.GetComponent<EnemyController>().health);
            Destroy(gameObject);
        }
        else if(collision.gameObject.tag != "Player")
        {
            Destroy(gameObject);
        }
    }
}
