using System;
using Verse;

namespace Avali
{
	// Token: 0x02000059 RID: 89
	public class PlaceWorker_NearTallBuildingOnly : PlaceWorker
	{
		// Token: 0x060001D7 RID: 471 RVA: 0x0000F8B0 File Offset: 0x0000DAB0
		public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 center, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			Thing thing2 = map.thingGrid.ThingAt(center, ThingCategory.Building);
			if (thing2 != null && (thing2.def == def || thing2.def.holdsRoof || thing2.def.blockLight))
			{
				return new AcceptanceReport("SpaceAlreadyOccupied".Translate());
			}
			int asInt = rot.AsInt;
			if (asInt == 2)
			{
				IntVec3 c = new IntVec3(center.x + 1, center.y, center.z);
				if (!c.Walkable(map))
				{
					c = new IntVec3(center.x + 1, center.y, center.z - 1);
					if (!c.Walkable(map))
					{
						return true;
					}
				}
				c = new IntVec3(center.x - 1, center.y, center.z);
				if (!c.Walkable(map))
				{
					c = new IntVec3(center.x - 1, center.y, center.z - 1);
					if (!c.Walkable(map))
					{
						return true;
					}
				}
				c = new IntVec3(center.x, center.y, center.z + 1);
				if (!c.Walkable(map))
				{
					c = new IntVec3(center.x, center.y, center.z - 2);
					if (!c.Walkable(map))
					{
						return true;
					}
				}
			}
			else if (asInt == 0)
			{
				IntVec3 c2 = new IntVec3(center.x + 1, center.y, center.z);
				if (!c2.Walkable(map))
				{
					c2 = new IntVec3(center.x + 1, center.y, center.z + 1);
					if (!c2.Walkable(map))
					{
						return true;
					}
				}
				c2 = new IntVec3(center.x - 1, center.y, center.z);
				if (!c2.Walkable(map))
				{
					c2 = new IntVec3(center.x - 1, center.y, center.z + 1);
					if (!c2.Walkable(map))
					{
						return true;
					}
				}
				c2 = new IntVec3(center.x, center.y, center.z - 1);
				if (!c2.Walkable(map))
				{
					c2 = new IntVec3(center.x, center.y, center.z + 2);
					if (!c2.Walkable(map))
					{
						return true;
					}
				}
			}
			else if (asInt == 1)
			{
				IntVec3 c3 = new IntVec3(center.x, center.y, center.z + 1);
				if (!c3.Walkable(map))
				{
					c3 = new IntVec3(center.x + 1, center.y, center.z + 1);
					if (!c3.Walkable(map))
					{
						return true;
					}
				}
				c3 = new IntVec3(center.x, center.y, center.z - 1);
				if (!c3.Walkable(map))
				{
					c3 = new IntVec3(center.x + 1, center.y, center.z - 1);
					if (!c3.Walkable(map))
					{
						return true;
					}
				}
				c3 = new IntVec3(center.x + 2, center.y, center.z);
				if (!c3.Walkable(map))
				{
					c3 = new IntVec3(center.x - 1, center.y, center.z);
					if (!c3.Walkable(map))
					{
						return true;
					}
				}
			}
			else if (asInt == 3)
			{
				IntVec3 c4 = new IntVec3(center.x, center.y, center.z + 1);
				if (!c4.Walkable(map))
				{
					c4 = new IntVec3(center.x - 1, center.y, center.z + 1);
					if (!c4.Walkable(map))
					{
						return true;
					}
				}
				c4 = new IntVec3(center.x, center.y, center.z - 1);
				if (!c4.Walkable(map))
				{
					c4 = new IntVec3(center.x - 1, center.y, center.z - 1);
					if (!c4.Walkable(map))
					{
						return true;
					}
				}
				c4 = new IntVec3(center.x - 2, center.y, center.z);
				if (!c4.Walkable(map))
				{
					c4 = new IntVec3(center.x + 1, center.y, center.z);
					if (!c4.Walkable(map))
					{
						return true;
					}
				}
			}
			return "MustPlaceNearTallBuildingOnly".Translate();
		}
	}
}
