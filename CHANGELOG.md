# Changelog

## 0.1.0-pre

+ Add `HexGridEditor` to create grids in editor ;
+ Add `Layout.GetHexCorners3(List<Hex> hexes)` to get corners of many tiles as one footprint ;
+ Add `Layout.GetHexCorners3(Hex hex)` to get corners of a tile as `Vector3[]` ;
+ Fix `Hex.Ring` method which was creating too large offsets ; 
- Rename `Layout.HexCorners` method to `Layout.GetHexCorners` ;

## 0.0.2-pre

+ Add `Layout.HexCorners(Hex hex)` to get corners of a tile as `Vector2[]` ;
+ Optimizing `Hex` struct (with `IEquitable<Hex>`) to be retrieved faster by `IEnumerable.FindItem` ;
 
## 0.0.1-pre

+ Add `Hex` struct and logics ;