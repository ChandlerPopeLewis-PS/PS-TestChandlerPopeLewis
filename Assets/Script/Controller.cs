using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour
{
    public Vector3[] lanes = new Vector3[] {
        new Vector3(1f, 0, 0),
        new Vector3(3f, 0, 0),
        new Vector3(5f, 0, 0),
    };
    public Vector3 speed = new Vector3(0, 0, 10);
    public float laneSwitchSpeed = 10;

    public int currentLane = 1;
    public int health = 5;

    public Text healthText;
    public Text pointsText;
    public GameObject pauseMenu;

    private Rigidbody _rigidbody;

    private int previousLane = 1;
    private float transitionTime = 0;

    private int points = 0;
    // Start is called before the first frame update
    void Start()
    {
        dragDistance = Screen.height * 5 / 100; //dragDistance is 15% height of the screen
        _rigidbody = GetComponent<Rigidbody>();
        
        healthText.text = health.ToString();
        pointsText.text = points.ToString();
    }
    private Vector3 fp;   //First touch position
    private Vector3 lp;   //Last touch position
    private float dragDistance;  //minimum distance for a swipe to be registered
 
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1) // user is touching the screen with a single touch
        {
            Touch touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                fp = touch.position;
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
            {
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {
                lp = touch.position;  //last touch position. Ommitted if you use list
 
                //Check if drag distance is greater than 20% of the screen height
                if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
                {//It's a drag
                 //check if the drag is vertical or horizontal
                    if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
                    {   //If the horizontal movement is greater than the vertical movement...
                        if ((lp.x > fp.x))  //If the movement was to the right)
                        {   //Right swipe
                            OnMove(1);
                        }
                        else
                        {   //Left swipe
                            OnMove(-1);
                        }
                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
        } else if(Input.GetKeyDown(KeyCode.A)) {
            OnMove(-1);
        } else if(Input.GetKeyDown(KeyCode.D)) {
            OnMove(1);
        }
    }
    public void OnMove(float direction) {
        if(direction > 0) {
            if(currentLane < lanes.Length - 1) {
                previousLane = currentLane++;
                transitionTime = 0;
            }
        } else if(direction < 0) {
            if(currentLane > 0) {
                previousLane = currentLane--;
                transitionTime = 0;
            }
        }
    }

    private void FixedUpdate() {
        Vector3 newPosition = transform.position + (speed * Time.fixedDeltaTime);
        if(transitionTime < 1 && transitionTime >= 0) {
            newPosition.x = Mathf.Lerp(lanes[previousLane].x, lanes[currentLane].x, transitionTime += (Time.fixedDeltaTime * laneSwitchSpeed));
        }

        _rigidbody.MovePosition(newPosition);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag.Equals("Obstacle")) {
            health -= 1;
            healthText.text = health.ToString();
            if(health <= 0) {
                string name = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(name);
            }
        } else if(other.tag.Equals("HoleObstacle")) {
            string name = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(name);
        } else if(other.tag.Equals("Collectible")) {
            points++;
            pointsText.text = points.ToString();
            ObjectPool.GetObjectPool("Collectible").Push(other.gameObject);
        }
    }

    public Action onPlayerChangedFloor;
    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag.Equals("Floor")) {
            onPlayerChangedFloor();
        }
    }
}
