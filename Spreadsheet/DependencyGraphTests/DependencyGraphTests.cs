namespace DependencyGraphTests;

using DependencyGraph;

/// <summary>
///   This is a test class for DependencyGraphTest and is intended
///   to contain all DependencyGraphTest Unit Tests
/// </summary>
[TestClass]
public class DependencyGraphTests
{
    /// <summary>
    ///   This stress test performs many AddDependency and RemoveDependency operations on a
    ///   DependencyGraph. It checks after a large number of changes, the graph correctly reports all
    ///   the dependents and dependees.
    /// </summary>
    [TestMethod]
    [Timeout( 2000, CooperativeCancellation = true)] // 2 second run time limit
    public void StressTest()
    {
        DependencyGraph dg = new();
        // A bunch of strings to use
        const int size = 200;
        string[] letters = new string[size];
        for ( int i = 0; i < size; i++ )
        {
            letters[i] = string.Empty + ( (char) ( 'a' + i ) );
        }
        // The correct answers
        HashSet<string>[] dependents = new HashSet<string>[size];
        HashSet<string>[] dependees = new HashSet<string>[size];
        for ( int i = 0; i < size; i++ )
        {
            dependents[i] = [];
            dependees[i] = [];
        }
        // Add a bunch of dependencies
        for ( int i = 0; i < size; i++ )
        {
            for ( int j = i + 1; j < size; j++ )
            {
                dg.AddDependency( letters[i], letters[j] );
                dependents[i].Add( letters[j] );
                dependees[j].Add( letters[i] );
            }
        }
        // Remove a bunch of dependencies
        for ( int i = 0; i < size; i++ )
        {
            for ( int j = i + 4; j < size; j += 4 )
            {
                dg.RemoveDependency( letters[i], letters[j] );
                dependents[i].Remove( letters[j] );
                dependees[j].Remove( letters[i] );
            }
        }
        // Add some back
        for ( int i = 0; i < size; i++ )
        {
            for ( int j = i + 1; j < size; j += 2 )
            {
                dg.AddDependency( letters[i], letters[j] );
                dependents[i].Add( letters[j] );
                dependees[j].Add( letters[i] );
            }
        }
        // Remove some more
        for ( int i = 0; i < size; i += 2 )
        {
            for ( int j = i + 3; j < size; j += 3 )
            {
                dg.RemoveDependency( letters[i], letters[j] );
                dependents[i].Remove( letters[j] );
                dependees[j].Remove( letters[i] );
            }
        }
        // Make sure everything is right
        for ( int i = 0; i < size; i++ )
        {
            Assert.IsTrue( dependents[i].SetEquals( new HashSet<string>( dg.GetDependents( letters[i] ) ) ) );
            Assert.IsTrue( dependees[i].SetEquals( new HashSet<string>( dg.GetDependees( letters[i] ) ) ) );
        }
    }

    // -- Construction/Size -- 
    
    [TestMethod]
    public void AddDependency_IncreaseSize_Work()
    {
        DependencyGraph dg = new();
        Assert.AreEqual(0, dg.Size);
        dg.AddDependency( "a", "b" );
        Assert.AreEqual(1, dg.Size);
    }
    
    [TestMethod]
    public void AddDependency_IncreaseSizeDup_Work()
    {
        DependencyGraph dg = new();
        dg.AddDependency( "a", "b" );
        dg.AddDependency( "a", "b" );
        Assert.AreEqual(1, dg.Size);
    }
    
    [TestMethod]
    public void RemoveDependency_DecreaseSize_Work()
    {
        DependencyGraph dg = new();
        dg.AddDependency( "a", "b" );
        dg.RemoveDependency( "a", "b" );
        Assert.AreEqual(0, dg.Size);
    }
    
    [TestMethod]
    public void GetDependees_Empty_Work()
    {
        DependencyGraph dg = new();
        Assert.AreEqual(0, dg.GetDependees( "a" ).Count() );
    }
    
    [TestMethod]
    public void GetDependent_Empty_Work()
    {
        DependencyGraph dg = new();
        Assert.AreEqual(0, dg.GetDependents( "a" ).Count() );
    }
    
    [TestMethod]
    public void HasDependees_Empty_Work()
    {
        DependencyGraph dg = new();
        Assert.IsFalse( dg.HasDependees("a") );
    }
    
    [TestMethod]
    public void HasDependent_Empty_Work()
    {
        DependencyGraph dg = new();
        Assert.IsFalse( dg.HasDependents("a") );
    }
    
    // -- Add/Remove Methods --
    
    [TestMethod]
    public void AddDependency_HasDep_Work()
    {
        DependencyGraph dg = new();
        
        dg.AddDependency( "a", "b" );
        dg.AddDependency( "p", "a" );
        Assert.IsTrue( dg.HasDependees( "a" ) );
        Assert.IsTrue( dg.HasDependents( "a" ) );
    }
    
    [TestMethod]
    public void AddDependency_GetDependents_Work()
    {
        DependencyGraph dg = new();
        
        dg.AddDependency( "a1", "b" );
        dg.AddDependency( "a1", "m" );
        Assert.AreEqual( 2, dg.GetDependents( "a1" ).Count() );
    }

    [TestMethod]
    public void RemoveDependency_HasDep_Work()
    {
        DependencyGraph dg = new();
        dg.AddDependency( "a", "b" );
        dg.RemoveDependency( "a", "b" );
        Assert.IsFalse( dg.HasDependees("a") );
        Assert.IsFalse( dg.HasDependents("b") );
    }
    
