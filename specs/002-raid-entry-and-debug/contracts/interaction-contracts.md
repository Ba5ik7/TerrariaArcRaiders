# Contracts: Raid Entry & Debug Controls

**Feature**: [../spec.md](../spec.md)
**Date**: 2026-01-04

These are *in-game interaction contracts* (not HTTP APIs). They document stable user-facing triggers and their expected behavior.

## Contract A: Hub Console Interaction

### Trigger

- Player right-clicks the hub console NPC.

### Behavior

- If the player is **not** in a raid:
  - Selecting “Enter Raid” attempts to start a raid session.
  - On success: player receives a confirmation message.
  - On refusal: player receives a clear refusal message.
- If the player **is** in a raid:
  - Selecting “Extract / Exit Raid” attempts to extract/end the session.
  - On success: player receives a confirmation message and is returned to hub context.
  - On refusal: player receives a clear refusal message.

### Non-goals

- No loot rules, no new raid map generation, no UI beyond standard NPC chat.

## Contract B: Dev Command

### Command

- `/arcraid <enter|exit|toggle>`

### Preconditions

- Dev tools must be enabled (config gate).

### Behavior

- `enter`: attempts to start a raid session for the invoking player.
- `exit`: attempts to extract/end the raid session for the invoking player.
- `toggle`: enters if not in raid; otherwise exits.

### Failure behavior

- If dev tools are disabled: command refuses and prints a short message.
- If action is invalid: command prints usage.
