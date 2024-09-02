using System;
using JetBrains.Annotations;
using QFSW.QC;
using UnityEngine;

namespace PixelDough.Bouncer
{
    public class PlayerController : MonoBehaviour
    {

        [SerializeField] private PlayerStuffManager playerStuffManager;
        
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private new Collider collider;
        private PhysicsMaterial _colliderMaterial;
        
        private Vector3 _inputMovement;

        private float _defaultBounciness = 1f;
        private float _defaultAngularDrag = 0f;
        private float _defaultDrag;
        
        [SerializeField] private float jumpBufferMax = 0.2f;
        private float _jumpBuffer = 0f;

        [SerializeField] private float coyoteTimeMax = 0.2f;
        private float _coyoteTime = 0f;

        private bool _isGrounded;
        private Vector3 _pastVelocity = Vector3.zero;

        private bool _isDamping;
        
        private Camera _camera;

        [SerializeField] private Transform cameraTiltRoot;
        
        [SerializeField] private ParticleSystem dampenParticle;
        [SerializeField] private Transform dampenBubble;

        [SerializeField] private Transform eyesRoot;
        private float _eyesTargetAngle = 0f;
        private float _eyesCurrentAngle = 0f;

        [Header("Audio Event Emitters")] 
        [SerializeField] private FMODUnity.StudioEventEmitter bounceEventEmitter;
        [SerializeField] private FMODUnity.StudioEventEmitter rollEventEmitter;
        [SerializeField] private FMODUnity.StudioEventEmitter windFastEventEmitter;

        public CheckpointController currentCheckpoint = null;
        private Vector3 _respawnPoint = Vector3.zero;
        private Vector3 _respawnForward = Vector3.forward;

        private int _noclip = 0;


        /*
        [SerializeField] private ParticleSystem squishParticle;
        [SerializeField] private ParticleSystem groundParticle;
        private bool _isGroundParticlePlaying = false;
        [SerializeField] private ParticleSystem airParticle;*/

        private void Start()
        {
            _camera = Camera.main;
            rigidbody.maxAngularVelocity = 100f;
            
            _colliderMaterial = collider.material;
            _defaultBounciness = _colliderMaterial.bounciness;
            _defaultAngularDrag = rigidbody.angularDamping;
            _defaultDrag = rigidbody.linearDamping;

            _respawnPoint = transform.position;
            _respawnForward = Vector3.forward;

            /*groundParticle.Stop();
            airParticle.Play();*/
        }

        private void Update()
        {
            HandleMovementInput();
            HandleNoclipMovement();
            //HandleDampen();

            windFastEventEmitter.EventInstance.setParameterByName("AirSpeed", rigidbody.linearVelocity.magnitude / 30f);
            
            if (_isGrounded)
            {
                rollEventEmitter.EventInstance.setParameterByName("BallRollSpeed",
                    Vector3.ProjectOnPlane(rigidbody.linearVelocity, Physics.gravity).magnitude / 30f);
            }
            else
            {
                rollEventEmitter.EventInstance.setParameterByName("BallRollSpeed", 0);
            }

            if (!rigidbody.isKinematic) rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            
            _isDamping = true;
            if (GameManager.Instance.Input.GetButton(RewiredConsts.Action.Jump))
            {
                _isDamping = false;
                _colliderMaterial.bounciness = _defaultBounciness;
            }
            else
            {
                _colliderMaterial.bounciness = 0.5f;
            }

            collider.material = _colliderMaterial;

            if (_inputMovement.sqrMagnitude < 0.1f)
                rigidbody.angularDamping = 2f;
            else
                rigidbody.angularDamping = _defaultAngularDrag;

            // Move the jump buffer towards 0
            _jumpBuffer = Mathf.MoveTowards(_jumpBuffer, 0f, Time.deltaTime);

            _coyoteTime = Mathf.MoveTowards(_coyoteTime, 0f, Time.deltaTime);
            if (_isGrounded) _coyoteTime = coyoteTimeMax;

            if (_jumpBuffer > 0f)
            {
                if (_isGrounded || _coyoteTime > 0f)
                {
                    Jump();
                }
            }

            /*if (_isGrounded && !_isGroundParticlePlaying)
            {
                groundParticle.Play();
                airParticle.Stop();
                _isGroundParticlePlaying = true;
            }
            else if (!_isGrounded && _isGroundParticlePlaying)
            {
                groundParticle.Stop();
                airParticle.Play();
                _isGroundParticlePlaying = false;
            }*/
            
            playerStuffManager.sandRollParticleSystem.transform.position = transform.position - Vector3.up / 4;
            playerStuffManager.sandBurstParticleSystem.transform.position = transform.position - Vector3.up / 4;
            
            // Important: Set this AFTER checking for a buffered jump, as isGrounded might have been set in OnCollisionEnter.
            if (Physics.SphereCast(transform.position, 0.24f, Vector3.down, out RaycastHit hit, 
                    0.02f, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore))
            {
                if (Physics.GetIgnoreCollision(collider, hit.collider)) return;

                _isGrounded = true;
                //groundParticle.transform.position = hit.point;
            } else { _isGrounded = false; }

            ParticleSystem.EmissionModule emissionModule = playerStuffManager.sandRollParticleSystem.emission;
            if (!_isGrounded)
            {
                emissionModule.rateOverDistance = 0;
            }
            else
            {
                emissionModule.rateOverDistance = rigidbody.linearVelocity.magnitude / 2;
                if (rigidbody.linearVelocity.normalized.sqrMagnitude > 0.001f)
                {
                    playerStuffManager.sandRollParticleSystem.transform.forward = -rigidbody.linearVelocity.normalized;
                }
            }
        }

