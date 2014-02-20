using System;

namespace cottontail.messaging
{
    public class Logger
    {
        public event EventHandler<MessengerEventArgs> Log;

		public void DebugWrite(string format, params object[] args)
        {
            MessengerEventArgs mEArgs=new MessengerEventArgs();
			mEArgs.Message=string.Format(format, args);
			if (Log != null)
				Log (this, mEArgs);
        }

        public void InfoWrite(string format, params object[] args)
        {
            MessengerEventArgs mEArgs=new MessengerEventArgs();
			mEArgs.Message=string.Format(format, args);
			if (Log != null)
				Log (this, mEArgs);
        }

        public void ErrorWrite(string format, params object[] args)
        {
            MessengerEventArgs mEArgs=new MessengerEventArgs();
			mEArgs.Message=string.Format(format, args);
			if (Log != null)
				Log (this, mEArgs);
        }

        public void ErrorWrite(Exception exception)
        {
            MessengerEventArgs mEArgs=new MessengerEventArgs();
			mEArgs.Message=exception.Message;
			if (Log != null)
				Log (this, mEArgs);
        }
    }}

