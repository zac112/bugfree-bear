using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field)]
public class RegexAttribute : PropertyAttribute
{
	public readonly string pattern;
	public readonly string helpMessage;

	public RegexAttribute(string pattern, string helpMessage)
	{
		this.pattern = pattern;
		this.helpMessage = helpMessage;
	}

	public RegexAttribute(string pattern) : this(pattern, string.Empty) { }
}