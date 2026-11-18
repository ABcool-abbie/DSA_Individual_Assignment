using FrontierDispatch.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FrontierDispatch.Tests
{
    /// <summary>
    /// Small contract examples for later assignment phases.
    ///
    /// These are not an exhaustive assignment test suite.
    /// Students are expected to enable the relevant tests as they reach each
    /// phase and add meaningful tests of their own, including edge cases.
    /// </summary>
    public sealed class AssignmentStarterTests
    {
        // These tests document later assignment contracts without making the
        // untouched baseline red. Remove Ignore when you reach the named phase.

        [Test]
        //Ignore("Phase 3: enable when implementing the binary min-heap priority queue.")
        public void PriorityQueue_EnqueueAndDequeue_ReturnsLowestPriorityFirst()
        {
            var queue = new MinPriorityQueue<string>();
            queue.Enqueue("expensive", 9);
            queue.Enqueue("cheap", 2);
            queue.Enqueue("medium", 5);
            Assert.That(queue.Dequeue(), Is.EqualTo("cheap"));
        }

        [Test]
        public void PriorityQueue_EmptyQueue_ThrowInvalidOperationException()
        {
            var queue = new MinPriorityQueue<string>();
            Assert.Throws<InvalidOperationException>(() => queue.Peek());
            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
            Assert.That(queue.Count, Is.EqualTo(0));
        }

        [Test]
        public void PriorityQueue_SingleItem_CountChanges()
        {
            var queue = new MinPriorityQueue<string>();
            queue.Enqueue("only", 1);

            Assert.That(queue.Peek(), Is.EqualTo("only"));
            Assert.That(queue.Count, Is.EqualTo(1));
            Assert.That(queue.Dequeue(), Is.EqualTo("only"));
            Assert.That(queue.Count, Is.EqualTo(0));
        }

        [Test]
        public void PriorityQueue_Duplicates_ItemsInAscending()
        {
            var queue = new MinPriorityQueue<int>();
            int[] priorities = { 7, 3, 9, 1, 3, 9, 4, 7, 0 };
            foreach (int p in priorities) queue.Enqueue(p, p);

            var result = new List<int>();
            while (queue.Count > 0) result.Add(queue.Dequeue());

            Assert.That(result, Is.EqualTo(new[] { 0, 1, 3, 3, 4, 7, 7, 9, 9 }));
        }

        [Test]

        //Ignore("Phase 4: enable when implementing Dijkstra route finding.")
        public void Dijkstra_StartEqualsGoal_ReturnsZeroCostRoute()
        {
            var finder = new DijkstraRouteFinder(WorldDatasetFactory.CreateSmall());
            RouteResult result = finder.FindRoute(0, 0);
            Assert.That(result.Found, Is.True);
            Assert.That(result.TotalCost, Is.EqualTo(0));
            Assert.That(result.LocationIds, Is.EqualTo(new[] { 0 }));
        }

        private static WorldData CreateTinyWorld()
        {
            var locations = new List<Location>();
            for (int i = 0; i < 4; ++i)
                locations.Add(new Location(i, "Outpost " + i, new WorldPosition(i, 0), "Outpost"));

            var connections = new List<Connection>
            {
            new Connection(0, 1, 10),
            new Connection(0, 2, 3),
            new Connection(2, 1, 1),
            new Connection(1, 3, 2),
            };

            return new WorldData(locations, connections);
        }

        [Test]
        public void Dijkstra_ChoosesLeastCostRoute()
        {
            var finder = new DijkstraRouteFinder(CreateTinyWorld());

            RouteResult toOne = finder.FindRoute(0, 1);
            Assert.That(toOne.TotalCost, Is.EqualTo(4));
            Assert.That(toOne.LocationIds, Is.EqualTo(new[] { 0, 2, 1 }));

            RouteResult toThree = finder.FindRoute(0, 3);
            Assert.That(toThree.TotalCost, Is.EqualTo(6));
            Assert.That(toThree.LocationIds, Is.EqualTo(new[] { 0, 2, 1, 3 }));
        }

        [Test]
        public void Dijkstra_UnreachableOrUnknownGoal_ReturnsNotFound()
        {
            var finder = new DijkstraRouteFinder(CreateTinyWorld());

            RouteResult wrongDirection = finder.FindRoute(3, 0);
            Assert.That(wrongDirection.Found, Is.False);
            Assert.That(wrongDirection.LocationIds, Is.Empty);

            Assert.That(finder.FindRoute(0, 99).Found, Is.False);
            Assert.That(finder.FindRoute(99, 99).Found, Is.False);
        }

        [Test]
        public void Dijkstra_OnSmallGrid_CostMatchesRoute()
        {
            WorldData world = WorldDatasetFactory.CreateSmall();
            var finder = new DijkstraRouteFinder(world);
            var queries = new WorldQueries(world);

            RouteResult result = finder.FindRoute(0, 24);

            Assert.That(result.Found, Is.True);
            Assert.That(result.LocationIds[0], Is.EqualTo(0));
            Assert.That(result.LocationIds[result.LocationIds.Count - 1], Is.EqualTo(24));

            int sum = 0;
            for (int i = 0; i + 1 < result.LocationIds.Count; ++i)
            {
                foreach (Connection c in queries.FindOutgoingConnections(result.LocationIds[i]))
                {
                    if (c.ToLocationId == result.LocationIds[i + 1]) { sum = sum + c.TravelCost; break; }
                }
            }

            Assert.That(sum, Is.EqualTo(result.TotalCost));
        }

        [Test, Ignore("Phase 5: enable when implementing bounded reachability.")]
        public void Reachability_ZeroBudget_IncludesStartAtZeroCost()
        {
            var query = new ReachabilityQuery(WorldDatasetFactory.CreateSmall());
            var result = query.FindReachable(0, 0);
            Assert.That(result.ContainsKey(0), Is.True);
            Assert.That(result[0], Is.EqualTo(0));
        }
    }
}
