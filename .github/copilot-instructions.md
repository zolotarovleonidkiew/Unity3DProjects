# Copilot Instructions

## Project Guidelines
- For obstacle climbing validation, when ObstacleOnTheMap.AllowedClimbing is true and Climbing_from_coords/Climbing_to_coords are populated, the WayPoint at Climbing_from_coords must allow movement in the direction of Climbing_to_coords.
- A SmallBox whose GameObject name starts with `RampTopBox_` must allow movement in all four directions; do not infer ramp entry sides from RampMarker or coordinates for this rule.
- Ramp movement validation must respect Ramp.allowedMovements and Ramp.disallowedMovements coordinate collections: these coordinates identify cells from which entering the ramp is allowed or forbidden when updating the ramp WayPoint and neighboring cells. For each listed source cell in Ramp.allowedMovements, the validator must identify the adjacent RampTopBox and set that source WayPoint's direction toward the ramp to Allowed=true; this explicit allow must not be skipped or overridden by generic obstacle logic. For Ramp.allowedMovements, take each listed source SmallBox coordinate, find that SmallBox and its WayPoint.AvailableDirections, determine whether the ramp's RampTopBox is Up/Down/Left/Right from that source coordinate, then set only that direction's Allowed=true.

## Small Boxes Validation
- Refactor SmallBoxesValidation into four private passes called sequentially: 
  - ValidateLayerSmallBoxed for ground bounds
  - ValidateObstacleSmallBoxed for blocking movement toward obstacles; explicitly process TopSmallBox cells as obstacle-top surfaces, handling adjacent TopSmallBox movement there, with climbing and ramp passes applying later overrides.
  - ValidateObstacleClimbing for climb transitions
  - ValidateRampsSmallBoxed for applying ramp allowed/disallowed coordinate movement rules.