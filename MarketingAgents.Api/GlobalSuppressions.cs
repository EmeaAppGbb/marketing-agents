// Copyright (c) Marketing Agents. All rights reserved.

using System.Diagnostics.CodeAnalysis;

// This file is used to suppress analyzer warnings across the entire project
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA0001:XML comment analysis is disabled", Justification = "Not required for all files")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File should have header", Justification = "Not required for all files")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1101:Prefix local calls with this", Justification = "Not our coding style")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Records can use PascalCase")]
[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1110:Opening parenthesis should be on declaration line", Justification = "Allow multi-line parameter lists")]
[assembly: SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1516:Elements should be separated by blank line", Justification = "Allow compact code")]
[assembly: SuppressMessage("SonarAnalyzer.CSharp", "S1118:Utility classes should not have public constructors", Justification = "Program class is auto-generated")]
