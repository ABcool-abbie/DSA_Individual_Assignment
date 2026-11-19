# REPORT.md - Individual Assignment

Keep this concise and specific to your implementation.

## 1. Initial analysis

- How are locations and connections represented in the baseline?
A: Locations and connections are stored in 2 unsorted arrays with no index, thats meant it can only work on linear search.

- Which operations appear frequent?
A: FindLocationById and FindOutgoingConnections appear frequently.

- Which operation(s) do you predict may scale poorly?
A: Operations that use O(n) linear operations e.g. FindOutgoingConnections. It scans all connections and allocates a new List on every call.

- What will you investigate first?
A: I will sort the arrays first (O(log n)) or consider to replace those O(n) operations with a prebuilt adjacency lookup which using a Dictionary.

## 2. Data representation

Describe important changes/retained choices, operations affected, and time/memory trade-offs.

- Important changes: Locations and connections stored in two unsorted arrays which every query had to scan them linearly. So I build two Dictionary indexes (Dictionary<int, Location> + Dictionary<int, List<Connection>>) once now.

- Operations affected: Those operations use average O(1) lookup now instead of O(n) scan which no new list is created for each query.

- Time/memory trade-offs: Building the indexes costs O(n + m) once. After that, queries are average O(1). The extra memory is O(n/m) for the two dictionaries and the per-location lists. The dictionaries are built once and reused, so the setup cost is paid back after only a few queries.


## 3. Algorithms and adaptation

Explain your route finder at a useful high level and how the travel-budget query adapts the same ideas.

- Explain route finder: The route finder uses prepared WorldQueries lookups to get a locations outgoing connections. The frontier is a min-heap of location IDs, a best dictionary with the cheapest known cost to each location and a cameFrom dictionary that remembers the previous location on that cheapest route. When the goal is reached, the search stops early. Then cameFrom is used to trace the route back, and the route is reversed. 

- Travel-budget query: A budget query uses the same structure, but there is no goal, so there is no early stop. Every point that is finally settled is added to the result. An edge is not put into the queue if its cost is over the budget. The check is candidate > budget, so a point with cost exactly equal to the budget is included. Zero-cost edges are also followed. Route reconstruction is not needed here, because only the costs are needed. 

## 4. Complexity

| Operation | Your representation/algorithm | Complexity | Short explanation |
| --- | --- | --- | --- |
| Find location by ID | Dictionary | O(1) | One hash lookup instead of scanning all locations |
| Get outgoing connections | Dictionary | O(1) | Connections are grouped by source once instead of scanning all connections |
| Priority queue enqueue | Binary min-heap | O(log n) | To insert, put the new entry at the end and move it up. This takes at most the height of the heap. |
| Priority queue dequeue-min | Binary min-heap | O(log n) | To remove, take the last entry, put it at the root, and move it down. This also takes at most the height. |
| Route finding | Dijkstra with the binary heap | O(log n) | Each location is finalized only once. Each connection adds at most one entry to the queue. Every queue operation costs O(log n). |

## 5. Correctness and testing

Important tests/edge cases and what they demonstrate.

### Minimum-priority queue tests: 
- Empty queue: Peek and Dequeue throw InvalidOperationException, and Count is 0.
- Single item: Peek does not remove the item, Dequeue removes it, and Count changes correctly.
- Duplicates priorities: items come out in ascending priority, and no duplicate is lost. This checks that the heap invariant still holds after many insertions and removals.

### Dijkstra route finder:
- Start equals goal: found, cost 0, and a route with only the start (supplied test).
- Chooses least cost route: on a small hand-checked world, the cheaper indirect route is chosen. The longer but cheaper route is also chosen.
- Unreachable or unknown goal: a wrong-direction request and unknown start/goal IDs return Found == false and an empty route.
- On small grid: on the 5x5 grid, the reported total cost equals the sum of the connections along the returned route, so the route and the cost agree. The grid has many cycles, so this also shows the search does not loop.

