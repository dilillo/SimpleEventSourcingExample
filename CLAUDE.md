# SimpleEventSourcingExample

> **BMAD Method** (agile, party-mode, multi-agent) + **Spec-Driven Development** (specify → plan → GAPS → tasks → TDD)

## Project Overview

This project uses BMAD-Speckit-SDD-Flow: the BMAD Method's agile workflows combined with Spec-Driven Development to create a rigorous, auditable AI-assisted development process.

### Core Principles

- **Five-layer architecture**: Product Brief → PRD → Architecture → Epic/Story → speckit specify/plan/GAPS/tasks → TDD implement → PR + human review
- **Mandatory audit loops**: Each stage requires code-review pass before proceeding
- **Critical Auditor**: Dedicated challenger role with >60% share in party-mode
- **TDD Red-Green-Refactor**: Strict test-driven development

---

## Development Workflows

### 1. Speckit Workflow (Spec-Driven Development)

**Phases** (must pass audit before proceeding):

1. **constitution** — Establish project principles, tech stack, architecture constraints
2. **specify** — Convert requirements to technical spec → `spec.md`
3. **plan** — Create implementation plan → `plan.md`
4. **GAPS** — Deep analysis for implementation gaps → `IMPLEMENTATION_GAPS.md`
5. **tasks** — Generate executable tasks → `tasks.md`
6. **implement** — Execute tasks using TDD Red-Green-Refactor

### 2. BMAD Story Assistant (Five-Layer Architecture)

**Purpose**: Full story lifecycle from Epic to implementation

**Commands**:
- `/bmad-bmm-create-story` - Create story from Epic
- `/bmad-bmm-dev-story` - Develop story (triggers speckit workflow)

### 3. BMAD Bug Assistant

**Purpose**: Structured bug fixing with Party-Mode

**Flow**: Bug description → Party-Mode → BUGFIX doc → Generate tasks → TDD fix

### 4. Standalone Tasks

**Purpose**: Execute existing TASKS/BUGFIX documents

---

## Testing

```bash
# Run all tests
npm test
```

### TDD Requirements (15 Ironclad Rules)

1. Architecture fidelity - follow plan.md architecture
2. No placeholder implementations
3. Active regression testing
4. TodoWrite for task tracking
5. Strict TDD: RED → GREEN → IMPROVE
6. No skipping refactoring
7. Verify before marking complete
8. One test failure at a time
9. Incremental implementation
10. Proper error handling
11. Input validation at boundaries
12. No hardcoded values
13. Functions < 50 lines
14. Files < 800 lines
15. No mutation (immutable patterns)

---

## Agent System

### Layer 4 Agents (Speckit Phases)

| Agent | Purpose | File |
|-------|---------|------|
| bmad-layer4-speckit-specify | Spec generation | `.claude/agents/layers/bmad-layer4-speckit-specify.md` |
| bmad-layer4-speckit-plan | Plan generation | `.claude/agents/layers/bmad-layer4-speckit-plan.md` |
| bmad-layer4-speckit-gaps | GAPS analysis | `.claude/agents/layers/bmad-layer4-speckit-gaps.md` |
| bmad-layer4-speckit-tasks | Tasks generation | `.claude/agents/layers/bmad-layer4-speckit-tasks.md` |
| bmad-layer4-speckit-implement | Task execution | `.claude/agents/layers/bmad-layer4-speckit-implement.md` |

### Auditor Agents

| Agent | Purpose | File |
|-------|---------|------|
| auditor-spec | Spec audit | `.claude/agents/auditors/auditor-spec.md` |
| auditor-plan | Plan audit | `.claude/agents/auditors/auditor-plan.md` |
| auditor-tasks | Tasks audit | `.claude/agents/auditors/auditor-tasks.md` |
| auditor-implement | Implementation audit | `.claude/agents/auditors/auditor-implement.md` |

---

## Best Practices & Conventions

### Code Style

- **Immutability**: Always create new objects, never mutate existing ones
- **File organization**: Many small files > few large files (200-400 lines typical, 800 max)
- **Error handling**: Handle errors explicitly at every level
- **Input validation**: Validate at system boundaries

### Git Worktree Convention

When creating git worktrees, **always** place them in a sibling directory of the project root:

```
# CORRECT
../SimpleEventSourcingExample-01-feature-name/

# WRONG — inside project root
.claude/worktrees/agent-xxx
```

**Naming rule**: `{repo-name}-{two-digit-index}-{kebab-slug}`

### Hooks Architecture

This project uses Claude Code CLI hooks (`.claude/settings.json`) for runtime enforcement.

| Hook | Event | Purpose |
|------|-------|---------|
| `pre-agent-summary.js` | `PreToolUse` (Agent) | Displays calling summary before subagent launch |
| `subagent-milestone-init.js` | `SubagentStart` | Initializes milestone tracking |
| `subagent-result-summary.js` | `SubagentStop` | Displays result summary when subagent finishes |
| `worktree-create-sibling.js` | `WorktreeCreate` | Redirects worktree creation to sibling directories |

**Quiet mode**: Set `BMAD_HOOKS_QUIET=1` to suppress stderr hook output when the model natively displays agent info.

### Git Workflow

```
<type>: <description>
```

Types: `feat`, `fix`, `refactor`, `docs`, `test`, `chore`, `perf`, `ci`

### Audit Requirements

- Each stage MUST call code-review skill
- No self-approval allowed
- Iteration required if audit fails
- Only proceed after explicit "验证通过"

### State Management

- Use `.speckit-state.yaml` for speckit phases
- Use `TodoWrite` for task tracking
- Use handoff documents for stage transitions