        private void LateUpdate()
        {
            _eyesCurrentAngle = Mathf.LerpAngle(_eyesCurrentAngle, _eyesTargetAngle, 10f * Time.deltaTime);
            Vector3 targetAngleVector = Vector3.up * _eyesCurrentAngle;

            float eyeMovementDot = Vector3.Dot(Quaternion.Euler(targetAngleVector) * Vector3.forward,
                Vector3.ProjectOnPlane(Vector3.ClampMagnitude(rigidbody.linearVelocity, 1f), Vector3.up));
            Vector3 targetTiltVector = Vector3.right * (15 * eyeMovementDot);
            eyesRoot.rotation = Quaternion.Euler(targetAngleVector) *
                                Quaternion.Euler(targetTiltVector);
            if (_inputMovement.sqrMagnitude < 0.01) return;
            
            _eyesTargetAngle = Vector3.SignedAngle(Vector3.back, _inputMovement, Vector3.up);
        }

        private void FixedUpdate()
        {
            if (!GameManager.DoPlayerPhysics || _noclip == 1) return;
            
            rigidbody.AddForce(Physics.gravity, ForceMode.Acceleration);
            
            if (_isGrounded)
            {
                float reverseMultiplier = 1f;
                Vector3 flattenedVelocity = new Vector3(rigidbody.linearVelocity.x, 0f, rigidbody.linearVelocity.z);
                if (Vector3.Angle(flattenedVelocity.normalized, _inputMovement) > 90) reverseMultiplier = 2f;
                    
                Vector3 inputConvertedToTorque = Quaternion.Euler(0, 90, 0) * _inputMovement;
                rigidbody.AddTorque(
                    new Vector3(inputConvertedToTorque.x, 0f, inputConvertedToTorque.z) *
                    (150f * reverseMultiplier * Time.fixedDeltaTime),
                    ForceMode.VelocityChange);
            }
            else
            {
                float reverseMultiplier = 1f;
                Vector3 flattenedVelocity = new Vector3(rigidbody.linearVelocity.x, 0f, rigidbody.linearVelocity.z);
                if (Vector3.Angle(flattenedVelocity.normalized, _inputMovement) > 90) reverseMultiplier = 2f;
                rigidbody.AddForce(_inputMovement * (10 * reverseMultiplier * Time.fixedDeltaTime), ForceMode.VelocityChange);
            }
            
            //rigidbody.AddForce(_inputMovement * (12 * Time.fixedDeltaTime), ForceMode.VelocityChange);

            //rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 30);

            float projectedMagnitude = Vector3.ProjectOnPlane(rigidbody.linearVelocity, Vector3.up).magnitude;
            rigidbody.linearDamping = Mathf.Lerp(rigidbody.linearDamping, (1f / (Mathf.Max(projectedMagnitude, 1) * 2)), Time.fixedDeltaTime);
            
            _pastVelocity = rigidbody.linearVelocity;
        }

