---
name: to-local-issue
description: Break a plan, spec, or PRD into independently-grabbable local issues as markdown files using tracer-bullet vertical slices. Use when user wants to convert a plan into local issues, create local implementation tickets, or break down work into tracked issues stored in the project's issues folder.
---

# To Local Issue

Break a plan into independently-grabbable local issue files using vertical slices (tracer bullets).

## Process

### 1. Gather context

Work from whatever is already in the conversation context. Use existing plans, specs, or PRDs.

### 2. Explore the codebase (optional)

If you have not already explored the codebase, do so to understand the current state of the code.

### 3. Draft vertical slices

Break the plan into **tracer bullet** issues. Each issue is a thin vertical slice that cuts through ALL integration layers end-to-end, NOT a horizontal slice of one layer.

Slices may be 'HITL' or 'AFK'. HITL slices require human interaction, such as an architectural decision or a design review. AFK slices can be implemented and merged without human interaction. Prefer AFK over HITL where possible.

<vertical-slice-rules>
- Each slice delivers a narrow but COMPLETE path through every layer (schema, API, UI, tests)
- A completed slice is demoable or verifiable on its own
- Prefer many thin slices over few thick ones
</vertical-slice-rules>

### 4. Quiz the user

Present the proposed breakdown as a numbered list. For each slice, show:

- **Title**: short descriptive name
- **Type**: HITL / AFK
- **Blocked by**: which other slices (if any) must complete first
- **User stories covered**: which user stories this addresses (if the source material has them)

Ask the user:

- Does the granularity feel right? (too coarse / too fine)
- Are the dependency relationships correct?
- Should any slices be merged or split further?
- Are the correct slices marked as HITL and AFK?

Iterate until the user approves the breakdown.

### 5. Create local issue files

For each approved slice, create a markdown file in the `issues/` folder using the filename pattern: `Open-{kebab-case-title}.md`

Create files in dependency order (blockers first) so you can reference real file names in the "Blocked by" field.

<issue-template>
# {Issue Title}

## Status

Open

## What to build

A concise description of this vertical slice. Describe the end-to-end behavior, not layer-by-layer implementation.

## Acceptance criteria

- [ ] Criterion 1
- [ ] Criterion 2
- [ ] Criterion 3

## Blocked by

- Blocked by [Issue Name](Open-issue-name.md) (if any)

Or "None - can start immediately" if no blockers.

## Notes

[Additional context, design decisions, or implementation hints]

</issue-template>

### 6. Track progress

Update issue files as work progresses. Change filename prefix from `Open-` to `In-Progress-` when work begins, and `Done-` when completed.

## Tips

- Use consistent kebab-case for filenames
- Cross-reference related issues with markdown links
- Keep acceptance criteria specific and verifiable
- Document assumptions and design decisions in Notes section
