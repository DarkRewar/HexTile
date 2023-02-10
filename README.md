# Hex Tile

[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](./LICENSE.md)

## Purpose

This package is based on the [@ManevilleF](https://github.com/ManevilleF)'s 
[hexx](https://github.com/ManevilleF/hexx) repo, written in Rust.

## Hex Grid Generation

### Hexagon

If you want to generate an hexagonal grid, you can use
[`Lignus.HexTile.Generation.Hexagon()`](./Runtime/Generation.cs#22)

```csharp
int gridRadius = 5;
List<Hex> grid = Generation.Hexagon(gridRadius);

// or
int gridRadius = 5;
Hex center = new(4,5);
List<Hex> grid = Generation.Hexagon(center, gridRadius);
```

### Parallelogram

If you want to generate