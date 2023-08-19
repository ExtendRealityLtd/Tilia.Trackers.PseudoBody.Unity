namespace Tilia.Trackers.PseudoBody
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Tilia.Interactions.Interactables.Interactables;
    using Tilia.Interactions.Interactables.Interactors;
    using UnityEngine;
    using UnityEngine.Events;
    using Zinnia.Cast;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Operation.Mutation;
    using Zinnia.Data.Type;
    using Zinnia.Extension;
    using Zinnia.Process;
    using Zinnia.Tracking.Collision;
    using Zinnia.Tracking.Follow;

    /// <summary>
    /// Sets up the PseudoBody prefab based on the provided user settings and implements the logic to represent a body.
    /// </summary>
    public class PseudoBodyProcessor : MonoBehaviour, IProcessable
    {
        /// <summary>
        /// The object that defines the main source of truth for movement.
        /// </summary>
        public enum MovementInterest
        {
            /// <summary>
            /// The source of truth for movement comes from <see cref="PseudoBodyProcessor.Character"/>.
            /// </summary>
            CharacterController,
            /// <summary>
            /// The source of truth for movement comes from <see cref="PseudoBodyProcessor.Character"/> until <see cref="rigidbody"/> is in the air, then <see cref="rigidbody"/> is the new source of truth.
            /// </summary>
            CharacterControllerUntilAirborne,
            /// <summary>
            /// The source of truth for movement comes from <see cref="rigidbody"/>.
            /// </summary>
            Rigidbody,
            /// <summary>
            /// The source of truth for movement comes from <see cref="rigidbody"/> until <see cref="PseudoBodyProcessor.Character"/> hits the ground, then <see cref="PseudoBodyProcessor.Character"/> is the new source of truth.
            /// </summary>
            RigidbodyUntilGrounded
        }

        /// <summary>
        /// The divergence state of the pseudo body.
        /// </summary>
        public enum DivergenceState
        {
            /// <summary>
            /// The pseudo body is not diverged.
            /// </summary>
            NotDiverged,
            /// <summary>
            /// the pseudo body has become diverged.
            /// </summary>
            BecameDiverged,
            /// <summary>
            /// the pseudo body is no longer diverged.
            /// </summary>
            BecameConverged,
            /// <summary>
            /// the pseudo body is still diverged.
            /// </summary>
            StillDiverged
        }

        #region Facade Settings
        [Header("Facade Settings")]
        [Tooltip("The public interface facade.")]
        [SerializeField]
        [Restricted]
        private PseudoBodyFacade facade;
        /// <summary>
        /// The public interface facade.
        /// </summary>
        public PseudoBodyFacade Facade
        {
            get
            {
                return facade;
            }
            set
            {
                facade = value;
            }
        }
        #endregion

        #region Movement Settings
        [Header("Movement Settings")]
        [Tooltip("Whether the processor should update the Facade.Source position.")]
        [SerializeField]
        private bool updateSourcePosition = true;
        /// <summary>
        /// Whether the processor should update the <see cref="Facade.Source"/> position.
        /// </summary>
        public bool UpdateSourcePosition
        {
            get
            {
                return updateSourcePosition;
            }
            set
            {
                updateSourcePosition = value;
            }
        }
        [Tooltip("The duration to smooth damp the alias Source and Offset movement by.")]
        [SerializeField]
        private float aliasMovementDuration = 0f;
        /// <summary>
        /// The duration to smooth damp the alias Source and Offset movement by.
        /// </summary>
        public float AliasMovementDuration
        {
            get
            {
                return aliasMovementDuration;
            }
            set
            {
                aliasMovementDuration = value;
            }
        }
        [Tooltip("The duration to smooth damp the collision resolution movement by.")]
        [SerializeField]
        private float collisionMovementDuration = 0f;
        /// <summary>
        /// The duration to smooth damp the collision resolution movement by.
        /// </summary>
        public float CollisionMovementDuration
        {
            get
            {
                return collisionMovementDuration;
            }
            set
            {
                collisionMovementDuration = value;
            }
        }
        #endregion

        #region Force Settings
        [Header("Force Settings")]
        [Tooltip("The direction to apply the force to on the `AddForce` method.")]
        [SerializeField]
        private Vector3 forceDirection = Vector3.up;
        /// <summary>
        /// The direction to apply the force to on the `AddForce` method.
        /// </summary>
        public Vector3 ForceDirection
        {
            get
            {
                return forceDirection;
            }
            set
            {
                forceDirection = value;
            }
        }
        [Tooltip("The ForceMode to apply the force to on the `AddForce` method.")]
        [SerializeField]
        private ForceMode forceType;
        /// <summary>
        /// The <see cref="ForceMode"/> to apply the force to on the `AddForce` method.
        /// </summary>
        public ForceMode ForceType
        {
            get
            {
                return forceType;
            }
            set
            {
                forceType = value;
            }
        }
        #endregion

        #region Reference Settings
        [Header("Reference Settings")]
        [Tooltip("The CharacterController that acts as the main representation of the body.")]
        [SerializeField]
        [Restricted]
        private CharacterController character;
        /// <summary>
        /// The <see cref="CharacterController"/> that acts as the main representation of the body.
        /// </summary>
        public CharacterController Character
        {
            get
            {
                return character;
            }
            set
            {
                character = value;
            }
        }
        [Tooltip("The Rigidbody that acts as the physical representation of the body.")]
        [SerializeField]
        [Restricted]
        private Rigidbody physicsBody;
        /// <summary>
        /// The <see cref="Rigidbody"/> that acts as the physical representation of the body.
        /// </summary>
        public Rigidbody PhysicsBody
        {
            get
            {
                return physicsBody;
            }
            set
            {
                physicsBody = value;
            }
        }
        [Tooltip("The CapsuleCollider that acts as the physical collider representation of the body.")]
        [SerializeField]
        [Restricted]
        private CapsuleCollider rigidbodyCollider;
        /// <summary>
        /// The <see cref="CapsuleCollider"/> that acts as the physical collider representation of the body.
        /// </summary>
        public CapsuleCollider RigidbodyCollider
        {
            get
            {
                return rigidbodyCollider;
            }
            set
            {
                rigidbodyCollider = value;
            }
        }
        [Tooltip("A CollisionIgnorer to manage ignoring collisions with the PseudoBody colliders.")]
        [SerializeField]
        [Restricted]
        private CollisionIgnorer collisionsToIgnore;
        /// <summary>
        /// A <see cref="CollisionIgnorer"/> to manage ignoring collisions with the PseudoBody colliders.
        /// </summary>
        public CollisionIgnorer CollisionsToIgnore
        {
            get
            {
                return collisionsToIgnore;
            }
            set
            {
                collisionsToIgnore = value;
            }
        }
        #endregion

        /// <summary>
        /// The private backing field for <see cref="Interest"/>.
        /// </summary>
        private MovementInterest interest = MovementInterest.CharacterControllerUntilAirborne;
        /// <summary>
        /// The object that defines the main source of truth for movement.
        /// </summary>
        public MovementInterest Interest
        {
            get
            {
                return interest;
            }
            set
            {
                interest = value;
                if (this.IsMemberChangeAllowed())
                {
                    OnAfterInterestChange();
                }
            }
        }
        /// <summary>
        /// The current divergence state of the pseudo body.
        /// </summary>
        public virtual DivergenceState CurrentDivergenceState => GetDivergenceState();
        /// <summary>
        /// Whether <see cref="Character"/> touches ground.
        /// </summary>
        public virtual bool IsCharacterControllerGrounded => wasCharacterControllerGrounded == true;
        /// <summary>
        /// Whether the PseudoBody is grounded using a detailed ground check.
        /// </summary>
        public virtual bool IsGrounded => CheckIfCharacterControllerIsGrounded();
        /// <summary>
        /// Whether <see cref="Facade.Source"/> has diverged from the <see cref="Character"/>.
        /// </summary>
        public virtual bool IsDiverged { get; protected set; }

        /// <summary>
        /// Movement to apply to <see cref="Character"/> to resolve collisions.
        /// </summary>
        protected static readonly Vector3 collisionResolutionMovement = Vector3.right * 0.001f;
        /// <summary>
        /// The colliders to ignore body collisions with.
        /// </summary>
        protected readonly HashSet<Collider> ignoredColliders = new HashSet<Collider>();
        /// <summary>
        /// The colliders to restore after an ungrab.
        /// </summary>
        protected readonly HashSet<Collider> restoreColliders = new HashSet<Collider>();
        /// <summary>
        /// The center of the <see cref="Character"/>.
        /// </summary>
        protected Vector3 CharacterCenter => Vector3.up * (Character.radius - Character.skinWidth - 0.001f);
        /// <summary>
        /// The previous position of <see cref="PhysicsBody"/>.
        /// </summary>
        protected Vector3 previousRigidbodyPosition;
        /// <summary>
        /// The previous position of the <see cref="Facade.Offset"/>.
        /// </summary>
        protected Vector3 previousOffsetPosition;
        /// <summary>
        /// The previous position for the <see cref="Character"/>.
        /// </summary>
        protected Vector3 previousCharacterControllerPosition;
        /// <summary>
        /// Whether <see cref="Character"/> was grounded previously.
        /// </summary>
        protected bool? wasCharacterControllerGrounded;
        /// <summary>
        /// The frame count of the last time <see cref="Interest"/> was set to <see cref="MovementInterest.Rigidbody"/> or <see cref="MovementInterest.RigidbodyUntilGrounded"/>.
        /// </summary>
        protected int rigidbodySetFrameCount;
        /// <summary>
        /// Stores the routine for ignoring Interactor collisions.
        /// </summary>
        protected Coroutine ignoreInteractorCollisions;
        /// <summary>
        /// An optional follower of <see cref="PseudoBodyFacade.Offset"/>.
        /// </summary>
        protected ObjectFollower offsetObjectFollower;
        /// <summary>
        /// An optional follower of <see cref="PseudoBodyFacade.Source"/>.
        /// </summary>
        protected ObjectFollower sourceObjectFollower;
        /// <summary>
        /// Whether <see cref="Facade.Source"/> was previously diverged from the <see cref="Character"/>.
        /// </summary>
        protected bool wasDiverged;
        /// <summary>
        /// The routine for checking to see if the <see cref="Facade.Source"/> is still diverged with the <see cref="Character"/> at the end of the frame.
        /// </summary>
        protected Coroutine checkDivergedAtEndOfFrameRoutine;
        /// <summary>
        /// The routine for resetting the <see cref="Interest"/> to <see cref="MovementInterest.RigidbodyUntilGrounded"/> after a force is added.
        /// </summary>
        protected Coroutine resetInterestAfterAddForceRoutine;
        /// <summary>
        /// A <see cref="YieldInstruction"/> that waits for the end of the fixed update process.
        /// </summary>
        protected YieldInstruction waitForEndOfFixedUpdate = new WaitForFixedUpdate();
        /// <summary>
        /// Whether to snap the dependents to the <see cref="Facade.Source"/> without any divergent checking.
        /// </summary>
        protected bool doSnapToSource;
        /// <summary>
        /// A reference to output any smooth damp velocity to.
        /// </summary>
        protected Vector3 smoothDampVelocity;
        /// <summary>
        /// A collection of actions to perform on each <see cref="TransformPositionMutator"/> to prevent mutations from occurring.
        /// </summary>
        protected Dictionary<TransformPositionMutator, UnityAction> preventMutateActions = new Dictionary<TransformPositionMutator, UnityAction>();
        /// <summary>
        /// A collection of actions to perform on each <see cref="TransformPositionMutator"/> to allow mutations to occur.
        /// </summary>
        protected Dictionary<TransformPositionMutator, UnityAction<Vector3>> allowMutateActions = new Dictionary<TransformPositionMutator, UnityAction<Vector3>>();

        /// <summary>
        /// Snaps the <see cref="Character"/> to the <see cref="Facade.Source"/> position.
        /// </summary>
        public virtual void SnapToSource()
        {
            doSnapToSource = true;
        }

        /// <summary>
        /// Positions, sizes and controls all variables necessary to make a body representation follow the given <see cref="PseudoBodyFacade.Source"/>.
        /// </summary>
        public virtual void Process()
        {
            if (!this.IsValidState())
            {
                return;
            }

            if (doSnapToSource)
            {
                SnapDependentsToSource();
                doSnapToSource = false;
                return;
            }

            UpdateAliasForNonCharacterControllerInterest();

            Vector3 characterControllerSourceMovement = UpdateAliasForRigidbodyControllerInterest(out Vector3 rigidbodyPhysicsMovement);
            bool isGrounded = CheckIfCharacterControllerIsGrounded();

            UpdateInterestType(isGrounded, rigidbodyPhysicsMovement);
            UpdateAliasForVerticalMovement(isGrounded, characterControllerSourceMovement);

            MatchRigidbodyAndColliderWithCharacterController();
            CheckDivergence();
            RememberCurrentPositions();
            EmitIsGroundedChangedEvent(isGrounded);
        }

        /// <summary>
        /// Solves body collisions by not moving the body in case it can't go to its current position.
        /// </summary>
        /// <remarks>
        /// If body collisions should be prevented this method needs to be called right before or right after applying any form of movement to the body.
        /// </remarks>
        public virtual void SolveBodyCollisions()
        {
            if (!this.IsValidState() || Facade.Source == null)
            {
                return;
            }

            ProcessObjectFollowers();
            Process();
            UpdateAliasForCollision();
            Process();
        }

        /// <summary>
        /// Checks to see if the given position will cause a divergence between the <see cref="Facade.Source"/> and the <see cref="Facade.Offset"/> to the <see cref="Character"/>.
        /// </summary>
        /// <param name="targetPosition">The new position to check for.</param>
        /// <returns>Whether a divergence will occur.</returns>
        public virtual bool CheckWillDiverge(Vector3 targetPosition)
        {
            Vector3 difference = targetPosition - Facade.Offset.transform.localPosition;
            Vector3 position = GetCharacterPosition(Facade.Source.transform.position + difference, out float _);
            Vector3 movement = position - Character.transform.position;
            Character.Move(movement);

            if (WillDiverge(Facade.Source.transform.position + difference, Facade.SourceDivergenceThreshold))
            {
                Facade.WillDiverge?.Invoke();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if the given position will cause a divergence between the <see cref="Facade.Source"/> and the <see cref="Facade.Offset"/> to the <see cref="Character"/>.
        /// </summary>
        /// <param name="targetPosition">The new position to check for.</param>
        public virtual void DoCheckWillDiverge(Vector3 targetPosition)
        {
            CheckWillDiverge(targetPosition);
        }

        /// <summary>
        /// Resolves any divergence between the <see cref="Character"/> position and the actual position of the <see cref="Facade.Source"/> and <see cref="Facade.Offset"/>.
        /// </summary>
        public virtual void ResolveDivergence()
        {
            UpdateAliasForDivergence();
            ProcessObjectFollowers();
        }

        /// <summary>
        /// Resolves any divergence between the <see cref="Character"/> position and the actual position of the <see cref="Facade.Source"/> and <see cref="Facade.Offset"/>.
        /// </summary>
        /// <param name="divergedPosition">The diverged position.</param>
        public virtual void ResolveDivergence(Vector3 divergedPosition)
        {
            ResolveDivergence();
        }

        /// <summary>
        /// Adds a force to the <see cref="PhysicsBody"/> in the <see cref="ForceDirection"/> using the <see cref="ForceType"/>.
        /// </summary>
        /// <param name="power">The amount of force to apply in the <see cref="forceDirection"/>.</param>
        public virtual void AddForce(float power)
        {
            if (PhysicsBody == null)
            {
                return;
            }

            Interest = MovementInterest.Rigidbody;
            PhysicsBody.AddForce(power * ForceDirection, ForceType);
            StopResetInterestAfterForceRoutine();
            resetInterestAfterAddForceRoutine = StartCoroutine(ResetInterestAfterForce());
        }

        /// <summary>
        /// Configures the source object follower based on the facade settings.
        /// </summary>
        public virtual void ConfigureSourceObjectFollower()
        {
            if (Facade.Source != null)
            {
                sourceObjectFollower = Facade.Source.GetComponent<ObjectFollower>();
            }
        }

        /// <summary>
        /// Configures the offset object follower based on the facade settings.
        /// </summary>
        public virtual void ConfigureOffsetObjectFollower()
        {
            if (Facade.Offset != null)
            {
                offsetObjectFollower = Facade.Offset.GetComponent<ObjectFollower>();
            }
        }

        /// <summary>
        /// Configures the character controller and capsule collider radius based on the facade settings.
        /// </summary>
        public virtual void ConfigureCharacterRadius()
        {
            Character.radius = Facade.CharacterRadius;
            RigidbodyCollider.radius = Facade.CharacterRadius;
        }

        /// <summary>
        /// Ignores all collisions between any found Interactor and this PsuedoBody.
        /// </summary>
        /// <param name="ignoredObject">The object to ignore.</param>
        public virtual void IgnoreInteractorsCollisions(GameObject ignoredObject)
        {
            InteractorFacade interactor = ignoredObject.GetComponent<InteractorFacade>();
            if (interactor != null)
            {
                interactor.Grabbed.AddListener(IgnoreInteractorGrabbedCollision);
                interactor.Ungrabbed.AddListener(ResumeInteractorUngrabbedCollision);
            }
        }

        /// <summary>
        /// Resumes all collisions between any found Interactor and this PsuedoBody.
        /// </summary>
        /// <param name="ignoredObject">The object being ignored.</param>
        public virtual void ResumeInteractorsCollisions(GameObject ignoredObject)
        {
            InteractorFacade interactor = ignoredObject.GetComponent<InteractorFacade>();
            if (interactor != null)
            {
                interactor.Grabbed.RemoveListener(IgnoreInteractorGrabbedCollision);
                interactor.Ungrabbed.RemoveListener(ResumeInteractorUngrabbedCollision);
            }
        }

        /// <summary>
        /// Adds a found <see cref="TransformPositionMutator"/> found in the given <see cref="GameObject"/>.
        /// </summary>
        /// <param name="mutatorContainer">The container to look for the Position Mutator in.</param>
        public virtual void AddPositionMutator(GameObject mutatorContainer)
        {
            if (!TryGetMutator(mutatorContainer, out TransformPositionMutator mutator))
            {
                return;
            }

            preventMutateActions[mutator] = () => mutator.AllowMutate = false;
            allowMutateActions[mutator] = (_) => mutator.AllowMutate = true;

            mutator.PreMutated.AddListener(DoCheckWillDiverge);
            mutator.MutationSkipped.AddListener(ResolveDivergence);
            mutator.MutationSkipped.AddListener(allowMutateActions[mutator]);
            Facade.WillDiverge.AddListener(preventMutateActions[mutator]);
        }

        /// <summary>
        /// Removes a found <see cref="TransformPositionMutator"/> found in the given <see cref="GameObject"/>.
        /// </summary>
        /// <param name="mutatorContainer">The container to look for the Position Mutator in.</param>
        public virtual void RemovePositionMutator(GameObject mutatorContainer)
        {
            if (!TryGetMutator(mutatorContainer, out TransformPositionMutator mutator))
            {
                return;
            }

            mutator.PreMutated.RemoveListener(DoCheckWillDiverge);
            mutator.MutationSkipped.RemoveListener(ResolveDivergence);
            mutator.MutationSkipped.RemoveListener(allowMutateActions[mutator]);
            Facade.WillDiverge.RemoveListener(preventMutateActions[mutator]);

            preventMutateActions.Remove(mutator);
            allowMutateActions.Remove(mutator);
        }

        /// <summary>
        /// Ignores all of the colliders on the Interactor collection.
        /// </summary>
        [Obsolete("Add `InteractorFacade.gameObject` to `PseudoBodyProcessor.CollisionsToIgnore.Targets` instead.")]
        public virtual void IgnoreInteractorsCollisions(InteractorFacade interactor)
        {
            CollisionsToIgnore.RunWhenActiveAndEnabled(() => CollisionsToIgnore.Targets.AddUnique(interactor.gameObject));
        }

        /// <summary>
        /// Resumes all of the colliders on the Interactor collection.
        /// </summary>
        [Obsolete("Remove `InteractorFacade.gameObject` to `PseudoBodyProcessor.CollisionsToIgnore.Targets` instead.")]
        public virtual void ResumeInteractorsCollisions(InteractorFacade interactor)
        {
            CollisionsToIgnore.RunWhenActiveAndEnabled(() => CollisionsToIgnore.Targets.Remove(interactor.gameObject));
        }

        protected virtual void Awake()
        {
            Physics.IgnoreCollision(Character, RigidbodyCollider, true);
        }

        protected virtual void OnEnable()
        {
            ConfigureSourceObjectFollower();
            ConfigureOffsetObjectFollower();
            ConfigureCharacterRadius();
            Interest = MovementInterest.CharacterControllerUntilAirborne;
            SnapDependentsToSource();
        }

        protected virtual void OnDisable()
        {
            StopCheckDivergenceAtEndOfFrameRoutine();
            StopResetInterestAfterForceRoutine();
            sourceObjectFollower = null;
            offsetObjectFollower = null;
        }

        /// <summary>
        /// Attempts to get the <see cref="TransformPositionMutator"/> component nested within the given <see cref="GameObject"/>.
        /// </summary>
        /// <param name="mutatorContainer">The container to look for the component in.</param>
        /// <param name="mutator">The found mutator.</param>
        /// <returns>Whether a mutator has been found.</returns>
        protected virtual bool TryGetMutator(GameObject mutatorContainer, out TransformPositionMutator mutator)
        {
            mutator = null;
            if (mutatorContainer == null)
            {
                return false;
            }

            mutator = mutatorContainer.TryGetComponent<TransformPositionMutator>(true);
            return mutator != null;
        }

        /// <summary>
        /// Updates the alias targets for when the <see cref="MovementInterest"/> is not of type CharacterController.
        /// </summary>
        protected virtual void UpdateAliasForNonCharacterControllerInterest()
        {
            if (Interest != MovementInterest.CharacterController && Facade.Offset != null)
            {
                Vector3 offsetPosition = Facade.Offset.transform.position;
                offsetPosition.y = PhysicsBody.position.y - Character.skinWidth;

                UpdateAliasPosition(offsetPosition, offsetPosition - previousOffsetPosition, false, true, false, AliasMovementDuration);
                previousOffsetPosition = offsetPosition;
            }
        }

        /// <summary>
        /// Updates the alias targets for when the <see cref="MovementInterest"/> is of a Rigidbody type.
        /// </summary>
        /// <param name="rigidbodyPhysicsMovement">The calculated <see cref="Rigidbody"/> movement position.</param>
        /// <returns>The current <see cref="Character"/> movement position.</returns>
        protected virtual Vector3 UpdateAliasForRigidbodyControllerInterest(out Vector3 rigidbodyPhysicsMovement)
        {
            // Handle walking down stairs/slopes and physics affecting the RigidBody in general.
            rigidbodyPhysicsMovement = PhysicsBody.position - previousRigidbodyPosition;
            if (Interest == MovementInterest.Rigidbody || Interest == MovementInterest.RigidbodyUntilGrounded)
            {
                previousCharacterControllerPosition = Character.transform.position;
                Character.Move(rigidbodyPhysicsMovement);
                if (Facade.Offset != null)
                {
                    Vector3 movement = Character.transform.position - previousCharacterControllerPosition;
                    UpdateAliasPosition(movement, movement, true, true, false, AliasMovementDuration);
                }
            }

            // Position the CharacterController and handle moving the source relative to the offset.
            Vector3 characterControllerPosition = Character.transform.position;
            previousCharacterControllerPosition = characterControllerPosition;
            MatchCharacterControllerWithSource(Facade.Source.transform.position, false);
            return characterControllerPosition - previousCharacterControllerPosition;
        }

        /// <summary>
        /// Updates the <see cref="MovementInterest"/> type based on whether the controller is grounded or not.
        /// </summary>
        /// <param name="isGrounded">Whether the controller is touching the ground.</param>
        /// <param name="rigidbodyPhysicsMovement">The calculated <see cref="Rigidbody"/> movement position.</param>
        protected virtual void UpdateInterestType(bool isGrounded, Vector3 rigidbodyPhysicsMovement)
        {
            // Allow moving the RigidBody via physics.
            if (Interest == MovementInterest.CharacterControllerUntilAirborne && !isGrounded)
            {
                Interest = MovementInterest.RigidbodyUntilGrounded;
            }
            else if (Interest == MovementInterest.RigidbodyUntilGrounded
                && isGrounded
                && rigidbodyPhysicsMovement.sqrMagnitude <= 1E-06F
                && rigidbodySetFrameCount > 0
                && rigidbodySetFrameCount + 1 < Time.frameCount)
            {
                Interest = MovementInterest.CharacterControllerUntilAirborne;
            }
        }

        /// <summary>
        /// Updates the alias targets for any vertical movement.
        /// </summary>
        /// <param name="isGrounded">Whether the controller is touching the ground.</param>
        /// <param name="characterControllerSourceMovement">The calculated <see cref="Character"/> movement position.</param>
        protected virtual void UpdateAliasForVerticalMovement(bool isGrounded, Vector3 characterControllerSourceMovement)
        {
            // Handle walking up stairs/slopes via the CharacterController.
            if (isGrounded && Facade.Offset != null && characterControllerSourceMovement.y > 0f)
            {
                UpdateAliasPosition(Vector3.up * characterControllerSourceMovement.y, default, true, true, true, AliasMovementDuration);
            }
        }

        /// <summary>
        /// Updates the position of the alias objects.
        /// </summary>
        /// <param name="newOffsetPosition">The new position for the <see cref="Facade.Offset"/>.</param>
        /// <param name="newSourcePosition">The new position for the <see cref="Facade.Source"/>.</param>
        /// <param name="incrementOffset">Whether to increment the <see cref="Facade.Offset"/> position or set it to a new value.</param>
        /// <param name="incrementSource">Whether to increment the <see cref="Facade.Source"/> position or set it to a new value.</param>
        /// <param name="ignoreSourcePosition">Whether to ignore setting the <see cref="Facade.Source"/> position.</param>
        /// <param name="dampDuration">The duration to dampen the movemnt of the position updates.</param>
        protected virtual void UpdateAliasPosition(Vector3 newOffsetPosition, Vector3 newSourcePosition, bool incrementOffset, bool incrementSource, bool ignoreSourcePosition, float dampDuration)
        {
            if (Facade.Offset != null)
            {
                Vector3 targetOffsetPosition = (incrementOffset ? Facade.Offset.transform.position : Vector3.zero) + newOffsetPosition;
                Facade.Offset.transform.position = dampDuration > 0f ?
                    Vector3.SmoothDamp(Facade.Offset.transform.position, targetOffsetPosition, ref smoothDampVelocity, dampDuration) :
                    targetOffsetPosition;
            }

            if (Facade.Source != null && UpdateSourcePosition && !ignoreSourcePosition)
            {
                Vector3 targetSourcePosition = (incrementSource ? Facade.Source.transform.position : Vector3.zero) + newSourcePosition;
                Facade.Source.transform.position = dampDuration > 0f ?
                    Vector3.SmoothDamp(Facade.Source.transform.position, targetSourcePosition, ref smoothDampVelocity, dampDuration) :
                    targetSourcePosition;
            }
        }

        /// <summary>
        /// Updates the alias targets to resolve any collisions.
        /// </summary>
        protected virtual void UpdateAliasForCollision()
        {
            Vector3 characterControllerPosition = Character.transform.position + Character.center;
            Vector3 difference = Facade.Source.transform.position - characterControllerPosition;
            difference.y = 0f;
            float minimumDistanceToColliders = Character.radius - Facade.SourceThickness;

            if (difference.magnitude < minimumDistanceToColliders && !IsDiverged)
            {
                return;
            }

            float newDistance = difference.magnitude - minimumDistanceToColliders;
            Vector3 newPosition = difference.normalized * newDistance * -1f;
            UpdateAliasPosition(newPosition, newPosition, true, true, false, CollisionMovementDuration);
        }

        /// <summary>
        /// Updates the alias targets to resolve any divergence.
        /// </summary>
        protected virtual void UpdateAliasForDivergence()
        {
            Vector3 characterControllerPosition = Character.transform.position;
            Vector3 difference = Facade.Source.transform.position - characterControllerPosition;
            difference.y = 0f;
            Vector3 newOffsetPosition = Facade.Offset.transform.position - difference - (Vector3.one * -Character.skinWidth);
            newOffsetPosition.y = Facade.Offset.transform.position.y;
            UpdateAliasPosition(-difference, -difference, true, true, false, CollisionMovementDuration);
        }

        /// <summary>
        /// Processes the object followers for the <see cref="Facade.Source"/> and <see cref="Facade.Offset"/>.
        /// </summary>
        protected virtual void ProcessObjectFollowers()
        {
            if (offsetObjectFollower != null)
            {
                offsetObjectFollower.Process();
            }

            if (sourceObjectFollower != null)
            {
                sourceObjectFollower.Process();
            }
        }

        /// <summary>
        /// Snaps the <see cref="CharacterController"/> and the <see cref="PhysicsBody"/> to the <see cref="Facade.Source"/>.
        /// </summary>
        protected virtual void SnapDependentsToSource()
        {
            MatchCharacterControllerWithSource(Facade.Source.transform.position, true);
            MatchRigidbodyAndColliderWithCharacterController();
            RememberCurrentPositions();
        }

        /// <summary>
        /// Ignores the Interactable grabbed by the Interactor.
        /// </summary>
        /// <param name="interactable">The Interactable to ignore.</param>
        protected virtual void IgnoreInteractorGrabbedCollision(InteractableFacade interactable)
        {
            CollisionsToIgnore.RunWhenActiveAndEnabled(() => CollisionsToIgnore.Targets.AddUnique(interactable.gameObject));
        }

        /// <summary>
        /// Resumes the Interactable ungrabbed by the Interactor.
        /// </summary>
        /// <param name="interactable">The Interactable to resume.</param>
        protected virtual void ResumeInteractorUngrabbedCollision(InteractableFacade interactable)
        {
            if (!Facade.IgnoredGameObjects.Contains(interactable.gameObject) &&
                (
                interactable.GrabbingInteractors.Count == 0 ||
                !Facade.IgnoredGameObjects.NonSubscribableElements.Intersect(GetGameObjectListFromInteractorFacadeList(interactable.GrabbingInteractors)).Any())
                )
            {
                CollisionsToIgnore.RunWhenActiveAndEnabled(() => CollisionsToIgnore.Targets.Remove(interactable.gameObject));
            }
        }

        /// <summary>
        /// Converts the <see cref="InteractorFacade"/> collection to a <see cref="GameObject"/> collection.
        /// </summary>
        /// <param name="interactorList">The list to convert.</param>
        /// <returns>The converted list.</returns>
        protected virtual IReadOnlyList<GameObject> GetGameObjectListFromInteractorFacadeList(IReadOnlyList<InteractorFacade> interactorList)
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            foreach (InteractorFacade interactor in interactorList)
            {
                gameObjectList.Add(interactor.gameObject);
            }

            return gameObjectList;
        }

        /// <summary>
        /// Gets the position of the <see cref="Character"/>.
        /// </summary>
        /// <param name="sourcePosition">The given position of the source.</param>
        /// <param name="height">The calculated height of the Character.</param>
        /// <returns>The world position of the Character.</returns>
        protected virtual Vector3 GetCharacterPosition(Vector3 sourcePosition, out float height)
        {
            height = Facade.Offset == null ? sourcePosition.y : Facade.Offset.transform.InverseTransformPoint(sourcePosition).y * Facade.Offset.transform.lossyScale.y;
            height -= Character.skinWidth;

            // CharacterController enforces a minimum height of twice its radius, so let's match that here.
            height = Mathf.Max(height, 2f * Character.radius);

            Vector3 position = sourcePosition;
            position.y -= height;

            if (Facade.Offset != null)
            {
                // The offset defines the source's "floor".
                position.y = Mathf.Max(position.y, Facade.Offset.transform.position.y + Character.skinWidth);
            }

            return position;
        }

        /// <summary>
        /// Changes the height and position of <see cref="Character"/> to match <see cref="PseudoBodyFacade.Source"/>.
        /// </summary>
        /// <param name="targetPosition">The position to update to.</param>
        /// <param name="setPositionDirectly">Whether to set the position directly or tell <see cref="Character"/> to move to it.</param>
        protected virtual void MatchCharacterControllerWithSource(Vector3 targetPosition, bool setPositionDirectly)
        {
            Vector3 position = GetCharacterPosition(targetPosition, out float height);

            if (setPositionDirectly)
            {
                Character.transform.position = position;
            }
            else
            {
                Vector3 movement = position - Character.transform.position;
                // The CharacterController doesn't resolve any potential collisions in case we don't move it.
                Character.Move(movement == Vector3.zero ? movement + collisionResolutionMovement : movement);
                if (movement == Vector3.zero)
                {
                    Character.Move(movement - collisionResolutionMovement);
                }
            }

            Character.height = height;

            Vector3 center = Character.center;
            center.y = height / 2f;
            Character.center = center;
        }

        /// <summary>
        /// Changes <see cref="RigidbodyCollider"/> to match the collider settings of <see cref="Character"/> and moves <see cref="PhysicsBody"/> to match <see cref="Character"/>.
        /// </summary>
        protected virtual void MatchRigidbodyAndColliderWithCharacterController()
        {
            RigidbodyCollider.height = Character.height + Character.skinWidth;

            Vector3 center = Character.center;
            center.y = (Character.height - Character.skinWidth) / 2f;
            RigidbodyCollider.center = center;

            PhysicsBody.position = Character.transform.position;
        }

        /// <summary>
        /// Checks whether <see cref="Character"/> is grounded.
        /// </summary>
        /// <remarks>
        /// <see cref="CharacterController.isGrounded"/> isn't accurate so this method does an additional check using <see cref="Physics"/>.
        /// </remarks>
        /// <returns>Whether <see cref="Character"/> is grounded.</returns>
        protected virtual bool CheckIfCharacterControllerIsGrounded()
        {
            return Character.isGrounded ? true : CheckForSurroundingCollisions(Character.transform.position + CharacterCenter, Character.radius);
        }

        /// <summary>
        /// Checks for any collisions in the surrounding area.
        /// </summary>
        /// <param name="center">The center point to start the spherecast check from.</param>
        /// <param name="radius">The radius to perform the spherecast check to.</param>
        /// <returns>Whether any collisions have occcured.</returns>
        protected virtual bool CheckForSurroundingCollisions(Vector3 center, float radius)
        {
            HeapAllocationFreeReadOnlyList<Collider> hitColliders = PhysicsCast.OverlapSphereAll(null, center, radius, 0);

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider != Character
                    && hitCollider != RigidbodyCollider
                    && !ignoredColliders.Contains(hitCollider)
                    && !Physics.GetIgnoreLayerCollision(hitCollider.gameObject.layer, Character.gameObject.layer)
                    && !Physics.GetIgnoreLayerCollision(hitCollider.gameObject.layer, PhysicsBody.gameObject.layer))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the previous position variables to remember the current state.
        /// </summary>
        protected virtual void RememberCurrentPositions()
        {
            previousRigidbodyPosition = PhysicsBody.position;
        }

        /// <summary>
        /// Emits <see cref="BodyRepresentationFacade.BecameGrounded"/> or <see cref="PseudoBodyFacade.BecameAirborne"/>.
        /// </summary>
        /// <param name="isCharacterControllerGrounded">The current state.</param>
        protected virtual void EmitIsGroundedChangedEvent(bool isCharacterControllerGrounded)
        {
            if (wasCharacterControllerGrounded == isCharacterControllerGrounded)
            {
                return;
            }

            wasCharacterControllerGrounded = isCharacterControllerGrounded;

            if (isCharacterControllerGrounded)
            {
                Facade.BecameGrounded?.Invoke();
            }
            else
            {
                Facade.BecameAirborne?.Invoke();
            }
        }

        /// <summary>
        /// Determines the divergence state of the pseudo body.
        /// </summary>
        /// <returns>The divergence state.</returns>
        protected virtual DivergenceState GetDivergenceState()
        {
            int isDivergedValue = IsDiverged ? 1 : 0;
            int wasDivergedValue = wasDiverged ? 2 : 0;

            switch (isDivergedValue + wasDivergedValue)
            {
                case 0:
                    return DivergenceState.NotDiverged;
                case 1:
                    return DivergenceState.BecameDiverged;
                case 2:
                    return DivergenceState.BecameConverged;
                case 3:
                    return DivergenceState.StillDiverged;
            }

            return DivergenceState.NotDiverged;
        }

        /// <summary>
        /// Determines whether the given position will diverge from the <see cref="Character"/> position.
        /// </summary>
        /// <param name="targetPosition">The position to check.</param>
        /// <param name="divergenceThreshold">The threshold in which to consider a divergence has occurred.</param>
        /// <returns>Whether there was a divergence between the two positions.</returns>
        protected virtual bool WillDiverge(Vector3 targetPosition, Vector3 divergenceThreshold)
        {
            return !targetPosition.WithinDistance(Character.transform.position + Character.center, divergenceThreshold);
        }

        /// <summary>
        /// Check to see if the <see cref="Facade.Source"/> has diverged or converged with the <see cref="Character"/>.
        /// </summary>
        protected virtual void CheckDivergence()
        {
            wasDiverged = IsDiverged;
            IsDiverged = WillDiverge(Facade.Source.transform.position, Facade.SourceDivergenceThreshold);

            switch (GetDivergenceState())
            {
                case DivergenceState.BecameDiverged:
                    Facade.Diverged?.Invoke();
                    break;
                case DivergenceState.StillDiverged:
                    StopCheckDivergenceAtEndOfFrameRoutine();
                    checkDivergedAtEndOfFrameRoutine = StartCoroutine(CheckDivergenceAtEndOfFrame());
                    break;
                case DivergenceState.BecameConverged:
                    StopCheckDivergenceAtEndOfFrameRoutine();
                    Facade.Converged?.Invoke();
                    break;
            }
        }

        /// <summary>
        /// Check to see if the <see cref="Facade.Source"/> is still diverged with the <see cref="Character"/> at the end of the frame.
        /// </summary>
        /// <returns>An Enumerator to manage the running of the Coroutine.</returns>
        protected virtual IEnumerator CheckDivergenceAtEndOfFrame()
        {
            yield return waitForEndOfFixedUpdate;
            if (IsDiverged)
            {
                Facade.StillDiverged?.Invoke();
            }

            checkDivergedAtEndOfFrameRoutine = null;
        }

        /// <summary>
        /// Stops the divergence check coroutine from running.
        /// </summary>
        protected virtual void StopCheckDivergenceAtEndOfFrameRoutine()
        {
            if (checkDivergedAtEndOfFrameRoutine != null)
            {
                StopCoroutine(checkDivergedAtEndOfFrameRoutine);
                checkDivergedAtEndOfFrameRoutine = null;
            }
        }

        /// <summary>
        /// Resets the <see cref="Interest"/> back to <see cref="MovementInterest.RigidbodyUntilGrounded"/> after a force is applied to the <see cref="PhysicsBody"/>.
        /// </summary>
        /// <returns>An Enumerator to manage the running of the Coroutine.</returns>
        protected IEnumerator ResetInterestAfterForce()
        {
            yield return new WaitForFixedUpdate();
            Interest = MovementInterest.RigidbodyUntilGrounded;
        }

        /// <summary>
        /// Stops the reset interest after force coroutine from running.
        /// </summary>
        protected virtual void StopResetInterestAfterForceRoutine()
        {
            if (resetInterestAfterAddForceRoutine != null)
            {
                StopCoroutine(resetInterestAfterAddForceRoutine);
                resetInterestAfterAddForceRoutine = null;
            }
        }

        /// <summary>
        /// Called after <see cref="Interest"/> has been changed.
        /// </summary>
        protected virtual void OnAfterInterestChange()
        {
            switch (Interest)
            {
                case MovementInterest.CharacterController:
                case MovementInterest.CharacterControllerUntilAirborne:
                    PhysicsBody.isKinematic = true;
                    rigidbodySetFrameCount = 0;
                    break;
                case MovementInterest.Rigidbody:
                case MovementInterest.RigidbodyUntilGrounded:
                    PhysicsBody.isKinematic = false;
                    rigidbodySetFrameCount = Time.frameCount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Interest), Interest, null);
            }
        }
    }
}