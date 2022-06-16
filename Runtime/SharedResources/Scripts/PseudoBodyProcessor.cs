namespace Tilia.Trackers.PseudoBody
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Tilia.Interactions.Interactables.Interactables;
    using Tilia.Interactions.Interactables.Interactors;
    using UnityEngine;
    using Zinnia.Cast;
    using Zinnia.Data.Attribute;
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
            protected set
            {
                facade = value;
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
            protected set
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
            protected set
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
            protected set
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
            protected set
            {
                collisionsToIgnore = value;
            }
        }
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
        /// Whether <see cref="Facade.Source"/> has diverged from the <see cref="Character"/>.
        /// </summary>
        public virtual bool IsDiverged { get; protected set; }

        /// <summary>
        /// Movement to apply to <see cref="Character"/> to resolve collisions.
        /// </summary>
        protected static readonly Vector3 collisionResolutionMovement = new Vector3(0.001f, 0f, 0f);
        /// <summary>
        /// The colliders to ignore body collisions with.
        /// </summary>
        protected readonly HashSet<Collider> ignoredColliders = new HashSet<Collider>();
        /// <summary>
        /// The colliders to restore after an ungrab.
        /// </summary>
        protected readonly HashSet<Collider> restoreColliders = new HashSet<Collider>();
        /// <summary>
        /// The previous position of <see cref="PhysicsBody"/>.
        /// </summary>
        protected Vector3 previousRigidbodyPosition;
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
        /// Whether to snap the dependents to the <see cref="Facade.Source"/> without any divergent checking.
        /// </summary>
        protected bool doSnapToSource;

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

            if (Interest != MovementInterest.CharacterController && Facade.Offset != null)
            {
                Vector3 offsetPosition = Facade.Offset.transform.position;
                Vector3 previousPosition = offsetPosition;

                offsetPosition.y = PhysicsBody.position.y - Character.skinWidth;

                Facade.Offset.transform.position = offsetPosition;
                if (UpdateSourcePosition)
                {
                    Facade.Source.transform.position += offsetPosition - previousPosition;
                }
            }

            Vector3 previousCharacterControllerPosition;

            // Handle walking down stairs/slopes and physics affecting the RigidBody in general.
            Vector3 rigidbodyPhysicsMovement = PhysicsBody.position - previousRigidbodyPosition;
            if (Interest == MovementInterest.Rigidbody || Interest == MovementInterest.RigidbodyUntilGrounded)
            {
                previousCharacterControllerPosition = Character.transform.position;
                Character.Move(rigidbodyPhysicsMovement);

                if (Facade.Offset != null)
                {
                    Vector3 movement = Character.transform.position - previousCharacterControllerPosition;
                    Facade.Offset.transform.position += movement;
                    if (UpdateSourcePosition)
                    {
                        Facade.Source.transform.position += movement;
                    }
                }
            }

            // Position the CharacterController and handle moving the source relative to the offset.
            Vector3 characterControllerPosition = Character.transform.position;
            previousCharacterControllerPosition = characterControllerPosition;
            MatchCharacterControllerWithSource(false);
            Vector3 characterControllerSourceMovement = characterControllerPosition - previousCharacterControllerPosition;

            bool isGrounded = CheckIfCharacterControllerIsGrounded();

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

            // Handle walking up stairs/slopes via the CharacterController.
            if (isGrounded && Facade.Offset != null && characterControllerSourceMovement.y > 0f)
            {
                Facade.Offset.transform.position += Vector3.up * characterControllerSourceMovement.y;
            }

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

            if (offsetObjectFollower != null)
            {
                offsetObjectFollower.Process();
            }

            if (sourceObjectFollower != null)
            {
                sourceObjectFollower.Process();
            }

            Process();
            Vector3 characterControllerPosition = Character.transform.position + Character.center;
            Vector3 difference = Facade.Source.transform.position - characterControllerPosition;
            difference.y = 0f;

            float minimumDistanceToColliders = Character.radius - Facade.SourceThickness;
            if (difference.magnitude < minimumDistanceToColliders && !IsDiverged)
            {
                return;
            }

            float newDistance = difference.magnitude - minimumDistanceToColliders;

            Vector3 newPosition = difference.normalized * newDistance;
            if (Facade.Offset == null)
            {
                if (UpdateSourcePosition)
                {
                    Facade.Source.transform.position -= newPosition;
                }
            }
            else
            {
                Facade.Offset.transform.position -= newPosition;
            }

            Process();
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
            Interest = MovementInterest.CharacterControllerUntilAirborne;
            SnapDependentsToSource();
        }

        protected virtual void OnDisable()
        {
            StopCheckDivergenceAtEndOfFrameRoutine();
            sourceObjectFollower = null;
            offsetObjectFollower = null;
        }

        /// <summary>
        /// Snaps the <see cref="CharacterController"/> and the <see cref="PhysicsBody"/> to the <see cref="Facade.Source"/>.
        /// </summary>
        protected virtual void SnapDependentsToSource()
        {
            MatchCharacterControllerWithSource(true);
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
        /// Changes the height and position of <see cref="Character"/> to match <see cref="PseudoBodyFacade.Source"/>.
        /// </summary>
        /// <param name="setPositionDirectly">Whether to set the position directly or tell <see cref="Character"/> to move to it.</param>
        protected virtual void MatchCharacterControllerWithSource(bool setPositionDirectly)
        {
            Vector3 sourcePosition = Facade.Source.transform.position;
            float height = Facade.Offset == null
                ? sourcePosition.y
                : Facade.Offset.transform.InverseTransformPoint(sourcePosition).y * Facade.Offset.transform.lossyScale.y;
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
            RigidbodyCollider.radius = Character.radius;
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
            if (Character.isGrounded)
            {
                return true;
            }

            HeapAllocationFreeReadOnlyList<Collider> hitColliders = PhysicsCast.OverlapSphereAll(
                null,
                Character.transform.position + (Vector3.up * (Character.radius - Character.skinWidth - 0.001f)),
                Character.radius,
                1 << Character.gameObject.layer);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider != Character
                    && hitCollider != RigidbodyCollider
                    && !ignoredColliders.Contains(hitCollider)
                    && !Physics.GetIgnoreLayerCollision(
                        hitCollider.gameObject.layer,
                        Character.gameObject.layer)
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
        /// Check to see if the <see cref="Facade.Source"/> has diverged or converged with the <see cref="Character"/>.
        /// </summary>
        protected virtual void CheckDivergence()
        {
            wasDiverged = IsDiverged;
            IsDiverged = !Facade.Source.transform.position.WithinDistance(Character.transform.position + Character.center, Facade.SourceDivergenceThreshold);

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
            yield return new WaitForEndOfFrame();
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