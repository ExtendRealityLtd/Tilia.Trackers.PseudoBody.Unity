# Class PseudoBodyProcessor

Sets up the PseudoBody prefab based on the provided user settings and implements the logic to represent a body.

## Contents

* [Inheritance]
* [Namespace]
* [Syntax]
* [Fields]
  * [allowMutateActions]
  * [checkDivergedAtEndOfFrameRoutine]
  * [collisionResolutionMovement]
  * [doSnapToSource]
  * [ignoredColliders]
  * [ignoreInteractorCollisions]
  * [offsetObjectFollower]
  * [preventMutateActions]
  * [previousCharacterControllerPosition]
  * [previousOffsetPosition]
  * [previousRigidbodyPosition]
  * [restoreColliders]
  * [rigidbodySetFrameCount]
  * [smoothDampVelocity]
  * [sourceObjectFollower]
  * [wasCharacterControllerGrounded]
  * [wasDiverged]
* [Properties]
  * [AliasMovementDuration]
  * [Character]
  * [CharacterCenter]
  * [CollisionMovementDuration]
  * [CollisionsToIgnore]
  * [CurrentDivergenceState]
  * [Facade]
  * [Interest]
  * [IsCharacterControllerGrounded]
  * [IsDiverged]
  * [PhysicsBody]
  * [RigidbodyCollider]
  * [UpdateSourcePosition]
* [Methods]
  * [AddPositionMutator(GameObject)]
  * [Awake()]
  * [CheckDivergence()]
  * [CheckDivergenceAtEndOfFrame()]
  * [CheckForSurroundingCollisions(Vector3, Single)]
  * [CheckIfCharacterControllerIsGrounded()]
  * [CheckWillDiverge(Vector3)]
  * [ConfigureCharacterRadius()]
  * [ConfigureOffsetObjectFollower()]
  * [ConfigureSourceObjectFollower()]
  * [DoCheckWillDiverge(Vector3)]
  * [EmitIsGroundedChangedEvent(Boolean)]
  * [GetCharacterPosition(Vector3, out Single)]
  * [GetDivergenceState()]
  * [GetGameObjectListFromInteractorFacadeList(IReadOnlyList<InteractorFacade>)]
  * [IgnoreInteractorGrabbedCollision(InteractableFacade)]
  * [IgnoreInteractorsCollisions(GameObject)]
  * [IgnoreInteractorsCollisions(InteractorFacade)]
  * [MatchCharacterControllerWithSource(Vector3, Boolean)]
  * [MatchRigidbodyAndColliderWithCharacterController()]
  * [OnAfterInterestChange()]
  * [OnDisable()]
  * [OnEnable()]
  * [Process()]
  * [ProcessObjectFollowers()]
  * [RememberCurrentPositions()]
  * [RemovePositionMutator(GameObject)]
  * [ResolveDivergence()]
  * [ResolveDivergence(Vector3)]
  * [ResumeInteractorsCollisions(GameObject)]
  * [ResumeInteractorsCollisions(InteractorFacade)]
  * [ResumeInteractorUngrabbedCollision(InteractableFacade)]
  * [SnapDependentsToSource()]
  * [SnapToSource()]
  * [SolveBodyCollisions()]
  * [StopCheckDivergenceAtEndOfFrameRoutine()]
  * [TryGetMutator(GameObject, out TransformPositionMutator)]
  * [UpdateAliasForCollision()]
  * [UpdateAliasForDivergence()]
  * [UpdateAliasForNonCharacterControllerInterest()]
  * [UpdateAliasForRigidbodyControllerInterest(out Vector3)]
  * [UpdateAliasForVerticalMovement(Boolean, Vector3)]
  * [UpdateAliasPosition(Vector3, Vector3, Boolean, Boolean, Boolean, Single)]
  * [UpdateInterestType(Boolean, Vector3)]
  * [WillDiverge(Vector3, Vector3)]
* [Implements]

## Details

##### Inheritance

* System.Object
* PseudoBodyProcessor

##### Implements

IProcessable

##### Namespace

* [Tilia.Trackers.PseudoBody]

##### Syntax

```
public class PseudoBodyProcessor : MonoBehaviour
```

### Fields

#### allowMutateActions

A collection of actions to perform on each TransformPositionMutator to allow mutations to occur.

##### Declaration

```
protected Dictionary<TransformPositionMutator, UnityAction<Vector3>> allowMutateActions
```

