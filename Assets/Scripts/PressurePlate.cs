using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField]
    GameObject door;

    bool open = false;
    bool close = false;

    Vector3 originalPos;
    float move = 3;

    AudioHandler audio;

    bool doorIsOpen = false;

    bool boxOnPlate = false;
    // Start is called before the first frame update
    void Start()
    {
        originalPos = door.transform.position;
        audio = GameObject.Find("AudioHandler").GetComponent<AudioHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if(open && door.transform.position.y <= originalPos.y + move)
        {
            door.transform.Translate(0, 0.1f, 0);
        }
        else if(close && door.transform.position.y >= originalPos.y)
        {
            door.transform.Translate(0, -0.1f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Box" && !doorIsOpen)
        {
            open = true;
            close = false;
            audio.openDoor();
            doorIsOpen = true;
            boxOnPlate = true;
        }
        if (collision.transform.tag == "Player" && !boxOnPlate)
        {
            open = true;
            close = false;
            doorIsOpen = false;
            audio.openDoor();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && !boxOnPlate)
        {
            open = false;
            close = true;
            doorIsOpen = false;
        }
        if (collision.transform.tag == "Box")
        {
            open = false;
            close = true;
            doorIsOpen = false;
            boxOnPlate = false;
        }

    }
    public void closeDoor()
    {
        close = true;
        open = false;
        doorIsOpen = false;
    }

}
