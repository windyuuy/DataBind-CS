namespace TestingCode
{
	public class TestWriteCodeCase1: VM.Host
	{
		/// <note>
		/// env::a
		/// </note>
		public TA a {get;set;} = new TA();
		public class TA
		{
			public TB b {get;set;} = new TB();
			public class TB
			{
				/// <usecase>
				/// a.b.c<br/>
				/// </usecase>
				public int c {get;set;}
			}
		}
		/// <note>
		/// env::fe
		/// </note>
		public TFe fe {get;set;} = new TFe();
		public class TFe
		{
			public TCx cx {get;set;} = new TCx();
			public class TCx
			{
				/// <usecase>
				/// fe.cx.xc<br/>
				/// </usecase>
				public int xc(TWf wf,object xc)
				{
					return 564;
				}
			}
		}
		/// <note>
		/// env::n
		/// </note>
		public TN n {get;set;} = new TN();
		public class TN
		{
			/// <usecase>
			/// n.wf<br/>
			/// </usecase>
			public object wf {get;set;}
		}
		/// <note>
		/// env::rrx
		/// </note>
		public TRrx rrx {get;set;} = new TRrx();
		public class TRrx
		{
			/// <usecase>
			/// rrx.xx<br/>
			/// </usecase>
			public object xx {get;set;}
		}
		/// <note>
		/// env::few
		/// </note>
		public object few {get;set;}
		/// <note>
		/// env::wf
		/// </note>
		public TWf wf {get;set;} = new TWf();
		public class TWf
		{
			public object f {get;set;}
		}
		/// <note>
		/// env::wef
		/// </note>
		public object wef {get;set;}
	}
}
