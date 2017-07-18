# Contributing

We are keen for developers to contribute to open source projects to
keep them great!
Whilst every effort is made to provide features that will assist a wide
range of development cases, it is inevitable that we will not cater for
all situations.
Therefore, if you feel that your custom solution is very general and
would assist other developers then by all means, we would love to
review your contribution.

There are, however, a few guidelines that we need contributors to
follow, so that we can have a chance of keeping on top things.

## Getting Started
  * Make sure you have a GitHub account
  * Create a new issue on the GitHub repository, providing one does
  not already exist
   * Clearly describe the issue including steps to reproduce when it
   is a bug
   * Make sure you fill in the earliest version that you know has
   the issue
  * Fork the repository on GitHub

## Making Changes

  * Create a topic branch from where you want to base your work
   * This is usually the master branch
   * Only target release branches if you are certain your fix must be
   on that branch
   * Name branches with the type of issue you are fixing;
   `feat`, `chore`, `docs`
   * Please avoid working directly on the master branch
  * Make sure you set the `Asset Serialization` mode in
  `Unity->Edit->Project Settings->Editor` to `Force Text`
  * Make commits of logical units
  * Make sure your commit messages are in the proper format

## Coding Conventions

To ensure all code is consistent and readable, we adhere to the
default coding conventions utilised in Visual Studio. The easiest
first step is to auto format the code within Visual Studio with
the key combination of `Ctrl + k` then `Ctrl + d` which will ensure
the code is correctly formatted and indented.

Spaces should be used instead of tabs for better readability across
a number of devices (e.g. spaces look better on Github source view.)

In regards to naming conventions we also adhere to the standard
.NET Framework naming convention system which can be
[viewed online here](https://msdn.microsoft.com/en-gb/library/x2dbyw72(v=vs.71).aspx)

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

All core classes should be within the `VRTK` namespace and the class
name should be prefixed with `VRTK_`.

Any example code should be within the`VRTK.Examples` namespace and any
Unity Event Helpers should be within the `VRTK.UnityEventHelper`
namespace.

Parameters should be defined at the top of the class before any methods
are defined.

It is acceptable to have multiple classes defined in the same file as
long as the subsequent defined classes are only used by the main class
defined in the same file.

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

## Documentation

All scripts that require documentation need to include a comment marker
on the first line of the script in this format:

`// <Script Title>|<Section>|<Position>`

 * **Script Title:** A user friendly name for the script.
 * **Section:** The section the script will appear in:
  * `Prefabs` - A script required for configuring an included prefab.
  * `Abstractions` - An abstract script providing user functionality.
  * `Scripts` - A script for providing user functionality.
  * `Controls3D` - A script for providing a 3D control.
 * **Position:** The position the text will appear in the section.

  > **Example**
  `// UI Pointer|Scripts|0060`

All core scripts, abstractions, controls and prefabs should contain
inline code documentation adhering to the .NET Framework XML
documentation comments convention which can be
[viewed online here](https://msdn.microsoft.com/en-us/library/b2s063f7.aspx)

Public classes, methods, delegate events and unity events should be
documented using the XML comments and contain a 1 line `<summary>`
with any additional lines included in `<remarks>`.

Public parameters that appear in the inspector do not need XML
comments and just require a `[Tooltip("")]` which is used to generate
the documentation. However, other public class variables that are
hidden from the inspector do need XML style comments to document them.

C# delegate events also require to reference the event payload `struct`
which also requires documenting using XML comments.

Any accompanying example scenes can be documented within the `class`
comments within the `<example>` tag with multiple lines being used
for each example rather than using multiple `<example>` tags.

Also, any example scene requires an entry in the `EXAMPLES.md` file.

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

  * feat: A new feature
  * fix: A bug fix
  * docs: Documentation only changes
  * refactor: A code change that neither fixes a bug or adds a feature
  * perf: A code change that improves performance
  * test: Adding missing tests
  * chore: Changes to the build process or auxiliary tools or
  libraries such as documentation generation

### Scope

The scope could be anything specifying the place of the commit change,
such as, `Controller`, `Interaction`, `Locomotion`, etc...

### Subject

The subject contains succinct description of the change:

  * use the imperative, present tense: "change" not "changed" nor
  "changes"
  * don't capitalize first letter, unless naming something,
  such as `Bootstrap`
  * no dot (.) at the end of the subject line

### Body

Just as in the subject, use the imperative, present tense: "change"
not "changed" nor "changes" The body should include the motivation for
the change and contrast this with previous behavior. References to
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
  * Submit a pull request to the repository
  `thestonefox/VRTK`.
  * The core team will aim to look at the pull request as soon as
  possible and provide feedback where required.
