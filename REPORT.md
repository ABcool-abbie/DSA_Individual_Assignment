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

## 3. Algorithms and adaptation

Explain your route finder at a useful high level and how the travel-budget query adapts the same ideas.

## 4. Complexity

| Operation | Your representation/algorithm | Complexity | Short explanation |
| --- | --- | --- | --- |
| Find location by ID | | | |
| Get outgoing connections | | | |
| Priority queue enqueue | | | |
| Priority queue dequeue-min | | | |
| Route finding | | | |

## 5. Correctness and testing

Important tests/edge cases and what they demonstrate.

## 6. Performance investigation

Record reproducible before/after measurements for loading/preparation, repeated runtime work, and relevant allocation behavior. Explain what changed and why.

### Baseline (untouched starter)
Baseline world-query workload
- Locations: 2500
- Connections: 9800
- Iterations: 5000
- Elapsed: 524 ms

## 7. Development reflection

Describe one bug, wrong assumption, failed approach, or surprising result and how you investigated it.

## 8. Tools and AI assistance

Briefly describe relevant assistance, if any.

## 9. VG extension

Only if attempting VG: heuristic, traversal constraints, correctness, and Dijkstra/A* comparison.
