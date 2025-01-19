using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchEffect : MonoBehaviour
{
    PlayerInput newInputSyste;

    InputAction touchEffect;

    [SerializeField]
    ParticleSystem touchEffParticle;
    [SerializeField]
    TrailRenderer trailRenderer;

    private void Awake()
    {
        newInputSyste = GetComponent<PlayerInput>();
        touchEffect = newInputSyste.actions["ScreenEffect"];
        touchEffect.performed += Click;
    }

    public void Click(InputAction.CallbackContext context)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>()) + Camera.main.transform.forward;

        if (Touchscreen.current.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
        {
            touchEffParticle.transform.position = pos;
            touchEffParticle.transform.localScale = Vector3.one * Camera.main.orthographicSize * 0.04f;
            touchEffParticle.Play();
            trailRenderer.enabled = false;
            trailRenderer.Clear();
            if (Time.timeScale != 0)
            {
                trailRenderer.transform.position = pos;
                trailRenderer.enabled = true;
            }
        }
        else if (Touchscreen.current.primaryTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            if (Time.timeScale != 0)
                trailRenderer.transform.position = pos;
        }

    }


}