#### checkDivergedAtEndOfFrameRoutine

The routine for checking to see if the Facade.Source is still diverged with the [Character] at the end of the frame.

##### Declaration

```
protected Coroutine checkDivergedAtEndOfFrameRoutine
```

#### collisionResolutionMovement

Movement to apply to [Character] to resolve collisions.

##### Declaration

```
protected static readonly Vector3 collisionResolutionMovement
```

#### doSnapToSource

Whether to snap the dependents to the Facade.Source without any divergent checking.

##### Declaration

```
protected bool doSnapToSource
```

#### ignoredColliders

The colliders to ignore body collisions with.

##### Declaration

```
protected readonly HashSet<Collider> ignoredColliders
```

#### ignoreInteractorCollisions

Stores the routine for ignoring Interactor collisions.

##### Declaration

```
protected Coroutine ignoreInteractorCollisions
```

#### offsetObjectFollower

An optional follower of [Offset].

##### Declaration

```
protected ObjectFollower offsetObjectFollower
```

#### preventMutateActions

A collection of actions to perform on each TransformPositionMutator to prevent mutations from occurring.

##### Declaration

```
protected Dictionary<TransformPositionMutator, UnityAction> preventMutateActions
```

#### previousCharacterControllerPosition

The previous position for the [Character].

##### Declaration

```
protected Vector3 previousCharacterControllerPosition
```

#### previousOffsetPosition

The previous position of the Facade.Offset.

##### Declaration

```
protected Vector3 previousOffsetPosition
```

#### previousRigidbodyPosition

The previous position of [PhysicsBody].

##### Declaration

```
protected Vector3 previousRigidbodyPosition
```

#### restoreColliders

The colliders to restore after an ungrab.

##### Declaration

```
protected readonly HashSet<Collider> restoreColliders
```

#### rigidbodySetFrameCount

The frame count of the last time [Interest] was set to [Rigidbody] or [RigidbodyUntilGrounded].

##### Declaration

```
protected int rigidbodySetFrameCount
```

#### smoothDampVelocity

A reference to output any smooth damp velocity to.

##### Declaration

```
protected Vector3 smoothDampVelocity
```

#### sourceObjectFollower

An optional follower of [Source].

##### Declaration

```
protected ObjectFollower sourceObjectFollower
```

#### wasCharacterControllerGrounded

Whether [Character] was grounded previously.

##### Declaration

```
protected bool? wasCharacterControllerGrounded
```

#### wasDiverged

Whether Facade.Source was previously diverged from the [Character].

##### Declaration

```
protected bool wasDiverged
```

### Properties

#### AliasMovementDuration

The duration to smooth damp the alias Source and Offset movement by.

##### Declaration

```
public float AliasMovementDuration { get; set; }
```

#### Character

The CharacterController that acts as the main representation of the body.

##### Declaration

```
public CharacterController Character { get; set; }
```

#### CharacterCenter

The center of the [Character].

##### Declaration

```
protected Vector3 CharacterCenter { get; }
```

#### CollisionMovementDuration

The duration to smooth damp the collision resolution movement by.

##### Declaration

```
public float CollisionMovementDuration { get; set; }
```

#### CollisionsToIgnore

A CollisionIgnorer to manage ignoring collisions with the PseudoBody colliders.

##### Declaration

```
public CollisionIgnorer CollisionsToIgnore { get; set; }
```

#### CurrentDivergenceState

The current divergence state of the pseudo body.

##### Declaration

```
public virtual PseudoBodyProcessor.DivergenceState CurrentDivergenceState { get; }
```

#### Facade

The public interface facade.

##### Declaration

```
public PseudoBodyFacade Facade { get; set; }
```

#### Interest

The object that defines the main source of truth for movement.

##### Declaration

```
public PseudoBodyProcessor.MovementInterest Interest { get; set; }
```

#### IsCharacterControllerGrounded

Whether [Character] touches ground.

##### Declaration

```
public virtual bool IsCharacterControllerGrounded { get; }
```

#### IsDiverged

Whether Facade.Source has diverged from the [Character].

##### Declaration

```
public virtual bool IsDiverged { get; protected set; }
```

#### PhysicsBody

The Rigidbody that acts as the physical representation of the body.

##### Declaration

```
public Rigidbody PhysicsBody { get; set; }
```

#### RigidbodyCollider

The CapsuleCollider that acts as the physical collider representation of the body.

##### Declaration

