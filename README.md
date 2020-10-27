# Avali Continued
 Continuation of Avali Continued mod for Rimworld. Currently looking for others to work together with on this.
 
 Patch notes:
 
 	-Updated about.xml for update 1.2.
	-Fixed issues regarding things_defs_races (thank you, bRektFest!)
	-Fixed Avali specific moods being applied to non-avali.
	-Changed Avali pack formation.
	-Existing race check only checked pawn.def, not pawn.def.race. New check fixes this.
	-Fixed bug where if multiple pawns were ready to join a pack at the same time, they would all join, bypassing the max pack size check. Existing check for this was unreachable code.
	-Probably? Fixed lag caused by hacking weapon. This is not tested yet.
	-Added null pointer check to UpdatePackHediff, hopefully fixing an issue with lag that has not been reproducible.
	-Fixed issue where Avali would break forcing anyone to wear clothes, also non-avali.

Balance changes:

-Avali running speed slightly reduced.


Todo:
Apparently there is a bug where raiders try to rescue downed pawns and are unable to leave the map. Haven't been able to reproduce, need some logs.
Texture glitch running sideways on running track. No idea what's causing this.
Fix some debug log warnings that come up due to improperly implementing support for various mods. The warnings only come up if you do not have those mods, and they do not affect anything.
The advanced armors are currently disabled. These need to be undisabled and the issues with them fixed.
