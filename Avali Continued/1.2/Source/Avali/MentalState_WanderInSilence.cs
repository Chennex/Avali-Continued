using System;
using RimWorld;

namespace Avali
{
	// Token: 0x02000058 RID: 88
	public class MentalState_WanderInSilence : MentalState_RaceDependant
	{
		// Token: 0x060001D5 RID: 469 RVA: 0x00002E48 File Offset: 0x00001048
		public override RandomSocialMode SocialModeMax()
		{
			return RandomSocialMode.Off;
		}
	}
}