```
public CapsuleCollider RigidbodyCollider { get; set; }
```

#### UpdateSourcePosition

Whether the processor should update the Facade.Source position.

##### Declaration

```
public bool UpdateSourcePosition { get; set; }
```

### Methods

#### AddPositionMutator(GameObject)

Adds a found TransformPositionMutator found in the given GameObject.

##### Declaration

```
public virtual void AddPositionMutator(GameObject mutatorContainer)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| GameObject | mutatorContainer | The container to look for the Position Mutator in. |

#### Awake()

##### Declaration

```
protected virtual void Awake()
```

#### CheckDivergence()

Check to see if the Facade.Source has diverged or converged with the [Character].

##### Declaration

```
protected virtual void CheckDivergence()
```

#### CheckDivergenceAtEndOfFrame()

Check to see if the Facade.Source is still diverged with the [Character] at the end of the frame.

##### Declaration

```
protected virtual IEnumerator CheckDivergenceAtEndOfFrame()
```

##### Returns

| Type | Description |
| --- | --- |
| System.Collections.IEnumerator | An Enumerator to manage the running of the Coroutine. |

#### CheckForSurroundingCollisions(Vector3, Single)

Checks for any collisions in the surrounding area.

##### Declaration

```
protected virtual bool CheckForSurroundingCollisions(Vector3 center, float radius)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| Vector3 | center | The center point to start the spherecast check from. |
| System.Single | radius | The radius to perform the spherecast check to. |

##### Returns

| Type | Description |
| --- | --- |
| System.Boolean | Whether any collisions have occcured. |

#### CheckIfCharacterControllerIsGrounded()

Checks whether [Character] is grounded.

##### Declaration

```
protected virtual bool CheckIfCharacterControllerIsGrounded()
```

##### Returns

| Type | Description |
| --- | --- |
| System.Boolean | Whether [Character] is grounded. |

##### Remarks

CharacterController.isGrounded isn't accurate so this method does an additional check using Physics.

#### CheckWillDiverge(Vector3)

Checks to see if the given position will cause a divergence between the Facade.Source and the Facade.Offset to the [Character].

##### Declaration

```
public virtual bool CheckWillDiverge(Vector3 targetPosition)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| Vector3 | targetPosition | The new position to check for. |

##### Returns

| Type | Description |
| --- | --- |
| System.Boolean | Whether a divergence will occur. |

#### ConfigureCharacterRadius()

Configures the character controller and capsule collider radius based on the facade settings.

##### Declaration

```
public virtual void ConfigureCharacterRadius()
```

#### ConfigureOffsetObjectFollower()

Configures the offset object follower based on the facade settings.

##### Declaration

```
public virtual void ConfigureOffsetObjectFollower()
```

#### ConfigureSourceObjectFollower()

Configures the source object follower based on the facade settings.

##### Declaration

```
public virtual void ConfigureSourceObjectFollower()
```

#### DoCheckWillDiverge(Vector3)

Checks to see if the given position will cause a divergence between the Facade.Source and the Facade.Offset to the [Character].

##### Declaration

```
public virtual void DoCheckWillDiverge(Vector3 targetPosition)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| Vector3 | targetPosition | The new position to check for. |

#### EmitIsGroundedChangedEvent(Boolean)

Emits BodyRepresentationFacade.BecameGrounded or [BecameAirborne].

##### Declaration

```
protected virtual void EmitIsGroundedChangedEvent(bool isCharacterControllerGrounded)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| System.Boolean | isCharacterControllerGrounded | The current state. |

#### GetCharacterPosition(Vector3, out Single)

Gets the position of the [Character].

##### Declaration

```
protected virtual Vector3 GetCharacterPosition(Vector3 sourcePosition, out float height)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| Vector3 | sourcePosition | The given position of the source. |
| System.Single | height | The calculated height of the Character. |

##### Returns

| Type | Description |
| --- | --- |
| Vector3 | The world position of the Character. |

#### GetDivergenceState()

Determines the divergence state of the pseudo body.

##### Declaration

```
protected virtual PseudoBodyProcessor.DivergenceState GetDivergenceState()
```

##### Returns

| Type | Description |
| --- | --- |
| [PseudoBodyProcessor.DivergenceState] | The divergence state. |

#### GetGameObjectListFromInteractorFacadeList(IReadOnlyList<InteractorFacade>)

Converts the InteractorFacade collection to a GameObject collection.

