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
  * [Jumped]
  * [StillDiverged]
  * [WillDiverge]
* [Properties]
  * [CharacterRadius]
  * [ExternalPositionMutators]
  * [IgnoredGameObjects]
  * [IgnoredInteractors]
  * [Interest]
  * [IsCharacterControllerGrounded]
  * [IsDiverged]
  * [Offset]
  * [PhysicsBody]
  * [PreventEnterGeometry]
  * [Processor]
  * [Source]
  * [SourceDivergenceThreshold]
  * [SourceThickness]
* [Methods]
  * [Awake()]
  * [CheckWillDiverge(Vector3)]
  * [ClearOffset()]
  * [ClearSource()]
  * [DoCheckWillDiverge(Vector3)]
  * [Jump(Single)]
  * [ListenToRigidbodyMovement()]
  * [OnAfterCharacterRadiusChange()]
  * [OnAfterOffsetChange()]
  * [OnAfterPreventEnterGeometryChange()]
  * [OnAfterSourceChange()]
  * [ResolveDivergence()]
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

#### Jumped

Emitted when a jump is initiated.

##### Declaration

```
public UnityEvent Jumped
```

#### StillDiverged

Emitted when the pseudo body continues no longer being within the threshold distance of the Source. for each subsequent frame.

##### Declaration

```
public UnityEvent StillDiverged
```

#### WillDiverge

Emitted when the pseudo body will become no longer within the threshold distance of the Source. if the updated position is applied.

##### Declaration

```
public UnityEvent WillDiverge
```

### Properties

#### CharacterRadius

The radius of the character controller and capsule collider.

##### Declaration

```
public float CharacterRadius { get; set; }
```

#### ExternalPositionMutators

A GameObject collection of any external controller that contains a Position Mutator that may change the location of the Character. This external Position Mutator will be prevented from causing a divergence of the Character and the Source/Offset.

##### Declaration

```
public GameObjectObservableList ExternalPositionMutators { get; set; }
```

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

#### PreventEnterGeometry

Whether to automatically solve body collisions and push the character controller back and source to a safe space outside of the colliding geometry.

##### Declaration

```
public bool PreventEnterGeometry { get; set; }
```

#### Processor

The linked Internal Setup.

##### Declaration

```
public PseudoBodyProcessor Processor { get; set; }
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

#### CheckWillDiverge(Vector3)

Checks to see if the given position will cause a divergence between the Facade.Source and the Facade.Offset to the Character.

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

#### DoCheckWillDiverge(Vector3)

Checks to see if the given position will cause a divergence between the Facade.Source and the Facade.Offset to the Character.

##### Declaration

```
public virtual void DoCheckWillDiverge(Vector3 targetPosition)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| Vector3 | targetPosition | The new position to check for. |

#### Jump(Single)

Adds force to the [PhysicsBody] to simulate a jump at the given force.

##### Declaration

```
public virtual void Jump(float force)
```

##### Parameters

| Type | Name | Description |
| --- | --- | --- |
| System.Single | force | The force to jump by. |

#### ListenToRigidbodyMovement()

Sets the source of truth for movement to come from [PhysicsBody] until [Character] hits the ground, then [Character] is the new source of truth.

##### Declaration

```
public virtual void ListenToRigidbodyMovement()
```

##### Remarks

This method needs to be called right before or right after applying any form of movement to the Rigidbody while the body is grounded, i.e. in the same frame and before [Process()] is called.

#### OnAfterCharacterRadiusChange()

Called after [CharacterRadius] has been changed.

##### Declaration

```
protected virtual void OnAfterCharacterRadiusChange()
```

#### OnAfterOffsetChange()

Called after [Offset] has been changed.

##### Declaration

```
protected virtual void OnAfterOffsetChange()
```

#### OnAfterPreventEnterGeometryChange()

Called after [PreventEnterGeometry] has been changed.

##### Declaration

```
protected virtual void OnAfterPreventEnterGeometryChange()
```

#### OnAfterSourceChange()

Called after [Source] has been changed.

##### Declaration

```
protected virtual void OnAfterSourceChange()
```

#### ResolveDivergence()

Resolves any divergence between the Character position and the actual position of the Facade.Source and Facade.Offset.

##### Declaration

```
public virtual void ResolveDivergence()
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
[PhysicsBody]: PseudoBodyFacade.md#PhysicsBody
[PhysicsBody]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_PhysicsBody
[Character]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_Character
[Character]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_Character
[Process()]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_Process
[CharacterRadius]: PseudoBodyFacade.md#CharacterRadius
[Offset]: PseudoBodyFacade.md#Offset
[PreventEnterGeometry]: PseudoBodyFacade.md#PreventEnterGeometry
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
[Jumped]: #Jumped
[StillDiverged]: #StillDiverged
[WillDiverge]: #WillDiverge
[Properties]: #Properties
[CharacterRadius]: #CharacterRadius
[ExternalPositionMutators]: #ExternalPositionMutators
[IgnoredGameObjects]: #IgnoredGameObjects
[IgnoredInteractors]: #IgnoredInteractors
[Interest]: #Interest
[IsCharacterControllerGrounded]: #IsCharacterControllerGrounded
[IsDiverged]: #IsDiverged
[Offset]: #Offset
[PhysicsBody]: #PhysicsBody
[PreventEnterGeometry]: #PreventEnterGeometry
[Processor]: #Processor
[Source]: #Source
[SourceDivergenceThreshold]: #SourceDivergenceThreshold
[SourceThickness]: #SourceThickness
[Methods]: #Methods
[Awake()]: #Awake
[CheckWillDiverge(Vector3)]: #CheckWillDivergeVector3
[ClearOffset()]: #ClearOffset
[ClearSource()]: #ClearSource
[DoCheckWillDiverge(Vector3)]: #DoCheckWillDivergeVector3
[Jump(Single)]: #JumpSingle
[ListenToRigidbodyMovement()]: #ListenToRigidbodyMovement
[OnAfterCharacterRadiusChange()]: #OnAfterCharacterRadiusChange
[OnAfterOffsetChange()]: #OnAfterOffsetChange
[OnAfterPreventEnterGeometryChange()]: #OnAfterPreventEnterGeometryChange
[OnAfterSourceChange()]: #OnAfterSourceChange
[ResolveDivergence()]: #ResolveDivergence
[SetSourceDivergenceThresholdX(Single)]: #SetSourceDivergenceThresholdXSingle
[SetSourceDivergenceThresholdY(Single)]: #SetSourceDivergenceThresholdYSingle
[SetSourceDivergenceThresholdZ(Single)]: #SetSourceDivergenceThresholdZSingle
[SnapToSource()]: #SnapToSource
[SolveBodyCollisions()]: #SolveBodyCollisions
