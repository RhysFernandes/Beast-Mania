using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {

    public float health = 100f;
    private float maxhealth = 100f;
    public GameObject beam;
    private GameObject player;
    private GameObject otherObjects;
    private Transform enemyTransform;
    //private bool playerFollow = false;
    private Animator enemy_Animate;
    private Rigidbody2D enemy_Rigidbody;
    public float speed = 5f;
    //private Vector3 smoothVelocity = Vector3.zero;
    private bool facingRight = false;
    public float jumpforce = 25f;
    private float floatHeight = 10f;
    private float liftForce = 100f;
    private float damping = 10;
    float damage = 10;
	public bool tookDamage = false;
	private bool paralyzed = false;
	float TimeDelay = 0.01f;

	private bool dead = true;

    bool grounded = true;
    public Transform groundCheck;
    float groundRadius = 0.2f;
    public LayerMask whatIsGround;
    public LayerMask other;

    public bool pushback = false;
    private float stunnedTime = 0f;

    private bool obstacle = true;
    public Transform collisionCheck;
    private float collisionCheckRadius = 0.2f;
    public LayerMask whatIsCollision;
    bool jab = false;

    SpriteRenderer enemySprite;
    public Sprite Hulk_Jab;

    public RectTransform healthBarRect;
    [SerializeField]
    private Text healthText;
    public Canvas StatusIndicator;

    float TimetoHit = 10;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        enemy_Animate = GetComponent<Animator>();
        enemy_Rigidbody = GetComponent<Rigidbody2D>();
        enemy_Rigidbody.freezeRotation = true;
        enemySprite = GetComponent<SpriteRenderer>();
		float direction = Random.Range (0, 1);
		if (direction == 1) {
			Flip ();
		}
	}
	
    void OnCollisionEnter2D(Collision2D collision)
    {

        if(collision.gameObject == beam)
        {
            health -= collision.gameObject.GetComponent<BeamController>().damage;
            Destroy(collision.gameObject);
        }
        /*else if (collision.gameObject.tag == "Player" && jab)
        {
            collision.gameObject.GetComponent<GameController>().curhealth -= damage;
            Debug.Log("Player health: " + collision.gameObject.GetComponent<GameController>().curhealth);
            jab = false;
        }*/

    }

    void HItPlayer()
    {
        if (enemySprite.sprite == Hulk_Jab)
        {
            //Debug.Log("True");
            jab = true;
        }
    }

	// Update is called once per frame
	void Update () {
		if(health <= 0 && dead)
        {
			dead = false;
			enemy_Rigidbody.isKinematic = false;
			enemy_Animate.SetBool ("dead", true);
			player.gameObject.GetComponent<GameController> ().live_enemies -= 1;
            Destroy(this.gameObject, 1);
        }
		//Debug.Log ("dead:" + dead);
	}

    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        enemy_Animate.SetBool("ground", grounded);

		enemy_Animate.SetFloat ("vspeed", enemy_Rigidbody.velocity.y);

        obstacle = Physics2D.OverlapCircle(collisionCheck.position, collisionCheckRadius, whatIsCollision);
        enemy_Animate.SetBool("collision", obstacle);

        if(player != null)
        {
            HItPlayer();

            Obstacle();

            //EnemyCollision();

            /*    if (player.transform.position.x < transform.position.x && facingRight)
            {
                Flip();
            }
            else if(player.transform.position.x > transform.position.x && !facingRight)
            {
                Flip();
            }*/
            if(stunnedTime > 0)
            {
                stunnedTime -= Time.deltaTime;
            }else
            {
                FollowPlayer();
            }
            

            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(transform.position.x, transform.position.y));
            //Debug.DrawRay(transform.position, new Vector3(transform.position.x, transform.position.y, 0), Color.black);
            if (hit.collider == player)
            {

                float distance = Mathf.Abs(hit.point.y - transform.position.y);
                float heightError = floatHeight - distance;
                float force = liftForce * heightError - enemy_Rigidbody.velocity.y * damping;
                enemy_Rigidbody.AddForce(Vector3.up * force);
            }
        }

        

		if (tookDamage == true) {
			enemy_Animate.SetBool ("damage", true);
			tookDamage = false;
			paralyzed = true;
			TimeDelay = 0.03f;
		}
		TimeDelay -= Time.deltaTime;
		if (TimeDelay <= 0) {
			enemy_Animate.SetBool ("damage", false);
			paralyzed = false;
		}

        if(health < 0)
        {
            SetHealth(0, maxhealth);
        }
        else
        {
            SetHealth(health, maxhealth);
        }

        if (pushback)
        {
            stunnedTime = 1f;
            IsHit();
            pushback = false;
        }
    }

    void Obstacle()
    {
        if (obstacle == true && grounded == true && facingRight == true)
        {
            enemy_Animate.SetBool("ground", false);
            grounded = false;

            Vector2 myVel = enemy_Rigidbody.velocity;
            float accelerate = 0;
            accelerate += Time.deltaTime * 200;
            Vector2 jump = new Vector2(accelerate * 2, Mathf.Pow(accelerate, 2) + accelerate);
            enemy_Rigidbody.velocity = jump;
        }
        else if (obstacle == true && grounded == true && !facingRight)
        {
            enemy_Animate.SetBool("ground", false);
            grounded = false;

            Vector2 myVel = enemy_Rigidbody.velocity;
            float accelerate = 0;
            accelerate += Time.deltaTime * 200;
            Vector2 jump = new Vector2(-accelerate * 2, Mathf.Pow(accelerate, 2) + accelerate);
            enemy_Rigidbody.velocity = jump;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        Vector3 StatusIndicatorScale = StatusIndicator.transform.localScale;
        theScale.x *= -1;
        StatusIndicatorScale.x *= -1;
        transform.localScale = theScale;
        StatusIndicator.transform.localScale = StatusIndicatorScale;
    }

	void changeDirection(){
		if (player.transform.position.x < transform.position.x && facingRight)
		{
			Flip();
		}
		else if(player.transform.position.x > transform.position.x && !facingRight)
		{
			Flip();
		}
	}

    void FollowPlayer()
    {
        // if player == null
        Vector2 distance = player.transform.position - transform.position;
        //Debug.Log(distance.x);
        Vector2 myVel = enemy_Rigidbody.velocity;
		if (Mathf.Pow (distance.y, 2) > 0 && Mathf.Pow (distance.y, 2) < 20 && grounded == true && paralyzed == false) {
			changeDirection ();	
			if (distance.x > 8 && distance.x < 20 && obstacle == false) {
				enemy_Animate.SetBool ("attackPlayer", false);
				enemy_Animate.SetBool ("followPlayer", true);

				myVel.x = speed;
				enemy_Rigidbody.velocity = myVel;
            
			} else if (distance.x < -8 && distance.x > -20 && obstacle == false) {
				enemy_Animate.SetBool ("attackPlayer", false);
				enemy_Animate.SetBool ("followPlayer", true);
				;
				myVel.x = -speed;
				enemy_Rigidbody.velocity = myVel;

			} else if ((distance.x >= -8 && distance.x <= 8)) {
				enemy_Animate.SetBool ("followPlayer", false);
				enemy_Animate.SetBool ("attackPlayer", true);
				myVel.x = 0;
				enemy_Rigidbody.velocity = myVel;
                EnemyCollision();
			}else if (grounded && !obstacle && paralyzed == false) {
				enemy_Animate.SetBool ("attackPlayer", false);
				enemy_Animate.SetBool ("followPlayer", false);

				myVel.x = 0;
				enemy_Rigidbody.velocity = myVel;
			}
		}
		else
		{
			if (grounded && !obstacle && paralyzed == false) {
				enemy_Animate.SetBool ("attackPlayer", false);
				enemy_Animate.SetBool ("followPlayer", false);

				myVel.x = 0;
				enemy_Rigidbody.velocity = myVel;
			}
		}
    }

    void EnemyCollision()
    {
        //var playerCollision = new Vector2(collisionCheck.position.x, collisionCheck.position.y);
        Collider2D hitColliders = null;
        hitColliders = Physics2D.OverlapCircle(collisionCheck.position, 0.4f);
        if (hitColliders == player.GetComponent<Collider2D>() && TimetoHit <= 0)
        {
            player.gameObject.GetComponent<GameController>().curhealth -= damage;
            //Debug.Log("Player health: " + player.gameObject.GetComponent<GameController>().curhealth);
            jab = false;
            TimetoHit = 5f;
        }
        else
        {
            TimetoHit -= Time.deltaTime * 5;
        }
    }

    void IsHit()
    {
        Vector2 myVel = enemy_Rigidbody.velocity;
        if (facingRight)
        {
            myVel.x = -5f;
        }else
        {
            myVel.x = 5f;
        }
        enemy_Rigidbody.velocity = myVel;
    }

    void SetHealth(float cur_health, float max_health)
    {
        float healthValue = cur_health / max_health;
        healthBarRect.localScale = new Vector3(healthValue, healthBarRect.localScale.y, healthBarRect.localScale.z);
        healthText.text = cur_health + "/" + max_health + " HP";
    }

}
