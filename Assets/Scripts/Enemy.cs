using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    int health = 6;

    float dir = 1;
    float speed = 0.05f;

    bool moveRight = false;

    bool attackingPlayer = false;

    GameObject player;

    [SerializeField]
    GameObject leftTrigger;

    [SerializeField]
    GameObject rightTrigger;

    bool idle = false;

    Animator anim;

    Rigidbody2D rb;

    bool stunned = false;
    float d =0;

    bool doingAttack = false;

    float dist = 0;

    RaycastHit2D hit;

    [SerializeField]
    GameObject target;

    [SerializeField]
    LayerMask playerLayer;

    bool canAttack = false;

    int missedHits = 0;

    UI ui;

    bool dead = false;

    Vector3 pos;

    AudioHandler audio;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        anim = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody2D>();
        ui = GameObject.Find("UI").GetComponent<UI>();
        audio = GameObject.Find("AudioHandler").GetComponent<AudioHandler>();

        this.GetComponent<SpriteRenderer>().flipX = true;
        anim.SetBool("isWalking", true);
        canAttack = true;

        
        Physics2D.queriesStartInColliders = false;
        pos = this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!dead)
        {
            if (!attackingPlayer && !idle && !stunned)
            {
                this.transform.position += new Vector3(speed * dir, 0, 0);
                anim.SetBool("isWalking", true);
            }
            else if (!idle && !stunned && !doingAttack)
            {
                d = this.transform.position.x - player.transform.position.x;
                dist = Vector2.Distance(this.transform.position, player.transform.position);

                if (dist < 2 && canAttack)
                {
                    anim.SetBool("isWalking", false);
                    anim.SetBool("doingAttack", true);

                    doingAttack = true;
                    canAttack = false;
                }

                if (d < 0)
                {
                    dir = 1;
                    this.GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    dir = -1;
                    this.GetComponent<SpriteRenderer>().flipX = false;
                }

                if (dist > 1.3f)
                {
                    this.transform.position += new Vector3(speed * dir, 0, 0);
                }
            }
        }
        else
        {
            Physics2D.IgnoreCollision(this.transform.GetComponent<CapsuleCollider2D>(), player.GetComponent<CapsuleCollider2D>());
        }
    }

    void doAttack()
    {
        int ran = Random.Range(0, 10);

        if (ran > 2 || missedHits == 1)
        {
            Vector3 direction;

            if (dir > 0)
            {
                direction = new Vector3(1, 0, 0);
            }
            else
            {
                direction = new Vector3(-1, 0, 0);
            }

            dist = Vector2.Distance(this.transform.position, player.transform.position);

            if (dist < 2 && !player.GetComponent<Player>().getIsBlocking())
            {
                player.transform.GetComponent<Player>().takeDamage();
            }

            missedHits = 0;
        }
        else
        {
            missedHits += 1;
        }

        if (!dead)
        {
            Invoke("letAttack", Random.Range(1, 2));
        }
    }

    void letAttack()
    {
        canAttack = true;
    }

    void stopAttack()
    {
        doingAttack = false;
        anim.SetBool("doingAttack", false);
        anim.SetBool("isWalking", true);
    }

    public void takeDamage()
    {
        rb.velocity = new Vector2(0, 0);
        if (d < 0)
        {
            rb.velocity += rb.velocity + Vector2.right * -5 + Vector2.up * 2;
        }
        else
        {
            rb.velocity += rb.velocity + Vector2.right * 5 + Vector2.up * 2;
        }   
        if (!dead)
        {
           
            health -= 2;

            if (health <= 0)
            {
                dead = true;
                anim.SetBool("takingDamage", false);
                anim.SetBool("isWalking", false);
                anim.SetBool("doingAttack", false);
                ui.enemyKilled();
                anim.SetTrigger("isDead");
                anim.SetBool("dead", true);
                audio.enemyKill();             
            }

            if (!dead)
            {
                anim.SetBool("takingDamage", true);
                anim.SetBool("isWalking", false);

                stunned = true;
                Invoke("unStun", Random.Range(0.5f, 0.7f));
            }
            
        }
    }

    void unStun()
    {
        if (!dead)
        {
            stunned = false;
            anim.SetBool("takingDamage", false);
            anim.SetBool("isWalking", true);
        }
    }

    void continueRoute()
    {
        idle = false;
        anim.SetBool("isWalking", true);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!dead)
        {
            if (collision.gameObject == rightTrigger)
            {
                dir = -1;
                this.GetComponent<SpriteRenderer>().flipX = false;
                idle = true;
                Invoke("continueRoute", 3);
                anim.SetBool("isWalking", false);
            }


            if (collision.gameObject == leftTrigger)
            {
                dir = 1;
                this.GetComponent<SpriteRenderer>().flipX = true;
                idle = true;
                Invoke("continueRoute", 3);
                anim.SetBool("isWalking", false);
            }

            if (collision.tag == "Player")
            {
                attackingPlayer = true;
                continueRoute();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!dead)
        {
            if (collision.tag == "Player" && !dead)
            {
                attackingPlayer = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!dead)
        {
            if (collision.transform.tag == "Ground" && dead)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }

    public void respawnEnemy()
    {
        dead = false;
        health = 6;
        doingAttack = false;
        canAttack = true;
        idle = false;

        this.transform.position = pos;
        anim.ResetTrigger("isDead");
        anim.SetBool("dead", false);
        continueRoute();
    }

    public bool getIsDead()
    {
        return dead;
    }
}
