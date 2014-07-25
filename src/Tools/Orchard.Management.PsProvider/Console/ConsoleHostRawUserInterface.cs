namespace Orchard.Management.PsProvider.Console
{
    using System;
    using System.Management.Automation.Host;

    public class ConsoleHostRawUserInterface : PSHostRawUserInterface
    {
        private readonly ConsoleHostUserInterface consoleHostUserInterface;

        public ConsoleHostRawUserInterface(ConsoleHostUserInterface consoleHostUserInterface)
        {
            this.consoleHostUserInterface = consoleHostUserInterface;
        }

        public override ConsoleColor ForegroundColor { get; set; }
        public override ConsoleColor BackgroundColor { get; set; }
        public override Coordinates CursorPosition { get; set; }
        public override Coordinates WindowPosition { get; set; }
        public override int CursorSize { get; set; }
        public override Size BufferSize { get; set; }
        public override Size WindowSize { get; set; }

        public override Size MaxWindowSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Size MaxPhysicalWindowSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool KeyAvailable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string WindowTitle { get; set; }

        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
            throw new NotImplementedException();
        }

        public override void FlushInputBuffer()
        {
            throw new NotImplementedException();
        }

        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
            throw new NotImplementedException();
        }

        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
            throw new NotImplementedException();
        }

        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            throw new NotImplementedException();
        }

        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
            throw new NotImplementedException();
        }
    }
}