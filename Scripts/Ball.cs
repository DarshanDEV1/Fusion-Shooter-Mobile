using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    [Networked] private TickTimer _life { get; set; }

    public void Init()
    {
        _life = TickTimer.CreateFromSeconds(Runner, 5.0f);
        //transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>().Play();
    }

    public override void FixedUpdateNetwork()
    {
        if(_life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else
            transform.position += 5 * transform.forward * Runner.DeltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.name);
    }
}
