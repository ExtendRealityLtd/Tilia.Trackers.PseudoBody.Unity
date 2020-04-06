﻿namespace Tilia.Trackers.PseudoBody
{
    using Malimbe.MemberChangeMethod;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Tilia.Interactions.Interactables.Interactors;
    using Tilia.Interactions.Interactables.Interactors.Collection;
    using UnityEngine;
    using UnityEngine.Events;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface for the PseudoBody prefab.
    /// </summary>
    public class PseudoBodyFacade : MonoBehaviour
    {
        #region Tracking Settings
        /// <summary>
        /// The object to follow.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Tracking Settings"), DocumentedByXml]
        public GameObject Source { get; set; }
        /// <summary>
        /// An optional offset for the <see cref="Source"/> to use.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject Offset { get; set; }
        #endregion

        #region Collision Settings
        /// <summary>
        /// The thickness of <see cref="Source"/> to be used when resolving body collisions.
        /// </summary>
        [Serialized]
        [field: Header("Collision Settings"), DocumentedByXml]
        public float SourceThickness { get; set; } = 0.25f;
        /// <summary>
        /// The distance the pseudo body has to be away from the <see cref="Source"/> to be considered diverged.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public Vector3 SourceDivergenceThreshold { get; set; } = new Vector3(0.01f, 2f, 0.01f);
        #endregion

        #region Interaction Settings
        /// <summary>
        /// A collection of interactors to exclude from physics collision checks.
        /// </summary>
        [Serialized]
        [field: Header("Interaction Settings"), DocumentedByXml]
        public InteractorFacadeObservableList IgnoredInteractors { get; set; }
        #endregion

        #region Body Events
        /// <summary>
        /// Emitted when the pseudo body starts no longer being within the threshold distance of the <see cref="Source."/>.
        /// </summary>
        [Header("Body Events"), DocumentedByXml]
        public UnityEvent Diverged = new UnityEvent();
        /// <summary>
        /// Emitted when the pseudo body is back within the threshold distance of the <see cref="Source."/> after being diverged.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Converged = new UnityEvent();
        /// <summary>
        /// Emitted when the body starts touching ground.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent BecameGrounded = new UnityEvent();
        /// <summary>
        /// Emitted when the body stops touching ground.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent BecameAirborne = new UnityEvent();
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public PseudoBodyProcessor Processor { get; protected set; }
        #endregion

        /// <summary>
        /// The object that defines the main source of truth for movement.
        /// </summary>
        public PseudoBodyProcessor.MovementInterest Interest
        {
            get { return Processor.Interest; }
            set { Processor.Interest = value; }
        }

        /// <summary>
        /// Whether the body touches ground.
        /// </summary>
        public bool IsCharacterControllerGrounded => Processor.IsCharacterControllerGrounded;

        /// <summary>
        /// The <see cref="Rigidbody"/> that acts as the physical representation of the body.
        /// </summary>
        public Rigidbody PhysicsBody => Processor.PhysicsBody;

        /// <summary>
        /// Whether the <see cref="Source"/> has diverged from the <see cref="PseudoBodyProcessor.Character"/>.
        /// </summary>
        public bool IsDiverged => Processor.IsDiverged;

        /// <summary>
        /// Sets the source of truth for movement to come from <see cref="PseudoBodyProcessor.PhysicsBody"/> until <see cref="PseudoBodyProcessor.Character"/> hits the ground, then <see cref="PseudoBodyProcessor.Character"/> is the new source of truth.
        /// </summary>
        /// <remarks>
        /// This method needs to be called right before or right after applying any form of movement to the rigidbody while the body is grounded, i.e. in the same frame and before <see cref="PseudoBodyProcessor.Process"/> is called.
        /// </remarks>
        public virtual void ListenToRigidbodyMovement()
        {
            Interest = PseudoBodyProcessor.MovementInterest.RigidbodyUntilGrounded;
        }

        /// <summary>
        /// Solves body collisions by not moving the body in case it can't go to its current position.
        /// </summary>
        /// <remarks>
        /// If body collisions should be prevented this method needs to be called right before or right after applying any form of movement to the body.
        /// </remarks>
        public virtual void SolveBodyCollisions()
        {
            Processor.SolveBodyCollisions();
        }

        protected virtual void OnEnable()
        {
            if (IgnoredInteractors == null)
            {
                return;
            }

            IgnoredInteractors.Added.AddListener(OnIgnoredInteractorAdded);
            IgnoredInteractors.Removed.AddListener(OnIgnoredInteractorRemoved);
        }

        protected virtual void OnDisable()
        {
            if (IgnoredInteractors == null)
            {
                return;
            }

            IgnoredInteractors.Added.RemoveListener(OnIgnoredInteractorAdded);
            IgnoredInteractors.Removed.RemoveListener(OnIgnoredInteractorRemoved);
        }

        /// <summary>
        /// Processes when a new <see cref="InteractorFacade"/> is added to the ignored collection.
        /// </summary>
        /// <param name="interactor">The interactor to ignore collisions from.</param>
        protected virtual void OnIgnoredInteractorAdded(InteractorFacade interactor)
        {
            Processor.IgnoreInteractorsCollisions(interactor);
        }

        /// <summary>
        /// Processes when a new <see cref="InteractorFacade"/> is removed from the ignored collection.
        /// </summary>
        /// <param name="interactor">The interactor to resume collisions with.</param>
        protected virtual void OnIgnoredInteractorRemoved(InteractorFacade interactor)
        {
            Processor.ResumeInteractorsCollisions(interactor);
        }

        /// <summary>
        /// Called after <see cref="Source"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Source))]
        protected virtual void OnAfterSourceChange()
        {
            Processor.ConfigureSourceObjectFollower();
        }

        /// <summary>
        /// Called after <see cref="Offset"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Offset))]
        protected virtual void OnAfterOffsetChange()
        {
            Processor.ConfigureOffsetObjectFollower();
        }

        /// <summary>
        /// Called after <see cref="IgnoredInteractors"/> has been changed.
        /// </summary>
        [CalledBeforeChangeOf(nameof(IgnoredInteractors))]
        protected virtual void OnBeforeIgnoredInteractorsChange()
        {
            if (IgnoredInteractors == null)
            {
                return;
            }

            IgnoredInteractors.Added.RemoveListener(OnIgnoredInteractorAdded);
            IgnoredInteractors.Removed.RemoveListener(OnIgnoredInteractorRemoved);
        }

        /// <summary>
        /// Called after <see cref="IgnoredInteractors"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(IgnoredInteractors))]
        protected virtual void OnAfterIgnoredInteractorsChange()
        {
            if (IgnoredInteractors == null)
            {
                return;
            }

            IgnoredInteractors.Added.AddListener(OnIgnoredInteractorAdded);
            IgnoredInteractors.Removed.AddListener(OnIgnoredInteractorRemoved);
        }
    }
}