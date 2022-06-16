# Class PseudoBodyProcessor

Sets up the PseudoBody prefab based on the provided user settings and implements the logic to represent a body.

## Contents

* [Inheritance]
* [Namespace]
* [Syntax]
* [Fields]
  * [checkDivergedAtEndOfFrameRoutine]
  * [collisionResolutionMovement]
  * [doSnapToSource]
  * [ignoredColliders]
  * [ignoreInteractorCollisions]
  * [offsetObjectFollower]
  * [previousRigidbodyPosition]
  * [restoreColliders]
  * [rigidbodySetFrameCount]
  * [sourceObjectFollower]
  * [wasCharacterControllerGrounded]
  * [wasDiverged]
* [Properties]
  * [Character]
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
  * [Awake()]
  * [CheckDivergence()]
  * [CheckDivergenceAtEndOfFrame()]
  * [CheckIfCharacterControllerIsGrounded()]
  * [ConfigureOffsetObjectFollower()]
  * [ConfigureSourceObjectFollower()]
  * [EmitIsGroundedChangedEvent(Boolean)]
  * [GetDivergenceState()]
  * [GetGameObjectListFromInteractorFacadeList(IReadOnlyList<InteractorFacade>)]
  * [IgnoreInteractorGrabbedCollision(InteractableFacade)]
  * [IgnoreInteractorsCollisions(GameObject)]
  * [IgnoreInteractorsCollisions(InteractorFacade)]
  * [MatchCharacterControllerWithSource(Boolean)]
  * [MatchRigidbodyAndColliderWithCharacterController()]
  * [OnAfterInterestChange()]
  * [OnDisable()]
  * [OnEnable()]
  * [Process()]
  * [RememberCurrentPositions()]
  * [ResumeInteractorsCollisions(GameObject)]
  * [ResumeInteractorsCollisions(InteractorFacade)]
  * [ResumeInteractorUngrabbedCollision(InteractableFacade)]
  * [SnapDependentsToSource()]
  * [SnapToSource()]
  * [SolveBodyCollisions()]
  * [StopCheckDivergenceAtEndOfFrameRoutine()]
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

#### Character

The CharacterController that acts as the main representation of the body.

##### Declaration

```
public CharacterController Character { get; protected set; }
```

#### CollisionsToIgnore

A CollisionIgnorer to manage ignoring collisions with the PseudoBody colliders.

##### Declaration

```
public CollisionIgnorer CollisionsToIgnore { get; protected set; }
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
public PseudoBodyFacade Facade { get; protected set; }
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
public Rigidbody PhysicsBody { get; protected set; }
```

#### RigidbodyCollider

The CapsuleCollider that acts as the physical collider representation of the body.

##### Declaration

```
public CapsuleCollider RigidbodyCollider { get; protected set; }
```

#### UpdateSourcePosition

Whether the processor should update the Facade.Source position.

##### Declaration

```
public bool UpdateSourcePosition { get; set; }
```

### Methods

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

#### MatchCharacterControllerWithSource(Boolean)

Changes the height and position of [Character] to match [Source].

##### Declaration

