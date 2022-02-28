using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SingleTargetCamera : MonoBehaviour
{
    public Camera Camera;
    public AudioListener AudioListener;
    public UniqueSoundSourceManager SoundSourceManager;
    public float StartDistance = 8f;
    public float SmoothTime = 1f;
    public float InsideWallDistance = 0.2f;
    public Vector2 StartRotation;
    public float MinWallThickness = 0.5f;
    public float ShakeSettleTime = 1f;
    public float ShakeFallOffDistance = 10f;
    public float ShakeAmplitudeMultiplier = 0.1f;

    public Transform Target { get; private set; }
    public PlayerInput Input { get; private set; }
    public bool AllowInput { get; set; }
    public bool ControlledByEventCamera { get; private set; }
    public CameraViewType CameraViewType { get; private set; }
    public float Distance { get; private set; }

    private Vector3 targetPoint;
    private Vector3 truePosition;
    private PlayerControls playerControls;

    private Vector3 heightOffset;

    private Vector3 moveVelocity;
    private Vector2 currentCameraRotation;
    private float currentZoom;

    private float rotationX;
    private float rotationY;

    private float gamepadSensitivityMultiplier = 10f;
    private float gamepadScrollMultiplier = 0.1f;

    private bool blockerIsActive = false;

    private Vector3 savedPosition;
    private Quaternion savedRotation;

    private float lastShakeStartTime;
    private float currentShakeMagnitude;

    private Health healthComponent;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    public void Initialize(Transform target, PlayerInput input, bool allowInput = true)
    {
        Target = target;
        Input = input;
        AllowInput = allowInput;

        Distance = StartDistance;
        rotationX = StartRotation.x;
        rotationY = StartRotation.y;

        targetPoint = Target.position;

        if (input != null)
        {
            heightOffset = new Vector3(0, 1f, 0);

            input.onActionTriggered += InputActionTriggered;

            if (input.currentControlScheme == "Keyboard")
            {
                playerControls.Gameplay.Zoom.performed += CheckForMouseInput;
                playerControls.Gameplay.RotateCameraWithMouse.performed += CheckForMouseInput;
                playerControls.Gameplay.RotateCameraWithMouse.canceled += MouseLookCancelled;

                playerControls.Enable();

                gamepadSensitivityMultiplier = 1f;
                gamepadScrollMultiplier = 1f;
            }

            healthComponent = target.gameObject.GetComponent<Health>();
            
            if (healthComponent != null)
                healthComponent.OnDamageWasTaken += FlashHurtScreen;
        }
    }

    private void FlashHurtScreen()
    {
        GameUi.Instance.HurtScreen.FlashScreen(CameraViewType, 0.5f);
    }

    private void OnDestroy()
    {
        if (healthComponent != null)
            healthComponent.OnDamageWasTaken -= FlashHurtScreen;
    }

    private Vector3 GetShakePosition(float time, float speed, float size)
    {
        float x = (Mathf.Sin(time * speed * 3) + Mathf.Cos(time * speed * 2)) * 0.5f * size;
        float y = (Mathf.Sin(1.5f + time * speed * 2) + Mathf.Cos(2 + time * speed * 3)) * 0.5f * size;
        float z = (Mathf.Sin(-1 + time * speed * 3) + Mathf.Cos(1 + time * speed * 2.5f)) * 0.5f * size;
        return new Vector3(x, y, z);
    }

    private void MouseLookCancelled(InputAction.CallbackContext context)
    {
        currentCameraRotation = new Vector2(0, 0);
    }

    private void CheckForMouseInput(InputAction.CallbackContext context)
    {
        if (context.control.ToString().ToLower().Contains("mouse"))
        {
            InputActionTriggered(context);
        }
    }

    private void InputActionTriggered(InputAction.CallbackContext actionContext)
    {
        if (actionContext.performed)
        {
            if (actionContext.action.id == playerControls.Gameplay.RotateCameraWithMouse.id)
            {
                Vector2 rotateValue = actionContext.ReadValue<Vector2>();
                currentCameraRotation = rotateValue;
            }
            else if (actionContext.action.id == playerControls.Gameplay.Zoom.id)
            {
                float zoomValue = actionContext.ReadValue<float>();

                if (zoomValue > 0)
                    currentZoom = -1;
                else if (zoomValue < 0)
                    currentZoom = 1;
                else
                    currentZoom = 0;
            }
        }
        else if (actionContext.canceled)
        {
            currentCameraRotation = new Vector2(0, 0);
        }
    }

    private void Update()
    {
        if (!ControlledByEventCamera && AllowInput && Target != null)
        {
            rotationX += currentCameraRotation.x * Time.deltaTime * 20f * gamepadSensitivityMultiplier;
            rotationY -= currentCameraRotation.y * Time.deltaTime * 20f * gamepadSensitivityMultiplier;
            rotationY = Mathf.Clamp(rotationY, -89, 89);

            Distance += currentZoom * gamepadScrollMultiplier * 15f * Time.deltaTime * Distance;
            Distance = Mathf.Clamp(Distance, 2, 20);

            Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);

            targetPoint = Vector3.SmoothDamp(targetPoint, Target.position + heightOffset, ref moveVelocity, SmoothTime);
            truePosition = targetPoint + rotation * new Vector3(0, 0, -Distance);

            //camera shake
            float time = Time.time;
            float shakeTime = time - lastShakeStartTime;
            if (shakeTime < ShakeSettleTime) //camera shake
            {
                currentShakeMagnitude = Mathf.Clamp(currentShakeMagnitude - (1 / ShakeSettleTime) * Time.deltaTime, 0, 1);
                float shakeSettleValue = 1 - Mathf.Clamp(shakeTime, 0, 1);
                Vector3 cameraShakeOffset = GetShakePosition(time + 100, shakeSettleValue * 8, currentShakeMagnitude * ShakeAmplitudeMultiplier);
                truePosition += cameraShakeOffset;
            }

            //avoid walls
            Ray wallCheckRay = new Ray(targetPoint, truePosition - targetPoint);
            RaycastHit[] hits = Physics.RaycastAll(wallCheckRay).Where(x => x.transform.CompareTag("Ground")).ToArray();
            if (hits.Length > 0)
            {
                RaycastHit hit = hits.OrderBy(x => x.distance).FirstOrDefault();
                if (Mathf.Pow(hit.distance, 2) < (truePosition - targetPoint).sqrMagnitude)
                {
                    float thickness = 10000;

                    RaycastHit[] thicknessHits = Physics.RaycastAll(new Ray(truePosition, targetPoint - truePosition)).Where(x => x.transform.CompareTag("Ground")).OrderBy(x => x.distance).ToArray();
                    if (thicknessHits.Length > 0)
                    {
                        RaycastHit thicknessHit = thicknessHits.First();
                        thickness = (thicknessHit.point - hit.point).sqrMagnitude;
                    }
                    else
                    {
                        Debug.LogWarning("Did not hit anything when checking thickness in SingleTargetCamera");
                    }

                    BlockInformationHolder blockInformationHolder = GameManager.Instance.GetBlockInformationHolder(hit.transform);

                    if (thickness > MinWallThickness || (blockInformationHolder != null && blockInformationHolder.Block.Info.BlockType == BlockType.Block))
                    {
                        truePosition = hit.point + (hit.distance > 0.5f ? (hit.normal).normalized * 0.3f : Vector3.zero);
                    }
                }
            }

            transform.position = truePosition;
            transform.rotation = Quaternion.LookRotation(targetPoint - transform.position);

            if (IsInsideObject())
            {
                if (!blockerIsActive)
                {
                    GameUi.Instance.CameraViewBlocker.ActivateBlocker(CameraViewType);
                    blockerIsActive = true;
                }
            }
            else
            {
                if (blockerIsActive)
                {
                    GameUi.Instance.CameraViewBlocker.DeactivateBlocker(CameraViewType);
                    blockerIsActive = false;
                }
            }
        }
        else if (AllowInput && Target == null)
        {
            Debug.LogWarning("Single target camera is null");
        }
    }

    public void StartShake(Vector3 shakeOrigin, float shakePower)
    {
        lastShakeStartTime = Time.time;
        currentShakeMagnitude += Mathf.Clamp((ShakeFallOffDistance - (transform.position - shakeOrigin).magnitude), 0, 100) * shakePower;
    }

    public void StartControlByEventCamera()
    {
        savedPosition = transform.position;
        savedRotation = transform.rotation;
        ControlledByEventCamera = true;

        Health targetHealth = Target.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.InvincibleOverride = true;

            PlayerMovement playerMovement = Target.GetComponent<PlayerMovement>();
            if (playerMovement != null)
                playerMovement.ControlsEnabled = false;
        }
    }

    public void StopControlByEventCamera()
    {
        transform.position = savedPosition;
        transform.rotation = savedRotation;
        ControlledByEventCamera = false;

        Health targetHealth = Target.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.InvincibleOverride = false;

            PlayerMovement playerMovement = Target.GetComponent<PlayerMovement>();
            if (playerMovement != null)
                playerMovement.ControlsEnabled = true;
        }
    }

    public void GetPositionFromEventCamera(EventCamera eventCamera)
    {
        transform.position = eventCamera.CameraOverridePosition;
        transform.rotation = eventCamera.CameraOverrideRotation;
    }

    public bool IsInsideObject()
    {
        return Physics.OverlapSphere(transform.position + transform.forward * InsideWallDistance, 0.001f).Where(x => x.transform.tag == "Ground" || x.transform.tag == "Water").Count() > 0;
    }

    public void DisableSound()
    {
        AudioListener.enabled = false;
        SoundSourceManager.enabled = false;
    }

    public void EnableSound()
    {
        AudioListener.enabled = true;
        SoundSourceManager.enabled = true;
    }

    public void SetViewType(CameraViewType viewType)
    {
        CameraViewType = viewType;

        switch (viewType)
        {
            case global::CameraViewType.Full:
                Camera.rect = new Rect(0, 0, 1, 1);
                EnableSound();
                break;
            case global::CameraViewType.Left:
                Camera.rect = new Rect(0, 0, 0.5f, 1);
                EnableSound();
                break;
            case global::CameraViewType.Right:
                Camera.rect = new Rect(0.5f, 0, 0.5f, 1);
                DisableSound();
                break;
            default:
                break;
        }
    }
}

public enum CameraViewType
{
    Full, Left, Right
}
