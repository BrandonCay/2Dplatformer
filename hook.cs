using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hook : MonoBehaviour
{
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject proj;
    playerMovement pm;
    private bool prevActive, active, fired, prevFired;
    private float maxDist = 5.0f;
    // Start is called before the first frame update

    /*
     * 1/13
     * Debating whether to use physics engine or manually script hook.
     * -Hook isn't realist and will extend from player to 
     * 
     * 
     */

    private void Awake()
    {
        pm = player.GetComponent<playerMovement>();
        crosshair.SetActive(false);
    }

    void Start()
    {
        
    }
   /*
    * dont forget to add ceiling check when crouched in pM.cs
    */

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            crosshair.SetActive(true);
            active = true;
            aim();
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (fired)
                {
                    proj.SetActive(true);
                    fired = true;
                }
                else
                {
                    fired = false;
                    proj.SetActive(false);

                }

            }
            
        }
        if(prevActive==true && active == false)
        {
            crosshair.SetActive(false);
            active = false;
        }
        prevActive = active;
        active = false;
    }

    private void FixedUpdate()
    {
        if (fired)
        {
            shoot();
        }
    }

    private void aim()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector3 diff = mouseWorldPos - transform.position;
        diff.z = 0f;
        Vector3 aimDirLine = diff.normalized;//makes vector mag = 1 but keeps direction/angle
        
        //direciton line/hyp from player i.e. player = center of unit circle and the difference = location of the mouse relative to player
        float angle = Mathf.Atan2(aimDirLine.y, aimDirLine.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);//switch back 
        if (diff.magnitude < maxDist)
        {
            crosshair.transform.position = mouseWorldPos;
        }
        else
        {
            //angle is somewhat off. Could be magnitude from previous 
             crosshair.transform.position = transform.position + aimDirLine * maxDist;   
        }
        pm.setLookAngle(angle);
        Debug.Log(mouseWorldPos.ToString() + " | " + angle.ToString());
    }

    //shoot. hold down left click to aim. right to cancel. release left click to throw. 
    //sticks to specific "hookable surfaces" (use layers or try raycast)
    private void shoot()  
    {
        Vector3 currVel = new Vector3(0,0,0); 
    }
    private void retract()
    {

    }
}
