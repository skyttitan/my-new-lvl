using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Kit.Extend
{
	public static class StringExtend
	{
		/// <summary>Splits the string[] of the string by length.</summary>
		/// <returns>Array of string</returns>
		/// <param name="string">string</param>
		/// <param name="length">length.</param>
		public static string[] SplitByLength(this string _string, int _length)
		{
			int _strLength = _string.Length;
			int _strCount = (_strLength + _length -1)/_length;
			string[] _rst = new string[_strCount];
			for(int i=0; i<_strCount; ++i)
			{
				_rst[i] = _string.Substring(i*_length, Mathf.Min (_length, _strLength));
				_strLength-=_length;
			}
			return _rst;
		}
		/// <summary>Split string to char array.</summary>
		/// <returns>char array.</returns>
		/// <param name="string">string.</param>
		public static char[] SplitToChar(this string _string)
		{
			char[] _char = new char[_string.Length-1];
			for(int i=0; i<_string.Length-1; i++)
			{
				_char[i] = (char)_string[i];
			}
			return _char;
		}
		/// <summary>Combine char array into string</summary>
		/// <returns>String of the array.</returns>
		/// <param name="separator">join array with separator.</param>
		public static string JoinArray(this char[] _char, string _separator = "")
		{
			StringBuilder _rst = new StringBuilder();
			for(int i=0; i<_char.Length-1; i++)
			{
				if( char.IsLetter(_char[i]) )
				{
					_rst.Append(_char[i].ToString());
					if( i!=(_char.Length-2) )_rst.Append(_separator);
				}
			}
			return _rst.ToString();
		}
		/// <summary>Combine string array into string</summary>
		/// <returns>String of the array.</returns>
		/// <param name="_separator">join array with separator.</param>
		public static string JoinArray(this string[] _strings, string _separator = "")
		{
			string _rst = string.Empty;
			for(int i=0; i<_strings.Length; i++)
			{
				if( i>0 )
					_rst+=_separator;
				if( _strings[i]!=null )
					_rst+=_strings[i];
			}
			return _rst;
		}

		/// <summary>string array values.</summary>
		/// <returns>The string value.</returns>
		/// <param name="strings">array of strings.</param>
		/// <param name="detail">If set to <c>true</c> display detail.</param>
		public static string ToString(this string[] _strings,bool _detail)
		{
			if( !_detail )
			{
				return _strings.JoinArray();
			}
			else
			{
				string _info="Length:"+_strings.Length+"\n";
				_info+="Range:"+_strings.Rank+"\n";
				_info+="ReadOnly:"+_strings.IsReadOnly+"\n";
				_info+="IsSynchronized:"+_strings.IsSynchronized+"\n";
				_info+="IsFixedSize:"+_strings.IsFixedSize+"\n";
				return _strings.JoinArray()+"\n"+_info;
			}
		}
	}
}