# Contract: Indicator Setting

**Feature**: `004-in-world-visual-indicators`  
**Date**: 2026-01-05

## Purpose

Defines the single opt-in control that enables or disables in-world worldgen visual indicators.

## Contract

- A single boolean setting exists: “Enable worldgen visual indicators”.
- Default value is disabled.
- When disabled, the system must not place any indicators during world generation.
- The setting affects Arc worlds only; non-Arc worlds never receive indicators.

## Acceptance Notes

- Can be validated by creating Arc worlds with the setting toggled on/off and inspecting the generated world.
