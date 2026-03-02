// Skeleton implementation written by Joe Zachary for CS 3500, September 2013
// Version 1.1 - Joe Zachary
//   (Fixed error in comment for RemoveDependency)
// Version 1.2 - Daniel Kopta Fall 2018
//   (Clarified meaning of dependent and dependee)
//   (Clarified names in solution/project structure)
// Version 1.3 - H. James de St. Germain Fall 2024

// <authors> Jesse Pan </authors>
// <date> 1/26/2026 </date>

namespace DependencyGraph;

/// <summary>
///   <para>
///     (s1,t1) is an ordered pair of strings, meaning t1 depends on s1.
///     (in other words: s1 must be evaluated before t1.)
///   </para>
///   <para>
///     A DependencyGraph can be modeled as a set of ordered pairs of strings.
///     Two ordered pairs (s1,t1) and (s2,t2) are considered equal if and only
///     if s1 equals s2 and t1 equals t2.
///   </para>
///   <remarks>
///     Recall that sets never contain duplicates.
///     If an attempt is made to add an element to a set, and the element is already
///     in the set, the set remains unchanged.
///   </remarks>
///   <para>
///     Given a DependencyGraph DG:
///   </para>
///   <list type="number">
///     <item>
///       If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
///       (The set of things that depend on s.)
///     </item>
///     <item>
///       If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
///       (The set of things that s depends on.)
///     </item>
///   </list>
///   <para>
///      For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d","d")}.
///   </para>
///   <code>
///     dependents("a") = {"b", "c"}
///     dependents("b") = {"d"}
///     dependents("c") = {}
///     dependents("d") = {"d"}
///     dependees("a") = {}
///     dependees("b") = {"a"}
///     dependees("c") = {"a"}
///     dependees("d") = {"b", "d"}
///   </code>
/// </summary>
public class DependencyGraph
{
    /// <summary>
    /// Map each node, stores the set of nodes it depends on
    /// </summary>
    private Dictionary<string, HashSet<string>> _dependees;
    
    /// <summary>
    /// Map each node, stores the set of nodes that depend on it
    /// </summary>
    private Dictionary<string, HashSet<string>> _dependents;
    
    /// <summary>
    /// Stores the total number of dependency pairs
    /// </summary>
    private int _size;
    
    /// <summary>
    ///   Initializes a new instance of the <see cref="DependencyGraph"/> class.
    ///   The initial DependencyGraph is empty.
    /// </summary>
    public DependencyGraph()
    {
        _dependees = new Dictionary<string, HashSet<string>>();
        _dependents = new Dictionary<string, HashSet<string>>();
        _size = 0;
    }
    
    /// <summary>
    /// The number of ordered pairs in the DependencyGraph.
    /// </summary>
    public int Size
    {
        get { return _size; }
    }
    
    /// <summary>
    ///   Reports whether the given node has dependents (i.e., other nodes depend on it).
    /// </summary>
    /// <param name="nodeName"> The name of the node.</param>
    /// <returns> true if the node has dependents. </returns>
    public bool HasDependents(string nodeName)
    {
        if (!_dependents.ContainsKey(nodeName))
        {
            return false;
        }

        if (_dependents[nodeName].Count == 0)
        {
            return false;
        }
        return true;
    }
    
    /// <summary>
    ///   Reports whether the given node has dependees (i.e., depends on one or more other nodes).
    /// </summary>
    /// <returns> true if the node has dependees.</returns>
    /// <param name="nodeName">The name of the node.</param>
    public bool HasDependees(string nodeName)
    {
        if (!_dependees.ContainsKey(nodeName))
        {
            return false;
        }

        if (_dependees[nodeName].Count == 0)
        {
            return false;
        } 
        return true;
    }
    
    /// <summary>
    ///   <para>
    ///     Returns the dependents of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependents of nodeName. </returns>
    public IEnumerable<string> GetDependents(string nodeName)
    {
        if (!_dependents.ContainsKey(nodeName))
        {
            return new List<string>();
        }
        return new List<string>(_dependents[nodeName]);
    }
    
