using System;
using Verse;

namespace Avali
{
	// Token: 0x02000020 RID: 32
	public class PlaceWorker_WormholePod : PlaceWorker
	{
		// Token: 0x06000088 RID: 136 RVA: 0x00006388 File Offset: 0x00004588
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			Thing thing2 = AvaliUtility.SpecifiedThingAtCellWithDefName(map.thingGrid.ThingsListAtFast(loc), "AvaliWormholePlatform");
			if (thing2 != null && loc == thing2.InteractionCell)
			{
				return true;
			}
			Thing avaliWormholePod = AvaliUtility.SpecifiedThingAtCellWithDefName(map.thingGrid.ThingsListAtFast(new IntVec3(loc.x, loc.y, loc.z + 1)), "AvaliWormholePod");
			if (PlaceWorker_WormholePod.DirectionCorrect(avaliWormholePod, loc))
			{
				return true;
			}
			avaliWormholePod = AvaliUtility.SpecifiedThingAtCellWithDefName(map.thingGrid.ThingsListAtFast(new IntVec3(loc.x, loc.y, loc.z - 1)), "AvaliWormholePod");
			if (PlaceWorker_WormholePod.DirectionCorrect(avaliWormholePod, loc))
			{
				return true;
			}
			avaliWormholePod = AvaliUtility.SpecifiedThingAtCellWithDefName(map.thingGrid.ThingsListAtFast(new IntVec3(loc.x + 1, loc.y, loc.z)), "AvaliWormholePod");
			if (PlaceWorker_WormholePod.DirectionCorrect(avaliWormholePod, loc))
			{
				return true;
			}
			avaliWormholePod = AvaliUtility.SpecifiedThingAtCellWithDefName(map.thingGrid.ThingsListAtFast(new IntVec3(loc.x - 1, loc.y, loc.z)), "AvaliWormholePod");
			if (PlaceWorker_WormholePod.DirectionCorrect(avaliWormholePod, loc))
			{
				return true;
			}
			return new AcceptanceReport("MustPlaceOnWormholePlatformIntCell".Translate());
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000064E8 File Offset: 0x000046E8
		public static bool DirectionCorrect(Thing avaliWormholePod, IntVec3 parentPos)
		{
			if (avaliWormholePod != null)
			{
				if (parentPos.x == avaliWormholePod.Position.x)
				{
					if (avaliWormholePod.Rotation == Rot4.East || avaliWormholePod.Rotation == Rot4.West)
					{
						return true;
					}
				}
				else if (parentPos.z == avaliWormholePod.Position.z && (avaliWormholePod.Rotation == Rot4.North || avaliWormholePod.Rotation == Rot4.South))
				{
					return true;
				}
			}
			return false;
		}
	}
}
