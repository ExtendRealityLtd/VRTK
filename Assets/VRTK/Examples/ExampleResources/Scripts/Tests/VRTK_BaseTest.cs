namespace VRTK.Examples.Tests
{
    using UnityEngine;
    using System.Collections;

    public abstract class VRTK_BaseTest : MonoBehaviour
    {
        protected string currentTest;
        protected string currentSetup;

        protected abstract void Test();

        protected virtual void OnEnable()
        {
            StartCoroutine(RunTests());
        }

        protected virtual void BeginTest(string name, int level = 1)
        {
            currentTest = name;
            Debug.Log("<color=darkblue><b>" + "".PadLeft(level, '#') + " Starting Tests for " + name + "</b></color>");
        }

        protected virtual void SetUp(string message)
        {
            currentSetup = message;
            Debug.Log("<color=blue><b>#### Preparing test for " + message + "</b></color>");
        }

        protected virtual void TearDown()
        {
            Debug.Log("==============================================================================");
        }

        protected virtual void Assert(string description, bool assertion, string failure, string success = "")
        {
            if (assertion)
            {
                Debug.Log("<color=teal><b>## [" + description + "] PASSED ##</b></color>");
            }
            else
            {
                Debug.Log("<color=maroon><b>## [" + description + "] FAILED INSIDE [" + currentTest + "." + currentSetup + "]##</b></color>");
            }

            if (!assertion)
            {
                Debug.LogException(new System.Exception(failure));
            }
            else if (success != "")
            {
                Debug.Log("<color=purple><i> ~~~~~> " + success + "</i></color>");
            }
        }

        protected virtual IEnumerator RunTests()
        {
            yield return new WaitForEndOfFrame();
            Test();
        }
    }
}