        private void OnCollisionEnter(Collision other)
        {
            foreach (var pointGround in other.contacts)
            {
                if (Vector3.Angle(pointGround.normal, -Physics.gravity) < 15f)
                {
                    if (Vector3.Project(rigidbody.linearVelocity, Physics.gravity).sqrMagnitude > 2)
                        playerStuffManager.sandBurstParticleSystem.Play();
                    _isGrounded = true;
                }
            }

            /*foreach (var point in other.contacts)
            {*/
                // If the angle is 
                //if (Vector3.Angle(-point.normal, _pastVelocity) > 80) continue;

                // If the velocity is heading towards the normal at a high enough speed
                ContactPoint point = other.contacts[0];
                float velTowardsNormal = Vector3.Dot(_pastVelocity, -point.normal);
                //bounceEventEmitter.Play();
                //bounceEventEmitter.EventInstance.setParameterByName("Strength", Mathf.InverseLerp(0f, 15f, Mathf.Abs(velTowardsNormal)));

                if (!_isDamping)
                {
                    if (velTowardsNormal > 3f)
                    {
                        //squishParticle.Play();
                        //Squish();
                        
                        // Hit wall
                        if (Mathf.Abs(point.normal.y) < 0.1f)
                        {
                            rigidbody.AddForce(point.normal * Vector3.ProjectOnPlane(_pastVelocity, Physics.gravity).magnitude / 10f,
                                ForceMode.VelocityChange);
                            rigidbody.AddForce(Vector3.up * Vector3.ProjectOnPlane(_pastVelocity, Physics.gravity).magnitude / 1.25f,
                                ForceMode.VelocityChange);

                        }
                        // Hit something that's not a wall
                        else if (Vector3.Angle(-point.normal, Physics.gravity) > 35f && velTowardsNormal > 9f)
                        {
                            rigidbody.AddForce(point.normal * velTowardsNormal / 2f, ForceMode.VelocityChange);
                        }
                    }
                }
            /*}*/
        }
        
        private void HandleMovementInput()
        {
            if (!GameManager.DoPlayerMovement || GameManager.Instance.quantumConsole.IsActive || _noclip == 1) return;
            
            Vector2 rawInputMovement = GameManager.Instance.Input.GetAxis2D(RewiredConsts.Action.MoveHorizontal,
                RewiredConsts.Action.MoveVertical);
            rawInputMovement.Normalize();
            Vector3 rawInputMovementVector3 = new Vector3(rawInputMovement.x, 0f, rawInputMovement.y);
            Vector3 cameraRelativeInput = CameraRelativeFlatten(rawInputMovementVector3);
            cameraRelativeInput = cameraRelativeInput.normalized * cameraRelativeInput.magnitude;

            _inputMovement = cameraRelativeInput;

            cameraTiltRoot.transform.rotation = Quaternion.Lerp(cameraTiltRoot.transform.rotation,
                Quaternion.Euler(-_inputMovement.z * 5f, 0f, _inputMovement.x * 5f), 3f * Time.unscaledDeltaTime);
            
            // If the player has pressed the jump button, reset the jump buffer to the max
            if (GameManager.Instance.Input.GetButtonDown(RewiredConsts.Action.Jump))
            {
                _jumpBuffer = jumpBufferMax;
            }
        }

