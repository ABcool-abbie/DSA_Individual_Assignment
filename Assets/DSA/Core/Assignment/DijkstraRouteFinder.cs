using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    // Assignment Phase 4.

    /// <summary>
    /// Finds minimum-total-cost routes through a Frontier Dispatch world
    /// using Dijkstra's shortest-path algorithm.
    ///
    /// This class is intentionally incomplete in the assignment starter.
    /// It should eventually use the student's prepared world representation
    /// and MinPriorityQueue implementation.
    /// </summary>
    public sealed class DijkstraRouteFinder
    {
        private readonly WorldData _world;
        private readonly WorldQueries _queries;

        /// <summary>
        /// Constructor. Creates a route finder operating on the supplied world.
        /// </summary>
        public DijkstraRouteFinder(WorldData world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _queries = new WorldQueries(world);
        }

        /// <summary>
        /// Finds a minimum-total-travel-cost route from the start location
        /// to the goal location.
        ///
        /// A successful RouteResult should contain the route as location IDs,
        /// including both the start and goal locations.
        /// </summary>
        public RouteResult FindRoute(int startLocationId, int goalLocationId)
        {
            // implement Dijkstra using your prepared world representation
            // and MinPriorityQueue<T>.

            if (_queries.FindLocationById(startLocationId) is null ||
                _queries.FindLocationById(goalLocationId) is null)
                return new RouteResult(false, new List<int>(), 0, 0);

            var dist = new Dictionary<int, int> { [startLocationId] = 0 };
            var prev = new Dictionary<int, int>();
            var closed = new HashSet<int>();
            var frontier = new MinPriorityQueue<int>();
            int expandedCount = 0;

            frontier.Enqueue(startLocationId, 0);

            while (frontier.Count > 0)
            {
                int current = frontier.Dequeue();

                if (!closed.Add(current)) continue;

                expandedCount++;

                if (current == goalLocationId) break;
                int currentDist = dist[current];

                foreach (Connection connection in _queries.FindOutgoingConnections(current))
                {
                    int to = connection.ToLocationId;
                    int newDist = currentDist + connection.TravelCost;

                    if (!dist.TryGetValue(to, out int best) || newDist < best)
                    {
                        dist[to] = newDist;
                        prev[to] = current;
                        frontier.Enqueue(to, newDist);
                    }
                }
            }

            if (!dist.ContainsKey(goalLocationId))
                return new RouteResult(false, new List<int>(), 0, expandedCount);

            var path = new List<int>();
            for (int at = goalLocationId; ; at = prev[at])
            {
                path.Add(at);
                if (at == startLocationId) break;
            }
            path.Reverse();

            return new RouteResult(true, path, dist[goalLocationId], expandedCount);
        }
    }
}
