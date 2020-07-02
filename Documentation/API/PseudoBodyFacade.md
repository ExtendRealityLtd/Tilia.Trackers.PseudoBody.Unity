# Class PseudoBodyFacade

The public interface for the PseudoBody prefab.

## Contents

* [Inheritance]
* [Namespace]
* [Syntax]
* [Fields]
  * [BecameAirborne]
  * [BecameGrounded]
  * [Converged]
  * [Diverged]
* [Properties]
  * [IgnoredInteractors]
  * [Interest]
  * [IsCharacterControllerGrounded]
  * [IsDiverged]
  * [Offset]
  * [PhysicsBody]
  * [Processor]
  * [Source]
  * [SourceDivergenceThreshold]
  * [SourceThickness]
* [Methods]
  * [ListenToRigidbodyMovement()]
  * [OnAfterIgnoredInteractorsChange()]
  * [OnAfterOffsetChange()]
  * [OnAfterSourceChange()]
  * [OnBeforeIgnoredInteractorsChange()]
  * [OnDisable()]
  * [OnEnable()]
  * [OnIgnoredInteractorAdded(InteractorFacade)]
  * [OnIgnoredInteractorRemoved(InteractorFacade)]
  * [SolveBodyCollisions()]

## Details

##### Inheritance

* System.Object
* PseudoBodyFacade

##### Namespace

* [Tilia.Trackers.PseudoBody]

##### Syntax

```
public class PseudoBodyFacade : MonoBehaviour
```

### Fields

#### BecameAirborne

Emitted when the body stops touching ground.

##### Declaration

```
public UnityEvent BecameAirborne
```

#### BecameGrounded

Emitted when the body starts touching ground.

##### Declaration

```
public UnityEvent BecameGrounded
```

#### Converged

Emitted when the pseudo body is back within the threshold distance of the Source. after being diverged.

##### Declaration

```
public UnityEvent Converged
```

#### Diverged

Emitted when the pseudo body starts no longer being within the threshold distance of the Source..

##### Declaration

```
public UnityEvent Diverged
```

### Properties

#### IgnoredInteractors

A collection of interactors to exclude from physics collision checks.

##### Declaration

```
public InteractorFacadeObservableList IgnoredInteractors { get; set; }
```

#### Interest

The object that defines the main source of truth for movement.

##### Declaration

```
public PseudoBodyProcessor.MovementInterest Interest { get; set; }
```

#### IsCharacterControllerGrounded

Whether the body touches ground.

##### Declaration

```
public bool IsCharacterControllerGrounded { get; }
```

#### IsDiverged

Whether the [Source] has diverged from the [Character].

##### Declaration

```
public bool IsDiverged { get; }
```

#### Offset

An optional offset for the [Source] to use.

##### Declaration

```
public GameObject Offset { get; set; }
```

#### PhysicsBody

The Rigidbody that acts as the physical representation of the body.

##### Declaration

```
public Rigidbody PhysicsBody { get; }
```

#### Processor

The linked Internal Setup.

##### Declaration

```
public PseudoBodyProcessor Processor { get; protected set; }
```

#### Source

The object to follow.

##### Declaration

```
public GameObject Source { get; set; }
```

#### SourceDivergenceThreshold

The distance the pseudo body has to be away from the [Source] to be considered diverged.

##### Declaration

```
public Vector3 SourceDivergenceThreshold { get; set; }
```

#### SourceThickness

The thickness of [Source] to be used when resolving body collisions.

##### Declaration

```
public float SourceThickness { get; set; }
```

### Methods

#### ListenToRigidbodyMovement()

Sets the source of truth for movement to come from [PhysicsBody] until [Character] hits the ground, then [Character] is the new source of truth.

##### Declaration

```
public virtual void ListenToRigidbodyMovement()
```

##### Remarks

This method needs to be called right before or right after applying any form of movement to the rigidbody while the body is grounded, i.e. in the same frame and before [Process()] is called.

#### OnAfterIgnoredInteractorsChange()