        private void HandleNoclipMovement()
        {
            if (!GameManager.DoPlayerMovement || GameManager.Instance.quantumConsole.IsActive || _noclip == 0) return;
            
            Vector2 rawInputMovement = GameManager.Instance.Input.GetAxis2D(RewiredConsts.Action.MoveHorizontal,
                RewiredConsts.Action.MoveVertical);
            rawInputMovement.Normalize();
            Vector3 rawInputMovementVector3 = new Vector3(rawInputMovement.x, 0f, rawInputMovement.y);
            Vector3 cameraRelativeInput = CameraRelativeFlatten(rawInputMovementVector3);
            cameraRelativeInput = cameraRelativeInput.normalized * cameraRelativeInput.magnitude;

            cameraRelativeInput.y += (Input.GetKey(KeyCode.Space) ? 1 : 0) + (Input.GetKey(KeyCode.C) ? -1 : 0);
            
            transform.Translate(cameraRelativeInput * (20f * Time.deltaTime), Space.World);
        }
        
        private void HandleDampen()
        {
            _isDamping = false;
            if (GameManager.Instance.Input.GetButton(RewiredConsts.Action.Dampen))
            {
                _isDamping = true;
                _colliderMaterial.bounciness = 0.5f;
                dampenBubble.gameObject.SetActive(true);
                if (!dampenParticle.isEmitting) dampenParticle.Play();
            }
            /*else if (GameManager.Instance.Input.GetButton(RewiredConsts.Action.Jump))
            {
                _colliderMaterial.bounciness = 0.9f;
            }*/
            else
            {
                _colliderMaterial.bounciness = _defaultBounciness;
                dampenBubble.gameObject.SetActive(false);
                if (dampenParticle.isPlaying) dampenParticle.Stop();
            }

            collider.material = _colliderMaterial;
        }

        private void Jump()
        {
            if (Mathf.Abs(rigidbody.linearVelocity.y) < 10f)
                rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, 10f, rigidbody.linearVelocity.z);
            _jumpBuffer = 0f;
            _coyoteTime = 0f;
            
            playerStuffManager.sandBurstParticleSystem.transform.position = transform.position - Vector3.up / 4;
            playerStuffManager.sandBurstParticleSystem.Play();
        }

        Vector3 CameraRelativeFlatten(Vector3 input)
        {
            return Quaternion.Euler( 0, _camera.transform.rotation.eulerAngles.y, 0) * input;
        }

        public void SetRespawnPoint(Vector3 position, Vector3 direction, [CanBeNull] CheckpointController checkpointController = null)
        {
            _respawnPoint = position;
            _respawnForward = direction;

            currentCheckpoint = checkpointController;
            
            LevelManager.Instance.LevelProgress.LockInCollectables();
        }

        public void Kill()
        {
            
            // Play a kill animation
            //rigidbody.velocity = Vector3.zero;
            GameManager.DoPlayerMovement = false;
            GameManager.DoPlayerPhysics = false;
            Respawn();
        }

        private void Respawn()
        {
            GameManager.DoPlayerMovement = false;
            GameManager.DoPlayerPhysics = false;
            GameManager.Instance.screenFadeController.FadeToBlack(0.5f).setOnComplete(() =>
            {
                transform.position = _respawnPoint;
                transform.forward = _respawnForward;
                playerStuffManager.SetCameraForward(_respawnForward);
                rigidbody.linearVelocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                
                LevelManager.Instance.LevelProgress.LoseCollectables();
                
                GameManager.Instance.screenFadeController.FadeFromBlack(0.5f).setOnComplete(() =>
                {
                    GameManager.DoPlayerMovement = true;
                    GameManager.DoPlayerPhysics = true;
                    
                });
            });
        }

        [Command("goto-checkpoint", MonoTargetType.Single)]
        private void GoToCheckpoint(int index)
        {
            foreach (var checkpointController in FindObjectsByType<CheckpointController>(FindObjectsSortMode.None))
            {
                if (checkpointController.index == index)
                {
                    SetRespawnPoint(checkpointController.transform.position, checkpointController.transform.forward);
                    Respawn();
                    return;
                }
            }
            
            Debug.Log($"No such checkpoint exists at index {index}.");
        }

        [Command("noclip", MonoTargetType.Single)]
        private void NoClip()
        {
            NoClip(_noclip == 1 ? 0 : 1);
        }
        
        [Command("noclip", MonoTargetType.Single)]
        private void NoClip(int value)
        {
            _noclip = value;
            
            rigidbody.isKinematic = _noclip == 1;
        }
    }
}

