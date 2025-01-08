using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Exceptions
{
	public class DuplicateException : Exception
	{
		public DuplicateException(string message) : base(message)
		{
		}
		public DuplicateException(string message, Exception innerException) : base(message, innerException)
		{
		}

	}
}
