## Contents

* [Inheritance]
* [Namespace]
* [Syntax]
* [Fields]

## Details

# Enum PseudoBodyProcessor.MovementInterest

The object that defines the main source of truth for movement.

##### Namespace

* [Tilia.Trackers.PseudoBody]

##### Syntax

```
public enum MovementInterest
```

### Fields

| Name | Description |
| --- | --- |
| CharacterController | The source of truth for movement comes from [Character]. |
| CharacterControllerUntilAirborne | The source of truth for movement comes from [Character] until rigidbody is in the air, then rigidbody is the new source of truth. |
| Rigidbody | The source of truth for movement comes from rigidbody. |
| RigidbodyUntilGrounded | The source of truth for movement comes from rigidbody until [Character] hits the ground, then [Character] is the new source of truth. |

[Tilia.Trackers.PseudoBody]: README.md
[Character]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_Character
[Character]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_Character
[Character]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_Character
[Character]: PseudoBodyProcessor.md#Tilia_Trackers_PseudoBody_PseudoBodyProcessor_Character
[Inheritance]: #Inheritance
[Namespace]: #Namespace
[Syntax]: #Syntax
[Fields]: #Fields
