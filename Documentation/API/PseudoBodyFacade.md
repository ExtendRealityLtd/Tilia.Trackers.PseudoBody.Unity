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
  * [StillDiverged]
* [Properties]
  * [IgnoredGameObjects]
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
  * [Awake()]
  * [ClearOffset()]
  * [ClearSource()]
  * [ListenToRigidbodyMovement()]
  * [OnAfterOffsetChange()]
  * [OnAfterSourceChange()]
  * [SetSourceDivergenceThresholdX(Single)]
  * [SetSourceDivergenceThresholdY(Single)]
  * [SetSourceDivergenceThresholdZ(Single)]
  * [SnapToSource()]
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

#### StillDiverged

Emitted when the pseudo body continues no longer being within the threshold distance of the Source. for each subsequent frame.

##### Declaration

```
public UnityEvent StillDiverged
```

### Properties

#### IgnoredGameObjects

A GameObject collection to exclude from physics collision checks.

##### Declaration

```
public GameObjectObservableList IgnoredGameObjects { get; set; }
```

#### IgnoredInteractors

A collection of Interactors to exclude from physics collision checks.

##### Declaration

```
[Obsolete("Use `IgnoredGameObjects` instead.")]
public InteractorFacadeObservableList IgnoredInteractors { get; set; }
```

#### Interest

The object that defines the main source of truth for movement.

##### Declaration

```
public virtual PseudoBodyProcessor.MovementInterest Interest { get; set; }
```

#### IsCharacterControllerGrounded

Whether the body touches ground.

##### Declaration

```
public virtual bool IsCharacterControllerGrounded { get; }
```

#### IsDiverged

Whether the [Source] has diverged from the [Character].

##### Declaration

```
public virtual bool IsDiverged { get; }
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
public virtual Rigidbody PhysicsBody { get; }
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

#### Awake()

##### Declaration

```
protected virtual void Awake()
```

#### ClearOffset()

Clears [Offset].

##### Declaration

```
public virtual void ClearOffset()
```

#### ClearSource()

Clears [Source].

##### Declaration

```
public virtual void ClearSource()
```

#### ListenToRigidbodyMovement()

Sets the source of truth for movement to come from [PhysicsBody] until [Character] hits the ground, then [Character] is the new source of truth.

##### Declaration

```
public virtual void ListenToRigidbodyMovement()
```

##### Remarks

This method needs to be called right before or right after applying any form of movement to the Rigidbody while the body is grounded, i.e. in the same frame and before [Process()] is called.

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

#### SetSourceDivergenceThresholdX(Single)

Sets the [SourceDivergenceThreshold] x value.

##### Declaration

```
public virtual void SetSourceDivergenceThresholdX(float value)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| System.Single | value | The value to set to. |

#### SetSourceDivergenceThresholdY(Single)

Sets the [SourceDivergenceThreshold] y value.

##### Declaration

```
public virtual void SetSourceDivergenceThresholdY(float value)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| System.Single | value | The value to set to. |

#### SetSourceDivergenceThresholdZ(Single)

Sets the [SourceDivergenceThreshold] z value.

##### Declaration

```
public virtual void SetSourceDivergenceThresholdZ(float value)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| System.Single | value | The value to set to. |

#### SnapToSource()

Snaps the Processor.Character to the [Source] position.

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

[Tilia.Trackers.PseudoBody]: README.md
[PseudoBodyProcessor.MovementInterest]: PseudoBodyProcessor.MovementInterest.md
[Source]: PseudoBodyFacade.md#Source
[Character]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_Character
[Source]: PseudoBodyFacade.md#Source
[PseudoBodyProcessor]: PseudoBodyProcessor.md
[Source]: PseudoBodyFacade.md#Source
[Source]: PseudoBodyFacade.md#Source
[Offset]: PseudoBodyFacade.md#Offset
[Source]: PseudoBodyFacade.md#Source
[PhysicsBody]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_PhysicsBody
[Character]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_Character
[Character]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_Character
[Process()]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_Process
[Offset]: PseudoBodyFacade.md#Offset
[Source]: PseudoBodyFacade.md#Source
[SourceDivergenceThreshold]: PseudoBodyFacade.md#SourceDivergenceThreshold
[SourceDivergenceThreshold]: PseudoBodyFacade.md#SourceDivergenceThreshold
[SourceDivergenceThreshold]: PseudoBodyFacade.md#SourceDivergenceThreshold
[Source]: PseudoBodyFacade.md#Source
[Inheritance]: #Inheritance
[Namespace]: #Namespace
[Syntax]: #Syntax
[Fields]: #Fields
[BecameAirborne]: #BecameAirborne
[BecameGrounded]: #BecameGrounded
[Converged]: #Converged
[Diverged]: #Diverged
[StillDiverged]: #StillDiverged
[Properties]: #Properties
[IgnoredGameObjects]: #IgnoredGameObjects
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
[Awake()]: #Awake
[ClearOffset()]: #ClearOffset
[ClearSource()]: #ClearSource
[ListenToRigidbodyMovement()]: #ListenToRigidbodyMovement
[OnAfterOffsetChange()]: #OnAfterOffsetChange
[OnAfterSourceChange()]: #OnAfterSourceChange
[SetSourceDivergenceThresholdX(Single)]: #SetSourceDivergenceThresholdXSingle
[SetSourceDivergenceThresholdY(Single)]: #SetSourceDivergenceThresholdYSingle
[SetSourceDivergenceThresholdZ(Single)]: #SetSourceDivergenceThresholdZSingle
[SnapToSource()]: #SnapToSource
[SolveBodyCollisions()]: #SolveBodyCollisions
