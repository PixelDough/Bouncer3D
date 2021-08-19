using System;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class PlayerController : MonoBehaviour
    {

        [SerializeField] private PlayerStuffManager playerStuffManager;
        
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private new Collider collider;
        private PhysicMaterial _colliderMaterial;
        
        private Vector3 _inputMovement;

        private float _defaultBounciness = 1;
        
        [SerializeField] private float jumpBufferMax = 0.2f;
        private float _jumpBuffer = 0f;

        [SerializeField] private float coyoteTimeMax = 0.2f;
        private float _coyoteTime = 0f;

        private bool _isGrounded;
        private Vector3 _pastVelocity = Vector3.zero;
        
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            rigidbody.maxAngularVelocity = 100f;
            
            _colliderMaterial = collider.material;
            _defaultBounciness = _colliderMaterial.bounciness;
        }

        private void Update()
        {
            HandleMovementInput();
            HandleDampen();

            // Move the jump buffer towards 0
            _jumpBuffer = Mathf.MoveTowards(_jumpBuffer, 0f, Time.deltaTime);
            // If the player has pressed the jump button, reset the jump buffer to the max
            if (GameManager.Instance.Input.GetButton(RewiredConsts.Action.Jump))
            {
                _jumpBuffer = jumpBufferMax;
            }

            _coyoteTime = Mathf.MoveTowards(_coyoteTime, 0f, Time.deltaTime);
            if (_isGrounded) _coyoteTime = coyoteTimeMax;

            if (_jumpBuffer > 0f)
            {
                if (_isGrounded || _coyoteTime > 0f)
                {
                    Jump();
                }
            }
            
            // Important: Set this AFTER checking for a buffered jump, as isGrounded might have been set in OnCollisionEnter.
            _isGrounded = false;
            if (Physics.SphereCast(transform.position, 0.24f, Vector3.down, out RaycastHit hit, 
                0.02f, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore))
            {
                if (!Physics.GetIgnoreCollision(collider, hit.collider))
                    _isGrounded = true;
            }
        }

        private void FixedUpdate()
        {
            rigidbody.AddForce(Physics.gravity);
            
            /*if (_isGrounded)
            {
                float reverseMultiplier = 1f;
                Vector3 flattenedVelocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
                if (Vector3.Angle(flattenedVelocity.normalized, _inputMovement) > 90) reverseMultiplier = 2f;
                    
                Vector3 inputConvertedToTorque = Quaternion.Euler(0, 90, 0) * _inputMovement;
                rigidbody.AddTorque(
                    new Vector3(inputConvertedToTorque.x, 0f, inputConvertedToTorque.z) *
                    (200f * reverseMultiplier * Time.fixedDeltaTime),
                    ForceMode.VelocityChange);
            }
            else
            {
                rigidbody.AddForce(_inputMovement / 5f * (60 * Time.fixedDeltaTime), ForceMode.VelocityChange);
            }*/
            
            rigidbody.AddForce(_inputMovement * (12 * Time.fixedDeltaTime), ForceMode.VelocityChange);

            rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 30);
            
            _pastVelocity = rigidbody.velocity;
        }

        private void OnCollisionEnter(Collision other)
        {
            foreach (var point in other.contacts)
            {
                if (Vector3.Angle(point.normal, -Physics.gravity) < 15f)
                {
                    _isGrounded = true;
                }
            }

            foreach (var point in other.contacts)
            {
                // If the angle is 
                //if (Vector3.Angle(-point.normal, _pastVelocity) > 80) continue;

                // If the velocity is heading towards the normal at a high enough speed
                if (Vector3.Dot(_pastVelocity, -point.normal) > 3f)
                {
                    //Squish();
                    if (Mathf.Clamp(point.normal.y, -0.1f, 0.1f) == point.normal.y)
                    {
                        rigidbody.AddForce(point.normal * rigidbody.velocity.magnitude / 8f, ForceMode.VelocityChange);
                        rigidbody.AddForce(Vector3.up * rigidbody.velocity.magnitude / 1.2f, ForceMode.VelocityChange);
                    }
                    break;
                }
            }
        }
        
        private void HandleMovementInput()
        {
            Vector2 rawInputMovement = GameManager.Instance.Input.GetAxis2D(RewiredConsts.Action.MoveHorizontal,
                RewiredConsts.Action.MoveVertical);
            Vector3 rawInputMovementVector3 = new Vector3(rawInputMovement.x, 0f, rawInputMovement.y);
            Vector3 cameraRelativeInput = CameraRelativeFlatten(rawInputMovementVector3);
            cameraRelativeInput = cameraRelativeInput.normalized * cameraRelativeInput.magnitude;

            _inputMovement = cameraRelativeInput;
        }
        
        private void HandleDampen()
        {
            if (GameManager.Instance.Input.GetButton(RewiredConsts.Action.Dampen))
            {
                _colliderMaterial.bounciness = 0.5f;
            }
            else if (GameManager.Instance.Input.GetButton(RewiredConsts.Action.Jump))
            {
                _colliderMaterial.bounciness = 0.9f;
            }
            else
            {
                _colliderMaterial.bounciness = _defaultBounciness;
            }

            collider.material = _colliderMaterial;
        }

        private void Jump()
        {
            if (Mathf.Abs(rigidbody.velocity.y) < 10f)
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, 10f, rigidbody.velocity.z);
            _jumpBuffer = 0f;
            _coyoteTime = 0f;
        }

        Vector3 CameraRelativeFlatten(Vector3 input)
        {
            return Quaternion.Euler( 0, _camera.transform.rotation.eulerAngles.y, 0) * input;
        }

        public void CollectShells(int count)
        {
            playerStuffManager.CollectShells(count);
        }
    }
}