##### Declaration

```
protected virtual IReadOnlyList<GameObject> GetGameObjectListFromInteractorFacadeList(IReadOnlyList<InteractorFacade> interactorList)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| System.Collections.Generic.IReadOnlyList<InteractorFacade\> | interactorList | The list to convert. |

##### Returns

| Type | Description |
| --- | --- |
| System.Collections.Generic.IReadOnlyList<GameObject\> | The converted list. |

#### IgnoreInteractorGrabbedCollision(InteractableFacade)

Ignores the Interactable grabbed by the Interactor.

##### Declaration

```
protected virtual void IgnoreInteractorGrabbedCollision(InteractableFacade interactable)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| InteractableFacade | interactable | The Interactable to ignore. |

#### IgnoreInteractorsCollisions(GameObject)

Ignores all collisions between any found Interactor and this PsuedoBody.

##### Declaration

```
public virtual void IgnoreInteractorsCollisions(GameObject ignoredObject)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| GameObject | ignoredObject | The object to ignore. |

#### IgnoreInteractorsCollisions(InteractorFacade)

Ignores all of the colliders on the Interactor collection.

##### Declaration

```
[Obsolete("Add `InteractorFacade.gameObject` to `PseudoBodyProcessor.CollisionsToIgnore.Targets` instead.")]
public virtual void IgnoreInteractorsCollisions(InteractorFacade interactor)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| InteractorFacade | interactor | n/a |

#### MatchCharacterControllerWithSource(Vector3, Boolean)

Changes the height and position of [Character] to match [Source].

##### Declaration

```
protected virtual void MatchCharacterControllerWithSource(Vector3 targetPosition, bool setPositionDirectly)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| Vector3 | targetPosition | The position to update to. |
| System.Boolean | setPositionDirectly | Whether to set the position directly or tell [Character] to move to it. |

#### MatchRigidbodyAndColliderWithCharacterController()

Changes [RigidbodyCollider] to match the collider settings of [Character] and moves [PhysicsBody] to match [Character].

##### Declaration

```
protected virtual void MatchRigidbodyAndColliderWithCharacterController()
```

#### OnAfterInterestChange()

Called after [Interest] has been changed.

##### Declaration

```
protected virtual void OnAfterInterestChange()
```

#### OnDisable()

##### Declaration

```
protected virtual void OnDisable()
```

#### OnEnable()

##### Declaration

```
protected virtual void OnEnable()
```

#### Process()

Positions, sizes and controls all variables necessary to make a body representation follow the given [Source].

##### Declaration

```
public virtual void Process()
```

#### ProcessObjectFollowers()

Processes the object followers for the Facade.Source and Facade.Offset.

##### Declaration

```
protected virtual void ProcessObjectFollowers()
```

#### RememberCurrentPositions()

Updates the previous position variables to remember the current state.

##### Declaration

```
protected virtual void RememberCurrentPositions()
```

#### RemovePositionMutator(GameObject)

Removes a found TransformPositionMutator found in the given GameObject.

##### Declaration

```
public virtual void RemovePositionMutator(GameObject mutatorContainer)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| GameObject | mutatorContainer | The container to look for the Position Mutator in. |

#### ResolveDivergence()

Resolves any divergence between the [Character] position and the actual position of the Facade.Source and Facade.Offset.

##### Declaration

```
public virtual void ResolveDivergence()
```

#### ResolveDivergence(Vector3)

Resolves any divergence between the [Character] position and the actual position of the Facade.Source and Facade.Offset.

##### Declaration

```
public virtual void ResolveDivergence(Vector3 divergedPosition)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| Vector3 | divergedPosition | The diverged position. |

#### ResumeInteractorsCollisions(GameObject)

Resumes all collisions between any found Interactor and this PsuedoBody.

##### Declaration

```
public virtual void ResumeInteractorsCollisions(GameObject ignoredObject)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| GameObject | ignoredObject | The object being ignored. |

#### ResumeInteractorsCollisions(InteractorFacade)

Resumes all of the colliders on the Interactor collection.

##### Declaration

```
[Obsolete("Remove `InteractorFacade.gameObject` to `PseudoBodyProcessor.CollisionsToIgnore.Targets` instead.")]
public virtual void ResumeInteractorsCollisions(InteractorFacade interactor)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| InteractorFacade | interactor | n/a |

#### ResumeInteractorUngrabbedCollision(InteractableFacade)

Resumes the Interactable ungrabbed by the Interactor.

##### Declaration

```
protected virtual void ResumeInteractorUngrabbedCollision(InteractableFacade interactable)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| InteractableFacade | interactable | The Interactable to resume. |

