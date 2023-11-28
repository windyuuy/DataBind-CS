using number = System.Double;

namespace TestingCode
{
	public class TestWriteCodeCase3: VM.Host
	{
		/// <note>
		/// env::a
		/// </note>
		/// &lt;summary&gt;
		/// lkjlkjwef
		/// &lt;/summary&gt;
		//wkjfel
		// lkjwlf
		public TA a {get;set;} = new TA();
		public class TA
		{
			public TB b {get;set;} = new TB();
			public class TB
			{
				public int c {get;set;}
			}
		}
		public TN n {get;set;} = new TN();
		public class TN
		{
			public object wf {get;set;}
		}
		/// <note>
		/// env::wf
		/// </note>
		public TWf wf {get;set;} = new TWf();
		public class TWf
		{
			public object f {get;set;}
		}
		public TKe ke {get;set;} = new TKe();
		public class TKe
		{
		}
		/// <note>
		/// env::jklwe
		/// </note>
		public TJklwe jklwe {get;set;} = new TJklwe();
		public class TJklwe
		{
			public TJx jx {get;set;} = new TJx();
			public class TJx
			{
			}
		}
		/// <note>
		/// env::fd
		/// </note>
		public TFd fd {get;set;} = new TFd();
		public class TFd
		{
			/// <usecase>
			/// fd.g<br/>
			/// </usecase>
			public object g {get;set;}
		}
	}
}
