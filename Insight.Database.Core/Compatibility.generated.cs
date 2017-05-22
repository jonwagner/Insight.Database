using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

#if NET35
namespace Insight.Database
{
	/// <summary>
	/// An action that takes 5 values.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first value.</typeparam>
	/// <typeparam name="T2">The type of the data in the second value.</typeparam>
	/// <typeparam name="T3">The type of the data in the third value.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth value.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth value.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		/// <param name="arg5">The value of the fifth item.</param>
	public delegate void Action<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

	/// <summary>
	/// An action that takes 6 values.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first value.</typeparam>
	/// <typeparam name="T2">The type of the data in the second value.</typeparam>
	/// <typeparam name="T3">The type of the data in the third value.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth value.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth value.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth value.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		/// <param name="arg5">The value of the fifth item.</param>
		/// <param name="arg6">The value of the sixth item.</param>
	public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

	/// <summary>
	/// An action that takes 7 values.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first value.</typeparam>
	/// <typeparam name="T2">The type of the data in the second value.</typeparam>
	/// <typeparam name="T3">The type of the data in the third value.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth value.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth value.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth value.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh value.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		/// <param name="arg5">The value of the fifth item.</param>
		/// <param name="arg6">The value of the sixth item.</param>
		/// <param name="arg7">The value of the seventh item.</param>
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

	/// <summary>
	/// An action that takes 8 values.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first value.</typeparam>
	/// <typeparam name="T2">The type of the data in the second value.</typeparam>
	/// <typeparam name="T3">The type of the data in the third value.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth value.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth value.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth value.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh value.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth value.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		/// <param name="arg5">The value of the fifth item.</param>
		/// <param name="arg6">The value of the sixth item.</param>
		/// <param name="arg7">The value of the seventh item.</param>
		/// <param name="arg8">The value of the eighth item.</param>
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