#### SnapDependentsToSource()

Snaps the CharacterController and the [PhysicsBody] to the Facade.Source.

##### Declaration

```
protected virtual void SnapDependentsToSource()
```

#### SnapToSource()

Snaps the [Character] to the Facade.Source position.

##### Declaration

```
public virtual void SnapToSource()
```

#### SolveBodyCollisions()

Solves body collisions by not moving the body in case it can't go to its current position.

##### Declaration

```
public virtual void SolveBodyCollisions()
```

##### Remarks

If body collisions should be prevented this method needs to be called right before or right after applying any form of movement to the body.

#### StopCheckDivergenceAtEndOfFrameRoutine()

Stops the divergence check coroutine from running.

##### Declaration

```
protected virtual void StopCheckDivergenceAtEndOfFrameRoutine()
```

#### TryGetMutator(GameObject, out TransformPositionMutator)

Attempts to get the TransformPositionMutator component nested within the given GameObject.

##### Declaration

```
protected virtual bool TryGetMutator(GameObject mutatorContainer, out TransformPositionMutator mutator)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| GameObject | mutatorContainer | The container to look for the component in. |
| TransformPositionMutator | mutator | The found mutator. |

##### Returns

| Type | Description |
| --- | --- |
| System.Boolean | Whether a mutator has been found. |

#### UpdateAliasForCollision()

Updates the alias targets to resolve any collisions.

##### Declaration

```
protected virtual void UpdateAliasForCollision()
```

#### UpdateAliasForDivergence()

Updates the alias targets to resolve any divergence.

##### Declaration

```
protected virtual void UpdateAliasForDivergence()
```

#### UpdateAliasForNonCharacterControllerInterest()

Updates the alias targets for when the [PseudoBodyProcessor.MovementInterest] is not of type CharacterController.

##### Declaration

```
protected virtual void UpdateAliasForNonCharacterControllerInterest()
```

#### UpdateAliasForRigidbodyControllerInterest(out Vector3)

Updates the alias targets for when the [PseudoBodyProcessor.MovementInterest] is of a Rigidbody type.

##### Declaration

```
protected virtual Vector3 UpdateAliasForRigidbodyControllerInterest(out Vector3 rigidbodyPhysicsMovement)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| Vector3 | rigidbodyPhysicsMovement | The calculated Rigidbody movement position. |

##### Returns

| Type | Description |
| --- | --- |
| Vector3 | The current [Character] movement position. |

#### UpdateAliasForVerticalMovement(Boolean, Vector3)

Updates the alias targets for any vertical movement.

##### Declaration

```
protected virtual void UpdateAliasForVerticalMovement(bool isGrounded, Vector3 characterControllerSourceMovement)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| System.Boolean | isGrounded | Whether the controller is touching the ground. |
| Vector3 | characterControllerSourceMovement | The calculated [Character] movement position. |

#### UpdateAliasPosition(Vector3, Vector3, Boolean, Boolean, Boolean, Single)

Updates the position of the alias objects.

##### Declaration

```
protected virtual void UpdateAliasPosition(Vector3 newOffsetPosition, Vector3 newSourcePosition, bool incrementOffset, bool incrementSource, bool ignoreSourcePosition, float dampDuration)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| Vector3 | newOffsetPosition | The new position for the Facade.Offset. |
| Vector3 | newSourcePosition | The new position for the Facade.Source. |
| System.Boolean | incrementOffset | Whether to increment the Facade.Offset position or set it to a new value. |
| System.Boolean | incrementSource | Whether to increment the Facade.Source position or set it to a new value. |
| System.Boolean | ignoreSourcePosition | Whether to ignore setting the Facade.Source position. |
| System.Single | dampDuration | The duration to dampen the movemnt of the position updates. |

#### UpdateInterestType(Boolean, Vector3)

Updates the [PseudoBodyProcessor.MovementInterest] type based on whether the controller is grounded or not.

##### Declaration

