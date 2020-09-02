using System;

namespace Common.Support.ThreadExt
{
	/// <summary>
	/// 
	/// </summary>
	public class Timecard
	{		
		public Timecard(TimeSpan timeInterval)
		{
			// 
			// TODO: Add constructor logic here
			//
			m_startTime=DateTime.Now;
			m_timeInterval=timeInterval;
			m_isInterval=true;
		}
		public Timecard(DateTime startTime,DateTime endTime,TimeSpan timeInterval)
		{
			m_isInterval=false;
			m_startTime=startTime;
			m_endTime=endTime;
			m_timeInterval=timeInterval;
		}
		public bool TryGoNextMilestone()
		{	
			DateTime now=DateTime.Now;
			if(now>=m_startTime)
			{
				//m_startTime+=m_timeInterval;
				m_startTime=now+m_timeInterval;
				return m_isInterval?true : (now<=m_endTime);
			}
			return false;
		}
		//*******************************************************************
		private DateTime m_startTime;
		private DateTime m_endTime;
		private TimeSpan m_timeInterval;
		private bool m_isInterval=true;
	}
}
