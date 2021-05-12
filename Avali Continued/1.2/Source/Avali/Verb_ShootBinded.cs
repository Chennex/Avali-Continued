using System;
using RimWorld;
using Verse;

namespace Avali
{
	// Token: 0x02000012 RID: 18
	public class Verb_ShootBinded : Verb_Shoot
	{
		// Token: 0x0600004F RID: 79 RVA: 0x00004864 File Offset: 0x00002A64
		public override bool Available()
		{
			if (base.CasterPawn == null)
			{
				return base.Available();
			}
			this.compWeaponAvali = base.EquipmentSource.TryGetComp<CompRangedWeaponAvali>();
			if (this.compWeaponAvali == null)
			{
				Log.Error(this.ownerPawn + ": CompWeaponAvali is required for Verb_ShootBinded", false);
				return false;
			}
			if (this.ownerPawn == null)
			{
				this.ownerPawn = this.compWeaponAvali.ownerPawn;
				if (this.ownerPawn == null)
				{
					this.ownerPawn = base.CasterPawn;
					this.compWeaponAvali.ownerPawn = base.CasterPawn;
				}
			}
			else if (this.compWeaponAvali.ownerPawn == null)
			{
				this.ownerPawn = base.CasterPawn;
				this.compWeaponAvali.ownerPawn = base.CasterPawn;
			}
			if (this.ownerPawn != null)
			{
				this.currentBindMode = this.compWeaponAvali.currentBindMode;
				if (this.currentBindMode == CompRangedWeaponAvali.bindMode.AnyPawnInFaction.ToString())
				{
					if (this.ownerPawn.Faction != base.CasterPawn.Faction)
					{
						return false;
					}
				}
				else if (this.currentBindMode == CompRangedWeaponAvali.bindMode.OwnerPawnOnly.ToString() && this.ownerPawn != base.CasterPawn)
				{
					return false;
				}
			}
			return base.Available();
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00004994 File Offset: 0x00002B94
		protected override bool TryCastShot()
		{
			bool flag = base.TryCastShot();
			if (flag && base.CasterIsPawn)
			{
				base.CasterPawn.records.Increment(RecordDefOf.ShotsFired);
			}
			return flag;
		}

		// Token: 0x04000038 RID: 56
		private CompRangedWeaponAvali compWeaponAvali;

		// Token: 0x04000039 RID: 57
		public string currentBindMode = CompRangedWeaponAvali.bindMode.OwnerPawnOnly.ToString();

		// Token: 0x0400003A RID: 58
		public Pawn ownerPawn;
	}
}
