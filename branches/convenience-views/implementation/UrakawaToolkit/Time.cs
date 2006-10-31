using System;

namespace urakawa.media
{
	/// <summary>
	/// The Time object represents a timestamp.  
	/// </summary>
	public class Time : ITime
	{
		private TimeSpan mTime;

    /// <summary>
    /// Default constructor initializing the instance to 0
    /// </summary>
		public Time()
		{
			mTime = TimeSpan.Zero;
		}

    /// <summary>
    /// Constructor initializing the instance with a given number of milliseconds
    /// </summary>
    /// <param name="val">The given number of milliseconds</param>
		public Time(long val)
		{
			setTime(val);
		}

		/// <summary>
		/// Constructor initializing the instance with a given number of milliseconds
		/// </summary>
		/// <param name="val">The given number of milliseconds</param>
		public Time(double val)
		{
			setTime(val);
		}

    /// <summary>
    /// Constructor initializing the instance with a given <see cref="TimeSpan"/>
    /// value
    /// </summary>
    /// <param name="val">The given <see cref="TimeSpan"/> value</param>
		public Time(TimeSpan val)
		{
			setTime(val);
		}

    /// <summary>
    /// Constructor initializing the instance with a given <see cref="string"/>
    /// representation of time.
    /// <see cref="getTimeAsString"/> member method of a description of the format 
    /// of the string representation.
    /// </summary>
    /// <param name="val">The <see cref="string"/> representation</param>
		public Time(string val)
		{
			setTime(Time.Parse(val).mTime);
		}

    /// <summary>
    /// Returns the <see cref="TimeSpan"/> equivalent of the instance
    /// </summary>
    /// <returns>The <see cref="TimeSpan"/> equivalent</returns>
		public TimeSpan getTime()
		{
			return mTime;
		}

    /// <summary>
    /// Sets the time to a given <see cref="TimeSpan"/> value
    /// </summary>
    /// <param name="newTime">The <see cref="TimeSpan"/> value</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="newTime"/> is <c>null</c>
		/// </exception>
		public void setTime(TimeSpan newTime)
		{
			if (newTime == null)
			{
				throw new exception.MethodParameterIsNullException("The time can not be null");
			}
			mTime = newTime;
		}

		/// <summary>
		/// Gets a string representation of the <see cref="Time"/>
		/// </summary>
		/// <returns>The string representation</returns>
		/// <remarks>
		/// The format of the string representation [-][d.]hh:mm:ss[.f],
		/// where d is a number of days, hh is two-digit hours between 00 and 23,
		/// mm is two-digit minutes between 00 and 59, 
		/// ss is two-digit seconds between 00 and 59 
		/// and where f is the second fraction with between 1 and 7 digits
		/// </remarks>
		public override string ToString()
		{
			return mTime.ToString();
		}

		/// <summary>
		/// Parses a string representation of a <see cref="Time"/>. 
		/// See <see cref="ToString"/> for a description of the format of the string representation
		/// </summary>
		/// <param name="stringRepresentation">The string representation</param>
		/// <returns>The parsed <see cref="Time"/></returns>
		/// <exception cref="exception.TimeStringRepresentationIsInvalidException">
		/// Thrown then the given string representation is not valid
		/// </exception>
		public static Time Parse(string stringRepresentation)
		{
			if (stringRepresentation == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not parse a null string");
			}
			if (stringRepresentation == String.Empty)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not parse an empty string");
			}
			try
			{
				return new Time(TimeSpan.Parse(stringRepresentation));
			}
			catch (Exception e)
			{
				throw new exception.TimeStringRepresentationIsInvalidException(
					"The string \"{0}\" is not a valid string representation of a Time");
			}
		}

		#region ITime Members

    /// <summary>
    /// Determines if the instance represents a negative time value
    /// </summary>
    /// <returns><c>true</c> if negative, <c>false</c> else</returns>
		public bool isNegativeTimeOffset()
		{
      return (mTime<TimeSpan.Zero);
		}

    /// <summary>
    /// Creates a copy of the <see cref="Time"/> instance
    /// </summary>
    /// <returns>The copy</returns>
		public ITime copy()
		{
			return new Time(mTime);
		}