/*
using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class SeveredHeadEffectController : EffectController
{

    [SerializeField] private new Rigidbody rigidbody;

    private Vector3 _movement;
    private bool _isSprinting = false;
    private bool _isGrounded = false;

    [SerializeField] private float jumpBufferMax = 0.1f;
    private float _jumpBuffer = 0f;

    [Header("Blood")]
    [SerializeField] private ParticleSystem bloodParticleSystem;
    [SerializeField] private ParticleSystem bloodTrailParticleSystem;
    private bool _isBloodTrailPlaying = false;
    [SerializeField] private ParticleSystem bloodTrailAirParticleSystem;

    [Header("Snow")]
    [SerializeField] private AnimationCurve snowballCurve;
    private float _snowballPercent = 0f;
    [SerializeField] private Transform snowball;
    [SerializeField] private ParticleSystem snowBurstParticleSystem;
    [SerializeField] private ParticleSystem snowBurstBigParticleSystem;
    [SerializeField] private ParticleSystem snowTrailParticleSystem;
    [SerializeField] private ParticleSystem snowTrailAirParticleSystem;

    private Vector3 _pastVelocity = Vector3.zero;
    
    private void Awake()
    {
        rigidbody.maxAngularVelocity = 100;

        bloodTrailParticleSystem.Stop();
        bloodTrailAirParticleSystem.Play();

        snowTrailParticleSystem.Stop();
        snowTrailAirParticleSystem.Stop();
    }

    public override void _DoUpdate()
    {
        // Count the jump buffer towards 0
        _jumpBuffer = Mathf.MoveTowards(_jumpBuffer, 0, Time.deltaTime);
        
        // Store the player's current directional inputs
        Vector2 input = GameManager.Instance.input.GetAxis2D(RewiredConsts.Action.TurnThirdPerson,
            RewiredConsts.Action.WalkThirdPerson);

        // Assume the player is not grounded
        _isGrounded = false;
        // SphereCast towards the ground, using the radius of the severed head and the approximate radius of the snowball
        if (Physics.SphereCast(playerController.root.transform.position, 0.15f + Mathf.Max(0, (snowball.localScale.y / 2) - 0.17f), Vector3.down, out RaycastHit hit, 0.1f))
        {
            // We know we're grounded
            _isGrounded = true;

            // If we're on the ground (true) and the jump buffer has not reached 0...
            if (_jumpBuffer > 0)
            {
                // Jump
                Jump();
                
                // Reset the jump buffer
                _jumpBuffer = 0f;
            }

            // Set the position and direction of the blood particles
            bloodTrailParticleSystem.transform.position = hit.point + Vector3.up * 0.025f;
            bloodTrailParticleSystem.transform.up = hit.normal;
            // Set the position and direction of the snow particles
            snowTrailParticleSystem.transform.position = hit.point + Vector3.up * 0.025f;
            snowTrailParticleSystem.transform.up = hit.normal;

            // If we're on snow...
            if (hit.collider.CompareTag("Snow"))
            {
                // Increase the snow percent
                _snowballPercent = Mathf.MoveTowards(_snowballPercent, 1, Time.deltaTime / 15f);
            }
            else
            {
                // Decrease the snow percent
                _snowballPercent = Mathf.MoveTowards(_snowballPercent, 0, Time.deltaTime / 10f);
            }
        }

        // If the snowball is too small to be considered active...
        if (snowballCurve.Evaluate(_snowballPercent) < 0.4f)
        {
            // Stop the snow particles
            snowTrailParticleSystem.Stop();
            snowTrailAirParticleSystem.Stop();
            
            // If the ground particles are playing but we're in the air...
            if (_isBloodTrailPlaying && !_isGrounded)
            {
                // Stop the ground particles, start the air particles, and set bool to false
                bloodTrailParticleSystem.Stop();
                bloodTrailAirParticleSystem.Play();
                _isBloodTrailPlaying = false;
            }
            
            // If the ground particles are NOT playing, and we are on the ground...
            else if (!_isBloodTrailPlaying && _isGrounded) 
            {
                // Start the ground particles, stop the air particles, and set bool to true
                bloodTrailParticleSystem.Play();
                bloodTrailAirParticleSystem.Stop();
                _isBloodTrailPlaying = true;
            }
        }
        // Otherwise, if the snowball is big enough to be considered active...
        else
        {
            // Stop the blood particles
            bloodTrailParticleSystem.Stop();
            bloodTrailAirParticleSystem.Stop();
            
            // If the ground particles are playing, but we're in the air...
            if (_isBloodTrailPlaying && !_isGrounded)
            {
                // Stop the ground particles, start the air particles, and set bool to false
                snowTrailParticleSystem.Stop();
                snowTrailAirParticleSystem.Play();
                _isBloodTrailPlaying = false;
            }
            
            // If the ground particles are NOT playing, and we are on the ground...
            else if (!_isBloodTrailPlaying && _isGrounded)
            {
                // Start the ground particles, stop the air particles, and set bool to true
                snowTrailParticleSystem.Play();
                snowTrailAirParticleSystem.Stop();
                _isBloodTrailPlaying = true;
            }
        }

        // Calculate the "final movement" (aka the desired direction given the camera angle)
        Vector3 finalMovement =
            MainCameraManager.Instance.movementTransform.TransformDirection(new Vector3(input.x, 0f, input.y));
        finalMovement.Normalize();
        finalMovement = Quaternion.Euler(0, 90, 0) * finalMovement;
        
        // Store the "final movement" for use in FixedUpdate
        _movement = finalMovement;

        // Set the snowball size, taking into account the evaluated curve value and percent coverage
        snowball.transform.localScale = Vector3.one * snowballCurve.Evaluate(_snowballPercent);
    }

    protected override void _UseEffectPrimary(ButtonState buttonState)
    {
        if (buttonState != ButtonState.Pressed) return;
        
        // Set the jump buffer to queue the jump for the next frame where the player is grounded
        _jumpBuffer = jumpBufferMax;
    }

    private void Jump()
    {
        // Set the y velocity to the jump velocity
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 5f, rigidbody.velocity.z);
        // Play the squish particles and sounds
        Squish();

        // If the snowball is big enough, play the big burst particles
        if (_snowballPercent > 0.2f)
            snowBurstBigParticleSystem.Play();
        
        // Remove snow from the snowball
        _snowballPercent /= 1.1f;
    }

    private void Squish()
    {
        // If the snowball is big enough
        if (snowballCurve.Evaluate(_snowballPercent) > 0.35f)
        {
            // Burst the snowball particles
            snowBurstParticleSystem.Play();
            // Play the snowball sound
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/PLAYER/FOOTSTEPS/Snow", gameObject);
            // Return to avoid playing the severed head particles and sounds
            return;
        }
        
        // Play the severed head squish sound
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/PLAYER/EFFECTS/SEVERED HEAD/HeadSquish", gameObject);
        // Play the blood squirt particles
        bloodParticleSystem.Play();
    }

    private void FixedUpdate()
    {
        float waterGravityMultiplier = 1f;
        float waterMovementMultiplier = 1f;
        if (playerController.isInWater)
        {
            waterGravityMultiplier = 0.25f;
            waterMovementMultiplier = 0.5f;
        }
        
        // Gravity
        rigidbody.AddForce(Physics.gravity * (2f * waterGravityMultiplier));

        // If you're on the ground, allow rolling
        if (_isGrounded)
        {
            float snowballModifier = Mathf.Max(1f, (_snowballPercent - 0.25f) * 10f);
            // Add torque based on movement input, multiplied by 3, and divided by a calculated value used to decrease
            // the torque of the snowball.
            rigidbody.AddTorque(
                new Vector3(_movement.x, 0f, _movement.z) * (3f * waterMovementMultiplier) / snowballModifier,
                ForceMode.VelocityChange);
        }
        else
        {
            // Old force method, might use for in-air movement
            //rigidbody.AddForce(new Vector3(_movement.x, _movement.y, _movement.z) / 10f, ForceMode.VelocityChange);
        }

        // Store the past velocity for use in collision detection.
        _pastVelocity = rigidbody.velocity;
    }

    public void CollisionHit(Collision other)
    {
        // Loop through all points in the collision
        foreach (var point in other.contacts)
        {
            // If the angle is 
            //if (Vector3.Angle(-point.normal, _pastVelocity) > 80) continue;

            // If the velocity is heading towards the normal at a high enough speed
            if (Vector3.Dot(_pastVelocity, -point.normal) > 3f)
            {
                Squish();
                
                // If the snowball is big enough, play the big burst
                if (_snowballPercent > 0.2f)
                    snowBurstBigParticleSystem.Play();
                
                // Remove snow from the snowball
                _snowballPercent /= 1.1f;

                // If the velocity is high enough, remove all snow
                if (Vector3.Dot(_pastVelocity, -point.normal) > 8f) _snowballPercent = 0f;
                break;
            }
        }
    }
}
*/

