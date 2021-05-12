using System;
using Verse;

namespace Avali
{
	// Token: 0x02000078 RID: 120
	public class PlaceWorker_UnderRoofOnly : PlaceWorker
	{
		// Token: 0x06000261 RID: 609 RVA: 0x000129E0 File Offset: 0x00010BE0
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			if (loc.GetRoof(map) == null)
			{
				return new AcceptanceReport("MustPlaceUnderRoof".Translate());
			}
			if (AvaliUtility.BuildingInPosition(map, loc, checkingDef) != null)
			{
				return new AcceptanceReport("SpaceAlreadyOccupied".Translate());
			}
			return true;
		}
	}
}