    [TestMethod]
    public void RemoveDependency_RemoveNonExistDep_Work()
    {
        DependencyGraph dg = new();
        dg.AddDependency( "a", "b" );
        dg.RemoveDependency( "d", "c" );
        Assert.IsTrue( dg.HasDependees("b") );
    }
    
    [TestMethod]
    public void HasDependents_PartialRemoval_Work()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");

        dg.RemoveDependency("a", "b");

        Assert.IsTrue(dg.HasDependents("a")); 
    }

    [TestMethod]
    public void HasDep_SameDupName_Work()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "a");
        
        Assert.IsTrue(dg.HasDependents("a"));
        Assert.IsTrue(dg.HasDependees("a"));
    }
    
    [TestMethod]
    public void GetDependents_Multiple_Work()
    {
        DependencyGraph dg = new();
        dg.AddDependency("o", "b");
        dg.AddDependency("o", "m");

        var expected = new HashSet<string> { "b", "m" };
        var actual = new HashSet<string>(dg.GetDependents("o"));

        Assert.IsTrue(expected.SetEquals(actual));
    }

    [TestMethod]
    public void GetDependees_Multiple_Work()
    {
        DependencyGraph dg = new();
        dg.AddDependency("o", "b");
        dg.AddDependency("o", "m");
        dg.AddDependency("m", "q");

        var expected = new HashSet<string> { "o" };
        var actual1 = new HashSet<string>(dg.GetDependees("b"));
        var actual2 = new HashSet<string>(dg.GetDependees("m"));

        Assert.IsTrue(expected.SetEquals(actual1));
        Assert.IsTrue(expected.SetEquals(actual2));
    }

    [TestMethod]
    public void HasDep_NonExist_Work()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        Assert.IsFalse(dg.HasDependees("jg"));
        Assert.IsFalse(dg.HasDependents("nnb"));
    }

    [TestMethod]
    public void RemoveDepAndAddDep_ManyTimes_Work()
    {
        DependencyGraph dg = new();
        
        dg.AddDependency("a", "b");
        dg.RemoveDependency("a", "b");
        dg.AddDependency("a", "b");
        
        Assert.IsTrue(dg.HasDependents("a"));
    }
    
    // -- Replace Methods --

    [TestMethod]
    public void ReplaceDependents_ReplaceOldWithNew_Work()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "d");
        
        dg.ReplaceDependents("a", new[] { "n", "x" });
        
        var expected = new HashSet<string> { "n", "x" };
        var actual = new HashSet<string>(dg.GetDependents("a"));
        
        Assert.IsTrue(expected.SetEquals(actual));
        
        Assert.IsFalse(dg.HasDependents("b"));
        Assert.IsFalse(dg.HasDependents("d"));
    }
    
    [TestMethod]
    public void ReplaceDependees_ReplaceOldWithNew_Work()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "d");
        
        dg.ReplaceDependees("b", new[] { "n", "p" });
        
        var expected = new HashSet<string> { "n", "p" };
        var actual = new HashSet<string>(dg.GetDependees("b"));
        
        Assert.IsTrue(expected.SetEquals(actual));
        
        Assert.AreEqual(1, dg.GetDependents( "a" ).Count());
    }

    [TestMethod]
    public void ReplaceDependees_EmptyNode_Work()
    {
        DependencyGraph dg = new();
        dg.ReplaceDependees("b", new[] { "n", "p" });
        
        var expected = new HashSet<string> { "n", "p" };
        var actual = new HashSet<string>(dg.GetDependees("b"));
        
        Assert.IsTrue(expected.SetEquals(actual));
        Assert.IsTrue(dg.HasDependees("b"));
    }

    [TestMethod]
    public void ReplaceDependees_WithEmptySet_Work()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.ReplaceDependees("b", Array.Empty<string>());
        
        Assert.IsFalse(dg.HasDependees("b"));
    }
    
    [TestMethod]
    public void ReplaceDependent_WithEmptySet_Work()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.ReplaceDependents("a", Array.Empty<string>());
        
        Assert.IsFalse(dg.HasDependents("a"));
    }

    /// <summary>
    ///   This mini stress test performs many ReplaceDependees operations on a
    ///   DependencyGraph. It checks after it new Replace method is called, reports all
    ///   the new dependees.
    /// </summary>
    [TestMethod]
    public void ReplaceDependees_MiniStressTest_Work()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        
        dg.ReplaceDependees("a", new[] { "p", "c" });
        var expected = new HashSet<string> { "p", "c" };
        var actual = new HashSet<string>(dg.GetDependees("a"));
        
        Assert.IsTrue(expected.SetEquals(actual));
        
        dg.ReplaceDependees("a", new[] { "l", "i3" });
        
        var expected2 = new HashSet<string> { "l", "i3" };
        var actual2 = new HashSet<string>(dg.GetDependees("a"));
        
        Assert.IsTrue(expected2.SetEquals(actual2));
        
        dg.ReplaceDependees("a", Array.Empty<string>());
        
        Assert.IsFalse(dg.HasDependees("a"));
        Assert.AreEqual(0, dg.GetDependees("a").Count());
        
        dg.ReplaceDependees("a", new[] {"b"});
        
        var expected4 = new HashSet<string> { "b" };
        var actual4 = new HashSet<string>(dg.GetDependees("a"));
        
        Assert.IsTrue(expected4.SetEquals(actual4));
    }
}