﻿namespace OrchardPs.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Proligence.PowerShell.Provider.Console;
    using Console = System.Console;

    internal class ConsoleInputBuffer
    {
        private readonly string prompt;
        private readonly IList<string> inputHistory;
        private readonly StringBuilder buffer = new StringBuilder();
        private readonly CommandCompletionProvider commandCompletionProvider;
        private int? historyPosition;
        private string completionCommand;
        private CompletionData completionResult;
        private int completionBufferIndex;
        private int? completionMatchIndex;

        public ConsoleInputBuffer(
            string prompt,
            IList<string> inputHistory,
            CommandCompletionProvider commandCompletionProvider = null)
        {
            this.prompt = prompt ?? string.Empty;
            this.inputHistory = inputHistory ?? new List<string>();
            this.commandCompletionProvider = commandCompletionProvider;
            
            Console.Write(prompt);
        }

        public string ReadLine()
        {
            var key = Console.ReadKey(true);
            while (true)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        return this.buffer.ToString();
                    case ConsoleKey.LeftArrow:
                        this.HandleLeftArrow();
                        break;
                    case ConsoleKey.RightArrow:
                        this.HandleRightArrow();
                        break;
                    case ConsoleKey.UpArrow:
                        this.HandleUpArrow();
                        break;
                    case ConsoleKey.DownArrow:
                        this.HandleDownArrow();
                        break;
                    case ConsoleKey.Backspace:
                        this.HandleBackspace();
                        break;
                    case ConsoleKey.Delete:
                        this.HandleDelete();
                        break;
                    case ConsoleKey.Escape:
                        this.HandleEscape();
                        break;
                    case ConsoleKey.Tab:
                        this.HandleTab(key.Modifiers.HasFlag(ConsoleModifiers.Shift));
                        break;
                    default:
                        this.InsertCharIntoBuffer(key.KeyChar);
                        break;
                }

                key = Console.ReadKey(true);
            }
        }

        private void HandleLeftArrow()
        {
            if (Console.CursorLeft > this.prompt.Length)
            {
                Console.CursorLeft--;                
            }
        }

        private void HandleRightArrow()
        {
            if (Console.CursorLeft < this.prompt.Length + this.buffer.Length)
            {
                Console.CursorLeft++;   
            }
        }

        private void HandleUpArrow()
        {
            if (this.historyPosition == null)
            {
                this.historyPosition = 0;
                this.RestoreHistoryInput();
            }
            else if (this.historyPosition < this.inputHistory.Count - 1)
            {
                this.historyPosition++;
                this.RestoreHistoryInput();
            }
        }

        private void HandleDownArrow()
        {
            if ((this.historyPosition != null) && (this.historyPosition.Value > 0))
            {
                this.historyPosition--;
                this.RestoreHistoryInput();
            }
        }

        private void RestoreHistoryInput()
        {
            if (this.historyPosition != null)
            {
                int index = this.historyPosition.Value;
                if ((index >= 0) && (index < this.inputHistory.Count))
                {
                    this.buffer.Clear();
                    this.buffer.Append(this.inputHistory[this.historyPosition.Value]);
                    Console.Write("\r" + this.prompt + this.buffer);

                    // Discard command completion data.
                    this.completionCommand = null;
                }
            }
        }

        private void HandleBackspace()
        {
            if (Console.CursorLeft > this.prompt.Length)
            {
                Console.Write("\b");

                int index = Console.CursorLeft;
                int bufferIndex = index - this.prompt.Length;
                if (this.buffer.Length > bufferIndex)
                {
                    this.buffer.Remove(bufferIndex, 1);
                    Console.Write(this.buffer.ToString().Substring(bufferIndex) + " ");
                    Console.CursorLeft = index;
                }
                else
                {
                    Console.Write(" \b");
                }

                // Discard command completion data.
                this.completionCommand = null;
            }
        }

        private void HandleDelete()
        {
            if (Console.CursorLeft < this.prompt.Length + this.buffer.Length)
            {
                int index = Console.CursorLeft;
                int bufferIndex = index - this.prompt.Length;
                string tail = this.buffer.ToString().Substring(bufferIndex + 1);
                this.buffer.Remove(bufferIndex, 1);
                Console.Write(tail + " ");
                Console.CursorLeft = index;

                // Discard command completion data.
                this.completionCommand = null;
            }
        }

        private void HandleEscape()
        {
            Console.Write("\r" + this.prompt + new string(' ', this.buffer.Length));
            Console.CursorLeft = this.prompt.Length;
            this.buffer.Clear();

            // Discard command completion data.
            this.completionCommand = null;
        }

        private void HandleTab(bool shift)
        {
            if (this.commandCompletionProvider != null)
            {
                if (this.completionCommand == null)
                {
                    this.LoadCommandCompletion();
                }

                if (this.completionMatchIndex == null)
                {
                    if (this.completionResult.Results.Any())
                    {
                        this.completionMatchIndex = 0;
                    }
                }
                else
                {
                    if (shift)
                    {
                        if (this.completionMatchIndex.Value == 0)
                        {
                            this.completionMatchIndex = this.completionResult.Results.Count - 1;
                        }
                        else
                        {
                            this.completionMatchIndex--;
                        }
                    }
                    else
                    {
                        this.completionMatchIndex++;
                        this.completionMatchIndex %= this.completionResult.Results.Count;
                    }
                }

                this.InsertCommandCompletion();
            }
        }

        private void LoadCommandCompletion()
        {
            this.completionCommand = this.buffer.ToString();
            this.completionBufferIndex = Console.CursorLeft - this.prompt.Length;
            this.completionMatchIndex = null;

            this.completionResult = this.commandCompletionProvider.GetCommandCompletion(
                this.completionCommand,
                this.completionBufferIndex);
        }

        private void InsertCommandCompletion()
        {
            if (this.completionMatchIndex != null)
            {
                int index = this.completionMatchIndex.Value;
                if ((index >= 0) && (index < this.completionResult.Results.Count))
                {
                    // Clear buffer line
                    Console.Write("\r" + this.prompt + new string(' ', this.buffer.Length));
                    Console.CursorLeft = this.prompt.Length;
                    this.buffer.Clear();

                    this.buffer.Append(this.completionCommand);
                    this.buffer.Remove(this.completionResult.ReplacementIndex, this.completionResult.ReplacementLength);
                    this.buffer.Insert(this.completionResult.ReplacementIndex, this.completionResult.Results[index]);
                    Console.Write("\r" + this.prompt + this.buffer);
                    
                    Console.CursorLeft = 
                        this.prompt.Length + 
                        this.completionResult.ReplacementIndex + 
                        this.completionResult.Results[index].Length;
                }
            }
        }

        private void InsertCharIntoBuffer(char chr)
        {
            if (Console.CursorLeft == (this.prompt.Length + this.buffer.Length))
            {
                this.buffer.Append(chr);
                Console.Write(chr);
            }
            else
            {
                int index = Console.CursorLeft - this.prompt.Length;
                string tail = this.buffer.ToString().Substring(index);
                this.buffer.Insert(index, chr);
                Console.Write(chr);
                Console.Write(tail);
                Console.CursorLeft = this.prompt.Length + index + 1;
            }

            // Discard command completion data.
            this.completionCommand = null;
        }
    }
}