```
protected virtual void MatchCharacterControllerWithSource(bool setPositionDirectly)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
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

#### RememberCurrentPositions()

Updates the previous position variables to remember the current state.

##### Declaration

```
protected virtual void RememberCurrentPositions()
```

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

### Implements

IProcessable

[Tilia.Trackers.PseudoBody]: README.md
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[Offset]: PseudoBodyFacade.md#Tilia_Trackers_PseudoBody_PseudoBodyFacade_Offset
[PhysicsBody]: PseudoBodyProcessor.md#PhysicsBody
[Interest]: PseudoBodyProcessor.md#Interest
[Rigidbody]: PseudoBodyProcessor.MovementInterest.md#MovementInterest_Rigidbody
[RigidbodyUntilGrounded]: PseudoBodyProcessor.MovementInterest.md#MovementInterest_RigidbodyUntilGrounded
[Source]: PseudoBodyFacade.md#Tilia_Trackers_PseudoBody_PseudoBodyFacade_Source
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[PseudoBodyFacade]: PseudoBodyFacade.md
[PseudoBodyProcessor.MovementInterest]: PseudoBodyProcessor.MovementInterest.md
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[Character]: PseudoBodyProcessor.md#Character
[BecameAirborne]: PseudoBodyFacade.md#Tilia_Trackers_PseudoBody_PseudoBodyFacade_BecameAirborne
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
[PhysicsBody]: PseudoBodyProcessor.md#PhysicsBody
[Character]: PseudoBodyProcessor.md#Character
[Inheritance]: #Inheritance
[Namespace]: #Namespace
[Syntax]: #Syntax
[Fields]: #Fields
[checkDivergedAtEndOfFrameRoutine]: #checkDivergedAtEndOfFrameRoutine
[collisionResolutionMovement]: #collisionResolutionMovement
[doSnapToSource]: #doSnapToSource
[ignoredColliders]: #ignoredColliders
[ignoreInteractorCollisions]: #ignoreInteractorCollisions
[offsetObjectFollower]: #offsetObjectFollower
[previousRigidbodyPosition]: #previousRigidbodyPosition
[restoreColliders]: #restoreColliders
[rigidbodySetFrameCount]: #rigidbodySetFrameCount
[sourceObjectFollower]: #sourceObjectFollower
[wasCharacterControllerGrounded]: #wasCharacterControllerGrounded
[wasDiverged]: #wasDiverged
[Properties]: #Properties
[Character]: #Character
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
[Awake()]: #Awake
[CheckDivergence()]: #CheckDivergence
[CheckDivergenceAtEndOfFrame()]: #CheckDivergenceAtEndOfFrame
[CheckIfCharacterControllerIsGrounded()]: #CheckIfCharacterControllerIsGrounded
[ConfigureOffsetObjectFollower()]: #ConfigureOffsetObjectFollower
[ConfigureSourceObjectFollower()]: #ConfigureSourceObjectFollower
[EmitIsGroundedChangedEvent(Boolean)]: #EmitIsGroundedChangedEventBoolean
[GetDivergenceState()]: #GetDivergenceState
[GetGameObjectListFromInteractorFacadeList(IReadOnlyList<InteractorFacade>)]: #GetGameObjectListFromInteractorFacadeListIReadOnlyList<InteractorFacade>
[IgnoreInteractorGrabbedCollision(InteractableFacade)]: #IgnoreInteractorGrabbedCollisionInteractableFacade
[IgnoreInteractorsCollisions(GameObject)]: #IgnoreInteractorsCollisionsGameObject
[IgnoreInteractorsCollisions(InteractorFacade)]: #IgnoreInteractorsCollisionsInteractorFacade
[MatchCharacterControllerWithSource(Boolean)]: #MatchCharacterControllerWithSourceBoolean
[MatchRigidbodyAndColliderWithCharacterController()]: #MatchRigidbodyAndColliderWithCharacterController
[OnAfterInterestChange()]: #OnAfterInterestChange
[OnDisable()]: #OnDisable
[OnEnable()]: #OnEnable
[Process()]: #Process
[RememberCurrentPositions()]: #RememberCurrentPositions
[ResumeInteractorsCollisions(GameObject)]: #ResumeInteractorsCollisionsGameObject
[ResumeInteractorsCollisions(InteractorFacade)]: #ResumeInteractorsCollisionsInteractorFacade
[ResumeInteractorUngrabbedCollision(InteractableFacade)]: #ResumeInteractorUngrabbedCollisionInteractableFacade
[SnapDependentsToSource()]: #SnapDependentsToSource
[SnapToSource()]: #SnapToSource
[SolveBodyCollisions()]: #SolveBodyCollisions
[StopCheckDivergenceAtEndOfFrameRoutine()]: #StopCheckDivergenceAtEndOfFrameRoutine
[Implements]: #Implements
