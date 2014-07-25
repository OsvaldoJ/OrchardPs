﻿namespace Proligence.PowerShell.Host
{
    using System;
    using Orchard.Management.PsProvider.Console;

    /// <summary>
    /// Represents a PowerShell user session.
    /// </summary>
    public class PsSession : IPsSession
    {
        public PsSession(ConsoleHost consoleHost)
        {
            this.ConsoleHost = consoleHost;
        }

        /// <summary>
        /// Gets the console host which provides input/output to the PowerShell engine.
        /// </summary>
        public ConsoleHost ConsoleHost { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.ConsoleHost != null)
                {
                    this.ConsoleHost.Dispose();
                }
            }
        }
    }
}