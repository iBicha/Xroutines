using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForCollision : CustomYieldInstruction
{
    private bool collisionDetected;
    private CollisionListenerBehaviour collisionListenerBehaviour;

    public WaitForCollision(Collider collider)
    {
        collisionListenerBehaviour = collider.gameObject.AddComponent<CollisionListenerBehaviour>();
        collisionListenerBehaviour.OnCollision += OnCollision;
    }

    public WaitForCollision(Collider2D collider2d)
    {
        collisionListenerBehaviour = collider2d.gameObject.AddComponent<CollisionListenerBehaviour>();
        collisionListenerBehaviour.OnCollision += OnCollision;
    }

    ~WaitForCollision()
    {
        CleanUp();
    }

    private void OnCollision()
    {
        collisionDetected = true;
        CleanUp();
    }

    private void CleanUp()
    {
        if(collisionListenerBehaviour != null)
        {
            collisionListenerBehaviour.OnCollision -= OnCollision;
            GameObject.Destroy(collisionListenerBehaviour);
            collisionListenerBehaviour = null;
        }
    }

    public override bool keepWaiting
    {
        get
        {
            return !collisionDetected;
        }
    }

    class CollisionListenerBehaviour : MonoBehaviour
    {
        public event Action OnCollision;

        private void OnCollisionEnter(Collision collision)
        {
            OnCollision();
        }

        private void OnCollisionStay(Collision collision)
        {
            OnCollision();
        }

        private void OnTriggerEnter(Collider other)
        {
            OnCollision();
        }

        private void OnTriggerStay(Collider other)
        {
            OnCollision();
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnCollision();
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            OnCollision();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnCollision();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            OnCollision();
        }

        //In case the Collider got destroyed, we're just going to fire up this event 
        //so the routine won't get stuck forever waiting for a collision on a deleted collider 
        private void OnDestroy()
        {
            Debug.LogWarning("Xroutine waiting on a deleted collider. Skipping routine.");
            OnCollision();
        }
    }

}
