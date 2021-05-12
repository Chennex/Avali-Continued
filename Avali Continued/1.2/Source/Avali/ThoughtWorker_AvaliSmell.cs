using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Avali
{
	// Token: 0x02000021 RID: 33
	public class ThoughtWorker_AvaliSmell : ThoughtWorker
	{
		// Token: 0x0600008B RID: 139 RVA: 0x00006570 File Offset: 0x00004770
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (p.def == ThingDefOfAvali.Avali)
			{
				return ThoughtState.Inactive;
			}
			List<Hediff> hediffs = p.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				Hediff hediff = hediffs[i];
				if (hediff != null && hediff.Part != null && hediff.Part.Label == "nose" && hediff.def == HediffDefOf.MissingBodyPart)
				{
					return ThoughtState.Inactive;
				}
			}
			List<Thing> list = new List<Thing>();
			Room room = p.GetRoom(RegionType.Set_Passable);
			if (room.PsychologicallyOutdoors)
			{
				return ThoughtState.Inactive;
			}
			List<Pawn> list2 = p.GetRoom(RegionType.Set_Passable).Map.mapPawns.FreeColonistsAndPrisoners.ToList<Pawn>();
			for (int j = 0; j < list2.Count; j++)
			{
				Thing thing = list2[j];
				if (thing != null)
				{
					list.Add(thing);
				}
			}
			if (list.Count == 0)
			{
				return ThoughtState.Inactive;
			}
			int num = AvaliUtility.FindAllThingsOnMapAtRange(p, ThingDefOfAvali.Avali, null, list, 3f, 6, false, false).Count;
			if (num > 0)
			{
				num--;
				if (num > 5)
				{
					num = 5;
				}
				return ThoughtState.ActiveAtStage(num);
			}
			return ThoughtState.Inactive;
		}

		// Token: 0x04000065 RID: 101
		private const int rangeInCells = 3;

		// Token: 0x04000066 RID: 102
		private const int maxAvaliPawns = 6;
	}
}
