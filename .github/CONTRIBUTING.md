# Contributing

We are keen for developers to contribute to open source projects to
keep them great!

Whilst every effort is made to provide features that will assist a wide
range of development cases, it is inevitable that we will not cater for
all situations.

Therefore, if you feel that your custom solution is generic and
would assist other developers then we would love to review your
contribution.

There are however, a few guidelines that we need contributors to
follow so that we can have a chance of keeping on top things.

## Getting Started
  * Make sure you have a GitHub account.
  * Create a new issue on the GitHub repository, providing one does
  not already exist.
   * Clearly describe the issue including steps to reproduce when it
   is a bug (fill out the issue template).
   * Make sure you fill in the earliest version that you know has
   the issue.
  * Fork the repository on GitHub.

## Making Changes

  * Create a topic branch from where you want to base your work.
   * If you're fixing a bug then target the `master` branch.
   * If you're creating a new feature then target a release branch.
   * Name your branch with the type of issue you are fixing;
   `feat`, `chore`, `docs`.
   * Please avoid working directly on your master branch.
  * Make sure you set the `Asset Serialization` mode in
  `Unity->Edit->Project Settings->Editor` to `Force Text`.
  * Make commits of logical units.
  * Make sure your commit messages are in the proper format.

Following the above method will ensure that all bug fixes are pushed
to the `master` branch while all new features will be pushed to the
relevant next release branch. This means that patch releases are
much easier to do as the `master` branch will only contain bug fixes
so will be used to fork into new patch releases. Then master will be
rebased into the relevant next release branch so the next release also
contains the updated bug fixes in the previous patch release.

## Coding Conventions

To ensure all code is consistent and readable, we adhere to the
default coding conventions utilised in Visual Studio. The easiest
first step is to auto format the code within Visual Studio with
the key combination of `Ctrl + k` then `Ctrl + d` which will ensure
the code is correctly formatted and indented.

Spaces should be used instead of tabs for better readability across
a number of devices (e.g. spaces look better on Github source view.)

