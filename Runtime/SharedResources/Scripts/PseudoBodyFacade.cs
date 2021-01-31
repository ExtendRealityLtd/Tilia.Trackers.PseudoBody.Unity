namespace Tilia.Trackers.PseudoBody
{
    using Malimbe.MemberChangeMethod;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using System;
    using Tilia.Interactions.Interactables.Interactors.Collection;
    using UnityEngine;
    using UnityEngine.Events;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection.List;

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
        /// A collection of Interactors to exclude from physics collision checks.
        /// </summary>
        [Serialized]
        [field: Header("Interaction Settings"), DocumentedByXml, Restricted]
        [Obsolete("Use `IgnoredGameObjects` instead.")]
        public InteractorFacadeObservableList IgnoredInteractors { get; set; }
        /// <summary>
        /// A <see cref="GameObject"/> collection to exclude from physics collision checks.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public GameObjectObservableList IgnoredGameObjects { get; set; }
        #endregion

        #region Body Events
        /// <summary>
        /// Emitted when the pseudo body starts no longer being within the threshold distance of the <see cref="Source."/>.
        /// </summary>
        [Header("Body Events"), DocumentedByXml]
        public UnityEvent Diverged = new UnityEvent();
        /// <summary>
        /// Emitted when the pseudo body continues no longer being within the threshold distance of the <see cref="Source."/> for each subsequent frame.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent StillDiverged = new UnityEvent();
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
    }
}