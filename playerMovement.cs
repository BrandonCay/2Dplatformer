using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 *NOTE 12/27
 *consider crouch with speed parameter. 
 *Features to add:
 *Grapple hook
 *Animations (free from unity)
 *
 *
 */

public class playerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController player;
    [SerializeField] private Transform floorCheck, grabCheck, playerCtrlrTrans;
    [SerializeField] private LayerMask ground, notPlayer;
    private float yVelocity, xVelocity, gravityA = -10f, xSpeed = .05f, walkSpeed = 4f, walkAcc = 3f, crouchSpeed= 1f, sprintSpeed=6f
    ,lookAngle;
    private bool onGround, crouched = false, grabbing = false, right = true, sliding=false;
    private float jumpHeight=1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics.CheckSphere(floorCheck.position, .1f, ground);
        if (grabbing)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                xSpeed = sprintSpeed;
            }
            else
            {
                xSpeed = walkSpeed;
            }
            xVelocity = calcXvelocity();
            if (Input.GetKeyDown("space"))
            {
                jump();
                grabbing = false;
            }else if (Input.GetKeyDown(KeyCode.E))
            {
                gravity();
                grabbing = false;
            }
            else
            {
                xVelocity = 0;
            }

        }
        else if (onGround)//make more efficient by removing slide and copying code (functions)
        {
            yVelocity = 0f;
            if (sliding) //slide prevents all inputs
            {
                slide();
            }
            else {
                if (Input.GetKey(KeyCode.LeftShift)) {

                       if (crouched)
                       {
                           stand();
                       }
                    xSpeed = sprintSpeed;

                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        crouch();
                        slide();
                        sliding = true;
                    
                    }
                }else if(Input.GetKeyDown(KeyCode.C))
                {
                    if (crouched)
                    {
                        stand();
                    }
                    else
                    {
                        xSpeed = crouchSpeed;
                        crouch();
                    }
                }
                else
                {
                    xSpeed = walkSpeed;
                }
                if (!checkAndSetGrab())
                {
                    xVelocity = calcXvelocity();
                    jumpInput();
                }
                
                    Debug.Log(xVelocity.ToString() + "|" + xSpeed.ToString());
            }
        }
        else
        {
            if(!checkAndSetGrab()) //checkAndSetGrab repeated to avoid unncessary check when already grabbing 
                gravity();

        }

       
        Vector3 move = new Vector3(xVelocity, yVelocity);
        if (!Mathf.Approximately(lookAngle,0f))
        {
            if ((lookAngle > 91f || lookAngle < -91f) && right)
            {
                flip();
                Debug.Log("Flip to left");
            }
            if((lookAngle < 90f && lookAngle > -90f) && !right)
            {
                flip();
                Debug.Log("Flip to left");
            }
            lookAngle = 0f;
        }
        else if((xVelocity < 0 && right) || (xVelocity > 0 && !right))
        {
            flip();
            Debug.Log("Flippp");
        }
        Debug.Log((xVelocity*Time.deltaTime).ToString()+""+ right);
        player.Move(move * Time.deltaTime);
    }

    private float calcXvelocity()
    {
        return Input.GetAxis("Horizontal") * xSpeed;
    }
    private void crouchCheckAndSet() //shortens collider and model
    {
        if (crouched)
        {
            //player.height *= 2f;
            stand();
           
        }
        else
        {
            //  player.height /= 2f;
            crouch();
        }
    }

    private void jumpInput()
    {
        if (Input.GetKeyDown("space"))
        {
            if (crouched)
                  stand(); //uncrouch before jumping
            jump();
        }
    }
    private void jump()
    {
        grabbing = false;
        if (crouched)
        {
            stand();
        }
        yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravityA); //kinematic equation without respect to time

    }
    
    private bool checkAndSetGrab()
    {
        if (Input.GetKeyDown(KeyCode.E) && !grabbing && Physics.CheckSphere(grabCheck.position, .5f, notPlayer))
        {
            grabOn();
            return true;
        }
        return false;

    }
    private void grabOn()
    {
        grabbing = true;
        xVelocity = 0f;
        yVelocity = 0f;
    }
    private void grabOff()
    {
        grabbing = false;
        gravity();
    }
    private void flip()
    {

        transform.Rotate(0, 180, 0);
        right = !right;
    }

    private void slide()
    {
        float slideScalar = 5f;
        float slideAcc = (xVelocity > 0) ? -slideScalar : slideScalar;
        if (Mathf.Abs(xVelocity) > crouchSpeed)
        {
            xVelocity = xVelocity + slideAcc * Time.deltaTime;
            Debug.Log("Sliding"+xVelocity.ToString());
        }
        else
        {
            sliding = false;
            Debug.Log("Sliding done:"+ xVelocity.ToString() +""+ sliding.ToString());
        }
    }

    private void gravity()
    {
        yVelocity += calcGravity();

    }
    private float calcGravity()
    {
        return gravityA * Time.deltaTime;
    }
    private void crouchInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            crouchCheckAndSet();
            xSpeed = crouchSpeed;
        }
        
    }

    private void crouch() //NOTE: actual crouch might only require a change in collision box while the animation does the model change
    {

        playerCtrlrTrans.localScale -= new Vector3(0f, 0.5f, 0f);
        player.enabled = false;
        playerCtrlrTrans.position -= new Vector3(0f, .5f, 0f);
        player.enabled = true;
        crouched = true;

    }
    private void stand()
    {
        playerCtrlrTrans.localScale += new Vector3(0f, 0.5f, 0f);
        player.enabled = false;
        playerCtrlrTrans.position += new Vector3(0f, .5f, 0f);//doesn't work. Try assignment
        player.enabled = true;
        crouched = false;

    }

    public void setLookAngle(float la)
    {
        lookAngle = la;
    }
 
}
/*
 * deaccerlation code
else if (xSpeed > maxSpeed)
{
    xSpeed -= 3f * Time.deltaTime;
}
else
{
    xSpeed = Mathf.Abs(xVelocity) + walkAcc * Time.deltaTime;
}*/