In regards to naming conventions we also adhere to the
[standard .NET Framework naming convention system](https://msdn.microsoft.com/en-gb/library/x2dbyw72(v=vs.71).aspx)
and use American spellings to denote classes, methods, fields, etc.

  > **Incorrect:**
  ```
  public Color fontColour;
  ```

  > **Correct:**
  ```
  public Color fontColor;
  ```

The only deviation from the MSDN conventions are fields should start
with lowercase -> `public Type myField` as this is inline with Unity's
naming convention.

Class methods and parameters should always denote their accessibility
level using the `public` `protected` `private` keywords.

  > **Incorrect:**
  ```
  void MyMethod()
  ```

  > **Correct:**
  ```
  private void MyMethod()
  ```

All core classes should be within the relevant `VRTK` namespace. Any
required `using` lines should be within the namespace block.

  > **Incorrect:**
  ```
  using System;
  namespace X.Y.Z
  {
    public class MyClass
	{
	}
  }
  ```

  > **Correct:**
  ```
  namespace X.Y.Z
  {
    using System;
    public class MyClass
	{
	}
  }
  ```

Parameters should be defined at the top of the class before any methods
are defined.

It is acceptable to have multiple classes defined in the same file as
long as the subsequent defined classes are only used by the main class
defined in the same file. However, don't nest types as it makes it
harder to find and reference them.

Where possible, the structure of the code should also flow with the
accessibility level of the method or parameters. So all `public`
parameters and methods should be defined first, followed by `protected`
parameters and methods with `private` parameters and methods being
defined last.

Blocks of code such as conditional statements and loops must always
contain the block of code in braces `{ }` even if it is just one line.

  > **Incorrect:**
  ```
  if (this == that) { do; }
  ```

  > **Correct:**
  ```
  if (this == that)
  {
    do;
  }
  ```

Any method or variable references should have the most simplified name
as possible, which means no additional references should be added where
it's not necessary.

  > `this.transform.rotation` *is simplified to* `transform.rotation`

  > `GameObject.FindObjectsOfType` *is simplified to* `FindObjectsOfType`

All MonoBehaviour inherited classes that implement a MonoBehaviour
[Message](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html)
method must at least be `protected virtual` to allow any further
inherited class to override and extend the methods.

When defining arrays, it is appropriate to instantiate the array with
the `System.Array.Empty<T>()` notation instead of doing `new Array[0]`.

All events should be UnityEvents and not C# delegates as this will
automatically provide inspector helpers for hooking up listeners. Any
custom UnityEvent should be defined as a nested class and the final
parameter should always be an `object` type so the sender of the event
can be passed in the payload.

When the actual UnityEvent is defined in code it should also be
instantiated so it is not null at runtime.

`public MyEventClass MyEventAction = new MyEventClass();`

## Documentation

All core scripts, abstractions, controls and prefabs should contain
inline code documentation adhering to the
[.NET Framework XML documentation comments convention](https://msdn.microsoft.com/en-us/library/b2s063f7.aspx)

All classes, methods and unity events should be documented using
the XML comments and contain a 1 line `<summary>` with any additional
lines included in `<remarks>`.

Public serialized parameters that appear in the inspector also require
XML comments and an additional `[Tooltip("")]` which is displayed in
the Unity Editor inspector panel. The order of documentation and
parameter attributes should be:

  * XML documentation
  * Optional attributes
  * Tooltip attribute

```
/// <summary>
/// Description here.
/// </summary>
[Optional-Attributes...]
[Tooltip("Description here.")]
public Type myType;
```

Whenever an inherited field, method, etc. is overridden then the
documentation should avoid repeating itself from the base class and
instead should use the `/// <inheritdoc />` notation instead. Despite
not being needed, being explicit about inheriting the documentation
simplifies checking contributions for proper documentation.

Any references to other classes, methods or parameters should also use
the `<see>` notation within the documentation.

## Commit Messages

All pull request commit messages are automatically checked using
[GitCop](http://gitcop.com) - this will inform you if there are any
issues with your commit message and give you an opportunity to rectify
any issues.

The commit message lines should never exceed 72 characters and should
be entered in the following format:

```
<type>(<scope>): <subject>
<BLANK LINE>
<body>
<BLANK LINE>
```

### Type

The type must be one of the following:

  * feat: A new feature.
  * fix: A bug fix.
  * docs: Documentation only changes.
  * refactor: A code change that neither fixes a bug or adds a feature.
  * perf: A code change that improves performance.
  * test: Adding missing tests.
  * chore: Changes to the build process or auxiliary tools or
  libraries such as documentation generation.

### Scope

The scope could be anything specifying the place of the commit change,
such as, `Controller`, `Interaction`, `Locomotion`, etc...

### Subject

The subject contains succinct description of the change:

  * use the imperative, present tense: "change" not "changed" nor
  "changes".
  * don't capitalize first letter, unless naming something, such as
  `Bootstrap`.
  * no dot (.) at the end of the subject line.

### Body

Just as in the subject, use the imperative, present tense: "change"
not "changed" nor "changes" The body should include the motivation for
the change and contrast this with previous behaviour. References to
previous commit hashes is actively encouraged if they are relevant.

  > **Incorrect commit summary:**
  ```
  Added feature to improve teleportation
  ```
  > **Incorrect commit summary:**
  ```
  feat(Teleport): Add feature
  ```
  > **Incorrect commit summary:**
  ```
  feat(my-teleport-feature): my feature.
  ```

  > **Correct commit summary:**
  ```
  feat(Teleport): add fade camera option on teleport
  ```

## Submitting Changes
  * Push your changes to your topic branch in your repository.
  * Submit a pull request to this repository.
  * The core team will aim to look at the pull request as soon as
  possible and provide feedback where required.
