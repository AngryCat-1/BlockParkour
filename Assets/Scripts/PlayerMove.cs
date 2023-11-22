using System.Collections;using System.Collections.Generic;using UnityEngine;using UnityEngine.UI;public class PlayerMove : MonoBehaviour{
    float x = -302.16f;
    float y = 66.83f;
    float z = 604.17f;

    [SerializeField] float jumpForce = 20f;
    [SerializeField] float gravity = 40f;

    [SerializeField] CharacterController controller;
    [SerializeField] float speed = 10f;
    [SerializeField] float check_speed = 10f;
    
    float time = 0;
    public int timerOxygen;
    [SerializeField] Text TimeText;
    private Vector3 direction;
    int health;
    [SerializeField] GameObject particle;
    // Start is called before the first frame update
    void Start()    {


    }
    // Update is called once per frame
    void Update()    {



        
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            direction = new Vector3(moveHorizontal, 0, moveVertical);
            direction = transform.TransformDirection(direction) * speed;
            if (Input.GetKey(KeyCode.LeftShift))
                speed = check_speed * 2;
            else
                speed = check_speed;
            if (Input.GetKey(KeyCode.Space))
                direction.y += jumpForce;
        }


        direction.y -= gravity * Time.deltaTime;
        controller.Move(direction * Time.deltaTime);






    }
}    