```
protected virtual void UpdateInterestType(bool isGrounded, Vector3 rigidbodyPhysicsMovement)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| System.Boolean | isGrounded | Whether the controller is touching the ground. |
| Vector3 | rigidbodyPhysicsMovement | The calculated Rigidbody movement position. |

#### WillDiverge(Vector3, Vector3)

Determines whether the given position will diverge from the [Character] position.

##### Declaration

```
protected virtual bool WillDiverge(Vector3 targetPosition, Vector3 divergenceThreshold)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| Vector3 | targetPosition | The position to check. |
| Vector3 | divergenceThreshold | The threshold in which to consider a divergence has occurred. |

##### Returns

| Type | Description |
| --- | --- |
| System.Boolean | Whether there was a divergence between the two positions. |

### Implements

IProcessable

[Tilia.Trackers.PseudoBody]: README.md
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[Offset]: PseudoBodyFacade.md#Tilia_Trackers_PseudoBody_PseudoBodyFacade_Offset
[Character]: PseudoBodyProcessor.md#Character
[PhysicsBody]: PseudoBodyProcessor.md#PhysicsBody
[Interest]: PseudoBodyProcessor.md#Interest
[Rigidbody]: PseudoBodyProcessor.MovementInterest.md#MovementInterest_Rigidbody
[RigidbodyUntilGrounded]: PseudoBodyProcessor.MovementInterest.md#MovementInterest_RigidbodyUntilGrounded
[Source]: PseudoBodyFacade.md#Tilia_Trackers_PseudoBody_PseudoBodyFacade_Source
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[PseudoBodyFacade]: PseudoBodyFacade.md
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[BecameAirborne]: PseudoBodyFacade.md#Tilia_Trackers_PseudoBody_PseudoBodyFacade_BecameAirborne
[Character]: PseudoBodyProcessor.md#Character
[PseudoBodyProcessor.DivergenceState]: PseudoBodyProcessor.DivergenceState.md
[Character]: PseudoBodyProcessor.md#Character
[Source]: PseudoBodyFacade.md#Tilia_Trackers_PseudoBody_PseudoBodyFacade_Source
[Character]: PseudoBodyProcessor.md#Character
[RigidbodyCollider]: PseudoBodyProcessor.md#RigidbodyCollider
[Character]: PseudoBodyProcessor.md#Character
[PhysicsBody]: PseudoBodyProcessor.md#PhysicsBody
[Character]: PseudoBodyProcessor.md#Character
[Interest]: PseudoBodyProcessor.md#Interest
[Source]: PseudoBodyFacade.md#Tilia_Trackers_PseudoBody_PseudoBodyFacade_Source
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[PhysicsBody]: PseudoBodyProcessor.md#PhysicsBody
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[PseudoBodyProcessor.MovementInterest]: PseudoBodyProcessor.MovementInterest.md
[Character]: PseudoBodyProcessor.md#Character
[Inheritance]: #Inheritance
[Namespace]: #Namespace
[Syntax]: #Syntax
[Fields]: #Fields
[allowMutateActions]: #allowMutateActions
[checkDivergedAtEndOfFrameRoutine]: #checkDivergedAtEndOfFrameRoutine
[collisionResolutionMovement]: #collisionResolutionMovement
[doSnapToSource]: #doSnapToSource
[ignoredColliders]: #ignoredColliders
[ignoreInteractorCollisions]: #ignoreInteractorCollisions
[offsetObjectFollower]: #offsetObjectFollower
[preventMutateActions]: #preventMutateActions
[previousCharacterControllerPosition]: #previousCharacterControllerPosition
[previousOffsetPosition]: #previousOffsetPosition
[previousRigidbodyPosition]: #previousRigidbodyPosition
[restoreColliders]: #restoreColliders
[rigidbodySetFrameCount]: #rigidbodySetFrameCount
[smoothDampVelocity]: #smoothDampVelocity
[sourceObjectFollower]: #sourceObjectFollower
[wasCharacterControllerGrounded]: #wasCharacterControllerGrounded
[wasDiverged]: #wasDiverged
[Properties]: #Properties
[AliasMovementDuration]: #AliasMovementDuration
[Character]: #Character
[CharacterCenter]: #CharacterCenter
[CollisionMovementDuration]: #CollisionMovementDuration
[CollisionsToIgnore]: #CollisionsToIgnore
[CurrentDivergenceState]: #CurrentDivergenceState
[Facade]: #Facade
[Interest]: #Interest
[IsCharacterControllerGrounded]: #IsCharacterControllerGrounded
[IsDiverged]: #IsDiverged
[PhysicsBody]: #PhysicsBody
[RigidbodyCollider]: #RigidbodyCollider
[UpdateSourcePosition]: #UpdateSourcePosition
[Methods]: #Methods
[AddPositionMutator(GameObject)]: #AddPositionMutatorGameObject
[Awake()]: #Awake
[CheckDivergence()]: #CheckDivergence
[CheckDivergenceAtEndOfFrame()]: #CheckDivergenceAtEndOfFrame
[CheckForSurroundingCollisions(Vector3, Single)]: #CheckForSurroundingCollisionsVector3-Single
[CheckIfCharacterControllerIsGrounded()]: #CheckIfCharacterControllerIsGrounded
[CheckWillDiverge(Vector3)]: #CheckWillDivergeVector3
[ConfigureCharacterRadius()]: #ConfigureCharacterRadius
[ConfigureOffsetObjectFollower()]: #ConfigureOffsetObjectFollower
[ConfigureSourceObjectFollower()]: #ConfigureSourceObjectFollower
[DoCheckWillDiverge(Vector3)]: #DoCheckWillDivergeVector3
[EmitIsGroundedChangedEvent(Boolean)]: #EmitIsGroundedChangedEventBoolean
[GetCharacterPosition(Vector3, out Single)]: #GetCharacterPositionVector3-out Single
[GetDivergenceState()]: #GetDivergenceState
[GetGameObjectListFromInteractorFacadeList(IReadOnlyList<InteractorFacade>)]: #GetGameObjectListFromInteractorFacadeListIReadOnlyList<InteractorFacade>
[IgnoreInteractorGrabbedCollision(InteractableFacade)]: #IgnoreInteractorGrabbedCollisionInteractableFacade
[IgnoreInteractorsCollisions(GameObject)]: #IgnoreInteractorsCollisionsGameObject
[IgnoreInteractorsCollisions(InteractorFacade)]: #IgnoreInteractorsCollisionsInteractorFacade
[MatchCharacterControllerWithSource(Vector3, Boolean)]: #MatchCharacterControllerWithSourceVector3-Boolean
[MatchRigidbodyAndColliderWithCharacterController()]: #MatchRigidbodyAndColliderWithCharacterController
[OnAfterInterestChange()]: #OnAfterInterestChange
[OnDisable()]: #OnDisable
[OnEnable()]: #OnEnable
[Process()]: #Process
[ProcessObjectFollowers()]: #ProcessObjectFollowers
[RememberCurrentPositions()]: #RememberCurrentPositions
[RemovePositionMutator(GameObject)]: #RemovePositionMutatorGameObject
[ResolveDivergence()]: #ResolveDivergence
[ResolveDivergence(Vector3)]: #ResolveDivergenceVector3
[ResumeInteractorsCollisions(GameObject)]: #ResumeInteractorsCollisionsGameObject
[ResumeInteractorsCollisions(InteractorFacade)]: #ResumeInteractorsCollisionsInteractorFacade
[ResumeInteractorUngrabbedCollision(InteractableFacade)]: #ResumeInteractorUngrabbedCollisionInteractableFacade
[SnapDependentsToSource()]: #SnapDependentsToSource
[SnapToSource()]: #SnapToSource
[SolveBodyCollisions()]: #SolveBodyCollisions
[StopCheckDivergenceAtEndOfFrameRoutine()]: #StopCheckDivergenceAtEndOfFrameRoutine
[TryGetMutator(GameObject, out TransformPositionMutator)]: #TryGetMutatorGameObject-out TransformPositionMutator
[UpdateAliasForCollision()]: #UpdateAliasForCollision
[UpdateAliasForDivergence()]: #UpdateAliasForDivergence
[UpdateAliasForNonCharacterControllerInterest()]: #UpdateAliasForNonCharacterControllerInterest
[UpdateAliasForRigidbodyControllerInterest(out Vector3)]: #UpdateAliasForRigidbodyControllerInterestout Vector3
[UpdateAliasForVerticalMovement(Boolean, Vector3)]: #UpdateAliasForVerticalMovementBoolean-Vector3
[UpdateAliasPosition(Vector3, Vector3, Boolean, Boolean, Boolean, Single)]: #UpdateAliasPositionVector3-Vector3-Boolean-Boolean-Boolean-Single
[UpdateInterestType(Boolean, Vector3)]: #UpdateInterestTypeBoolean-Vector3
[WillDiverge(Vector3, Vector3)]: #WillDivergeVector3-Vector3
[Implements]: #Implements
