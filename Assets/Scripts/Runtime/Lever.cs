using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lever : MonoBehaviour
{
    private bool playerInRange = false;
    private bool on = false;
    private InputAction interactAction;

    [SerializeField]
    private float switchTime;

    [SerializeField]
    private Transform onPosition;

    [SerializeField]
    private Transform offPosition;

    [SerializeField]
    private GameObject leverHandle;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.interactAction = InputSystem.actions.FindAction("Interact");
    }

    IEnumerator InterpolateLeverCoroutine()
    {
        Vector3 startPosition, targetPosition;
        Quaternion startRotation, targetRotation;

        if(this.on)
        {
            startPosition = this.offPosition.position;
            targetPosition = this.onPosition.position;

            startRotation = this.offPosition.rotation;
            targetRotation = this.onPosition.rotation;

        } else
        {
            startPosition = this.onPosition.position;
            targetPosition = this.offPosition.position;

            startRotation = this.onPosition.rotation;
            targetRotation = this.offPosition.rotation;
        }

        float currentInterpolationTime = 0.0f;
        while (currentInterpolationTime < switchTime)
        {
            float percentage = currentInterpolationTime / switchTime;

            var currentPosition = Vector3.Lerp(startPosition, targetPosition, percentage);
            var currentRotation = Quaternion.Slerp(startRotation, targetRotation, percentage);

            this.leverHandle.transform.SetPositionAndRotation(currentPosition, currentRotation);

            yield return null; 
            
            currentInterpolationTime += Time.deltaTime;
        }

        this.leverHandle.transform.SetPositionAndRotation(targetPosition, targetRotation);
    } 

    void ToggleLever()
    {
        this.on = !this.on;
        this.StartCoroutine(this.InterpolateLeverCoroutine());
    }

    void FixedUpdate()
    {
        if(this.playerInRange && this.interactAction.WasPressedThisFrame())
        {
            this.ToggleLever();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var mask = LayerMask.NameToLayer("Player");
        if(other.gameObject.layer == mask)
        {
            this.playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var mask = LayerMask.NameToLayer("Player");
        if (other.gameObject.layer == mask)
        {
            this.playerInRange = false;
        }
    }
}
