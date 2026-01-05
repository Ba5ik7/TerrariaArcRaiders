# Contract: Indicator Legend

**Feature**: `004-in-world-visual-indicators`  
**Date**: 2026-01-05

## Purpose

Defines how a developer can interpret the in-world markers and map them back to Arc worldgen stages without requiring log access.

## Contract

- Each Arc worldgen stage has a unique, distinguishable marker.
- There is a documented legend describing:
  - Stage identifier (A–H)
  - Stage name (human readable)
  - Marker appearance (what to look for)
  - Marker placement rule (where it will appear relative to the planned hub region)

## Acceptance Notes

- A developer can identify the stage associated with each marker by using this legend alone.
