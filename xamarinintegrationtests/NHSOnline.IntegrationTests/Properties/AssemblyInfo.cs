using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: CLSCompliant(false)]
[assembly: Parallelize(Workers = 5, Scope = ExecutionScope.MethodLevel)]
