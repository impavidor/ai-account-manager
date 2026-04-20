---
name: issue-to-code
description: Implement a local issue using test-driven development. Use when given an open local issue file, want to implement an issue end-to-end with TDD, or need to convert an issue into working code.
---

# Issue to Code

Convert a local issue into working code using test-driven development (tracer bullets).

## Process

### 1. Load the issue

Read the local issue file from the `issues/` folder. Extract:

- **Title**: Issue name
- **What to build**: End-to-end behavior description
- **Acceptance criteria**: Checklist of behaviors to verify
- **Blocked by**: Any dependency issues (confirm they're completed)
- **Notes**: Design decisions or implementation hints

Confirm the issue is `Open` status and no blockers remain incomplete.

### 2. Update issue status

Rename the issue file from `Open-{title}.md` to `In-Progress-{title}.md`.

### 3. Plan the TDD approach

Ask the user:

- Does the acceptance criteria list make sense as testable behaviors?
- Should any criteria be split or combined?
- Which behaviors are highest priority?
- Are there edge cases to test?
- What's the target module/file for this implementation?

### 4. Execute with TDD

Use the **/tdd skill** to implement each acceptance criterion using the tracer bullet workflow (RED→GREEN→REFACTOR for each test).

### 5. Verify and close

- [ ] All acceptance criteria checkboxes ticked
- [ ] Code reviewed for quality (no cruft, SOLID principles applied)
- [ ] Tests pass locally
- [ ] Behavior matches "What to build" description

Rename issue file from `In-Progress-{title}.md` to `Done-{title}.md`.

## Tips

- **One test per criterion** - Keep tests focused to one acceptance criterion at a time
- **Ask for clarification** - If acceptance criteria are ambiguous, get user input before coding