		/// <summary>
		/// Gets the (signed) <see cref="ITimeDelta"/> between a given <see cref="ITime"/> and <c>this</c>,
		/// that is <c>this-<paramref name="t"/></c>
		/// </summary>
		/// <param name="t">The given <see cref="ITime"/></param>
		/// <returns>
		/// The difference as an <see cref="ITimeDelta"/>
		/// </returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="t"/> is <c>null</c>
		/// </exception>
		public ITimeDelta getTimeDelta(ITime t)
		{

			if (t == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The time with which to compare can not be null");
			}
			if (t is Time)
			{
				Time otherTime = (Time)t;
				if (mTime > otherTime.mTime)
				{
					return new TimeDelta(mTime.Subtract(otherTime.mTime));
				}
				else
				{
					return new TimeDelta(otherTime.mTime.Subtract(mTime));
				}
			}
			else
			{
				double msDiff = getTimeAsMillisecondFloat() - t.getTimeAsMillisecondFloat();
				if (msDiff < 0) msDiff = -msDiff;
				return new TimeDelta(msDiff);
			}
		}

		public long getTimeAsMilliseconds()
		{
			return mTime.Ticks / TimeSpan.TicksPerMillisecond;
		}

		public double getTimeAsMillisecondFloat()
		{
			return ((double)mTime.Ticks) / ((double)TimeSpan.TicksPerMillisecond);
		}

		/// <summary>
		/// Sets the time to a given number of milliseconds
		/// </summary>
		/// <param name="newTime">The number of milliseconds</param>
		public void setTime(long newTime)
		{
			mTime = TimeSpan.FromTicks(newTime * TimeSpan.TicksPerMillisecond);
		}

		/// <summary>
		/// Sets the time to a given number of milliseconds
		/// </summary>
		/// <param name="newTime">The number of milliseconds</param>
		public void setTime(double newTime)
		{
			mTime = TimeSpan.FromTicks((long)(newTime * TimeSpan.TicksPerMillisecond));
		}

		/// <summary>
		/// Determines is <c>this</c> is greater than a given other <see cref="ITime"/>.
		/// </summary>
		/// <param name="otherTime">The other <see cref="ITime"/></param>
		/// <returns>
		/// <c>true</c> if <c>this</c> is greater than <paramref name="otherTime"/>, otherwise <c>false</c>
		/// </returns>
		/// <exception cref="">
		/// Thrown when <paramref name="otherTime"/> is <c>null</c>
		/// </exception>
		public bool isGreaterThan(ITime otherTime)
		{
			if (otherTime == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not compare to a null ITime");
			}
			if (otherTime is Time)
			{
				return mTime > ((Time)otherTime).mTime;
			}
			else
			{
				return getTimeAsMillisecondFloat() > otherTime.getTimeAsMillisecondFloat();
			}
		}


		/// <summary>
		/// Determines is <c>this</c> is less than a given other <see cref="ITime"/>.
		/// </summary>
		/// <param name="otherTime">The other <see cref="ITime"/></param>
		/// <returns>
		/// <c>true</c> if <c>this</c> is less than <paramref name="otherTime"/>, otherwise <c>false</c>
		/// </returns>
		/// <exception cref="">
		/// Thrown when <paramref name="otherTime"/> is <c>null</c>
		/// </exception>
		public bool isLessThan(ITime otherTime)
		{
			return otherTime.isGreaterThan(this);
		}

		/// <summary>
		/// Determines is <c>this</c> value equal to a given other <see cref="ITime"/>
		/// </summary>
		/// <param name="otherTime">The other <see cref="ITime"/></param>
		/// <returns>
		/// <c>true</c> if <c>this</c> and <paramref name="otherTime"/> are value equal,
		/// otherwise <c>false</c>
		/// </returns>
		/// <exception cref="">
		/// Thrown when <paramref name="otherTime"/> is <c>null</c>
		/// </exception>
		public bool isEqualTo(ITime otherTime)
		{
			if (isGreaterThan(otherTime)) return false;
			if (otherTime.isGreaterThan(this)) return false;
			return true;
		}

		#endregion
	}
}
