﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardProviderContainer.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider 
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Autofac;
    using Orchard.Management.PsProvider.Agents;
    using Orchard.Management.PsProvider.Host;
    using Proligence.PowerShell.Vfs;

    /// <summary>
    /// Manages the dependency injection container for the Orchard PS provider.
    /// </summary>
    public static class OrchardProviderContainer 
    {
        /// <summary>
        /// The object used to synchronize access to the <see cref="container"/> variable.
        /// </summary>
        private static readonly object ContainerLock = new object();

        /// <summary>
        /// The dependency injection container.
        /// </summary>
        private static volatile IContainer container;

        /// <summary>
        /// Gets the dependency injection container for the Orchard PS provider.
        /// </summary>
        /// <returns>The dependency injection container instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IContainer GetContainer() 
        {
            if (container == null) 
            {
                lock (ContainerLock) 
                {
                    if (container == null) 
                    {
                        container = BuildContainer();
                    }
                }
            }

            return container;
        }

        /// <summary>
        /// Builds a new dependency injection container for the Orchard PS provider.
        /// </summary>
        /// <param name="registrations">Additional container registrations.</param>
        /// <returns>The created dependency injection container.</returns>
        public static IContainer BuildContainer(Action<ContainerBuilder> registrations = null) 
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<PowerShellVfsModule>();

            builder.RegisterType<OrchardDriveInfo>().InstancePerLifetimeScope();
            builder.RegisterType<OrchardSession>().As<IOrchardSession>().InstancePerLifetimeScope();
            builder.RegisterType<AgentManager>().As<IAgentManager>().InstancePerLifetimeScope();

            if (registrations != null) 
            {
                registrations(builder);
            }

            return builder.Build();
        }
    }
}