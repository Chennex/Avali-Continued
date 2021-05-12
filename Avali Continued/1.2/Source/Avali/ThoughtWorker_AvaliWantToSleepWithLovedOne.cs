using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Avali
{
	// Token: 0x0200005C RID: 92
	public class ThoughtWorker_AvaliWantToSleepWithLovedOne : ThoughtWorker
	{
		// Token: 0x060001E3 RID: 483 RVA: 0x00010164 File Offset: 0x0000E364
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			DirectPawnRelation directPawnRelation = LovePartnerRelationUtility.ExistingMostLikedLovePartnerRel(p, false);
			if (directPawnRelation == null)
			{
				return false;
			}
			if (!directPawnRelation.otherPawn.IsColonist || directPawnRelation.otherPawn.IsWorldPawn() || !directPawnRelation.otherPawn.relations.everSeenByPlayer)
			{
				return false;
			}
			if (p.relations.OpinionOf(directPawnRelation.otherPawn) <= 0)
			{
				return false;
			}
			Building_Bed ownedBed = p.ownership.OwnedBed;
			if (ownedBed != null)
			{
				Room room = ownedBed.GetRoom(RegionType.Set_Passable);
				if (room != null && (room.Role == RoomRoleDefOf.Barracks || room.Role == RoomRoleDefOf.Bedroom))
				{
					List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
					for (int i = 0; i < containedAndAdjacentThings.Count; i++)
					{
						Thing thing = containedAndAdjacentThings[i];
						Building_Bed building_Bed = thing as Building_Bed;
						if (building_Bed != null)
						{
							for (int j = 0; j < building_Bed.OwnersForReading.Count; j++)
							{
								Pawn pawn = building_Bed.OwnersForReading[j];
								if (pawn == directPawnRelation.otherPawn)
								{
									return false;
								}
							}
						}
					}
				}
			}
			return true;
		}
	}
}
