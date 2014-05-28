using System;
using Vexe.RuntimeHelpers.Classes;

namespace ShowEmAll
{
	[Serializable]
	public class UnityDateTime : SerializedClass<DateTime>
	{
		public static DateTime Now { get { return DateTime.Now; } }

		public DateTime Date { get { return Value.Date; } set { Value = value; } }
		public int Year { get { return Value.Year; } set { AddYears(value - Year); } }
		public int Month { get { return Value.Month; } set { AddMonths(value - Month); } }
		public int Day { get { return Value.Day; } set { AddDays(value - Day); } }
		public int Hour { get { return Value.Hour; } set { AddHours(value - Hour); } }
		public int Minute { get { return Value.Minute; } set { AddMinutes(value - Minute); } }
		public int Second { get { return Value.Second; } set { AddSeconds(value - Second); } }
		public int Millisecond { get { return Value.Millisecond; } set { AddMilliseconds(value - Millisecond); } }

		public void AddYears(int value)
		{
			Value = Value.AddYears(value);
		}
		public void AddMonths(int value)
		{
			Value = Value.AddMonths(value);
		}
		public void AddDays(double value)
		{
			Value = Value.AddDays(value);
		}
		public void AddHours(double value)
		{
			Value = Value.AddHours(value);
		}
		public void AddMinutes(double value)
		{
			Value = Value.AddMinutes(value);
		}
		public void AddSeconds(double value)
		{
			Value = Value.AddSeconds(value);
		}
		public void AddMilliseconds(double value)
		{
			Value = Value.AddMilliseconds(value);
		}
	}
}