using System;
using System.Collections.Generic;
using System.Linq;

// https://www.codingame.com/training/medium/scrabble
internal class Scrabble
{
	private static void Main(string[] args)
	{
		int N = int.Parse(Console.ReadLine());
		var words = new Dictionary<string, Word>(N);
		for (int i = 0; i < N; i++)
		{
			string W = Console.ReadLine();
			var word = new Word()
			{
				Text = W,
				Order = i,
				Value = ComputeWordValue(W),
			};
			var key = word.GetKey();
			Console.Error.WriteLine($"{word} : {key}");
			if (!words.ContainsKey(key))
			{
				// další slova ze stejných písmen nás nezajímají
				// dle zadání chceme stejně to první
				words.Add(key, word);
			}
		}
		string LETTERS = Console.ReadLine();
		var lettersByAlphabet = new string(LETTERS.OrderBy(c => c).ToArray());

		List<Word> wordsFound = new();
		foreach (var word in words)
		{
			if (IsWordCoveredByLetters(lettersByAlphabet, word.Key))
			{
				wordsFound.Add(word.Value);
				Console.Error.WriteLine($"Found: {word.Value}");
			}
		}

		var bestWord = wordsFound.OrderByDescending(w => w.Value).ThenBy(w => w.Order).First();
		Console.WriteLine(bestWord.Text);
	}

	private static bool IsWordCoveredByLetters(string lettersSorted, string wordSorted)
	{
		// merge-based algoritmus dvou seřazených polí
		int lettersIndex = 0;
		int wordIndex = 0;

		while ((lettersIndex < lettersSorted.Length) && (wordIndex < wordSorted.Length))
		{
			if (lettersSorted[lettersIndex] < wordSorted[wordIndex])
			{
				// hledané písmeno to není, ale může být dále
				lettersIndex++;
			}
			else if (lettersSorted[lettersIndex] > wordSorted[wordIndex])
			{
				// hledané písmeno to není a už nemůže být dále
				return false;
			}
			else // mainList[mainIndex] == subList[subIndex]
			{
				// shoda písmen, posouváme se v obou polích
				lettersIndex++;
				wordIndex++;
			}
		}

		return (wordIndex == wordSorted.Length);
	}

	private static int ComputeWordValue(IEnumerable<char> word)
	{
		int value = 0;
		foreach (var letter in word)
		{
			value += GetLetterValue(letter);
		}

		return value;
	}

	private static int GetLetterValue(char letter)
	{
		return letter switch
		{
			'e' or 'a' or 'i' or 'o' or 'n' or 'r' or 't' or 'l' or 's' or 'u' => 1,
			'd' or 'g' => 2,
			'b' or 'c' or 'm' or 'p' => 3,
			'f' or 'h' or 'v' or 'w' or 'y' => 4,
			'k' => 5,
			'j' or 'x' => 8,
			'q' or 'z' => 10,
			_ => throw new ArgumentException($"Invalid letter: {letter}"),
		};
	}

	public record Word
	{
		public string Text { get; init; }
		public int Order { get; init; }
		public int Value { get; init; }

		public string GetKey()
		{
			return new string(Text.OrderBy(c => c).ToArray());
		}
	}
}