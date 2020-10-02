using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boxes : MonoBehaviour
{
    Rigidbody2D rb;
    bool holding = false;

    GameObject player;

    Vector3 pos;

    AudioHandler audio;

    Vector3 lastPos;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        pos = this.transform.position;
        audio = GameObject.Find("AudioHandler").GetComponent<AudioHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(rb.velocity.y < 0)
        {
            Physics2D.IgnoreCollision(this.GetComponent<BoxCollider2D>(), player.GetComponent<CapsuleCollider2D>(), true);
        }
        else
        {
            Physics2D.IgnoreCollision(this.GetComponent<BoxCollider2D>(), player.GetComponent<CapsuleCollider2D>(), false);
        }
    }

    public void setHolding(bool set)
    {
        holding = set;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.transform.tag == "Ground" || collision.transform.tag == "Box") && !holding)
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

                rb.constraints = RigidbodyConstraints2D.FreezeAll;

            }

            audio.playDrop();

        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((collision.transform.tag == "Ground" || collision.transform.tag == "Box") && !holding)
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
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((collision.transform.tag == "Ground" || collision.transform.tag == "Box"))
        {
            Physics2D.IgnoreCollision(collision.collider, this.transform.GetComponent<BoxCollider2D>(), false);
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void setLastPos(Vector3 p)
    {
        lastPos = p;
    }
    public void moveLastPos()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        this.transform.localPosition = lastPos + new Vector3(0, 1, 0);
    }

    public void resetBox()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        this.transform.localPosition = pos;
    }

}
