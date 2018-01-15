using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walking : MonoBehaviour {
    private float speed = 7.0f;
    //public float jumpheight = 10.0f;
    public float jumpForce = 700f;

    private Rigidbody2D playerbody;
    private Animator animate_walk;
   // Vector3 position;
    public bool faceright = true;

    bool grounded = true;
    public Transform groundCheck;
    float groundRadius = 0.2f;
    public LayerMask whatIsGround;


	// Use this for initialization
	void Start () {
        animate_walk = GetComponent<Animator>();
        playerbody = GetComponent<Rigidbody2D>();
        //position = transform.position;
        playerbody.freezeRotation = true;
    }
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        animate_walk.SetFloat("speed", Mathf.Abs(horizontal));
        playerbody.velocity = new Vector2(horizontal * speed, playerbody.velocity.y);

        animate_walk.SetFloat("vspeed", playerbody.velocity.y);

;       grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        animate_walk.SetBool("Ground", grounded);

        if (horizontal > 0 && !faceright)
        {
            Flip();
        }else if(horizontal < 0 && faceright)
        {
            Flip();
        }
    }

    public void Flip()
    {
        faceright = !faceright;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        var hi = 1;
    }
    // Update is called once per frame
    void Update()
    {
        if (grounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            animate_walk.SetBool("Ground", false); 
            playerbody.AddForce(new Vector2(0, jumpForce));
        }
    }
}
