// Copyright (c) Marketing Agents. All rights reserved.

using System.Diagnostics.CodeAnalysis;

// This file is used to suppress analyzer warnings across the entire project
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA0001:XML comment analysis is disabled", Justification = "Not required for all files")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File should have header", Justification = "Not required for all files")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1101:Prefix local calls with this", Justification = "Not our coding style")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Records can use PascalCase")]
[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1110:Opening parenthesis should be on declaration line", Justification = "Allow multi-line parameter lists")]
[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1516:Elements should be separated by blank line", Justification = "Allow compact code")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1200:Using directives should be placed correctly", Justification = "Using file-scoped namespaces")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:Field names should not begin with underscore", Justification = "Allow underscore prefix for private fields")]
[assembly: SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1206:Declaration keywords should follow order", Justification = "Record syntax with required modifier")]
[assembly: SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Analyzer doesn't support record declarations")]
[assembly: SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1011:Closing square brackets should be spaced correctly", Justification = "Allow compact nullable array syntax")]
[assembly: SuppressMessage("SonarAnalyzer.CSharp", "S1118:Utility classes should not have public constructors", Justification = "Program class is auto-generated")]
[assembly: SuppressMessage("SonarAnalyzer.CSharp", "S3260:Non-derived private classes and records should be sealed", Justification = "Record declarations")]
