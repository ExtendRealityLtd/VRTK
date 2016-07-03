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

In regards to naming conventions we also adhere to the standard
.NET Framework naming convention system which can be
[viewed online here](https://msdn.microsoft.com/en-gb/library/x2dbyw72(v=vs.71).aspx)

Class methods and parameters should always denote their accessibility
level using the `public` `protected` `private` keywords.

All core classes should be within the `VRTK` namespace and the class
name should be prefixed with `VRTK_`.

Parameters should be defined at the top of the class before any methods
are defined.

It is acceptable to have two classes defined in the same file as long
as the second defined class is only used by the other class defined in
the same file.

Where possible, the structure of the code should also flow with the
accessibility level of the method or parameters. So all `public`
parameters and methods should be defined first, followed by `protected`
paramters and methods with `private` parameters and methods being
defined last.

Blocks of code such as conditional statements and loops must always
contain the block of code in braces `{ }` even if it is just one line.

## Documentation

If a new feature is being added then the `README.md` should also be
updated to contain information about the relevant elements such as:

  * New general purpose prefabs added.
  * New general purpose scripts.
  * New example scenes showcasing the feature.

The `README.md` lines should not exceed 72 characters unless the line
cannot be split (such in the case of a long markdown link or url).

## Commit Messages

All pull request commit messages are automatically checked using
[GitCop](http://gitcop.com) - this will inform you if there are any
issues with your commit message and give you an opportunity to rectify
any issues.

The commit message lines should never exceed 72 lines and should be
entered in the following format:

```
<type>(<scope>): <subject>
<BLANK LINE>
<body>
<BLANK LINE>
```

### Type

The type must be one of the folowing:

  * feat: A new feature
  * fix: A bug fix
  * docs: Documentation only changes
  * refactor: A code change that neither fixes a bug or adds a feature
  * perf: A code change that improves performance
  * test: Adding missing tests
  * chore: Changes to the build process or auxiliary tools an
  libraries such as documentation generation

### Scope

The scope could be anything specifiyng the place of the commit change,
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

## Submitting Changes
  * Push your changes to your topic branch in your repository.
  * Submit a pull request to the repository
  `thestonefox/SteamVR_Unity_Plugin`.
  * The core team will aim to look at the pull request as soon as
  possible and provide feedback where required.
