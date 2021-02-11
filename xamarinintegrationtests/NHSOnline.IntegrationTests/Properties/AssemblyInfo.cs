using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: CLSCompliant(false)]
[assembly: Parallelize(Workers = 3, Scope = ExecutionScope.MethodLevel)]