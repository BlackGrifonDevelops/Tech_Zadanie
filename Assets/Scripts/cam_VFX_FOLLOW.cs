using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam_VFX_FOLLOW : MonoBehaviour
{
    public Transform player_pos;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(player_pos.position.x, player_pos.position.y+1, player_pos.position.z);

        
        
    }
}
