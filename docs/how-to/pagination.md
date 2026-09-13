# Pagination

Not implemented. REST list endpoints return `count`, `next`, `previous`, and `results`. The client will follow `next` when you use the auto-pagination APIs.

GraphQL (v2) uses Relay cursors with `first` ≤ 100.

See [coding notes](../planning/coding-notes.md).
