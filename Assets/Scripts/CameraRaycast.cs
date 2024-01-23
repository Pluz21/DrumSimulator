using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using Unity.VisualScripting;


public class CameraRaycast : MonoBehaviour
{

    public event Action<RaycastHit> OnRaycastHit;
    [SerializeField,Range(5,100)] float detectionDistance = 0;
    [SerializeField] bool drumDetect = false;
    [SerializeField] Ray screenRay;
    [SerializeField] LayerMask drumLayer;
    [SerializeField] Stick stick = null;
    [SerializeField] GameObject drumElementHit= null;
    [SerializeField] Drumkit bassDrum = null;
    [SerializeField] RaycastHit drumHit;

    //Inputs 
    [SerializeField] MyInputs controls = null;
    [SerializeField] InputAction mousePos = null;
    [SerializeField] InputAction hit = null;

    [SerializeField] InputAction charleston = null;
    [SerializeField] InputAction crash = null;
    [SerializeField] InputAction highTom = null;
    [SerializeField] InputAction mediumTom = null;
    [SerializeField] InputAction lowTom = null;
    [SerializeField] InputAction ride = null;
    [SerializeField] InputAction bass = null;


    float initialDetectionDistance = 0;
    void Awake()
    { 
        controls = new MyInputs();
    }
    void Start()
    {
        Init();
    }

    void Init()
    {
        initialDetectionDistance = detectionDistance;
        bass.performed += InteractWithBass;
    }

    void Update()
    {
        Detect();
    }

    void Detect()
    {
        Vector2 _pos2D = mousePos.ReadValue<Vector2>();
        Vector3 _pos = new Vector3(_pos2D.x, _pos2D.y, detectionDistance);
        screenRay = Camera.main.ScreenPointToRay(_pos);
        bool _hitDrum = Physics.Raycast(screenRay, out RaycastHit _drumHitResult, detectionDistance,drumLayer);
        Debug.DrawRay(screenRay.origin, screenRay.direction * 20);
        drumDetect = _hitDrum; 
            Debug.Log($"hit drum : {drumDetect}");
        if (drumDetect)
        {
            drumHit = _drumHitResult;
            detectionDistance = _drumHitResult.distance + 2;
            UpdateStickPosition(_drumHitResult.point);
            hit.performed += HitDrum;
            OnRaycastHit?.Invoke(_drumHitResult);

        }
        else 
        {
            detectionDistance = initialDetectionDistance;
            hit.performed -= HitDrum;
        }
        
    }

    private void HitDrum(InputAction.CallbackContext _context)
    {
        Debug.Log("Hit the drum");
        InteractWithDrumElement(drumHit);
    }

    void UpdateStickPosition(Vector3 _pos)
    { 
        stick.transform.position = _pos;
    }

    void InteractWithDrumElement(RaycastHit _hitResult)
    {
        drumElementHit = _hitResult.transform.gameObject;
        if (!drumElementHit) return;
        Drumkit _drumKitElement = drumElementHit.GetComponent<Drumkit>();
        if (!_drumKitElement) return;
        _drumKitElement.PlaySound();
    }

    void InteractWithBass(InputAction.CallbackContext _context)
    {
        Debug.Log("called interact with bass"); 
        if (bassDrum.drumType == Drumkit.DrumTypes.BASS)
            bassDrum.PlaySound();
    }
    void OnEnable()
    {
        mousePos = controls.Player.MousePos;
        mousePos.Enable();

        hit = controls.Player.Hit;
        hit.Enable();

        charleston = controls.Player.Charleston;
        charleston.Enable();

        crash = controls.Player.Crash;
        crash.Enable();

        ride = controls.Player.Ride;
        ride.Enable();

        highTom = controls.Player.HighTom;
        highTom.Enable();

        mediumTom = controls.Player.MediumTom;
        mediumTom.Enable();

        lowTom = controls.Player.LowTom;
        lowTom.Enable();

        bass = controls.Player.Bass;
        bass.Enable();
    }


    
    
    
    
    
    

}
