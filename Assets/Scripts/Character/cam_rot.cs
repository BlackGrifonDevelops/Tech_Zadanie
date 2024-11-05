using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam_rot : MonoBehaviour
{
    public Transform player_pos;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(player_pos.position.x, player_pos.position.y+15, player_pos.position.z);

        if (Input.GetKey(KeyCode.E))
        {
            this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y + 1, this.transform.rotation.eulerAngles.z);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y - 1, this.transform.rotation.eulerAngles.z);
        }
        
    }
}
