# backend-workflow data contract

The invocation prompt sent to the `backend-workflow` sub-agent and the value it returns are consumed by a machine (the orchestrator or the sub-agent), never read by a human. Both are a single JSON object matching the schemas below — do not wrap them in explanatory prose.

## Invocation input (orchestrator → sub-agent)

Send exactly one JSON object:

```json
{
  "request": "<the back-end change to implement, in the user's terms>",
  "resume": null
}
```

For a brand-new change, set `"resume": null`. The sub-agent is stateless — it re-reads its instructions from the top and retains no memory of the previous turn — so when re-invoking it to continue after a gate, `resume` MUST be fully populated; otherwise it restarts from the Specification phase and duplicates completed work:

```json
{
  "request": "<the original change, unchanged across invocations>",
  "resume": {
    "gate_cleared": "service-name | openapi-spec | red-phase-tests",
    "service_name": "<the user-approved C# identifier; present only when gate_cleared = service-name>",
    "approved": true,
    "resume_phase": "specification | red | green | refactor",
    "resume_step": 9,
    "completed": [
      "<work already done that must NOT be redone, e.g. 'OpenAPI spec updated and approved — do not modify it or regenerate NSwag config', 'Red-phase failing tests written and approved — go straight to Green'>"
    ]
  }
}
```

Include `service_name` only when `gate_cleared` is `service-name`; include `approved: true` only when the cleared gate was an approval gate (`openapi-spec` or `red-phase-tests`). Never invent a service name or set `approved` without the user's explicit input.

## Return value (sub-agent → orchestrator)

Return one JSON object. When stopping at a gate:

```json
{
  "status": "stopped_at_gate",
  "gate": "service-name | openapi-spec | red-phase-tests",
  "review": {
    "summary": "<what the user must provide or review>",
    "files": ["docs/<ServiceName>.yaml", "..."]
  },
  "completed": ["<phases/steps already done this run>"],
  "remaining": "<what remains after the gate is cleared>"
}
```

When the cycle is complete (green and refactored):

```json
{
  "status": "cycle_complete",
  "summary": "<feature, bug fix, or refactoring implemented this cycle>",
  "files_changed": [
    { "path": "app-backend/...", "kind": "production | test", "change": "added | modified" }
  ],
  "ready_for": "manual testing and commit"
}
```
