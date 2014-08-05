﻿namespace Proligence.PowerShell.Commands.Cmdlets 
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using Autofac;
    using Orchard;
    using Orchard.Commands;
    using Orchard.Data;
    using Proligence.PowerShell.Commands.Items;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Utilities;

    /// <summary>
    /// Implements the <c>Invoke-OrchardCommand</c> cmdlet.
    /// </summary>
    [CmdletAlias("ioc")]
    [Cmdlet(VerbsLifecycle.Invoke, "OrchardCommand", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class InvokeOrchardCommand : OrchardCmdlet
    {
        /// <summary>
        /// Gets or sets the name and arguments of the command to execute.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="OrchardCommand"/> object which represents the orchard command to execute.
        /// </summary>
        [Parameter(ParameterSetName = "CommandObject", ValueFromPipeline = true)]
        public OrchardCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the switches which will be passed to the executed command.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Parameter(ParameterSetName = "Default", Position = 2, ValueFromRemainingArguments = true)]
        [Parameter(ParameterSetName = "CommandObject", Position = 2, ValueFromRemainingArguments = true)]
        public ArrayList Parameters { get; set; }

        /// <summary>
        /// Provides a record-by-record processing functionality for the cmdlet. 
        /// </summary>
        protected override void ProcessRecord() 
        {
            string tenantName = this.GetCurrentTenantName() ?? "Default";

            string output = null;
            switch (this.ParameterSetName) 
            {
                case "Default":
                    output = this.InvokeDefault(tenantName, this.Name, this.Parameters);
                    break;
                case "CommandObject":
                    output = this.InvokeCommandObject(tenantName, this.Command, this.Parameters);
                    break;
            }

            this.WriteObject(output);
        }

        /// <summary>
        /// Invokes the Orchard command using the 'Default' parameters set.
        /// </summary>
        /// <param name="tenantName">The name of the Orchard tenant on which the command will be executed.</param>
        /// <param name="commandName">The name and arguments of the command to execute.</param>
        /// <param name="parameters">The switches which will be passed to the executed command.</param>
        /// <returns>The command's output.</returns>
        private string InvokeDefault(string tenantName, string commandName, ArrayList parameters) 
        {
            var arguments = new List<string>();
            var switches = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(commandName)) 
            {
                arguments.Add(commandName);
            }

            return this.InvokeWithParameters(tenantName, arguments, switches, parameters);
        }

        /// <summary>
        /// Invokes the Orchard command using the 'CommandObject' parameters set.
        /// </summary>
        /// <param name="tenantName">The name of the Orchard tenant on which the command will be executed.</param>
        /// <param name="command">
        /// The <see cref="OrchardCommand"/> object which represents the orchard command to execute.
        /// </param>
        /// <param name="parameters">The switches which will be passed to the executed command.</param>
        /// <returns>The command's output.</returns>
        private string InvokeCommandObject(string tenantName, OrchardCommand command, ArrayList parameters) 
        {
            var arguments = new List<string>(command.CommandName.Split(new[] { ' ' }));
            var switches = new Dictionary<string, string>();
            return this.InvokeWithParameters(tenantName, arguments, switches, parameters);
        }

        /// <summary>
        /// Invokes a legacy Orchard command.
        /// </summary>
        /// <param name="tenantName">The name of the Orchard tenant on which the command will be executed.</param>
        /// <param name="arguments">The name and arguments of the command to execute.</param>
        /// <param name="switches">The switches which will be passed to the executed command.</param>
        /// <param name="parameters">The command parameters to parse.</param>
        /// <returns>The command's output.</returns>
        private string InvokeWithParameters(
            string tenantName, 
            List<string> arguments, 
            Dictionary<string, string> switches, 
            ArrayList parameters)
        {
            if (parameters != null) 
            {
                IList<string> parsedArgs;
                IDictionary<string, string> parsedSwitches;
                if (this.ParseParameters(this.Parameters.ToArray().Cast<string>(), out parsedArgs, out parsedSwitches)) 
                {
                    arguments.AddRange(parsedArgs);
                    foreach (KeyValuePair<string, string> sw in parsedSwitches) 
                    {
                        switches.Add(sw.Key, sw.Value);
                    }
                }
                else 
                {
                    return null;
                }
            }

            if (this.ShouldProcess("Tenant: " + tenantName, "Invoke Command '" + string.Join(" ", arguments) + "'")) 
            {
                return this.ExecuteCommand(
                    tenantName,
                    arguments.ToArray(),
                    new Dictionary<string, string>(switches));
            }

            return null;
        }

        /// <summary>
        /// Parses the specified command arguments and switches.
        /// </summary>
        /// <param name="args">The command arguments and switches to parse.</param>
        /// <param name="arguments">Parsed arguments.</param>
        /// <param name="switches">Parsed switches.</param>
        /// <returns>
        /// <c>true</c> if all arguments and switches were parsed successfully or <c>false</c> if an error occurred.
        /// </returns>
        private bool ParseParameters(
            IEnumerable<string> args, 
            out IList<string> arguments, 
            out IDictionary<string, string> switches) 
        {
            arguments = new List<string>();
            switches = new Dictionary<string, string>();

            foreach (var arg in args) 
            {
                if (arg[0] == '/') 
                {
                    int index = arg.IndexOf(':');
                    string switchName = index < 0 ? arg.Substring(1) : arg.Substring(1, index - 1);
                    string switchValue = index < 0 || index >= arg.Length ? string.Empty : arg.Substring(index + 1);

                    if (string.IsNullOrEmpty(switchName)) 
                    {
                        string message = string.Format(
                            CultureInfo.CurrentCulture,
                            "Invalid switch syntax: \"{0}\". Valid syntax is /<switchName>[:<switchValue>].", 
                            arg);

                        var exception = new ArgumentException(message);
                        this.WriteError(exception, "InvalidCommandSwitchSyntax", ErrorCategory.SyntaxError);
                        return false;
                    }

                    switches.Add(switchName, switchValue);
                }
                else 
                {
                    arguments.Add(arg);
                }
            }

            return true;
        }

        /// <summary>
        /// Executes the specified legacy command.
        /// </summary>
        /// <param name="tenantName">The name of the tenant on which the command will be executed.</param>
        /// <param name="args">Command name and arguments.</param>
        /// <param name="switches">Command switches.</param>
        /// <returns>The command's output.</returns>
        private string ExecuteCommand(
            string tenantName,
            IEnumerable<string> args,
            Dictionary<string, string> switches)
        {
            return this.UsingWorkContextScope(
                tenantName,
                scope =>
                    {
                        var commandManager = scope.Resolve<ICommandManager>();

                        using (TextWriter writer = new StringWriter(CultureInfo.CurrentCulture))
                        {
                            var parameters = new CommandParameters
                            {
                                Arguments = args,
                                Switches = switches,
                                Input = TextReader.Null,
                                Output = writer
                            };

                            try
                            {
                                commandManager.Execute(parameters);
                            }
                            catch (Exception ex)
                            {
                                this.WriteError(ex, "FailedToExecuteLegacyCommand", ErrorCategory.NotSpecified);
                                return null;
                            }

                            return writer.ToString();
                        }
                    });
        }
    }
}