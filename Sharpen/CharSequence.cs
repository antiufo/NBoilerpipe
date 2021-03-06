namespace Sharpen
{
	internal class CharSequence
	{
		public static implicit operator CharSequence (string str)
		{
			return new StringCharSequence (str);
		}
		
		public static implicit operator CharSequence (System.Text.StringBuilder str)
		{
			return new StringCharSequence (str.ToString ());
		}
	}
	
	internal class StringCharSequence: CharSequence
	{
		string str;
		
		public StringCharSequence (string str)
		{
			this.str = str;
		}
		
		public override string ToString ()
		{
			return str;
		}
	}
}
