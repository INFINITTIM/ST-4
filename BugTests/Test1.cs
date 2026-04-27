using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;
using System.Linq;

namespace BugTests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void Constructor_DefaultStateIsNew()
    {
        var bug = new Bug();
        Assert.AreEqual(BugState.New, bug.CurrentState);
    }

    [TestMethod]
    public void FireStart_NewToOpen()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Start);
        Assert.AreEqual(BugState.Open, bug.CurrentState);
    }

    [TestMethod]
    public void FireAssign_OpenToInProgress()
    {
        var bug = new Bug(); bug.Fire(BugTrigger.Start);
        bug.Fire(BugTrigger.Assign);
        Assert.AreEqual(BugState.InProgress, bug.CurrentState);
    }

    [TestMethod]
    public void FireReject_OpenToRejected()
    {
        var bug = new Bug(); bug.Fire(BugTrigger.Start);
        bug.Fire(BugTrigger.Reject);
        Assert.AreEqual(BugState.Rejected, bug.CurrentState);
    }

    [TestMethod]
    public void FireDefer_OpenToDeferred()
    {
        var bug = new Bug(); bug.Fire(BugTrigger.Start);
        bug.Fire(BugTrigger.Defer);
        Assert.AreEqual(BugState.Deferred, bug.CurrentState);
    }

    [TestMethod]
    public void FireFix_InProgressToFixed()
    {
        var bug = new Bug(); bug.Fire(BugTrigger.Start); bug.Fire(BugTrigger.Assign);
        bug.Fire(BugTrigger.Fix);
        Assert.AreEqual(BugState.Fixed, bug.CurrentState);
    }

    [TestMethod]
    public void FireAbandon_InProgressToOpen()
    {
        var bug = new Bug(); bug.Fire(BugTrigger.Start); bug.Fire(BugTrigger.Assign);
        bug.Fire(BugTrigger.Abandon);
        Assert.AreEqual(BugState.Open, bug.CurrentState);
    }

    [TestMethod]
    public void FireVerify_FixedToResolved()
    {
        var bug = new Bug(); bug.Fire(BugTrigger.Start); bug.Fire(BugTrigger.Assign); bug.Fire(BugTrigger.Fix);
        bug.Fire(BugTrigger.Verify);
        Assert.AreEqual(BugState.Resolved, bug.CurrentState);
    }

    [TestMethod]
    public void FireReopen_FixedToReopened()
    {
        var bug = new Bug(); bug.Fire(BugTrigger.Start); bug.Fire(BugTrigger.Assign); bug.Fire(BugTrigger.Fix);
        bug.Fire(BugTrigger.Reopen);
        Assert.AreEqual(BugState.Reopened, bug.CurrentState);
    }

    [TestMethod]
    public void FireClose_ResolvedToClosed()
    {
        var bug = new Bug(); bug.Fire(BugTrigger.Start); bug.Fire(BugTrigger.Assign); bug.Fire(BugTrigger.Fix); bug.Fire(BugTrigger.Verify);
        bug.Fire(BugTrigger.Close);
        Assert.AreEqual(BugState.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void FireReopen_ResolvedToReopened()
    {
        var bug = new Bug(); bug.Fire(BugTrigger.Start); bug.Fire(BugTrigger.Assign); bug.Fire(BugTrigger.Fix); bug.Fire(BugTrigger.Verify);
        bug.Fire(BugTrigger.Reopen);
        Assert.AreEqual(BugState.Reopened, bug.CurrentState);
    }

    [TestMethod]
    public void FireResume_DeferredToOpen()
    {
        var bug = new Bug(); bug.Fire(BugTrigger.Start); bug.Fire(BugTrigger.Defer);
        bug.Fire(BugTrigger.Resume);
        Assert.AreEqual(BugState.Open, bug.CurrentState);
    }

    [TestMethod]
    public void FullHappyPath_NewToClosed()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Start);
        bug.Fire(BugTrigger.Assign);
        bug.Fire(BugTrigger.Fix);
        bug.Fire(BugTrigger.Verify);
        bug.Fire(BugTrigger.Close);
        Assert.AreEqual(BugState.Closed, bug.CurrentState);
    }

    [TestMethod]
    public void CanFire_ReturnsTrueForPermittedTrigger()
    {
        var bug = new Bug(); bug.Fire(BugTrigger.Start);
        Assert.IsTrue(bug.CanFire(BugTrigger.Assign));
    }

    [TestMethod]
    public void CanFire_ReturnsFalseForForbiddenTrigger()
    {
        var bug = new Bug();
        Assert.IsFalse(bug.CanFire(BugTrigger.Close));
    }

    [TestMethod]
    public void InvalidTransition_NewToAssign_Throws()
    {
        var bug = new Bug();
        try
        {
            bug.Fire(BugTrigger.Assign);
            Assert.Fail("Ожидалось InvalidOperationException");
        }
        catch (InvalidOperationException)
        {
        }
    }

    [TestMethod]
    public void InvalidTransition_OpenToFix_Throws()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Start);
        try
        {
            bug.Fire(BugTrigger.Fix);
            Assert.Fail("Ожидалось InvalidOperationException");
        }
        catch (InvalidOperationException)
        {
        }
    }

    [TestMethod]
    public void InvalidTransition_FixedToClose_Throws()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Start);
        bug.Fire(BugTrigger.Assign);
        bug.Fire(BugTrigger.Fix);
        try
        {
            bug.Fire(BugTrigger.Close);
            Assert.Fail("Ожидалось InvalidOperationException");
        }
        catch (InvalidOperationException)
        {
        }
    }

    [TestMethod]
    public void InvalidTransition_ClosedToAny_Throws()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Start);
        bug.Fire(BugTrigger.Assign);
        bug.Fire(BugTrigger.Fix);
        bug.Fire(BugTrigger.Verify);
        bug.Fire(BugTrigger.Close);
        try
        {
            bug.Fire(BugTrigger.Reopen);
            Assert.Fail("Ожидалось InvalidOperationException");
        }
        catch (InvalidOperationException)
        {
        }
    }

    [TestMethod]
    public void InvalidTransition_RejectedToVerify_Throws()
    {
        var bug = new Bug();
        bug.Fire(BugTrigger.Start);
        bug.Fire(BugTrigger.Reject);
        try
        {
            bug.Fire(BugTrigger.Verify);
            Assert.Fail("Ожидалось InvalidOperationException");
        }
        catch (InvalidOperationException)
        {
        }
    }

    [TestMethod]
    public void Constructor_SetsTitleCorrectly()
    {
        var bug = new Bug("API-500");
        Assert.AreEqual("API-500", bug.Title);
    }

    [TestMethod]
    public void PermittedTriggers_ContainsExpectedInOpenState()
    {
        var bug = new Bug(); bug.Fire(BugTrigger.Start);
        var allowed = bug.PermittedTriggers.ToList();
        Assert.IsTrue(allowed.Contains(BugTrigger.Assign));
        Assert.IsTrue(allowed.Contains(BugTrigger.Reject));
        Assert.IsTrue(allowed.Contains(BugTrigger.Defer));
        Assert.AreEqual(3, allowed.Count);
    }
}