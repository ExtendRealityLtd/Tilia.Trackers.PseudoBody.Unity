namespace Tilia.Trackers.PseudoBody
{
    using System;
    using Tilia.Interactions.Interactables.Interactors.Collection;
    using UnityEngine;
    using UnityEngine.Events;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection.List;
    using Zinnia.Extension;

    /// <summary>
    /// The public interface for the PseudoBody prefab.
    /// </summary>
    public class PseudoBodyFacade : MonoBehaviour
    {
        #region Tracking Settings
        [Header("Tracking Settings")]
        [Tooltip("The object to follow.")]
        [SerializeField]
        private GameObject source;
        /// <summary>
        /// The object to follow.
        /// </summary>
        public GameObject Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                if (this.IsMemberChangeAllowed())
                {
                    OnAfterSourceChange();
                }
            }
        }
        [Tooltip("An optional offset for the Source to use.")]
        [SerializeField]
        private GameObject offset;
        /// <summary>
        /// An optional offset for the <see cref="Source"/> to use.
        /// </summary>
        public GameObject Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
                if (this.IsMemberChangeAllowed())
                {
                    OnAfterOffsetChange();
                }
            }
        }
        #endregion

        #region Collision Settings
        [Header("Collision Settings")]
        [Tooltip("The thickness of Source to be used when resolving body collisions.")]
        [SerializeField]
        private float sourceThickness = 0.25f;
        /// <summary>
        /// The thickness of <see cref="Source"/> to be used when resolving body collisions.
        /// </summary>
        public float SourceThickness
        {
            get
            {
                return sourceThickness;
            }
            set
            {
                sourceThickness = value;
            }
        }
        [Tooltip("The distance the pseudo body has to be away from the Source to be considered diverged.")]
        [SerializeField]
        private Vector3 sourceDivergenceThreshold = new Vector3(0.01f, 2f, 0.01f);
        /// <summary>
        /// The distance the pseudo body has to be away from the <see cref="Source"/> to be considered diverged.
        /// </summary>
        public Vector3 SourceDivergenceThreshold
        {
            get
            {
                return sourceDivergenceThreshold;
            }
            set
            {
                sourceDivergenceThreshold = value;
            }
        }
        #endregion

        #region Interaction Settings
        [Header("Interaction Settings")]
        [Tooltip("A collection of Interactors to exclude from physics collision checks.")]
        [SerializeField]
        [Restricted]
        [Obsolete("Use `IgnoredGameObjects` instead.")]
        private InteractorFacadeObservableList ignoredInteractors;
        /// <summary>
        /// A collection of Interactors to exclude from physics collision checks.
        /// </summary>
        [Obsolete("Use `IgnoredGameObjects` instead.")]
        public InteractorFacadeObservableList IgnoredInteractors
        {
#pragma warning disable 0618
            get
            {
                return ignoredInteractors;
            }
            set
            {
                ignoredInteractors = value;
            }
#pragma warning restore 0618
        }
        [Tooltip("A GameObject collection to exclude from physics collision checks.")]
        [SerializeField]
        private GameObjectObservableList ignoredGameObjects;
        /// <summary>
        /// A <see cref="GameObject"/> collection to exclude from physics collision checks.
        /// </summary>
        public GameObjectObservableList IgnoredGameObjects
        {
            get
            {
                return ignoredGameObjects;
            }
            set
            {
                ignoredGameObjects = value;
            }
        }
        #endregion

        #region Body Events
        /// <summary>
        /// Emitted when the pseudo body starts no longer being within the threshold distance of the <see cref="Source."/>.
        /// </summary>
        [Header("Body Events")]
        public UnityEvent Diverged = new UnityEvent();
        /// <summary>
        /// Emitted when the pseudo body continues no longer being within the threshold distance of the <see cref="Source."/> for each subsequent frame.
        /// </summary>
        public UnityEvent StillDiverged = new UnityEvent();
        /// <summary>
        /// Emitted when the pseudo body is back within the threshold distance of the <see cref="Source."/> after being diverged.
        /// </summary>
        public UnityEvent Converged = new UnityEvent();
        /// <summary>
        /// Emitted when the body starts touching ground.
        /// </summary>
        public UnityEvent BecameGrounded = new UnityEvent();
        /// <summary>
        /// Emitted when the body stops touching ground.
        /// </summary>
        public UnityEvent BecameAirborne = new UnityEvent();
        #endregion

        #region Reference Settings
        [Header("Reference Settings")]
        [Tooltip("The linked Internal Setup.")]
        [SerializeField]
        [Restricted]
        private PseudoBodyProcessor processor;
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        public PseudoBodyProcessor Processor
        {
            get
            {
                return processor;
            }
            protected set
            {
                processor = value;
            }
        }
        #endregion

        /// <summary>
        /// The object that defines the main source of truth for movement.
        /// </summary>
        public virtual PseudoBodyProcessor.MovementInterest Interest
        {
            get
            {
                return Processor.Interest;
            }
            set
            {
                Processor.Interest = value;
            }
        }

        /// <summary>
        /// Whether the body touches ground.
        /// </summary>
        public virtual bool IsCharacterControllerGrounded => Processor.IsCharacterControllerGrounded;

        /// <summary>
        /// The <see cref="Rigidbody"/> that acts as the physical representation of the body.
        /// </summary>
        public virtual Rigidbody PhysicsBody => Processor.PhysicsBody;

        /// <summary>
        /// Whether the <see cref="Source"/> has diverged from the <see cref="PseudoBodyProcessor.Character"/>.
        /// </summary>
        public virtual bool IsDiverged => Processor.IsDiverged;

        /// <summary>
        /// Clears <see cref="Source"/>.
        /// </summary>
        public virtual void ClearSource()
        {
            if (!this.IsValidState())
            {
                return;
            }

            Source = default;
        }

        /// <summary>
        /// Clears <see cref="Offset"/>.
        /// </summary>
        public virtual void ClearOffset()
        {
            if (!this.IsValidState())
            {
                return;
            }

            Offset = default;
        }

        /// <summary>
        /// Sets the <see cref="SourceDivergenceThreshold"/> x value.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public virtual void SetSourceDivergenceThresholdX(float value)
        {
            SourceDivergenceThreshold = new Vector3(value, SourceDivergenceThreshold.y, SourceDivergenceThreshold.z);
        }

        /// <summary>
        /// Sets the <see cref="SourceDivergenceThreshold"/> y value.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public virtual void SetSourceDivergenceThresholdY(float value)
        {
            SourceDivergenceThreshold = new Vector3(SourceDivergenceThreshold.x, value, SourceDivergenceThreshold.z);
        }

        /// <summary>
        /// Sets the <see cref="SourceDivergenceThreshold"/> z value.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public virtual void SetSourceDivergenceThresholdZ(float value)
        {
            SourceDivergenceThreshold = new Vector3(SourceDivergenceThreshold.x, SourceDivergenceThreshold.y, value);
        }

        /// <summary>
        /// Sets the source of truth for movement to come from <see cref="PseudoBodyProcessor.PhysicsBody"/> until <see cref="PseudoBodyProcessor.Character"/> hits the ground, then <see cref="PseudoBodyProcessor.Character"/> is the new source of truth.
        /// </summary>
        /// <remarks>
        /// This method needs to be called right before or right after applying any form of movement to the <see cref="Rigidbody"/> while the body is grounded, i.e. in the same frame and before <see cref="PseudoBodyProcessor.Process"/> is called.
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

        /// <summary>
        /// Snaps the <see cref="Processor.Character"/> to the <see cref="Source"/> position.
        /// </summary>
        public virtual void SnapToSource()
        {
            Processor.SnapToSource();
        }

        protected virtual void Awake()
        {
#pragma warning disable 0618
            if (IgnoredInteractors.NonSubscribableElements.Count > 0)
            {
                Debug.LogWarning("`PsuedoBodyFacade.IgnoredInteractors` list has been deprecated. Use the `PsuedoBodyFacade.IgnoredGameObjects` list instead.", gameObject);
            }
#pragma warning restore 0618
        }

        /// <summary>
        /// Called after <see cref="Source"/> has been changed.
        /// </summary>
        protected virtual void OnAfterSourceChange()
        {
            Processor.ConfigureSourceObjectFollower();
        }

        /// <summary>
        /// Called after <see cref="Offset"/> has been changed.
        /// </summary>
        protected virtual void OnAfterOffsetChange()
        {
            Processor.ConfigureOffsetObjectFollower();
        }
    }
}