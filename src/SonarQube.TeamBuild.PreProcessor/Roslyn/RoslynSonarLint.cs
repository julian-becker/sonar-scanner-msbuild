﻿/*
 * SonarQube Scanner for MSBuild
 * Copyright (C) 2016-2018 SonarSource SA
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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using SonarQube.Common;
using SonarQube.TeamBuild.PreProcessor.Roslyn.Model;

namespace SonarQube.TeamBuild.PreProcessor.Roslyn
{
    internal static class RoslynSonarLint
    {
        public static string GenerateXml(IEnumerable<ActiveRule> activeRules, IDictionary<string, string> serverSettings,
            string language, string repoKey)
        {
            var repoActiveRules = activeRules.Where(ar => repoKey.Equals(ar.RepoKey));
            var settings = serverSettings.Where(a => a.Key.StartsWith("sonar." + language + "."));

            var builder = new StringBuilder();
            builder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            builder.AppendLine("<AnalysisInput>");

            builder.AppendLine("  <Settings>");
            foreach (var pair in settings)
            {
                if (!Utilities.IsSecuredServerProperty(pair.Key))
                {
                    WriteSetting(builder, pair.Key, pair.Value);
                }
            }
            builder.AppendLine("  </Settings>");

            builder.AppendLine("  <Rules>");

            foreach (var activeRule in repoActiveRules)
            {
                builder.AppendLine("    <Rule>");
                var templateKey = activeRule.TemplateKey;
                var ruleKey = templateKey ?? activeRule.RuleKey;
                builder.AppendLine("      <Key>" + EscapeXml(ruleKey) + "</Key>");

                if (activeRule.Parameters != null && activeRule.Parameters.Any())
                {
                    builder.AppendLine("      <Parameters>");
                    foreach (var entry in activeRule.Parameters)
                    {
                        builder.AppendLine("        <Parameter>");
                        builder.AppendLine("          <Key>" + EscapeXml(entry.Key) + "</Key>");
                        builder.AppendLine("          <Value>" + EscapeXml(entry.Value) + "</Value>");
                        builder.AppendLine("        </Parameter>");
                    }
                    builder.AppendLine("      </Parameters>");
                }
                builder.AppendLine("    </Rule>");
            }

            builder.AppendLine("  </Rules>");

            builder.AppendLine("  <Files>");
            builder.AppendLine("  </Files>");
            builder.AppendLine("</AnalysisInput>");

            return builder.ToString();
        }

        private static void WriteSetting(StringBuilder builder, string key, string value)
        {
            builder.AppendLine("    <Setting>");
            builder.AppendLine("      <Key>" + EscapeXml(key) + "</Key>");
            builder.AppendLine("      <Value>" + EscapeXml(value) + "</Value>");
            builder.AppendLine("    </Setting>");
        }

        private static string EscapeXml(string str)
        {
            return str.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("'", "&apos;").Replace("<", "&lt;").Replace(">", "&gt;");
        }
    }
}