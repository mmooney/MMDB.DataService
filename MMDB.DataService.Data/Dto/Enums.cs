using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;

namespace MMDB.DataService.Data.Dto
{
	public enum JobScheduleType
	{
		[EnumDisplayValue("A regular repeating task")]
		Simple,
		[EnumDisplayValue("A task scheduled to run at one or more fixed times in a day/week/etc")]
		Cron
	}

	public enum EnumJobStatus
	{
		[EnumDisplayValue("New")]
		New,
		[EnumDisplayValue("In Process")]
		InProcess,
		[EnumDisplayValue("Complete")]
		Complete,
		[EnumDisplayValue("Error")]
		Error
	}
}
