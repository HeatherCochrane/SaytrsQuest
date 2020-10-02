using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float move = 0.1f;
    float jump = 6;
    float fallMult = 3f;

    bool isFalling = false;
    Rigidbody2D rb;

    bool facingRight = false;

    bool holdingBox = false;
    GameObject boxInHand;

    float throwForce = 2;

    SpriteRenderer sp;

    [SerializeField]
    Transform target;

    Animator anim;
    [SerializeField]
    Sprite stand;
    [SerializeField]
    Sprite fall;

    [SerializeField]
    Sprite standHold;
    [SerializeField]
    Sprite fallHold;

    bool attacking = false;

    AnimatorClipInfo[] info;

    RaycastHit2D hit;

    [SerializeField]
    LayerMask enemy;


    [SerializeField]
    RuntimeAnimatorController normal;

    [SerializeField]
    RuntimeAnimatorController hold;

    [SerializeField]
    GameObject hitTarget;

    int lives = 5;

    bool blocking = false;

    [SerializeField]
    GameObject cam;

    [SerializeField]
    UI ui;

    [SerializeField]
    Vector3 offset;

    bool gameOver = true;

    bool isMoving = false;

    [SerializeField]
    GameObject boxParent;

    [SerializeField]
    AudioHandler audio;

    GameObject undoBoxObj;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        sp = this.GetComponent<SpriteRenderer>();
        anim = this.GetComponent<Animator>();
        Physics2D.queriesStartInColliders = false;
    }


    private void FixedUpdate()
    {
        if (!gameOver)
        {
            if (Input.GetKey(KeyCode.RightArrow) && !attacking && !blocking)
            {
                this.transform.position += new Vector3(move, 0, 0);
                facingRight = true;
                sp.flipX = true;

                anim.SetBool("isRunning", true);
                anim.SetBool("isIdle", false);

                isMoving = true;
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && !attacking && !blocking)
            {
                this.transform.position -= new Vector3(move, 0, 0);
                facingRight = false;
                sp.flipX = false;
                anim.SetBool("isRunning", true);
                anim.SetBool("isIdle", false);

                isMoving = true;
            }
            else if (!attacking || isFalling || blocking)
            {
                //anim.SetBool("isRunning", false);
                anim.SetBool("isIdle", true);
                isMoving = false;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            cam.transform.position = new Vector3(this.transform.position.x + offset.x, this.transform.position.y + offset.y, offset.z);
            cam.transform.position = new Vector3(cam.transform.position.x, Mathf.Clamp(cam.transform.position.y, -10, 3f), -10);

            if (Input.GetKeyDown(KeyCode.E) && !holdingBox && !attacking && !blocking)
            {
                attacking = true;
                anim.SetBool("isRunning", false);
                anim.SetBool("isIdle", false);
                anim.SetBool("isAttacking", true);
                Invoke("stopAttack", 0.8f);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                blocking = true;
                anim.SetBool("isIdle", false);
                anim.SetBool("isBlocking", true);
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                blocking = false;
                anim.SetBool("isBlocking", false);
                anim.SetBool("isRunning", true);
            }


            if (Input.GetKeyDown(KeyCode.UpArrow) && !holdingBox)
            {
                if (boxInHand != null)
                {
                    undoBoxObj = boxInHand;
                    undoBoxObj.GetComponent<Boxes>().setLastPos(boxInHand.transform.position);
                    boxInHand.transform.position = target.position;
                    boxInHand.gameObject.transform.SetParent(this.transform);
                    boxInHand.GetComponent<Rigidbody2D>().isKinematic = true;
                    boxInHand.GetComponent<Boxes>().setHolding(true);
                    holdingBox = true;

                    anim.runtimeAnimatorController = hold;
                    anim.SetBool("isJumping", false);
                    audio.playPickUp();
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && holdingBox)
            {
                boxInHand.transform.parent = null;
                boxInHand.GetComponent<Rigidbody2D>().isKinematic = false;

                if (facingRight)
                {
                    boxInHand.GetComponent<Rigidbody2D>().velocity = Vector2.right * throwForce;
                }
                else
                {
                    boxInHand.GetComponent<Rigidbody2D>().velocity = -Vector2.right * throwForce;
                }

                holdingBox = false;
                boxInHand.GetComponent<Boxes>().setHolding(false);
                boxInHand.transform.SetParent(boxParent.transform);
                boxInHand = null;

                anim.runtimeAnimatorController = normal;
            }


            if (Input.GetKeyDown(KeyCode.Space) && !isFalling && !attacking)
            {
                rb.velocity += Vector2.up * jump;
                isFalling = true;
                anim.SetBool("isIdle", false);
                anim.SetBool("isJumping", true);
                anim.SetBool("isRunning", false);
                audio.playJump();

            }

            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * (fallMult - 1) * Physics2D.gravity.y * Time.deltaTime;
            }

        }
    }

    public bool getIsBlocking()
    {
        return blocking;
    }

    void stopAttack()
    {
        attacking = false;
        anim.SetBool("isAttacking", false);
    }

    void checkHit()
    {
        Vector3 dir;

        if(facingRight)
        {
            dir = new Vector3 (1, 0, 0);
        }
        else
        {
            dir = new Vector3(-1, 0, 0);
        }

        hit = Physics2D.Raycast(hitTarget.transform.position, dir , 1f, enemy);

        if (hit.collider != null && !hit.transform.GetComponent<Enemy>().getIsDead())
        {
            hit.transform.GetComponent<Enemy>().takeDamage();
            audio.playHitNoise();
        }
    }

    public void takeDamage()
    {
        lives -=1;

        rb.velocity = new Vector2(0, 0);

        ui.loseHeart(lives);
        audio.takeDamage();

        if (lives <= 0)
        {
            ui.restartGame();
        }
        else
        {
            if (facingRight)
            {
                rb.velocity += rb.velocity + Vector2.right * -5 + Vector2.up * 5;
            }
            else
            {
                rb.velocity += rb.velocity + Vector2.right * 5 + Vector2.up * 5;
            }

            anim.SetBool("isJumping", false);
            anim.SetBool("isRunning", false);
            anim.SetBool("takingDamage", true);

            Invoke("stopStun", 0.5f);
        }
    }

    void stopStun()
    {
        anim.SetBool("takingDamage", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.transform.tag == "Ground" || collision.transform.tag == "Box") && !gameOver)
        {
            int t = 0;

            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (collision.contacts[i].normal == new Vector2(0, 1))
                {
                    t += 1;
                }
            }
            if (t > 0)
            {
                isFalling = false;
                anim.SetBool("isJumping", false);
                anim.SetBool("isRunning", true);
            }
        }

        if (collision.transform.tag == "Box" && !holdingBox)
        {
            if (collision.contacts[0].normal == new Vector2(1, 0) && !facingRight)
            {
                boxInHand = collision.transform.gameObject;
            }
            else if (collision.contacts[0].normal == new Vector2(-1, 0) && facingRight)
            {
                boxInHand = collision.transform.gameObject;
            }
        }

        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground" || collision.transform.tag == "Box")
        {
            isFalling = true;
            anim.SetBool("isJumping", true);
        }
        if (collision.transform.tag == "Box" && !holdingBox)
        {
            boxInHand = null;
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag == "Stone")
        {
            if (!gameOver)
            {
                ui.gameOver();
            }
            gameOver = true;
            anim.SetBool("isRunning", false);
            anim.SetBool("isIdle", true);
        }
        if ((collision.transform.tag == "Ground" || collision.transform.tag == "Box") && !gameOver)
        {
            int t = 0;

            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (collision.contacts[i].normal == new Vector2(0, 1))
                {
                    t += 1;
                }
            }

            if (t > 0)
            {

                isFalling = false;
                anim.SetBool("isJumping", false);

                if (isMoving)
                {

                    anim.SetBool("isRunning", true);
                    anim.SetBool("isIdle", false);
                }
                else
                {
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isIdle", true);
                }
            }
        }

        if (collision.contacts[0].normal == new Vector2(-1, 0) && collision.contacts.Length == 1)
        {
            isFalling = true;
        }
        if (collision.contacts[0].normal == new Vector2(1, 0) && collision.contacts.Length == 1)
        {
            isFalling = true;
        }

        if (collision.transform.tag == "Box" && !holdingBox)
        {
            if (collision.contacts[0].normal == new Vector2(1, 0) && !facingRight)
            {
                boxInHand = collision.transform.gameObject;
            }
            else if (collision.contacts[0].normal == new Vector2(-1, 0) && facingRight)
            {
                boxInHand = collision.transform.gameObject;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Health" && lives < 5)
        {
            lives += 1;
            ui.loseHeart(lives);
            audio.playPickup();
            collision.transform.gameObject.SetActive(false);
        }
    }
    public void resetPlayer()
    {
        lives = 5;
        ui.loseHeart(lives);
        gameOver = false;
        this.transform.position = new Vector3(-18, -2, 0);
    }

    public void setGameStart(bool set)
    {
        gameOver = set;
    }

    void playerStep()
    {
        audio.playPlayerStep();
    }

    public void moveLastBox()
    {
        if (undoBoxObj != null && !holdingBox)
        {
            undoBoxObj.GetComponent<Boxes>().moveLastPos();
        }
    }

}
