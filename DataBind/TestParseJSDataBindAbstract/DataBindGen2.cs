﻿using DataBind.CollectionExt;
using number = System.Double;
using Action = System.Action;

namespace TestingCode
{
	public class TestWriteCodeCase2: DataBind.IStdHost
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
			// lkjwf
			/// <summary>
			/// lkwjef
			/// </summary>
			public TCx cx {get;set;} = new TCx();
			/// <summary>
			/// lkjwelkfj
			/// </summary>
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
		/// env::fex
		/// </note>
		public Dictionary<string, object> fex {get;set;}
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
			/// <usecase>
			/// wf.f<br/>
			/// </usecase>
			public object f {get;set;}
		}
		/// <note>
		/// env::wef
		/// </note>
		public bool wef {get;set;}
		/// <note>
		/// env::ke
		/// </note>
		public TKe ke {get;set;} = new TKe();
		public class TKe
		{
			/// <usecase>
			/// ke.jf<br/>
			/// </usecase>
			/// <summary>
			/// wjflkjwelj
			/// klew//we///welkj
			/// </summary>
			public void jf()
			{
			}
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
				/// <usecase>
				/// jklwe.jx.jfj<br/>
				/// </usecase>
				public void jfj(number p_4,object g,TFe.TCx cx,object kxx)
				{
				}
				/// <usecase>
				/// jklwe.jx.jfj2<br/>
				/// </usecase>
				public void jfj2(number p_4,string p_1,bool p_False,TFe.TCx cx)
				{
				}
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
		/// <note>
		/// env::kxx
		/// </note>
		public object kxx {get;set;}
	}
}
