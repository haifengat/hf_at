using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HaiFeng
{

	public class ConfigATP
	{
		[Category("配置"), DisplayName("追单设置"), Browsable(true)]
		public FollowConfig FloConfig { get; set; } = new FollowConfig();

		//public RunTime[] RunTimes { get; set; } = new[] { new RunTime { Begin = TimeSpan.Parse("08:45:00"), End = TimeSpan.Parse("15:30:00")} };
		[Category("7*24"), DisplayName("开始时间I")]
		public DateTime OpenTime1 { get; set; } = DateTime.Today.Add(TimeSpan.Parse("08:40:00"));

		[Category("7*24"), DisplayName("开始时间II")]
		public DateTime OpenTime2 { get; set; } = DateTime.Today.Add(TimeSpan.Parse("20:40:00"));
	}
}
