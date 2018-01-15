using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    private float speed = 15.0f;
    public float jumpheight = 10.0f;
    public float jumpForce = 700f;
    private bool doubleJump = true;

    private Rigidbody2D playerbody;
    private Animator animate_walk;
    Vector3 position;
    public bool faceright = true;

    bool grounded = true;
    public Transform groundCheck;
    float groundRadius = 0.2f;
    public LayerMask whatIsGround;

    //bool fire = false;
    private float maxChargeBeam = 10;
    private float ChargeBeam = 0;
    public GameObject beam;
    public Transform firePoint;
    Vector3 beamPos;
    public float beamSpeed = 5;
    //private float beamDelay = 0.02f;
    public float fireRate = 1;
    float timetoFire = 0;
	private bool okaytoFire = true;
	private bool canFire = true;
    private bool canMove = true;
    private float timeOut = 1f;
    private bool canHit = true;
    float timetoHit = 0f;

    public BoxCollider2D firePointCollider;

    public float curhealth = 100;
    private float maxhealth = 100;
    public RectTransform healthBarRect;
    [SerializeField]
    private Text healthText;
    public Canvas StatusIndicator;

    public Transform[] enemyspawn;
	public Transform[] enemycurrentspawns;
	public int Wave = 1;
	public GameObject enemy;
	public int live_enemies = 3;
	private int saveindex;
    public Text waveText;

    public Canvas PauseMenu;
    private bool paused = false;
    

    // Use this for initialization
    void Start () {
        animate_walk = GetComponent<Animator>();
        playerbody = GetComponent<Rigidbody2D>();
        position = transform.position;
        playerbody.freezeRotation = true;
		enemycurrentspawns = enemyspawn;
		int index = 0;
		for (int i = 0; i < live_enemies; i++) {
            //Debug.Log("i: " + i);
			if (i == 0) {
				index = Random.Range (0, enemycurrentspawns.Length);
				saveindex = index;
				//Debug.Log("Saveindex: " + saveindex);
			} else {
				Transform[] currentspawns = new Transform[enemycurrentspawns.Length - 1];
				int m = 0;
				for (int k = 0; k < enemycurrentspawns.Length; k++) {
					//Debug.Log ("K: " + k);
					if (saveindex == k) {
						//Debug.Log ("M: " + m);
						continue;
					} else {
                        //Debug.Log("M: " + m);
                        currentspawns [m] = enemycurrentspawns [k];
						m += 1;
					}
				}
				enemycurrentspawns = currentspawns;
				index = Random.Range (0, enemycurrentspawns.Length);
				saveindex = index;
			}
			Instantiate (enemy, enemycurrentspawns [index].position, enemycurrentspawns[index].rotation);
		}
        PauseMenu = PauseMenu.GetComponent<Canvas>();
        PauseMenu.enabled = false;
    }
    void FixedUpdate()
    {

        float horizontal = Input.GetAxis("Horizontal");
        if (canMove)
        {
            animate_walk.SetFloat("speed", Mathf.Abs(horizontal));
            playerbody.velocity = new Vector2(horizontal * speed, playerbody.velocity.y);
        }else
        {
            playerbody.velocity = new Vector2(horizontal * 0, playerbody.velocity.y);
            if(timeOut <= 0)
            {
                canMove = true;
                timeOut = 1f;
            }else
            {
                timeOut -= Time.deltaTime;
            }
        }

        animate_walk.SetFloat("vspeed", playerbody.velocity.y);

        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        animate_walk.SetBool("Ground", grounded);

        if (horizontal > 0 && !faceright)
        {
            Flip();
        }
        else if(horizontal < 0 && faceright)
        {
            Flip();
        }
			
		PauseGame ();

        SetHealth(curhealth, maxhealth);

        SetWave(Wave);
    }

	void PauseGame(){
		//float savedTimeScale = 0;
		if (Input.GetKeyDown (KeyCode.P) && !paused /*&& Time.timeScale == 1.0f*/) {
            PauseMenu.enabled = true;
            //savedTimeScale = Time.timeScale;
            //Debug.Log ("Timescale:" + savedTimeScale);
            //Time.timeScale = 0f;
            paused = !paused;
        } else if(Input.GetKeyDown(KeyCode.P) && paused){
            //Debug.Log("Timescale:" + Time.timeScale);
            //Time.timeScale = 1.0f;
            PauseMenu.enabled = false;
            paused = !paused;
		}
	}

    void Flip()
    {
        faceright = !faceright;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    // Update is called once per frame
    void Update()
    {
        if (grounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            animate_walk.SetBool("Ground", false); 
            playerbody.AddForce(new Vector2(0, jumpForce));
            doubleJump = true;
        }
        else if(doubleJump && Input.GetKeyDown(KeyCode.UpArrow))
        {
            playerbody.AddForce(new Vector2(0, jumpForce));
            doubleJump = false;
        }

        if (curhealth <= 0)
        {
            Destroy(this.gameObject);
        }

        Fire();

        Punch();

		if (live_enemies == 0) {
			Wave += 1;
            //Debug.Log("Wave: " + Wave);
			live_enemies = 3 + 2 * (Wave - 1);
			enemycurrentspawns = enemyspawn;
			int index;
			for (int i = 0; i < 3 + 2 * (Wave - 1); i++) {
				if (i == 0) {
					index = Random.Range (0, enemycurrentspawns.Length);
					saveindex = index;
				} else {
					Transform[] currentspawns = new Transform[enemycurrentspawns.Length - 1];
					int m = 0;
					for (int k = 0; k < enemycurrentspawns.Length; k++) {
						if (saveindex == k) {
							continue;
						} else {
							currentspawns [m] = enemycurrentspawns [k];
							m += 1;
						}
					}
					enemycurrentspawns = currentspawns;
					index = Random.Range (0, enemycurrentspawns.Length);
					saveindex = index;
				}
				Instantiate (enemy, enemycurrentspawns [index].position, Quaternion.identity);
			}
		}

    }

    void Fire()
    {
		
		float timedelay = 0;
		if (canFire == false) {
			timedelay = 1f;
		}
		timedelay -= Time.deltaTime * 50;
		if (timedelay <= 0) {
			canFire = true;
		}
		if (Input.GetKeyDown(KeyCode.B) && Time.time > timetoFire && okaytoFire)
        {
            canMove = false;
            animate_walk.Play("ChargingBeam");
            animate_walk.SetBool("Fire", true);
			okaytoFire = false;
        }
		if (Input.GetKey(KeyCode.B) && Time.time > timetoFire && !okaytoFire)
        {
            ChargeBeam += Time.deltaTime * 50;
            //Debug.Log("Charge:" + ChargeBeam);
        }

		if (Input.GetKeyUp(KeyCode.B) && Time.time > timetoFire && !okaytoFire)
        {
            //Debug.Log("Fire");
            animate_walk.Play("Shoot");
            animate_walk.SetBool("Fire", false);
            if (ChargeBeam > maxChargeBeam)
            {
                ChargeBeam = maxChargeBeam;
            }
            else
            {
                ChargeBeam = 5;
            }
            playerShoot(ChargeBeam);
            ChargeBeam = 0;
            timetoFire = Time.time + 1 / fireRate;
			okaytoFire = true;
			canFire = false;
            
            //Debug.Log("Time to fire:" + timetoFire + " " + fireRate);
        }
    }

    void playerShoot(float damage)
    {
        //Debug.Log("firePoint x: " + firePoint.position.x);
        //Debug.Log("firePoint y: " + firePoint.position.y);
        beamPos = new Vector3(firePoint.position.x, firePoint.position.y, 0);
        // create new beam
        GameObject newBeam = GameObject.Instantiate(beam, beamPos, Quaternion.identity) as GameObject;
        newBeam.GetComponent<Rigidbody2D>().freezeRotation = true;
        // Give beam energy point
        newBeam.GetComponent<BeamController>().damage = damage;
        // Direction of Beam
        if (faceright)
        {
            newBeam.GetComponent<Rigidbody2D>().velocity = new Vector2(beamSpeed, newBeam.GetComponent<Rigidbody2D>().velocity.y);
        }
        else
        {
            newBeam.GetComponent<Rigidbody2D>().velocity = new Vector2(-beamSpeed, newBeam.GetComponent<Rigidbody2D>().velocity.y);
        }
    }

    void Punch()
    {
        RaycastHit2D fistHit = Physics2D.CircleCast(firePoint.position, 0.2f, firePoint.up, speed * Time.deltaTime, LayerMask.GetMask("Enemy"));
        if (Input.GetKeyDown(KeyCode.X) && groundCheck && canHit)
        {
            animate_walk.SetBool("punch", canHit);
            if (fistHit)
            {
                fistHit.collider.gameObject.GetComponent<EnemyController>().pushback = true;
                fistHit.collider.gameObject.GetComponent<EnemyController>().health -= 10f;
                fistHit.collider.gameObject.GetComponent<EnemyController>().tookDamage = true;
            }
            canHit = false;
            timetoHit = 1f;
            
        }
        if (timetoHit > 0)
        {
            timetoHit -= Time.deltaTime * 2;
        }else
        {
            animate_walk.SetBool("punch", false);
            canHit = true;
        }
    }

    void SetWave(int cur_Wave)
    {
        waveText.text = "Wave: " + cur_Wave;
    }

    void SetHealth(float cur_health, float max_health)
    {
        float healthValue = cur_health / max_health;
        healthBarRect.localScale = new Vector3(healthValue, healthBarRect.localScale.y, healthBarRect.localScale.z);
        healthText.text = cur_health + "/" + max_health + " HP";
    }

}