    /// <summary>
    ///   <para>
    ///     Returns the dependees of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependees of nodeName. </returns>
    public IEnumerable<string> GetDependees(string nodeName)
    {
        if (!_dependees.ContainsKey(nodeName))
        {
            return new List<string>();
        }
        return new List<string>(_dependees[nodeName]);
    }
    
    /// <summary>
    /// <para>Adds the ordered pair (dependee, dependent), if it doesn't exist.</para>
    ///
    /// <para>
    ///   This can be thought of as: dependee must be evaluated before dependent
    /// </para>
    /// </summary>
    /// <param name="dependee"> the name of the node that must be evaluated first</param>
    /// <param name="dependent"> the name of the node that cannot be evaluated until after dependee</param>
    public void AddDependency(string dependee, string dependent)
    {
        if (!_dependents.ContainsKey(dependee))
        {
            _dependents[dependee] = new HashSet<string>();
        }
        if (!_dependees.ContainsKey(dependent))
        {
            _dependees[dependent] = new HashSet<string>();
        }
        if (!_dependents[dependee].Contains(dependent)) 
        {
            _dependents[dependee].Add(dependent);
            _dependees[dependent].Add(dependee);
            _size++;
        }
    }
    
    /// <summary>
    ///   <para>
    ///     Removes the ordered pair (dependee, dependent), if it exists.
    ///   </para>
    /// </summary>
    /// <param name="dependee"> The name of the node that must be evaluated first</param>
    /// <param name="dependent"> The name of the node that cannot be evaluated until after dependee</param>
    public void RemoveDependency(string dependee, string dependent)
    {
        if (!_dependents.ContainsKey(dependee) || !_dependees.ContainsKey(dependent))
        {
            return;
        }
        if (!_dependents[dependee].Contains(dependent) || !_dependees[dependent].Contains(dependee))
        {
            return;
        }
        _dependents[dependee].Remove(dependent);
        _dependees[dependent].Remove(dependee);
        _size--;
    }
    
    /// <summary>
    ///   Removes all existing ordered pairs of the form (nodeName, *). Then, for each
    ///   t in newDependents, adds the ordered pair (nodeName, t).
    /// </summary>
    /// <param name="nodeName"> The name of the node whose dependents are being replaced </param>
    /// <param name="newDependents"> The new dependents for nodeName</param>
    public void ReplaceDependents(string nodeName, IEnumerable<string> newDependents)
    {
        ReplaceList(nodeName, GetDependents(nodeName), newDependents, false);
    }
    
    /// <summary>
    ///   <para>
    ///     Removes all existing ordered pairs of the form (*, nodeName). Then, for each
    ///     t in newDependees, adds the ordered pair (t, nodeName).
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependees are being replaced</param>
    /// <param name="newDependees"> The new dependees for nodeName</param>
    public void ReplaceDependees(string nodeName, IEnumerable<string> newDependees)
    {
        ReplaceList(nodeName, GetDependees(nodeName), newDependees, true);
    }

    /// <summary>
    ///     Helper method that removes all existing dependency and adds in new
    ///     dependency, in either dependees or dependents
    /// </summary>
    /// <param name="nodeName"></param>
    /// <param name="list"></param>
    /// <param name="newList"></param>
    /// <param name="isDependees"></param>
    private void ReplaceList(string nodeName, IEnumerable<string> list, IEnumerable<string> newList, bool isDependees)
    {
        foreach (string oldDep in list)
        {
            if (isDependees)
            {
                RemoveDependency(oldDep, nodeName);
            }
            else
            {
                RemoveDependency(nodeName, oldDep);
            }
        }
        foreach (string newDep in newList)
        {
            if (isDependees)
            {
                AddDependency(newDep, nodeName);
            }
            else
            {
                AddDependency(nodeName, newDep);
            }
        }
    }
}