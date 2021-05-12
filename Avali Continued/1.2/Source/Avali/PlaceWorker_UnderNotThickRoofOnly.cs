using System;
using Verse;

namespace Avali
{
	// Token: 0x0200001F RID: 31
	public class PlaceWorker_UnderNotThickRoofOnly : PlaceWorker
	{
		// Token: 0x06000086 RID: 134 RVA: 0x00006314 File Offset: 0x00004514
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			RoofDef roof = loc.GetRoof(map);
			if (roof == null)
			{
				return new AcceptanceReport("MustPlaceUnderRoof".Translate());
			}
			if (roof.isThickRoof)
			{
				return new AcceptanceReport("MustPlaceUnderNotThickRoof".Translate());
			}
			if (AvaliUtility.BuildingInPosition(map, loc, checkingDef) != null)
			{
				return new AcceptanceReport("SpaceAlreadyOccupied".Translate());
			}
			return true;
		}
	}
}
