using System;

namespace DbModelFramework
{

	[Serializable]
	public class CreateModelException : Exception
	{
		public CreateModelException() { }
		public CreateModelException(string message) : base(message) { }
		public CreateModelException(string message, Exception inner) : base(message, inner) { }
		protected CreateModelException(
		 System.Runtime.Serialization.SerializationInfo info,
		 System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