### Reachability query:
- Zero budget: the start at cost 0, plus locations reachable through zero-cost connections.
- Uses total cost within budget: a location with cost exactly equal to the budget is included. A location reachable both directly and indirectly gets the cheaper cost. A location whose cheapest cost is above the budget is excluded. This shows the budget is compared with total cost, not edge count.
- Dead end: a location with no outgoing connections returns only itself.

## 6. Performance investigation

Record reproducible before/after measurements for loading/preparation, repeated runtime work, and relevant allocation behavior. Explain what changed and why.

<!-- ### Baseline (untouched starter)
- Locations: 2500
- Connections: 9800
- Iterations: 5000
- Elapsed: 524 ms
- Checksum: 6267100

### After using Dictionary
- Locations: 2500
- Connections: 9800
- Iterations: 5000
- Elapsed: 0 ms
- Checksum: 6267100 -->

### Baseline (untouched starter)
Baseline world-query workload
Locations: 2500
Connections: 9800
Iterations: 5000
Prepare (CreateMedium + WorldQueries): 1.482 ms
Elapsed: 189 ms
Elapsed (precise): 189.609 ms
Allocated during query loop: 440000 bytes
Checksum: 6267100
Absolute time varies by machine. Keep this result for before/after comparison.

### After using Dictionary
Baseline world-query workload
Locations: 2500
Connections: 9800
Iterations: 5000
Prepare (CreateMedium + WorldQueries): 5.656 ms
Elapsed: 0 ms
Elapsed (precise): 0.444 ms
Allocated during query loop: 0 bytes
Checksum: 6267100
Absolute time varies by machine. Keep this result for before/after comparison.

| Operation / workload | Before | After | Expected complexity | Reason / interpretation |
| --- | --- | --- | --- | --- |
| Find location by ID + get outgoing connections | 189.609 ms | 0.444 ms  | Before: O(n) lookup, O(m) outgoing scan, per query. After: average O(1) each. | Dictionary indexes replace two linear scans. The outgoing-connections scan touched all 9800 connections per call, so it dominated. About 427 times faster. Checksum is identical (6267100). |
| Allocation in the query loop | 440000 bytes | 0 bytes | Before: O(output) allocation per query. After: none. | The starter created a new `List` each call. This size is consistent with one `List` object plus a small backing array. Now a prebuilt list is returned. |
| Load / prepare medium world | 1.482 ms | 5.656 ms | O(n + m) before and after | The indexes are built once, so preparation costs about 4 ms more (same big-O, larger constant). Paid back after roughly 110 iterations. |
## 7. Development reflection

Describe one bug, wrong assumption, failed approach, or surprising result and how you investigated it.
- After I added the dictionary indexes, the supplied timer printed `Elapsed: 0 ms`. I first could not tell if this meant "extremely fast" or "the measurement is broken". So I asked AI for help (because I never use or measure baseline before) and it suggested me to print `sw.Elapsed.TotalMilliseconds` with three decimals, which gave 0.444 ms.

- I also started from my own Lab 4B heap and Lab 6 Dijkstra, but they did not fit the assignment directly. My Lab 4B heap returned a `PriorityEntry<T>` from `PeekNext`/`DequeueNext`. The assignment's `MinPriorityQueue<T>` has private `Entry` struct (`Item`, `Priority`) and `Peek`/`Dequeue` return only the item `T`. I kept the heap logic (sift up, sift down, parent/child indexes) and changed the entry type and the return values. In Lab 6, the frontier returned the entry with its cost, so I skipped stale queue entries with `if (cost > best[current]) continue;`. With the assignment queue I only get the location ID back, so I could not compare costs. I changed to a `closed` set instead: a location is final the first time it is removed and later copies are skipped. Before this fix my first version expanded the same location several times, which also inflated `ExpandedCount`.

## 8. Tools and AI assistance

Briefly describe relevant assistance, if any.
- Reviewed my baseline code and told me what are those and how to measure them.
- Suggested additional test ideas. I then implemented and created the test cases using this suggestion.
- Reviewed my first Dijkstra version and pointed out bugs (a compile error + no handling of repeated queue entries). I then rewrote my version using this feedback.

## 9. VG extension

Only if attempting VG: heuristic, traversal constraints, correctness, and Dijkstra/A* comparison.
