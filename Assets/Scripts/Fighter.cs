﻿using UnityEngine;
using System.Collections;

public class Fighter : Destructable
{
    private static Vector3 gravity = Physics.gravity;

    public float movespeed = 3f;
    public float weaponRange = 3f;
    public float reloadTime = 0.1f;
    public float damage = 20;

    public GameObject target;

    private CharacterController controller;
    private GameObject currentTarget;
    private float lastShotFired = 0f;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (target == null)
            target = FindClosestBase();
        currentTarget = target;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTarget == null && currentTarget != target)
        {
            currentTarget = target;
        }

        Vector3 direction = Vector3.zero;
        if (currentTarget != null)
        {
            // Determine the direction to the target
            direction = currentTarget.transform.position - transform.position;
            // Ignore its height
            direction.y = 0;
            // Look at the target
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        // Always apply gravity
        Vector3 movement = gravity;
        // Move towards the target if its out of range
        if (direction.magnitude > weaponRange)
        {
            direction.Normalize();
            movement += direction * movespeed;
        }
        else if (currentTarget != null)
        {
            float now = Time.time;
            if ((now - lastShotFired) > reloadTime)
            {
                lastShotFired = now;
                Debug.Log(name + ": Firing at " + currentTarget.name);
                currentTarget.SendMessage("OnFiredAt", damage);
            }
        }
        controller.Move(movement * Time.deltaTime);
    }

    GameObject FindClosestBase()
    {
        GameObject[] boards = GameObject.FindGameObjectsWithTag("Base");
        GameObject closestBaseTransform = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject board in boards)
        {
            float dist = Vector3.Distance(board.transform.position, currentPos);
            if (dist < minDist)
            {
                closestBaseTransform = board;
                minDist = dist;
            }
        }
        return closestBaseTransform;
    }

    void OnTriggerEnter(Collider contact)
    {
        GameObject contactObject = contact.gameObject;
        Destructable enemy = contactObject.GetComponent<Destructable>();
        if (enemy == null || enemy.gameObject == this.gameObject)
            return;
        currentTarget = enemy.gameObject;
    }

    void onTriggerExit(Collider contact)
    {
        if (contact.gameObject == currentTarget)
            currentTarget = target;
    }
}