	/// <summary>
	/// An action that takes 9 values.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first value.</typeparam>
	/// <typeparam name="T2">The type of the data in the second value.</typeparam>
	/// <typeparam name="T3">The type of the data in the third value.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth value.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth value.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth value.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh value.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth value.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth value.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		/// <param name="arg5">The value of the fifth item.</param>
		/// <param name="arg6">The value of the sixth item.</param>
		/// <param name="arg7">The value of the seventh item.</param>
		/// <param name="arg8">The value of the eighth item.</param>
		/// <param name="arg9">The value of the nineth item.</param>
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);

	/// <summary>
	/// An action that takes 10 values.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first value.</typeparam>
	/// <typeparam name="T2">The type of the data in the second value.</typeparam>
	/// <typeparam name="T3">The type of the data in the third value.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth value.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth value.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth value.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh value.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth value.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth value.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth value.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		/// <param name="arg5">The value of the fifth item.</param>
		/// <param name="arg6">The value of the sixth item.</param>
		/// <param name="arg7">The value of the seventh item.</param>
		/// <param name="arg8">The value of the eighth item.</param>
		/// <param name="arg9">The value of the nineth item.</param>
		/// <param name="arg10">The value of the tenth item.</param>
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);

	/// <summary>
	/// An action that takes 11 values.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first value.</typeparam>
	/// <typeparam name="T2">The type of the data in the second value.</typeparam>
	/// <typeparam name="T3">The type of the data in the third value.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth value.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth value.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth value.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh value.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth value.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth value.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth value.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh value.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		/// <param name="arg5">The value of the fifth item.</param>
		/// <param name="arg6">The value of the sixth item.</param>
		/// <param name="arg7">The value of the seventh item.</param>
		/// <param name="arg8">The value of the eighth item.</param>
		/// <param name="arg9">The value of the nineth item.</param>
		/// <param name="arg10">The value of the tenth item.</param>
		/// <param name="arg11">The value of the eleventh item.</param>
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);

	/// <summary>
	/// An action that takes 12 values.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first value.</typeparam>
	/// <typeparam name="T2">The type of the data in the second value.</typeparam>
	/// <typeparam name="T3">The type of the data in the third value.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth value.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth value.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth value.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh value.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth value.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth value.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth value.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh value.</typeparam>
	/// <typeparam name="T12">The type of the data in the twelfth value.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		/// <param name="arg5">The value of the fifth item.</param>
		/// <param name="arg6">The value of the sixth item.</param>
		/// <param name="arg7">The value of the seventh item.</param>
		/// <param name="arg8">The value of the eighth item.</param>
		/// <param name="arg9">The value of the nineth item.</param>
		/// <param name="arg10">The value of the tenth item.</param>
		/// <param name="arg11">The value of the eleventh item.</param>
		/// <param name="arg12">The value of the twelfth item.</param>
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);

	/// <summary>
	/// An action that takes 13 values.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first value.</typeparam>
	/// <typeparam name="T2">The type of the data in the second value.</typeparam>
	/// <typeparam name="T3">The type of the data in the third value.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth value.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth value.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth value.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh value.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth value.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth value.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth value.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh value.</typeparam>
	/// <typeparam name="T12">The type of the data in the twelfth value.</typeparam>
	/// <typeparam name="T13">The type of the data in the thirteenth value.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		/// <param name="arg5">The value of the fifth item.</param>
		/// <param name="arg6">The value of the sixth item.</param>
		/// <param name="arg7">The value of the seventh item.</param>
		/// <param name="arg8">The value of the eighth item.</param>
		/// <param name="arg9">The value of the nineth item.</param>
		/// <param name="arg10">The value of the tenth item.</param>
		/// <param name="arg11">The value of the eleventh item.</param>
		/// <param name="arg12">The value of the twelfth item.</param>
		/// <param name="arg13">The value of the thirteenth item.</param>
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);

	/// <summary>
	/// An action that takes 14 values.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first value.</typeparam>
	/// <typeparam name="T2">The type of the data in the second value.</typeparam>
	/// <typeparam name="T3">The type of the data in the third value.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth value.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth value.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth value.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh value.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth value.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth value.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth value.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh value.</typeparam>
	/// <typeparam name="T12">The type of the data in the twelfth value.</typeparam>
	/// <typeparam name="T13">The type of the data in the thirteenth value.</typeparam>
	/// <typeparam name="T14">The type of the data in the fourteenth value.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		/// <param name="arg5">The value of the fifth item.</param>
		/// <param name="arg6">The value of the sixth item.</param>
		/// <param name="arg7">The value of the seventh item.</param>
		/// <param name="arg8">The value of the eighth item.</param>
		/// <param name="arg9">The value of the nineth item.</param>
		/// <param name="arg10">The value of the tenth item.</param>
		/// <param name="arg11">The value of the eleventh item.</param>
		/// <param name="arg12">The value of the twelfth item.</param>
		/// <param name="arg13">The value of the thirteenth item.</param>
		/// <param name="arg14">The value of the fourteenth item.</param>
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);

	/// <summary>
	/// An action that takes 15 values.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first value.</typeparam>
	/// <typeparam name="T2">The type of the data in the second value.</typeparam>
	/// <typeparam name="T3">The type of the data in the third value.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth value.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth value.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth value.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh value.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth value.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth value.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth value.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh value.</typeparam>
	/// <typeparam name="T12">The type of the data in the twelfth value.</typeparam>
	/// <typeparam name="T13">The type of the data in the thirteenth value.</typeparam>
	/// <typeparam name="T14">The type of the data in the fourteenth value.</typeparam>
	/// <typeparam name="T15">The type of the data in the fifteenth value.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		/// <param name="arg5">The value of the fifth item.</param>
		/// <param name="arg6">The value of the sixth item.</param>
		/// <param name="arg7">The value of the seventh item.</param>
		/// <param name="arg8">The value of the eighth item.</param>
		/// <param name="arg9">The value of the nineth item.</param>
		/// <param name="arg10">The value of the tenth item.</param>
		/// <param name="arg11">The value of the eleventh item.</param>
		/// <param name="arg12">The value of the twelfth item.</param>
		/// <param name="arg13">The value of the thirteenth item.</param>
		/// <param name="arg14">The value of the fourteenth item.</param>
		/// <param name="arg15">The value of the fifteenth item.</param>
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);

	/// <summary>
	/// An action that takes 16 values.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first value.</typeparam>
	/// <typeparam name="T2">The type of the data in the second value.</typeparam>
	/// <typeparam name="T3">The type of the data in the third value.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth value.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth value.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth value.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh value.</typeparam>
	/// <typeparam name="T8">The type of the data in the eighth value.</typeparam>
	/// <typeparam name="T9">The type of the data in the nineth value.</typeparam>
	/// <typeparam name="T10">The type of the data in the tenth value.</typeparam>
	/// <typeparam name="T11">The type of the data in the eleventh value.</typeparam>
	/// <typeparam name="T12">The type of the data in the twelfth value.</typeparam>
	/// <typeparam name="T13">The type of the data in the thirteenth value.</typeparam>
	/// <typeparam name="T14">The type of the data in the fourteenth value.</typeparam>
	/// <typeparam name="T15">The type of the data in the fifteenth value.</typeparam>
	/// <typeparam name="T16">The type of the data in the sixteenth value.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		/// <param name="arg5">The value of the fifth item.</param>
		/// <param name="arg6">The value of the sixth item.</param>
		/// <param name="arg7">The value of the seventh item.</param>
		/// <param name="arg8">The value of the eighth item.</param>
		/// <param name="arg9">The value of the nineth item.</param>
		/// <param name="arg10">The value of the tenth item.</param>
		/// <param name="arg11">The value of the eleventh item.</param>
		/// <param name="arg12">The value of the twelfth item.</param>
		/// <param name="arg13">The value of the thirteenth item.</param>
		/// <param name="arg14">The value of the fourteenth item.</param>
		/// <param name="arg15">The value of the fifteenth item.</param>
		/// <param name="arg16">The value of the sixteenth item.</param>
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);


	/// <summary>
	/// Provides methods to create Tuples.
	/// </summary>
	public static class Tuple
	{
		/// <summary>
		/// Creates a tuple that contains 1 items.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first item.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		public static Tuple<T1> Create<T1>(T1 arg1)
		{
			return new Tuple<T1>(arg1);
		}

		/// <summary>
		/// Creates a tuple that contains 2 items.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first item.</typeparam>
		/// <typeparam name="T2">The type of the data in the second item.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		public static Tuple<T1, T2> Create<T1, T2>(T1 arg1, T2 arg2)
		{
			return new Tuple<T1, T2>(arg1, arg2);
		}

		/// <summary>
		/// Creates a tuple that contains 3 items.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first item.</typeparam>
		/// <typeparam name="T2">The type of the data in the second item.</typeparam>
		/// <typeparam name="T3">The type of the data in the third item.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
		{
			return new Tuple<T1, T2, T3>(arg1, arg2, arg3);
		}

		/// <summary>
		/// Creates a tuple that contains 4 items.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first item.</typeparam>
		/// <typeparam name="T2">The type of the data in the second item.</typeparam>
		/// <typeparam name="T3">The type of the data in the third item.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth item.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			return new Tuple<T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
		}

		/// <summary>
		/// Creates a tuple that contains 5 items.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first item.</typeparam>
		/// <typeparam name="T2">The type of the data in the second item.</typeparam>
		/// <typeparam name="T3">The type of the data in the third item.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth item.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth item.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		/// <param name="arg5">The value of the fifth item.</param>
		public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			return new Tuple<T1, T2, T3, T4, T5>(arg1, arg2, arg3, arg4, arg5);
		}

		/// <summary>
		/// Creates a tuple that contains 6 items.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first item.</typeparam>
		/// <typeparam name="T2">The type of the data in the second item.</typeparam>
		/// <typeparam name="T3">The type of the data in the third item.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth item.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth item.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth item.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		/// <param name="arg5">The value of the fifth item.</param>
		/// <param name="arg6">The value of the sixth item.</param>
		public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			return new Tuple<T1, T2, T3, T4, T5, T6>(arg1, arg2, arg3, arg4, arg5, arg6);
		}

		/// <summary>
		/// Creates a tuple that contains 7 items.
		/// </summary>
		/// <typeparam name="T1">The type of the data in the first item.</typeparam>
		/// <typeparam name="T2">The type of the data in the second item.</typeparam>
		/// <typeparam name="T3">The type of the data in the third item.</typeparam>
		/// <typeparam name="T4">The type of the data in the fourth item.</typeparam>
		/// <typeparam name="T5">The type of the data in the fifth item.</typeparam>
		/// <typeparam name="T6">The type of the data in the sixth item.</typeparam>
		/// <typeparam name="T7">The type of the data in the seventh item.</typeparam>
		/// <param name="arg1">The value of the first item.</param>
		/// <param name="arg2">The value of the second item.</param>
		/// <param name="arg3">The value of the third item.</param>
		/// <param name="arg4">The value of the fourth item.</param>
		/// <param name="arg5">The value of the fifth item.</param>
		/// <param name="arg6">The value of the sixth item.</param>
		/// <param name="arg7">The value of the seventh item.</param>
		public static Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			return new Tuple<T1, T2, T3, T4, T5, T6, T7>(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
		}


		internal static int CombineHashCodes(params int[] hashes)
		{
			int h = hashes[0];

			for (int i = 1; i < hashes.Length; i++)
				h = (((h << 5) + h) ^ hashes[i]);

			return h;
		}

		internal static int GetHashCode<T>(T t)
		{
			if (t == null)
				return 0;
			else
				return t.GetHashCode();
		}
	}

	/// <summary>
	/// A tuple that contains 1 items.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first item.</typeparam>
	public class Tuple<T1>
	{
		/// <summary>
		/// Gets or sets the value of the first item in the tuple.
		/// </summary>
		public T1 Item1 { get; private set; }

		/// <summary>
		/// Initializes a new instance of the Tuple class.
		/// </summary>
		public Tuple(T1 arg1)
		{
			Item1 = arg1;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return Tuple.CombineHashCodes(Tuple.GetHashCode(Item1));
		}

		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			Tuple<T1> other = obj as Tuple<T1>;
			if (other == null)
				return false;

			if (Item1 == null && other.Item1 != null)
				return false;
			if (!Item1.Equals(other.Item1))
				return false;

			return true;
		}
	}

	/// <summary>
	/// A tuple that contains 2 items.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first item.</typeparam>
	/// <typeparam name="T2">The type of the data in the second item.</typeparam>
	public class Tuple<T1, T2>
	{
		/// <summary>
		/// Gets or sets the value of the first item in the tuple.
		/// </summary>
		public T1 Item1 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the second item in the tuple.
		/// </summary>
		public T2 Item2 { get; private set; }

		/// <summary>
		/// Initializes a new instance of the Tuple class.
		/// </summary>
		public Tuple(T1 arg1, T2 arg2)
		{
			Item1 = arg1;
			Item2 = arg2;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return Tuple.CombineHashCodes(Tuple.GetHashCode(Item1), Tuple.GetHashCode(Item2));
		}

		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			Tuple<T1, T2> other = obj as Tuple<T1, T2>;
			if (other == null)
				return false;

			if (Item1 == null && other.Item1 != null)
				return false;
			if (!Item1.Equals(other.Item1))
				return false;
			if (Item2 == null && other.Item2 != null)
				return false;
			if (!Item2.Equals(other.Item2))
				return false;

			return true;
		}
	}

	/// <summary>
	/// A tuple that contains 3 items.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first item.</typeparam>
	/// <typeparam name="T2">The type of the data in the second item.</typeparam>
	/// <typeparam name="T3">The type of the data in the third item.</typeparam>
	public class Tuple<T1, T2, T3>
	{
		/// <summary>
		/// Gets or sets the value of the first item in the tuple.
		/// </summary>
		public T1 Item1 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the second item in the tuple.
		/// </summary>
		public T2 Item2 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the third item in the tuple.
		/// </summary>
		public T3 Item3 { get; private set; }

		/// <summary>
		/// Initializes a new instance of the Tuple class.
		/// </summary>
		public Tuple(T1 arg1, T2 arg2, T3 arg3)
		{
			Item1 = arg1;
			Item2 = arg2;
			Item3 = arg3;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return Tuple.CombineHashCodes(Tuple.GetHashCode(Item1), Tuple.GetHashCode(Item2), Tuple.GetHashCode(Item3));
		}

		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			Tuple<T1, T2, T3> other = obj as Tuple<T1, T2, T3>;
			if (other == null)
				return false;

			if (Item1 == null && other.Item1 != null)
				return false;
			if (!Item1.Equals(other.Item1))
				return false;
			if (Item2 == null && other.Item2 != null)
				return false;
			if (!Item2.Equals(other.Item2))
				return false;
			if (Item3 == null && other.Item3 != null)
				return false;
			if (!Item3.Equals(other.Item3))
				return false;

			return true;
		}
	}

	/// <summary>
	/// A tuple that contains 4 items.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first item.</typeparam>
	/// <typeparam name="T2">The type of the data in the second item.</typeparam>
	/// <typeparam name="T3">The type of the data in the third item.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth item.</typeparam>
	public class Tuple<T1, T2, T3, T4>
	{
		/// <summary>
		/// Gets or sets the value of the first item in the tuple.
		/// </summary>
		public T1 Item1 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the second item in the tuple.
		/// </summary>
		public T2 Item2 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the third item in the tuple.
		/// </summary>
		public T3 Item3 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the fourth item in the tuple.
		/// </summary>
		public T4 Item4 { get; private set; }

		/// <summary>
		/// Initializes a new instance of the Tuple class.
		/// </summary>
		public Tuple(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			Item1 = arg1;
			Item2 = arg2;
			Item3 = arg3;
			Item4 = arg4;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return Tuple.CombineHashCodes(Tuple.GetHashCode(Item1), Tuple.GetHashCode(Item2), Tuple.GetHashCode(Item3), Tuple.GetHashCode(Item4));
		}

		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			Tuple<T1, T2, T3, T4> other = obj as Tuple<T1, T2, T3, T4>;
			if (other == null)
				return false;

			if (Item1 == null && other.Item1 != null)
				return false;
			if (!Item1.Equals(other.Item1))
				return false;
			if (Item2 == null && other.Item2 != null)
				return false;
			if (!Item2.Equals(other.Item2))
				return false;
			if (Item3 == null && other.Item3 != null)
				return false;
			if (!Item3.Equals(other.Item3))
				return false;
			if (Item4 == null && other.Item4 != null)
				return false;
			if (!Item4.Equals(other.Item4))
				return false;

			return true;
		}
	}

	/// <summary>
	/// A tuple that contains 5 items.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first item.</typeparam>
	/// <typeparam name="T2">The type of the data in the second item.</typeparam>
	/// <typeparam name="T3">The type of the data in the third item.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth item.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth item.</typeparam>
	public class Tuple<T1, T2, T3, T4, T5>
	{
		/// <summary>
		/// Gets or sets the value of the first item in the tuple.
		/// </summary>
		public T1 Item1 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the second item in the tuple.
		/// </summary>
		public T2 Item2 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the third item in the tuple.
		/// </summary>
		public T3 Item3 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the fourth item in the tuple.
		/// </summary>
		public T4 Item4 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the fifth item in the tuple.
		/// </summary>
		public T5 Item5 { get; private set; }

		/// <summary>
		/// Initializes a new instance of the Tuple class.
		/// </summary>
		public Tuple(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			Item1 = arg1;
			Item2 = arg2;
			Item3 = arg3;
			Item4 = arg4;
			Item5 = arg5;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return Tuple.CombineHashCodes(Tuple.GetHashCode(Item1), Tuple.GetHashCode(Item2), Tuple.GetHashCode(Item3), Tuple.GetHashCode(Item4), Tuple.GetHashCode(Item5));
		}

		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			Tuple<T1, T2, T3, T4, T5> other = obj as Tuple<T1, T2, T3, T4, T5>;
			if (other == null)
				return false;

			if (Item1 == null && other.Item1 != null)
				return false;
			if (!Item1.Equals(other.Item1))
				return false;
			if (Item2 == null && other.Item2 != null)
				return false;
			if (!Item2.Equals(other.Item2))
				return false;
			if (Item3 == null && other.Item3 != null)
				return false;
			if (!Item3.Equals(other.Item3))
				return false;
			if (Item4 == null && other.Item4 != null)
				return false;
			if (!Item4.Equals(other.Item4))
				return false;
			if (Item5 == null && other.Item5 != null)
				return false;
			if (!Item5.Equals(other.Item5))
				return false;

			return true;
		}
	}

	/// <summary>
	/// A tuple that contains 6 items.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first item.</typeparam>
	/// <typeparam name="T2">The type of the data in the second item.</typeparam>
	/// <typeparam name="T3">The type of the data in the third item.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth item.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth item.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth item.</typeparam>
	public class Tuple<T1, T2, T3, T4, T5, T6>
	{
		/// <summary>
		/// Gets or sets the value of the first item in the tuple.
		/// </summary>
		public T1 Item1 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the second item in the tuple.
		/// </summary>
		public T2 Item2 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the third item in the tuple.
		/// </summary>
		public T3 Item3 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the fourth item in the tuple.
		/// </summary>
		public T4 Item4 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the fifth item in the tuple.
		/// </summary>
		public T5 Item5 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the sixth item in the tuple.
		/// </summary>
		public T6 Item6 { get; private set; }

		/// <summary>
		/// Initializes a new instance of the Tuple class.
		/// </summary>
		public Tuple(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			Item1 = arg1;
			Item2 = arg2;
			Item3 = arg3;
			Item4 = arg4;
			Item5 = arg5;
			Item6 = arg6;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return Tuple.CombineHashCodes(Tuple.GetHashCode(Item1), Tuple.GetHashCode(Item2), Tuple.GetHashCode(Item3), Tuple.GetHashCode(Item4), Tuple.GetHashCode(Item5), Tuple.GetHashCode(Item6));
		}

		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			Tuple<T1, T2, T3, T4, T5, T6> other = obj as Tuple<T1, T2, T3, T4, T5, T6>;
			if (other == null)
				return false;

			if (Item1 == null && other.Item1 != null)
				return false;
			if (!Item1.Equals(other.Item1))
				return false;
			if (Item2 == null && other.Item2 != null)
				return false;
			if (!Item2.Equals(other.Item2))
				return false;
			if (Item3 == null && other.Item3 != null)
				return false;
			if (!Item3.Equals(other.Item3))
				return false;
			if (Item4 == null && other.Item4 != null)
				return false;
			if (!Item4.Equals(other.Item4))
				return false;
			if (Item5 == null && other.Item5 != null)
				return false;
			if (!Item5.Equals(other.Item5))
				return false;
			if (Item6 == null && other.Item6 != null)
				return false;
			if (!Item6.Equals(other.Item6))
				return false;

			return true;
		}
	}

	/// <summary>
	/// A tuple that contains 7 items.
	/// </summary>
	/// <typeparam name="T1">The type of the data in the first item.</typeparam>
	/// <typeparam name="T2">The type of the data in the second item.</typeparam>
	/// <typeparam name="T3">The type of the data in the third item.</typeparam>
	/// <typeparam name="T4">The type of the data in the fourth item.</typeparam>
	/// <typeparam name="T5">The type of the data in the fifth item.</typeparam>
	/// <typeparam name="T6">The type of the data in the sixth item.</typeparam>
	/// <typeparam name="T7">The type of the data in the seventh item.</typeparam>
	public class Tuple<T1, T2, T3, T4, T5, T6, T7>
	{
		/// <summary>
		/// Gets or sets the value of the first item in the tuple.
		/// </summary>
		public T1 Item1 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the second item in the tuple.
		/// </summary>
		public T2 Item2 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the third item in the tuple.
		/// </summary>
		public T3 Item3 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the fourth item in the tuple.
		/// </summary>
		public T4 Item4 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the fifth item in the tuple.
		/// </summary>
		public T5 Item5 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the sixth item in the tuple.
		/// </summary>
		public T6 Item6 { get; private set; }
		/// <summary>
		/// Gets or sets the value of the seventh item in the tuple.
		/// </summary>
		public T7 Item7 { get; private set; }

		/// <summary>
		/// Initializes a new instance of the Tuple class.
		/// </summary>
		public Tuple(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			Item1 = arg1;
			Item2 = arg2;
			Item3 = arg3;
			Item4 = arg4;
			Item5 = arg5;
			Item6 = arg6;
			Item7 = arg7;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return Tuple.CombineHashCodes(Tuple.GetHashCode(Item1), Tuple.GetHashCode(Item2), Tuple.GetHashCode(Item3), Tuple.GetHashCode(Item4), Tuple.GetHashCode(Item5), Tuple.GetHashCode(Item6), Tuple.GetHashCode(Item7));
		}

		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			Tuple<T1, T2, T3, T4, T5, T6, T7> other = obj as Tuple<T1, T2, T3, T4, T5, T6, T7>;
			if (other == null)
				return false;

			if (Item1 == null && other.Item1 != null)
				return false;
			if (!Item1.Equals(other.Item1))
				return false;
			if (Item2 == null && other.Item2 != null)
				return false;
			if (!Item2.Equals(other.Item2))
				return false;
			if (Item3 == null && other.Item3 != null)
				return false;
			if (!Item3.Equals(other.Item3))
				return false;
			if (Item4 == null && other.Item4 != null)
				return false;
			if (!Item4.Equals(other.Item4))
				return false;
			if (Item5 == null && other.Item5 != null)
				return false;
			if (!Item5.Equals(other.Item5))
				return false;
			if (Item6 == null && other.Item6 != null)
				return false;
			if (!Item6.Equals(other.Item6))
				return false;
			if (Item7 == null && other.Item7 != null)
				return false;
			if (!Item7.Equals(other.Item7))
				return false;

			return true;
		}
	}

}
#endif