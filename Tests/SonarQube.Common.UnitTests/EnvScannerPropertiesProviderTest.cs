﻿/*
 * SonarQube Scanner for MSBuild
 * Copyright (C) 2016-2017 SonarSource SA
 * mailto:info AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */
 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestUtilities;

namespace SonarQube.Common.UnitTests
{
    [TestClass]
    public class EnvScannerPropertiesProviderTest
    {
        [TestMethod]
        public void ParseValidJson()
        {
            EnvScannerPropertiesProvider provider = new EnvScannerPropertiesProvider("{ \"sonar.host.url\": \"http://myhost\"}");
            Assert.AreEqual(provider.GetAllProperties().First().Id, "sonar.host.url");
            Assert.AreEqual(provider.GetAllProperties().First().Value, "http://myhost");
            Assert.AreEqual(1, provider.GetAllProperties().Count());
        }

        [TestCleanup]
        public void Cleanup()
        {
            Environment.SetEnvironmentVariable("SONARQUBE_SCANNER_PARAMS", null);
        }

        [TestMethod]
        public void ParseInvalidJson()
        {
            IAnalysisPropertyProvider provider;
            TestLogger logger = new TestLogger();
            Environment.SetEnvironmentVariable("SONARQUBE_SCANNER_PARAMS", "trash");
            bool result = EnvScannerPropertiesProvider.TryCreateProvider(logger, out provider);
            Assert.IsFalse(result);
            logger.AssertErrorLogged("Failed to parse properties from the environment variable 'SONARQUBE_SCANNER_PARAMS'");
        }

        [TestMethod]
        public void NonExistingEnvVar()
        {
            EnvScannerPropertiesProvider provider = new EnvScannerPropertiesProvider(null);
            Assert.AreEqual(0, provider.GetAllProperties().Count());
        }
    }
}