Called after [IgnoredInteractors] has been changed.

##### Declaration

```
protected virtual void OnAfterIgnoredInteractorsChange()
```

#### OnAfterOffsetChange()

Called after [Offset] has been changed.

##### Declaration

```
protected virtual void OnAfterOffsetChange()
```

#### OnAfterSourceChange()

Called after [Source] has been changed.

##### Declaration

```
protected virtual void OnAfterSourceChange()
```

#### OnBeforeIgnoredInteractorsChange()

Called after [IgnoredInteractors] has been changed.

##### Declaration

```
protected virtual void OnBeforeIgnoredInteractorsChange()
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

#### OnIgnoredInteractorAdded(InteractorFacade)

Processes when a new InteractorFacade is added to the ignored collection.

##### Declaration

```
protected virtual void OnIgnoredInteractorAdded(InteractorFacade interactor)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| InteractorFacade | interactor | The interactor to ignore collisions from. |

#### OnIgnoredInteractorRemoved(InteractorFacade)

Processes when a new InteractorFacade is removed from the ignored collection.

##### Declaration

```
protected virtual void OnIgnoredInteractorRemoved(InteractorFacade interactor)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| InteractorFacade | interactor | The interactor to resume collisions with. |

#### SolveBodyCollisions()

Solves body collisions by not moving the body in case it can't go to its current position.

##### Declaration

```
public virtual void SolveBodyCollisions()
```

##### Remarks

If body collisions should be prevented this method needs to be called right before or right after applying any form of movement to the body.

[Tilia.Trackers.PseudoBody]: README.md
[PseudoBodyProcessor.MovementInterest]: PseudoBodyProcessor.MovementInterest.md
[Source]: PseudoBodyFacade.md#Source
[Character]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_Character
[Source]: PseudoBodyFacade.md#Source
[PseudoBodyProcessor]: PseudoBodyProcessor.md
[Source]: PseudoBodyFacade.md#Source
[Source]: PseudoBodyFacade.md#Source
[PhysicsBody]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_PhysicsBody
[Character]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_Character
[Character]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_Character
[Process()]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_Process
[IgnoredInteractors]: PseudoBodyFacade.md#IgnoredInteractors
[Offset]: PseudoBodyFacade.md#Offset
[Source]: PseudoBodyFacade.md#Source
[IgnoredInteractors]: PseudoBodyFacade.md#IgnoredInteractors
[Inheritance]: #Inheritance
[Namespace]: #Namespace
[Syntax]: #Syntax
[Fields]: #Fields
[BecameAirborne]: #BecameAirborne
[BecameGrounded]: #BecameGrounded
[Converged]: #Converged
[Diverged]: #Diverged
[Properties]: #Properties
[IgnoredInteractors]: #IgnoredInteractors
[Interest]: #Interest
[IsCharacterControllerGrounded]: #IsCharacterControllerGrounded
[IsDiverged]: #IsDiverged
[Offset]: #Offset
[PhysicsBody]: #PhysicsBody
[Processor]: #Processor
[Source]: #Source
[SourceDivergenceThreshold]: #SourceDivergenceThreshold
[SourceThickness]: #SourceThickness
[Methods]: #Methods
[ListenToRigidbodyMovement()]: #ListenToRigidbodyMovement
[OnAfterIgnoredInteractorsChange()]: #OnAfterIgnoredInteractorsChange
[OnAfterOffsetChange()]: #OnAfterOffsetChange
[OnAfterSourceChange()]: #OnAfterSourceChange
[OnBeforeIgnoredInteractorsChange()]: #OnBeforeIgnoredInteractorsChange
[OnDisable()]: #OnDisable
[OnEnable()]: #OnEnable
[OnIgnoredInteractorAdded(InteractorFacade)]: #OnIgnoredInteractorAddedInteractorFacade
[OnIgnoredInteractorRemoved(InteractorFacade)]: #OnIgnoredInteractorRemovedInteractorFacade
[SolveBodyCollisions()]: #SolveBodyCollisions
