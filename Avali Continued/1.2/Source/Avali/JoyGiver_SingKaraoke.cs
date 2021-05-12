using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000054 RID: 84
	public class JoyGiver_SingKaraoke : JoyGiver_InteractBuilding
	{
		// Token: 0x060001CB RID: 459 RVA: 0x0000F6A4 File Offset: 0x0000D8A4
		protected override Job TryGivePlayJob(Pawn pawn, Thing thing)
		{
			CompPowerTrader compPowerTrader = thing.TryGetComp<CompPowerTrader>();
			if (compPowerTrader != null && (compPowerTrader.PowerNet.CurrentStoredEnergy() <= 0f || thing.IsBrokenDown() || !FlickUtility.WantsToBeOn(thing)))
			{
				return null;
			}
			Room room = pawn.GetRoom(RegionType.Set_Passable);
			if (room != null)
			{
				List<Building_Bed> list = room.ContainedBeds.ToList<Building_Bed>();
				for (int i = 0; i < list.Count<Building_Bed>(); i++)
				{
					Building_Bed building_Bed = list[i];
					if (building_Bed.OwnersForReading != null)
					{
						for (int j = 0; j < building_Bed.OwnersForReading.Count; j++)
						{
							Pawn p = building_Bed.OwnersForReading[j];
							if (p.CurrentBed() == building_Bed)
							{
								return null;
							}
						}
					}
				}
			}
			List<IntVec3> list2 = WatchBuildingUtility.CalculateWatchCells(thing.def, thing.Position, thing.Rotation, thing.Map).ToList<IntVec3>();
			for (int k = 0; k < list2.Count<IntVec3>(); k++)
			{
				int index = Rand.RangeInclusive(0, list2.Count<IntVec3>());
				IntVec3 c = list2[index];
				if (c.Standable(pawn.Map))
				{
					return new Job(this.def.jobDef, thing, c);
				}
			}
			return null;
		}
	}
}
